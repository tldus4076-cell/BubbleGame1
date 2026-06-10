using UnityEngine;

// BubbleGridManager는 스테이지 버블 칸을 관리하는 스크립트입니다.
// 물리 충돌로 아무 곳에 붙이는 방식이 아니라, 먼저 target cell을 고르고 그 칸에 정확히 붙입니다.
public class BubbleGridManager : MonoBehaviour
{
    [Header("격자 크기 설정")]
    [Tooltip("전체 줄 수입니다. 처음 4줄만 채워져 있어도, 발사 버블이 들어갈 5번째 줄이 필요하므로 8~12 정도를 추천합니다.")]
    [SerializeField] private int rows = 12;

    [Tooltip("한 줄의 기본 칸 수입니다. Stage 1은 보통 6칸입니다.")]
    [SerializeField] private int cols = 6;

    [Tooltip("현재 처음부터 채워져 있는 줄 수입니다. 스테이지 버블이 4칸까지 채워져 있으면 4로 둡니다.")]
    [SerializeField] private int initialOccupiedRows = 4;

    [Tooltip("체크하면 버블슈터처럼 홀수 줄이 반 칸 오른쪽으로 밀립니다.")]
    [SerializeField] private bool useStaggeredRows = true;

    [Header("격자 위치 설정")]
    [Tooltip("체크하면 StageBubbleLayout의 벽/천장/간격 값을 읽어서 격자 위치를 자동으로 맞춥니다.")]
    [SerializeField] private bool autoSyncWithStageLayout = true;

    [Tooltip("0번째 줄, 0번째 칸의 월드 위치입니다. 가장 위쪽 왼쪽 버블 중심 위치로 맞추세요.")]
    [SerializeField] private Vector3 topLeftCellWorldPosition = new Vector3(-2.5f, 3f, 0f);

    [Tooltip("칸과 칸 사이의 가로 간격입니다. 스테이지 버블 중심 사이 거리입니다.")]
    [SerializeField] private float cellSpacing = 0.7f;

    [Tooltip("체크하면 시작할 때 씬 안의 Bubble_ 오브젝트들을 찾아 가장 가까운 칸에 자동 등록합니다.")]
    [SerializeField] private bool registerSceneBubblesOnStart = true;

    [Header("디버그 설정")]
    [Tooltip("체크하면 Scene 창에서 격자 칸을 작은 구체로 보여줍니다.")]
    [SerializeField] private bool drawDebugSlots = true;

    private BubbleSlot[,] slots;
    private float cachedLeftX;
    private float cachedRightX;
    private float cachedCeilingY;
    private float cachedBubbleDiameter;
    private bool hasCachedPlayAreaBounds;
    private readonly System.Collections.Generic.List<BubbleSlot> connectedSameColorSlots = new System.Collections.Generic.List<BubbleSlot>();
    private readonly System.Collections.Generic.Queue<BubbleSlot> searchQueue = new System.Collections.Generic.Queue<BubbleSlot>();

    // ============================================================
    // [기능 34] 같은 색 버블이 실제로 제거됐을 때 알려주는 이벤트입니다.
    // event(이벤트)는 "일이 일어났다고 알려주는 종"이라고 생각하면 됩니다.
    // 지금은 점수를 직접 올리지 않고, 제거된 개수만 밖으로 알려줍니다.
    // 나중에 점수 기능은 이 이벤트를 구독해서 점수를 올리면 됩니다.
    // ============================================================
    public event System.Action<int> MatchedBubblesRemoved;

    // ============================================================
    // [기능 40] 떠 있는 버블이 떨어졌을 때 알려주는 이벤트입니다.
    // 떨어진 버블 개수를 밖으로 알려줍니다.
    // 점수 기능은 이 이벤트를 구독해서 떨어진 버블 수만큼 점수를 올리면 됩니다.
    // ============================================================
    public event System.Action<int> FloatingBubblesDropped;

    [Header("제거 효과 연결")]
    [Tooltip("버블이 사라질 때 시각적 효과를 보여주는 BubbleRemovalEffectController를 연결합니다.")]
    [SerializeField] private BubbleRemovalEffectController removalEffectController;

    [Header("떠 있는 버블 떨어뜨리기 설정")]
    [Tooltip("떠 있는 버블이 아래로 떨어지는 속도입니다. 숫자가 클수록 빠르게 떨어집니다.")]
    [SerializeField] private float dropFallSpeed = 8f;

    [Tooltip("떠 있는 버블이 떨어지는 거리입니다. 화면 아래로 충분히 내려가야 사라집니다.")]
    [SerializeField] private float dropDistance = 10f;

    public int Rows => rows;
    public int Cols => cols;
    public float CellSpacing => cellSpacing;

    private void Awake()
    {
        SyncGridWithStageLayout();
        BuildGrid();
    }

    private void Start()
    {
        if (registerSceneBubblesOnStart)
        {
            // StageBubbleLayout이 Start에서 버블을 만든 뒤 등록되도록 아주 잠깐 늦게 실행합니다.
            Invoke(nameof(RefreshGridRegistration), 0.05f);
        }

        // [기능 37] 게임 시작 시 천장과 연결된 버블 개수를 로그로 출력합니다.
        // 스테이지 버블 등록이 끝난 뒤 확인해야 하므로 0.1초 뒤에 실행합니다.
        Invoke(nameof(LogCeilingConnectedBubbles), 0.1f);

        // [기능 38] 게임 시작 시 떠 있는 버블 개수도 로그로 출력합니다.
        Invoke(nameof(LogFloatingBubbles), 0.15f);
    }

    private void OnValidate()
    {
        rows = Mathf.Max(1, rows);
        cols = Mathf.Max(1, cols);
        initialOccupiedRows = Mathf.Clamp(initialOccupiedRows, 0, rows);
        cellSpacing = Mathf.Max(0.05f, cellSpacing);
        SyncGridWithStageLayout();
        BuildGrid();
    }

    // 격자 칸들을 새로 만듭니다.
    public void BuildGrid()
    {
        slots = new BubbleSlot[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 worldPosition = GetCellWorldPosition(row, col);
                BubbleSlot slot = new BubbleSlot(row, col, worldPosition);

                slots[row, col] = slot;
            }
        }
    }

    // row, col을 월드 좌표로 바꿉니다.
    public Vector3 GetCellWorldPosition(int row, int col)
    {
        float verticalSpacing = useStaggeredRows ? cellSpacing * Mathf.Sqrt(3f) / 2f : cellSpacing;
        float xOffset = useStaggeredRows && row % 2 == 1 ? cellSpacing / 2f : 0f;

        return new Vector3(
            topLeftCellWorldPosition.x + xOffset + col * cellSpacing,
            topLeftCellWorldPosition.y - row * verticalSpacing,
            topLeftCellWorldPosition.z
        );
    }

    // 발사 전 조준선 방향으로 들어갈 target cell을 찾습니다.
    public bool TryFindTargetSlot(Vector3 rayOrigin, Vector2 aimDirection, out BubbleSlot targetSlot)
    {
        targetSlot = null;

        if (slots == null || slots.GetLength(0) != rows || slots.GetLength(1) != cols)
        {
            BuildGrid();
        }

        // 발사 직전에 실제 스테이지 버블 위치를 다시 등록합니다.
        // 그래야 이미 있는 버블 칸을 빈칸으로 착각해서 겹치는 일을 막을 수 있습니다.
        RefreshGridRegistration();

        if (aimDirection.sqrMagnitude < 0.001f)
        {
            return false;
        }

        aimDirection.Normalize();

        // 아래로 향하는 방향이면 반대로 뒤집습니다.
        // 버블슈터에서는 항상 위쪽 스테이지를 향해 발사해야 합니다.
        if (aimDirection.y < 0f)
        {
            aimDirection = -aimDirection;
        }

        // 위쪽 줄부터 아래쪽 줄로 내려오며 확인합니다.
        // 처음 4줄이 차 있고 5번째 줄이 비어 있으면, 5번째 줄이 먼저 선택됩니다.
        for (int row = 0; row < rows; row++)
        {
            float rowY = GetCellWorldPosition(row, 0).y;

            // 이 줄 높이까지 조준선이 도달하지 않으면 건너뜁니다.
            if (aimDirection.y <= 0.001f)
            {
                continue;
            }

            float t = (rowY - rayOrigin.y) / aimDirection.y;
            if (t < 0f)
            {
                continue;
            }

            float aimedX = rayOrigin.x + aimDirection.x * t;
            int nearestCol = GetNearestColumnInRow(row, aimedX);

            if (!IsValidCell(row, nearestCol))
            {
                continue;
            }

            BubbleSlot slot = slots[row, nearestCol];

            // WallRoot의 LeftWall, RightWall, Ceiling 안쪽 칸만 target으로 사용합니다.
            if (!IsSlotInsidePlayArea(slot))
            {
                continue;
            }

            if (slot.occupied)
            {
                continue;
            }

            // 공중에 혼자 떠 있는 칸은 사용하지 않습니다.
            // 기존 버블 또는 천장과 연결될 수 있는 칸만 target으로 잡습니다.
            if (!IsAttachableSlot(row, nearestCol))
            {
                continue;
            }

            targetSlot = slot;
            return true;
        }

        return false;
    }

    // 화면에 실제로 보이는 dotted line 경로를 기준으로 target cell을 찾습니다.
    // dotted line 경로를 위(천장)에서 아래(슈터) 방향으로 따라가면서,
    // 조준선이 지나가는 줄(row)에서 가장 먼저 발견되는 유효한 빈칸을 반환합니다.
    // 조준선이 꺾여도, 끝점이 멀어도, 조준선이 가리키는 "가장 위쪽 빈칸"에 버블이 붙습니다.
    public bool TryFindTargetSlotOnAimPath(Vector3[] aimLinePoints, out BubbleSlot targetSlot)
    {
        targetSlot = null;

        if (aimLinePoints == null || aimLinePoints.Length < 2)
        {
            return false;
        }

        RefreshGridRegistration();

        float maxDistanceFromLine = cellSpacing * 0.55f;

        // ============================================================
        // 조준선 경로를 위(천장)에서 아래(슈터) 방향으로 한 줄씩 확인합니다.
        // row 0(천장)부터 시작해서, 조준선이 지나가는 줄에서
        // 가장 먼저 발견되는 유효한 빈칸을 target으로 반환합니다.
        // ============================================================
        for (int row = 0; row < rows; row++)
        {
            float rowY = GetCellWorldPosition(row, 0).y;

            // 이 줄(row)의 Y 좌표에서 조준선의 X 좌표를 구합니다.
            // 조준선이 이 줄을 지나가지 않으면 false를 돌려줍니다.
            if (!TryGetAimedXOnPathAtY(aimLinePoints, rowY, out float aimedX))
            {
                continue;
            }

            // 이 줄에서 조준선에 가장 가까운 유효한 빈칸을 찾습니다.
            BubbleSlot bestSlotInRow = null;
            float bestDistanceInRow = float.MaxValue;

            for (int col = 0; col < GetColsInRow(row); col++)
            {
                BubbleSlot slot = slots[row, col];

                if (slot.occupied)
                {
                    continue;
                }

                if (!IsSlotInsidePlayArea(slot))
                {
                    continue;
                }

                if (!IsAttachableSlot(row, col))
                {
                    continue;
                }

                // 조준선과의 거리를 확인합니다.
                float distanceFromLine = Mathf.Abs(slot.worldPosition.x - aimedX);
                if (distanceFromLine > maxDistanceFromLine)
                {
                    continue;
                }

                if (distanceFromLine < bestDistanceInRow)
                {
                    bestDistanceInRow = distanceFromLine;
                    bestSlotInRow = slot;
                }
            }

            // 이 줄에서 유효한 빈칸을 찾았으면, 그것이 target입니다.
            // 더 아래 줄은 확인하지 않습니다. (가장 위쪽 빈칸이 우선)
            if (bestSlotInRow != null)
            {
                targetSlot = bestSlotInRow;
                Debug.Log($"[GridTarget] target cell: row {targetSlot.row}, col {targetSlot.col}, 위치: {targetSlot.worldPosition}");
                return true;
            }
        }

        // ============================================================
        // 조준선 경로 위에 유효한 빈칸이 없으면,
        // 조준선 끝점에서 가장 가까운 유효한 빈칸을 격자 전체에서 찾습니다.
        // (거리 제한 없음)
        // ============================================================
        Vector3 aimEndPoint = aimLinePoints[aimLinePoints.Length - 1];
        float bestTotalDistance = float.MaxValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                BubbleSlot slot = slots[row, col];

                if (slot.occupied)
                {
                    continue;
                }

                if (!IsSlotInsidePlayArea(slot))
                {
                    continue;
                }

                if (!IsAttachableSlot(row, col))
                {
                    continue;
                }

                float distanceToEnd = Vector2.Distance(slot.worldPosition, aimEndPoint);
                if (distanceToEnd < bestTotalDistance)
                {
                    bestTotalDistance = distanceToEnd;
                    targetSlot = slot;
                }
            }
        }

        if (targetSlot != null)
        {
            Debug.Log($"[GridTarget] target cell (예비): row {targetSlot.row}, col {targetSlot.col}, 위치: {targetSlot.worldPosition}");
            return true;
        }

        Debug.LogWarning("[GridTarget] 유효한 target cell을 찾지 못했습니다.");
        return false;
    }

    // dotted line이 실제로 도달하는 "occupied 근처 지점"을 찾습니다.
    // dotted line을 끝점부터 시작점 방향으로 되짚어 올라가면서,
    // occupied 칸이 가장 가까운 지점을 찾습니다.
    // 이 지점이 버블이 실제로 붙어야 하는 위치입니다.
    private Vector3 FindAimLandingPoint(Vector3[] aimLinePoints)
    {
        // dotted line 위의 여러 샘플 지점을 확인합니다.
        // 끝점부터 시작점까지 20개 샘플을均匀하게 나눕니다.
        int sampleCount = 20;
        Vector3 lastPoint = aimLinePoints[aimLinePoints.Length - 1];
        Vector3 firstPoint = aimLinePoints[0];
        Vector3 bestLandingPoint = lastPoint;
        float bestOccupiedDistance = float.MaxValue;

        for (int s = sampleCount; s >= 0; s--)
        {
            float t = (float)s / sampleCount;
            Vector3 samplePoint = Vector3.Lerp(firstPoint, lastPoint, t);

            // 샘플 지점에서 가장 가까운 occupied 칸의 거리를 찾습니다.
            float nearestOccupiedDistance = FindNearestOccupiedDistance(samplePoint);

            // occupied 칸이 충분히 가까우면 이 지점이 "도착점"입니다.
            // 버블이 여기서 멈추고, 여기서 가장 가까운 빈칸에 붙어야 합니다.
            if (nearestOccupiedDistance < cellSpacing * 1.5f)
            {
                if (nearestOccupiedDistance < bestOccupiedDistance)
                {
                    bestOccupiedDistance = nearestOccupiedDistance;
                    bestLandingPoint = samplePoint;
                }
            }
        }

        return bestLandingPoint;
    }

    // 어떤 월드 위치에서 가장 가까운 occupied 칸까지의 거리를 찾습니다.
    private float FindNearestOccupiedDistance(Vector3 worldPosition)
    {
        float bestDistance = float.MaxValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                BubbleSlot slot = slots[row, col];

                if (!slot.occupied || slot.bubbleObject == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(slot.worldPosition, worldPosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                }
            }
        }

        // occupied 칸이 하나도 없으면 천장(최상단)까지의 거리를 돌려줍니다.
        if (bestDistance >= float.MaxValue * 0.5f)
        {
            bestDistance = Mathf.Abs(worldPosition.y - GetCellWorldPosition(0, 0).y);
        }

        return bestDistance;
    }

    private bool TryGetAimedXOnPathAtY(Vector3[] aimLinePoints, float targetY, out float aimedX)
    {
        aimedX = 0f;

        // 먼저 마지막 조준 방향을 ray처럼 사용합니다.
        // dotted line 그림이 짧게 끝나도, 실제 발사 방향은 그 끝쪽 방향으로 계속 이어진다고 봐야 합니다.
        // 그래야 빨간 동그라미처럼 조준선 끝보다 더 위에 있는 빈칸도 먼저 선택할 수 있습니다.
        Vector3 finalStartPoint = aimLinePoints[aimLinePoints.Length - 2];
        Vector3 finalEndPoint = aimLinePoints[aimLinePoints.Length - 1];
        float finalDeltaY = finalEndPoint.y - finalStartPoint.y;

        if (Mathf.Abs(finalDeltaY) >= 0.001f)
        {
            float rayT = (targetY - finalStartPoint.y) / finalDeltaY;

            if (rayT >= -0.001f)
            {
                aimedX = finalStartPoint.x + (finalEndPoint.x - finalStartPoint.x) * rayT;
                return true;
            }
        }

        // 마지막 방향 ray로 계산할 수 없는 줄은 예비로 실제 dotted line 조각들을 검사합니다.
        // 조준선이 벽에 튕기면 같은 높이(row)를 두 선분이 지나갈 수 있으므로 끝쪽 선분부터 확인합니다.
        for (int i = aimLinePoints.Length - 2; i >= 0; i--)
        {
            Vector3 startPoint = aimLinePoints[i];
            Vector3 endPoint = aimLinePoints[i + 1];

            float minY = Mathf.Min(startPoint.y, endPoint.y);
            float maxY = Mathf.Max(startPoint.y, endPoint.y);

            if (targetY < minY || targetY > maxY)
            {
                continue;
            }

            if (Mathf.Abs(endPoint.y - startPoint.y) < 0.001f)
            {
                aimedX = startPoint.x;
                return true;
            }

            float t = (targetY - startPoint.y) / (endPoint.y - startPoint.y);
            t = Mathf.Clamp01(t);
            aimedX = Mathf.Lerp(startPoint.x, endPoint.x, t);
            return true;
        }

        return false;
    }

    private int GetNearestColumnInRow(int row, float aimedX)
    {
        Vector3 firstCell = GetCellWorldPosition(row, 0);
        int nearestCol = Mathf.RoundToInt((aimedX - firstCell.x) / cellSpacing);
        return Mathf.Clamp(nearestCol, 0, GetColsInRow(row) - 1);
    }

    private bool IsAttachableSlot(int row, int col)
    {
        if (row == 0)
        {
            return true;
        }

        int[,] neighborOffsets = GetNeighborOffsets(row);

        for (int i = 0; i < neighborOffsets.GetLength(0); i++)
        {
            int neighborRow = row + neighborOffsets[i, 0];
            int neighborCol = col + neighborOffsets[i, 1];

            if (IsValidCell(neighborRow, neighborCol) && slots[neighborRow, neighborCol].occupied)
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterBubble(BubbleSlot slot, GameObject bubbleObject)
    {
        if (slot == null || bubbleObject == null)
        {
            return;
        }

        slot.occupied = true;
        slot.bubbleObject = bubbleObject;
        bubbleObject.name = "Bubble_GridStopped";
        bubbleObject.transform.position = slot.worldPosition;
        bubbleObject.transform.SetParent(transform, true);
        // 기능 34: 같은 색 매칭 규칙을 확인하고, 3개 이상이면 실제로 제거합니다.
        CheckMatchRule(slot);
    }

    // ============================================================
    // [기능 34] 같은 색 매칭 규칙을 확인합니다.
    // 버블이 격자에 붙은 뒤, 같은 색으로 연결된 개수를 세서
    // 3개 이상이면 실제로 제거합니다.
    // 2개 이하면 "제거하지 않음" 로그만 출력하고 그대로 둡니다.
    //
    // [언제 호출되는가?]
    // RegisterBubble()이 호출될 때, 버블이 격자에 붙은 직후에 자동으로 호출됩니다.
    // 즉, 버블이 발사되어 벽/천장/다른 버블에 닿아서 멈추는 순간 실행됩니다.
    // ============================================================
    private void CheckMatchRule(BubbleSlot startSlot)
    {
        // 같은 색으로 연결된 버블을 담을 리스트를 깨끗이 비웁니다.
        connectedSameColorSlots.Clear();

        // BFS(너비 우선 탐색)용 큐를 비웁니다.
        // BFS란? 그림으로 설명하면, 물이 퍼지듯이 한 칸씩 이웃을 확인하는 방법입니다.
        // 시작 버블에서 시작해서, 옆에 같은 색이 있으면 그 버블도 추가하고,
        // 그 버블의 옆에도 같은 색이 있으면 또 추가하는 방식입니다.
        searchQueue.Clear();

        // 시작 버블이 없으면 아무것도 하지 않습니다.
        if (startSlot == null || startSlot.bubbleObject == null)
        {
            return;
        }

        // 이미 방문한 칸을 표시하는 배열입니다.
        // 같은 칸을 두 번 세지 않기 위해서 필요합니다.
        bool[,] visited = new bool[rows, cols];

        // 시작 칸을 큐에 넣고 방문 표시를 합니다.
        searchQueue.Enqueue(startSlot);
        visited[startSlot.row, startSlot.col] = true;

        // BFS 탐색: 큐가 빌 때까지 반복합니다.
        while (searchQueue.Count > 0)
        {
            // 큐에서 하나 꺼냅니다.
            BubbleSlot currentSlot = searchQueue.Dequeue();

            // 연결된 버블 목록에 추가합니다.
            connectedSameColorSlots.Add(currentSlot);

            // 지금 버블의 이웃 중에서 같은 색인 것을 찾아서 큐에 넣습니다.
            AddSameColorNeighborSlots(currentSlot, visited);
        }

        // 연결된 버블 개수를 셉니다.
        int connectedCount = connectedSameColorSlots.Count;

        // 버블 색 이름을 가져옵니다. (빨강, 파랑, 노랑 등)
        string colorName = GetBubbleColorName(startSlot.bubbleObject);

        // 기능 34 핵심 규칙입니다.
        // 3개 이상이면 제거하고, 2개 이하면 제거하지 않습니다.
        if (ShouldRemoveMatchedBubbles(connectedCount))
        {
            RemoveMatchedBubbles(colorName);
        }
        else
        {
            // 1개 또는 2개이면 제거하지 않습니다.
            // 버블은 그대로 남아 있어야 합니다.
            Debug.Log($"[기능 34] 제거하지 않음. 색: {colorName}, 연결 개수: {connectedCount}개");
        }
    }

    // ============================================================
    // 같은 색으로 연결된 버블을 제거해야 하는지 알려주는 함수입니다.
    // 버블슈터 규칙은 "같은 색 3개 이상이면 제거"입니다.
    // 그래서 connectedCount가 3 이상이면 true를 돌려줍니다.
    // true는 "맞다", false는 "아니다"라는 뜻입니다.
    // ============================================================
    private bool ShouldRemoveMatchedBubbles(int connectedCount)
    {
        return connectedCount >= 3;
    }

    // ============================================================
    // 같은 색으로 연결된 버블과 그 옆에 붙은 버블들을 실제로 제거합니다.
    //
    // [규칙]
    // 같은 색 3개 이상이 제거될 때, 그 옆에 붙어있는 다른 색 버블도 같이 제거됩니다.
    // 예: 빨강 3개가 모여서 제거되면, 그 옆에 붙은 파랑 버블도 같이 사라집니다.
    // 이렇게 해야 화면이 깔끔하게 정리됩니다.
    //
    // connectedSameColorSlots: 같은 색으로 연결된 버블 칸들 (3개 이상)
    // ============================================================
    private void RemoveMatchedBubbles(string colorName)
    {
        // 제거할 버블 칸을 모으는 리스트입니다.
        // 같은 색 버블 + 옆에 붙은 다른 색 버블을 모두 담습니다.
        System.Collections.Generic.List<BubbleSlot> slotsToRemove = new System.Collections.Generic.List<BubbleSlot>();

        // 1단계: 같은 색으로 연결된 버블을 먼저 담습니다.
        for (int i = 0; i < connectedSameColorSlots.Count; i++)
        {
            slotsToRemove.Add(connectedSameColorSlots[i]);
        }

        // 2단계: 같은 색 버블 옆에 붙은 다른 색 버블을 찾습니다.
        // 같은 색 버블 하나하나의 이웃을 확인해서, 제거 목록에 없는 이웃이면 추가합니다.
        for (int i = 0; i < connectedSameColorSlots.Count; i++)
        {
            BubbleSlot matchedSlot = connectedSameColorSlots[i];
            int[,] neighborOffsets = GetNeighborOffsets(matchedSlot.row);

            for (int n = 0; n < neighborOffsets.GetLength(0); n++)
            {
                int neighborRow = matchedSlot.row + neighborOffsets[n, 0];
                int neighborCol = matchedSlot.col + neighborOffsets[n, 1];

                if (!IsValidCell(neighborRow, neighborCol))
                {
                    continue;
                }

                BubbleSlot neighborSlot = slots[neighborRow, neighborCol];

                if (!neighborSlot.occupied || neighborSlot.bubbleObject == null)
                {
                    continue;
                }

                // 이미 제거 목록에 있으면 다시 추가하지 않습니다.
                if (slotsToRemove.Contains(neighborSlot))
                {
                    continue;
                }

                // 옆에 붙은 버블을 제거 목록에 추가합니다.
                slotsToRemove.Add(neighborSlot);
            }
        }

        // 3단계: 모아둔 버블을 실제로 제거합니다.
        int removedCount = slotsToRemove.Count;

        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            ClearBubbleSlot(slotsToRemove[i]);
        }

        Debug.Log($"[기능 34] 버블 제거 완료! 색: {colorName}, 같은 색: {connectedSameColorSlots.Count}개, 옆 버블 포함 총 제거: {removedCount}개");

        // 점수는 여기서 직접 올리지 않습니다.
        // 나중에 점수 기능이 이 이벤트를 구독해서 제거 개수만큼 점수를 올리면 됩니다.
        MatchedBubblesRemoved?.Invoke(removedCount);

        // ============================================================
        // [기능 37] 버블이 제거된 뒤, 천장과 연결된 버블이 몇 개인지 확인합니다.
        // 이건 실제 제거나 점수 변화 없이, 로그만 출력합니다.
        // 기능 38~40에서 "떨어질 버블"을 찾기 위한 준비 단계입니다.
        // ============================================================
        FindCeilingConnectedBubbles();

        // ============================================================
        // [기능 38] 천장과 연결되지 않은 떠 있는 버블이 몇 개인지 확인합니다.
        // ============================================================
        FindFloatingBubbles();

        // ============================================================
        // [기능 39] 떠 있는 버블을 아래로 떨어뜨립니다.
        // 떠 있는 버블이 없으면 아무 일도 일어나지 않습니다.
        // ============================================================
        DropFloatingBubbles();
    }

    // ============================================================
    // 버블 하나가 들어 있던 격자 칸을 비우고, 화면의 버블 오브젝트를 제거합니다.
    // slot.occupied = false는 "이 칸은 이제 비었다"는 뜻입니다.
    // slot.bubbleObject = null은 "이 칸에 연결된 버블 오브젝트가 없다"는 뜻입니다.
    // 이렇게 해야 다음 발사 버블이 이 칸에 다시 들어갈 수 있습니다.
    // ============================================================
    private void ClearBubbleSlot(BubbleSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        GameObject bubbleObject = slot.bubbleObject;

        // ============================================================
        // [기능 36] 버블을 실제로 없애기 전에 제거 효과를 보여줍니다.
        // 효과 컨트롤러가 Inspector에서 연결되어 있으면 효과를 재생합니다.
        // ============================================================
        if (removalEffectController != null && bubbleObject != null)
        {
            // 버블이 있던 월드 위치를 저장합니다.
            Vector3 effectPosition = bubbleObject.transform.position;

            // 버블 색을 가져옵니다. (SpriteRenderer의 color)
            SpriteRenderer bubbleRenderer = bubbleObject.GetComponent<SpriteRenderer>();
            Color effectColor = bubbleRenderer != null ? bubbleRenderer.color : Color.white;

            // 효과를 재생합니다.
            removalEffectController.PlayRemovalEffect(effectPosition, effectColor);
        }

        // 슬롯을 빈칸으로 만듭니다.
        slot.occupied = false;
        slot.bubbleObject = null;

        if (bubbleObject == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(bubbleObject);
        }
        else
        {
            DestroyImmediate(bubbleObject);
        }
    }

    // ============================================================
    // 이웃 칸 중에서 같은 색 버블을 찾아서 큐에 넣습니다.
    // BFS 탐색에서 "다음에 확인할 버블"을 찾는 함수입니다.
    //
    // [언제 호출되는가?]
    // CheckMatchRule() 안의 while 루프에서 매번 호출됩니다.
    // 지금 확인하고 있는 버블의 이웃 6칸을 확인합니다.
    //
    // [이웃 6칸이란?]
    // 벌집 모양 격자에서 한 버블 주위에 붙어 있는 6개 칸입니다.
    // 왼쪽, 오른쪽, 위쪽 2칸, 아래쪽 2칸입니다.
    // ============================================================
    private void AddSameColorNeighborSlots(BubbleSlot currentSlot, bool[,] visited)
    {
        int[,] neighborOffsets = GetNeighborOffsets(currentSlot.row);

        for (int i = 0; i < neighborOffsets.GetLength(0); i++)
        {
            int neighborRow = currentSlot.row + neighborOffsets[i, 0];
            int neighborCol = currentSlot.col + neighborOffsets[i, 1];

            if (!IsValidCell(neighborRow, neighborCol) || visited[neighborRow, neighborCol])
            {
                continue;
            }

            BubbleSlot neighborSlot = slots[neighborRow, neighborCol];
            if (!neighborSlot.occupied || neighborSlot.bubbleObject == null)
            {
                continue;
            }

            if (!IsSameBubbleColor(currentSlot.bubbleObject, neighborSlot.bubbleObject))
            {
                continue;
            }

            // 매칭된 이웃 버블 정보를 로그로 출력합니다.
            string currentColor = GetBubbleColorName(currentSlot.bubbleObject);
            string neighborColor = GetBubbleColorName(neighborSlot.bubbleObject);
            Debug.Log($"[매칭] ({currentSlot.row},{currentSlot.col}){currentColor} ↔ ({neighborRow},{neighborCol}){neighborColor}");

            visited[neighborRow, neighborCol] = true;
            searchQueue.Enqueue(neighborSlot);
        }
    }

    // ============================================================
    // [기능 37] 천장과 연결된 모든 버블을 찾습니다.
    //
    // [왜 필요한가요?]
    // 버블슈터에서 "떨어질 버블"을 찾으려면 먼저 "떨어지지 않을 버블"을 찾아야 합니다.
    // 천장(row 0)에 붙은 버블에서 시작해서, 그 옆에 붙은 버블들을 차례로 따라가면
    // 천장과 연결된 모든 버블(떨어지지 않을 버블)을 알 수 있습니다.
    // 이걸 알아야 나중에 "천장과 연결 안 된 떠 있는 버블"을 골라서 떨어뜨릴 수 있어요.
    //
    // [어떻게 찾나요?]
    // BFS(너비 우선 탐색)를 사용합니다.
    // 1. row 0(천장 줄)에 있는 모든 occupied 칸을 시작점으로 큐에 넣습니다.
    // 2. 큐에서 하나씩 꺼내면서, 그 이웃 중 occupied 칸을 모두 큐에 넣습니다.
    // 3. 색깔은 상관없이, occupied면 전부 연결된 것으로 봅니다.
    // 4. 큐가 비면 탐색이 끝납니다.
    //
    // [반환값]
    // 천장과 연결된 모든 BubbleSlot의 리스트.
    // 이 리스트에 없는 occupied 슬롯은 "떨어질 버블"입니다.
    // ============================================================
    public System.Collections.Generic.List<BubbleSlot> FindCeilingConnectedBubbles()
    {
        // 결과를 담을 새 리스트를 만듭니다.
        // (기존 connectedSameColorSlots와 섞이지 않게 새 리스트를 씁니다.)
        System.Collections.Generic.List<BubbleSlot> ceilingConnectedSlots = new System.Collections.Generic.List<BubbleSlot>();

        // BFS용 큐를 비웁니다.
        searchQueue.Clear();

        // 이미 방문한 칸을 표시하는 배열입니다.
        // 같은 칸을 두 번 세지 않기 위해 필요합니다.
        bool[,] visited = new bool[rows, cols];

        // ============================================================
        // 1단계: 천장 줄(row 0)의 모든 occupied 칸을 시작점으로 큐에 넣습니다.
        // ============================================================
        for (int col = 0; col < GetColsInRow(0); col++)
        {
            BubbleSlot ceilingSlot = slots[0, col];

            if (ceilingSlot.occupied && ceilingSlot.bubbleObject != null)
            {
                searchQueue.Enqueue(ceilingSlot);
                visited[0, col] = true;
            }
        }

        // ============================================================
        // 2단계: BFS 탐색 - 큐가 빌 때까지 반복합니다.
        // ============================================================
        while (searchQueue.Count > 0)
        {
            // 큐에서 하나 꺼냅니다.
            BubbleSlot currentSlot = searchQueue.Dequeue();

            // 결과 리스트에 추가합니다.
            ceilingConnectedSlots.Add(currentSlot);

            // 현재 버블의 이웃 중 occupied인 것을 큐에 넣습니다.
            AddOccupiedNeighborSlots(currentSlot, visited);
        }

        // 결과 개수를 로그로 출력합니다.
        Debug.Log($"[기능 37] 천장 연결 버블: {ceilingConnectedSlots.Count}개");

        return ceilingConnectedSlots;
    }

    // ============================================================
    // [기능 37 도우미] 천장 연결 버블 개수를 로그로 출력합니다.
    // Start()에서 Invoke로 호출됩니다.
    // ============================================================
    private void LogCeilingConnectedBubbles()
    {
        FindCeilingConnectedBubbles();
    }

    // ============================================================
    // [기능 38] 천장과 연결되지 않은 떠 있는 버블을 찾습니다.
    //
    // [왜 필요한가요?]
    // 같은 색 3개 이상이 제거되면, 그 옆에 붙어있던 버블이 천장과 연결이 끊어져서
    // 공중에 떠 있는 상태가 됩니다. 이런 버블은 떨어뜨려야 게임이 깔끔해집니다.
    // 떨어뜨리기 전에 먼저 "떠 있는 버블"을 정확히 찾아야 합니다.
    //
    // [어떻게 찾나요?]
    // 1. 기능 37의 FindCeilingConnectedBubbles()로 천장 연결 버블 목록을 받습니다.
    // 2. 전체 격자를 돌면서, occupied인 칸 중 천장 연결 버블에 없는 것을 모읍니다.
    // 3. 그게 바로 "떠 있는 버블"입니다.
    //
    // [반환값]
    // 천장과 연결되지 않은 떠 있는 BubbleSlot의 리스트.
    // 이 리스트에 있는 버블들은 나중에 떨어뜨릴 수 있습니다.
    // ============================================================
    public System.Collections.Generic.List<BubbleSlot> FindFloatingBubbles()
    {
        // 떠 있는 버블을 담을 새 리스트를 만듭니다.
        System.Collections.Generic.List<BubbleSlot> floatingSlots = new System.Collections.Generic.List<BubbleSlot>();

        // 기능 37의 함수를 호출해서 천장 연결 버블 목록을 받습니다.
        System.Collections.Generic.List<BubbleSlot> ceilingConnectedSlots = FindCeilingConnectedBubbles();

        // Contains()를 빠르게 쓰기 위해 HashSet으로 변환합니다.
        // (List.Contains는 느리지만, HashSet.Contains는 빠릅니다.)
        System.Collections.Generic.HashSet<BubbleSlot> ceilingSet = new System.Collections.Generic.HashSet<BubbleSlot>();
        for (int i = 0; i < ceilingConnectedSlots.Count; i++)
        {
            ceilingSet.Add(ceilingConnectedSlots[i]);
        }

        // 전체 격자를 돌면서, occupied이지만 천장 연결 버블이 아닌 칸을 찾습니다.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                BubbleSlot slot = slots[row, col];

                // occupied 아니면 무시합니다.
                if (!slot.occupied || slot.bubbleObject == null)
                {
                    continue;
                }

                // 천장 연결 버블에 들어 있으면 떠 있는 버블이 아닙니다.
                if (ceilingSet.Contains(slot))
                {
                    continue;
                }

                // 위 두 조건에 모두 해당하지 않으면, 이 칸은 떠 있는 버블입니다.
                floatingSlots.Add(slot);
            }
        }

        // 결과 개수를 로그로 출력합니다.
        Debug.Log($"[기능 38] 떠 있는 버블: {floatingSlots.Count}개");

        return floatingSlots;
    }

    // ============================================================
    // [기능 38 도우미] 떠 있는 버블 개수를 로그로 출력합니다.
    // Start()에서 Invoke로 호출됩니다.
    // ============================================================
    private void LogFloatingBubbles()
    {
        FindFloatingBubbles();
    }

    // ============================================================
    // [기능 37 도우미] 천장에서 아래로 매달린 occupied 칸을 큐에 넣습니다.
    // 색깔은 상관없이 occupied면 연결된 것으로 봅니다.
    // 단, 옆으로만 이어진 버블까지 모두 천장 연결로 착각하지 않도록
    // 천장 -> 아래 방향으로 이어지는 칸만 따라갑니다.
    // ============================================================
    private void AddOccupiedNeighborSlots(BubbleSlot currentSlot, bool[,] visited)
    {
        int[,] neighborOffsets = GetDownwardSupportOffsets(currentSlot.row);

        for (int i = 0; i < neighborOffsets.GetLength(0); i++)
        {
            int neighborRow = currentSlot.row + neighborOffsets[i, 0];
            int neighborCol = currentSlot.col + neighborOffsets[i, 1];

            if (!IsValidCell(neighborRow, neighborCol) || visited[neighborRow, neighborCol])
            {
                continue;
            }

            BubbleSlot neighborSlot = slots[neighborRow, neighborCol];
            if (!neighborSlot.occupied || neighborSlot.bubbleObject == null)
            {
                continue;
            }

            // 색깔은 비교하지 않습니다.
            // 천장에서 아래로 매달린 방향에 있고 occupied면 큐에 넣습니다.

            visited[neighborRow, neighborCol] = true;
            searchQueue.Enqueue(neighborSlot);
        }
    }

    // ============================================================
    // [기능 37 도우미] 천장 연결 확인용 아래 방향 이웃을 돌려줍니다.
    //
    // 일반 같은 색 찾기는 주변 6칸을 모두 봅니다.
    // 하지만 지금 테스트 기준에서는 옆/대각선으로 살짝 이어진 버블까지
    // 전부 천장 연결로 보면 "떠 있는 버블: 0개"가 계속 나옵니다.
    // 그래서 천장 연결 확인은 아주 단순하게 "바로 아래 칸"만 따라갑니다.
    //
    // 예:
    // 천장 버블 바로 아래에 있으면 연결
    // 옆이나 대각선에만 있으면 떠 있는 버블 후보
    // ============================================================
    private int[,] GetDownwardSupportOffsets(int row)
    {
        return new int[,]
        {
            { 1, 0 }
        };
    }

    private int[,] GetNeighborOffsets(int row)
    {
        if (useStaggeredRows && row % 2 == 1)
        {
            return new int[,]
            {
                { 0, -1 }, { 0, 1 },
                { -1, 0 }, { -1, 1 },
                { 1, 0 }, { 1, 1 }
            };
        }

        return new int[,]
        {
            { 0, -1 }, { 0, 1 },
            { -1, 0 }, { -1, -1 },
            { 1, 0 }, { 1, -1 }
        };
    }

    // ============================================================
    // 두 버블이 같은 색인지 확인합니다.
    //
    // [중요]
    // 버블들이 같은 원형 Sprite를 쓰고 SpriteRenderer Color만 다를 수 있습니다.
    // 이때 Sprite만 비교하면 빨강, 파랑, 노랑이 모두 같은 버블로 잘못 계산됩니다.
    // 그래서 색칠(renderer.color)이 들어간 버블은 Sprite보다 색칠 값을 먼저 비교합니다.
    // ============================================================
    private bool IsSameBubbleColor(GameObject firstBubble, GameObject secondBubble)
    {
        SpriteRenderer firstRenderer = firstBubble.GetComponent<SpriteRenderer>();
        SpriteRenderer secondRenderer = secondBubble.GetComponent<SpriteRenderer>();

        if (firstRenderer == null || secondRenderer == null)
        {
            return false;
        }

        // 1단계: 둘 중 하나라도 색칠된 버블이면 색칠 값을 먼저 비교합니다.
        // 예: 같은 원형 Sprite를 쓰지만 첫 번째는 파랑, 두 번째는 빨강일 수 있습니다.
        if (IsTintColorUsed(firstRenderer.color) || IsTintColorUsed(secondRenderer.color))
        {
            return IsSimilarRendererColor(firstRenderer.color, secondRenderer.color);
        }

        // 2단계: 색칠이 둘 다 흰색이면 Sprite 자체 이미지로 색을 구분합니다.
        // 예: red.png, blue.png, yellow.png처럼 Sprite 이미지가 다를 때 사용합니다.
        if (firstRenderer.sprite != null && secondRenderer.sprite != null)
        {
            return firstRenderer.sprite == secondRenderer.sprite;
        }

        // 3단계: Sprite가 비어 있으면 마지막으로 색상 값을 비교합니다.
        return IsSimilarRendererColor(firstRenderer.color, secondRenderer.color);
    }

    // SpriteRenderer의 Color가 흰색이 아니면 색칠된 버블로 봅니다.
    // 흰색은 "Sprite 원래 색을 그대로 보여줘"라는 뜻으로 많이 사용합니다.
    private bool IsTintColorUsed(Color color)
    {
        float distanceFromWhite = Mathf.Abs(color.r - 1f)
            + Mathf.Abs(color.g - 1f)
            + Mathf.Abs(color.b - 1f);

        return distanceFromWhite > 0.05f;
    }

    // 두 Renderer Color가 거의 같은지 확인합니다.
    private bool IsSimilarRendererColor(Color firstColor, Color secondColor)
    {
        // 색상 차이 = |R차이| + |G차이| + |B차이|
        // 빨강(1,0,0)과 파랑(0,0,1)의 차이 = 1 + 0 + 1 = 2 (다른 색)
        // 같은 파랑끼리의 차이 = 0 (같은 색)
        float colorDistance = Mathf.Abs(firstColor.r - secondColor.r)
            + Mathf.Abs(firstColor.g - secondColor.g)
            + Mathf.Abs(firstColor.b - secondColor.b);

        // 0.05보다 작으면 거의 같은 색으로 봅니다.
        return colorDistance < 0.05f;
    }

    private string GetBubbleColorName(GameObject bubbleObject)
    {
        SpriteRenderer renderer = bubbleObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            return "알 수 없음";
        }

        // 색칠된 버블이면 Sprite 이름보다 Renderer Color를 먼저 봅니다.
        if (IsTintColorUsed(renderer.color))
        {
            return GetColorNameFromRendererColor(renderer.color);
        }

        if (renderer.sprite != null)
        {
            string spriteName = renderer.sprite.name.ToLowerInvariant();
            if (spriteName.Contains("red"))
            {
                return "빨강";
            }

            if (spriteName.Contains("blue"))
            {
                return "파랑";
            }

            if (spriteName.Contains("yellow"))
            {
                return "노랑";
            }

            return renderer.sprite.name;
        }

        return GetColorNameFromRendererColor(renderer.color);
    }

    private string GetColorNameFromRendererColor(Color color)
    {
        if (color.r >= color.g && color.r >= color.b)
        {
            return "빨강";
        }

        if (color.b >= color.r && color.b >= color.g)
        {
            return "파랑";
        }

        return "노랑";
    }

    // 현재 씬에 있는 스테이지 버블들을 격자 칸에 다시 등록합니다.
    public void RefreshGridRegistration()
    {
        SyncGridWithStageLayout();

        // StageBubbleLayout에서 읽은 실제 위치/간격이 바뀌었을 수 있으므로,
        // 발사 직전에는 격자 칸 좌표를 반드시 다시 만듭니다.
        // 이걸 하지 않으면 실제 스테이지 버블 위치와 GridManager 칸 위치가 어긋나서 겹쳐 붙을 수 있습니다.
        BuildGrid();

        ClearSlotOccupancy();

        int registeredCount = RegisterSceneBubblesToNearestSlots();

        // 아직 실제 버블이 생성되기 전이면, 테스트용으로 처음 몇 줄을 occupied 처리합니다.
        if (registeredCount == 0)
        {
            ApplyInitialOccupiedRows();
        }
    }

    private void SyncGridWithStageLayout()
    {
        if (!autoSyncWithStageLayout)
        {
            return;
        }

        StageBubbleLayout stageLayout = FindFirstObjectByType<StageBubbleLayout>();
        if (stageLayout == null)
        {
            hasCachedPlayAreaBounds = false;
            return;
        }

        if (!stageLayout.TryGetPlayAreaWorldBounds(out float leftX, out float rightX, out float ceilingY))
        {
            hasCachedPlayAreaBounds = false;
            return;
        }

        cols = Mathf.Max(1, stageLayout.cols);
        useStaggeredRows = stageLayout.useStaggeredRows;
        cellSpacing = stageLayout.GetBubbleSpacing();

        float bubbleDiameter = stageLayout.GetBubbleDiameter();
        cachedLeftX = leftX;
        cachedRightX = rightX;
        cachedCeilingY = ceilingY;
        cachedBubbleDiameter = bubbleDiameter;
        hasCachedPlayAreaBounds = true;

        topLeftCellWorldPosition = new Vector3(
            leftX + cellSpacing / 2f,
            ceilingY - bubbleDiameter / 2f - stageLayout.startYOffset,
            transform.position.z
        );
    }

    private void ClearSlotOccupancy()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                slots[row, col].occupied = false;
                slots[row, col].bubbleObject = null;
            }
        }
    }

    private void ApplyInitialOccupiedRows()
    {
        for (int row = 0; row < Mathf.Min(initialOccupiedRows, rows); row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                slots[row, col].occupied = true;
            }
        }
    }

    private int RegisterSceneBubblesToNearestSlots()
    {
        int registeredCount = 0;
        Transform[] sceneTransforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);

        for (int i = 0; i < sceneTransforms.Length; i++)
        {
            Transform child = sceneTransforms[i];
            if (!child.name.StartsWith("Bubble_"))
            {
                continue;
            }

            if (TryFindNearestSlot(child.position, out BubbleSlot nearestSlot))
            {
                nearestSlot.occupied = true;
                nearestSlot.bubbleObject = child.gameObject;
                child.position = nearestSlot.worldPosition;
                registeredCount++;
            }
        }

        return registeredCount;
    }

    private bool TryFindNearestSlot(Vector3 worldPosition, out BubbleSlot nearestSlot)
    {
        nearestSlot = null;
        float nearestDistance = float.MaxValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                float distance = Vector3.Distance(worldPosition, slots[row, col].worldPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSlot = slots[row, col];
                }
            }
        }

        return nearestSlot != null;
    }

    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < GetColsInRow(row);
    }

    private int GetColsInRow(int row)
    {
        // 지그재그 줄은 오른쪽으로 반 칸 밀리므로 마지막 칸을 하나 줄여야 RightWall 밖으로 나가지 않습니다.
        if (useStaggeredRows && row % 2 == 1)
        {
            return Mathf.Max(1, cols - 1);
        }

        return cols;
    }

    private bool IsSlotInsidePlayArea(BubbleSlot slot)
    {
        if (!hasCachedPlayAreaBounds)
        {
            return true;
        }

        float radius = cachedBubbleDiameter / 2f;
        Vector3 position = slot.worldPosition;

        if (position.x - radius < cachedLeftX)
        {
            return false;
        }

        if (position.x + radius > cachedRightX)
        {
            return false;
        }

        if (position.y + radius > cachedCeilingY)
        {
            return false;
        }

        return true;
    }

    // ============================================================
    // [기능 39] 떠 있는 버블을 아래로 떨어뜨립니다.
    //
    // [왜 필요한가요?]
    // 같은 색 3개 이상이 제거되면, 그 옆에 붙어있던 버블이
    // 천장과 연결이 끊어져서 공중에 떠 있게 됩니다.
    // 이런 버블은 아래로 떨어뜨려야 게임이 깔끔해집니다.
    //
    // [실행 흐름]
    // 1. FindFloatingBubbles()로 떠 있는 버블 목록을 받습니다.
    // 2. 떠 있는 버블이 0개이면 아무것도 하지 않습니다.
    // 3. 떠 있는 버블이 있으면, 각 버블에 대해:
    //    a. BubbleSlot을 비웁니다 (occupied = false, bubbleObject = null).
    //    b. 버블 오브젝트를 아래로 부드럽게 이동시키는 Coroutine을 시작합니다.
    //    c. Coroutine이 끝나면 버블 오브젝트를 Destroy()합니다.
    // ============================================================
    private void DropFloatingBubbles()
    {
        // 떠 있는 버블 목록을 받습니다.
        System.Collections.Generic.List<BubbleSlot> floatingSlots = FindFloatingBubbles();

        // 떠 있는 버블이 0개이면 아무것도 하지 않습니다.
        if (floatingSlots.Count == 0)
        {
            return;
        }

        Debug.Log($"[기능 39] 떠 있는 버블 {floatingSlots.Count}개를 아래로 떨어뜨립니다.");

        // ============================================================
        // [기능 40] 떨어진 버블 개수를 이벤트로 알려줍니다.
        // 점수 기능이 이 이벤트를 구독해서 떨어진 버블 수만큼 점수를 올립니다.
        // ============================================================
        FloatingBubblesDropped?.Invoke(floatingSlots.Count);

        // 각 떠 있는 버블에 대해 떨어뜨리기를 실행합니다.
        for (int i = 0; i < floatingSlots.Count; i++)
        {
            BubbleSlot floatingSlot = floatingSlots[i];
            GameObject bubbleObject = floatingSlot.bubbleObject;

            // BubbleSlot을 먼저 비웁니다.
            // 비워야 다음 발사 버블이 이 칸에 다시 들어갈 수 있습니다.
            ClearFloatingBubbleSlot(floatingSlot);

            // 버블 오브젝트가 있으면 아래로 떨어뜨리는 Coroutine을 시작합니다.
            if (bubbleObject != null)
            {
                StartCoroutine(DropBubbleObject(bubbleObject));
            }
        }
    }

    // ============================================================
    // [기능 39 도우미] 떠 있는 버블의 BubbleSlot을 비웁니다.
    // ClearBubbleSlot()과 비슷하지만, 제거 효과는 보여주지 않습니다.
    // (떠 있는 버블은 이미 연결이 끊어진 상태이므로 효과가 필요 없습니다.)
    // ============================================================
    private void ClearFloatingBubbleSlot(BubbleSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        slot.occupied = false;
        slot.bubbleObject = null;
    }

    // ============================================================
    // [기능 39 도우미] 버블 오브젝트를 아래로 부드럽게 이동시키는 Coroutine입니다.
    //
    // [Coroutine이란?]
    // 여러 프레임에 걸쳐서 천천히 실행되는 함수입니다.
    // 한 프레임에 끝나지 않고, 시간에 따라 변화하는 효과를 만들 때 사용합니다.
    // 비유: "0.5초 동안 아래로 미끄러지듯이 내려가는 애니메이션"
    //
    // [실행 흐름]
    // 1. 버블의 현재 위치를 기억합니다.
    // 2. 매 프레임마다 dropFallSpeed만큼 아래로 이동합니다.
    // 3. dropDistance만큼 이동했으면 Coroutine을 끝냅니다.
    // 4. 버블 오브젝트를 Destroy()로 제거합니다.
    // ============================================================
    private System.Collections.IEnumerator DropBubbleObject(GameObject bubbleObject)
    {
        if (bubbleObject == null)
        {
            yield break;
        }

        // 버블의 시작 위치를 기억합니다.
        Vector3 startPosition = bubbleObject.transform.position;

        // 버블이 아래로 이동한 거리를 측정합니다.
        float movedDistance = 0f;

        // dropDistance만큼 이동할 때까지 반복합니다.
        while (movedDistance < dropDistance)
        {
            // 버블이 중간에 사라졌으면 Coroutine을 끝냅니다.
            if (bubbleObject == null)
            {
                yield break;
            }

            // 이번 프레임에 이동할 거리를 계산합니다.
            // Time.deltaTime은 "지난 프레임부터 지금까지 걸린 시간(초)"입니다.
            // 이렇게 하면 컴퓨터 속도에 관계없이 항상 같은 속도로 떨어집니다.
            float moveThisFrame = dropFallSpeed * Time.deltaTime;

            // 버블을 아래로 이동합니다.
            // Vector3.down은 (0, -1, 0)으로, 아래쪽 방향입니다.
            bubbleObject.transform.position += Vector3.down * moveThisFrame;

            // 이동한 거리를 누적합니다.
            movedDistance += moveThisFrame;

            // 다음 프레임까지 기다립니다.
            yield return null;
        }

        // 떨어뜨리기가 끝났으면 버블 오브젝트를 제거합니다.
        if (bubbleObject != null)
        {
            Destroy(bubbleObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugSlots)
        {
            return;
        }

        if (slots == null || slots.GetLength(0) != rows || slots.GetLength(1) != cols)
        {
            BuildGrid();
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < GetColsInRow(row); col++)
            {
                BubbleSlot slot = slots[row, col];
                Gizmos.color = slot.occupied ? Color.red : Color.green;
                Gizmos.DrawWireSphere(slot.worldPosition, cellSpacing * 0.12f);
            }
        }
    }
}
