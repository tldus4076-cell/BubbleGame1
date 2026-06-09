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
    // dotted line이 실제로 도달하는 "occupied 근처 지점"에서 가장 가까운 빈칸을 선택합니다.
    // 이렇게 해야 제거로 생긴 빈칸 때문에 dotted line이 가리키는 위치가 무시되는 문제를 막을 수 있습니다.
    public bool TryFindTargetSlotOnAimPath(Vector3[] aimLinePoints, out BubbleSlot targetSlot)
    {
        targetSlot = null;

        if (aimLinePoints == null || aimLinePoints.Length < 2)
        {
            return false;
        }

        RefreshGridRegistration();

        float maxDistanceFromLine = cellSpacing * 0.55f;

        // dotted line이 실제로 도달하는 "occupied 근처 지점"을 찾습니다.
        // dotted line을 끝점부터 시작점 방향으로 되짚어 올라가면서,
        // 가장 가까운 occupied 칸이 있는 지점을 찾습니다.
        // 이 지점이 버블이 실제로 붙어야 하는 위치입니다.
        Vector3 aimEndPoint = FindAimLandingPoint(aimLinePoints);

        float bestTotalDistance = float.MaxValue;

        // 모든 빈칸을 확인해서 aimEndPoint에 가장 가까운 유효한 빈칸을 찾습니다.
        for (int row = 0; row < rows; row++)
        {
            float rowY = GetCellWorldPosition(row, 0).y;

            if (!TryGetAimedXOnPathAtY(aimLinePoints, rowY, out float aimedX))
            {
                continue;
            }

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

                float distanceFromLine = Mathf.Abs(slot.worldPosition.x - aimedX);
                if (distanceFromLine > maxDistanceFromLine)
                {
                    continue;
                }

                float distanceToLanding = Vector2.Distance(slot.worldPosition, aimEndPoint);
                if (distanceToLanding < bestTotalDistance)
                {
                    bestTotalDistance = distanceToLanding;
                    targetSlot = slot;
                }
            }
        }

        if (targetSlot != null)
        {
            Debug.Log($"[GridTarget] target cell: row {targetSlot.row}, col {targetSlot.col}, 위치: {targetSlot.worldPosition}, 도착점: {aimEndPoint}");
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

        for (int i = 0; i < aimLinePoints.Length - 1; i++)
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
