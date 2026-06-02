using UnityEngine;

// ============================================================
// BubbleLauncherController는 "버블 발사"를 담당하는 스크립트입니다.
// 마우스 왼쪽 버튼을 클릭하면 현재 버블을 조준 방향으로 발사합니다.
// 발사된 버블은 벽에 닿으면 반사되고, 천장이나 스테이지 버블에 닿으면 멈춥니다.
// 이 스크립트는 ShooterRoot 오브젝트에 붙여서 사용합니다.
// ============================================================
public class BubbleLauncherController : MonoBehaviour
{
    // ---- Inspector에서 설정할 수 있는 변수들 ----

    [Header("발사 속도 설정")]
    [Tooltip("버블이 날아가는 속도입니다. 숫자가 클수록 빠르게 날아갑니다.")]
    [SerializeField] private float launchSpeed = 10f;

    [Header("반사 설정")]
    [Tooltip("버블이 벽에 닿았을 때 튕기는 정도입니다. 1이면 완전 반사, 0이면 안 튕깁니다.")]
    [SerializeField] private float bounciness = 1f;

    [Tooltip("발사된 버블이 스테이지 버블과 충돌할 때 사용할 Layer입니다.")]
    [SerializeField] private LayerMask stageBubbleLayerMask = -1;

    [Header("디버그 설정")]
    [Tooltip("체크하면 마우스 클릭 대신 Space 키로도 발사할 수 있습니다.")]
    [SerializeField] private bool useSpaceKeyToLaunch = false;

    // ---- 스크립트 연결용 변수들 ----

    // BubbleCurrentController는 "현재 발사할 버블"을 보여주는 스크립트입니다.
    private BubbleCurrentController currentController;

    // BubbleSwapController는 "다음 버블을 현재 버블로 바꾸는 로직"을 담당하는 스크립트입니다.
    private BubbleSwapController swapController;

    // ShooterAimController는 "슈터 조준 방향"을 담당하는 스크립트입니다.
    private ShooterAimController aimController;

    // ShooterAimLineController는 화면에 보이는 조준선을 담당하는 스크립트입니다.
    // 발사 방향을 조준선과 똑같이 맞추기 위해 사용합니다.
    private ShooterAimLineController aimLineController;

    // StageBubbleLayout은 "스테이지 버블 배치"를 담당하는 스크립트입니다.
    // 멈춘 버블을 스테이지 버블 자식으로 넣을 때 사용합니다.
    private StageBubbleLayout stageBubbleLayout;

    // ---- 상태 변수들 ----

    // 지금 발사된 버블이 날아가는 중인지 확인하는 변수입니다.
    // 한 번에 하나만 발사해야 하기 때문에 사용합니다.
    private bool isLaunching = false;

    // 발사 후 다시 발사할 수 있을 때까지 기다리는 시간입니다.
    private float launchCooldown = 0.05f;

    // 마지막으로 발사한 시간을 기록합니다.
    private float lastLaunchTime = -1f;

    // ============================================================
    // Awake는 Start보다 먼저 한 번 호출됩니다.
    // 여기서는 필요한 스크립트 연결을 준비합니다.
    // ============================================================
    private void Awake()
    {
        // ShooterRoot에 붙어 있는 스크립트들을 찾습니다.
        currentController = GetComponent<BubbleCurrentController>();
        swapController = GetComponent<BubbleSwapController>();

        // ShooterAimController는 ShooterRoot나 다른 오브젝트에 붙어 있을 수 있습니다.
        aimController = GetComponentInChildren<ShooterAimController>();
        if (aimController == null)
        {
            aimController = FindFirstObjectByType<ShooterAimController>();
        }

        aimLineController = GetComponentInChildren<ShooterAimLineController>();
        if (aimLineController == null)
        {
            aimLineController = FindFirstObjectByType<ShooterAimLineController>();
        }

        // StageBubbleLayout은 WallsRoot에 붙어 있습니다.
        stageBubbleLayout = FindFirstObjectByType<StageBubbleLayout>();
    }

    // ============================================================
    // Update는 매 프레임 호출됩니다.
    // 여기서는 마우스 입력을 확인해서 버블을 발사합니다.
    // ============================================================
    private void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했는지 확인합니다.
        bool mouseClicked = Input.GetMouseButtonDown(0);

        // Space 키로도 발사할 수 있는 옵션이 켜져 있으면 확인합니다.
        bool spacePressed = useSpaceKeyToLaunch && Input.GetKeyDown(KeyCode.Space);

        // 마우스 클릭 또는 Space 키를 누르면 발사합니다.
        if (mouseClicked || spacePressed)
        {
            TryLaunchBubble();
        }
    }

    // ============================================================
    // 버블을 발사하려고 시도하는 함수입니다.
    // ============================================================
    private void TryLaunchBubble()
    {
        // 이미 발사된 버블이 날아가는 중이면 새 버블을 발사하지 않습니다.
        if (isLaunching)
        {
            return;
        }

        // 마지막 발사 후 쿨다운 시간이 지나지 않았으면 발사하지 않습니다.
        if (Time.time - lastLaunchTime < launchCooldown)
        {
            return;
        }

        // 현재 버블이 없으면 발사할 수 없습니다.
        if (currentController == null)
        {
            return;
        }

        // 조준 방향을 가져옵니다.
        Vector2 launchDirection = GetLaunchDirection();

        // 조준 방향이 너무 작으면 발사하지 않습니다.
        if (launchDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        // 버블을 발사합니다.
        LaunchBubble(launchDirection);
    }

    // ============================================================
    // 조준 방향을 가져오는 함수입니다.
    // ============================================================
    private Vector2 GetLaunchDirection()
    {
        // 조준선 스크립트가 있으면 조준선이 실제로 사용하는 방향을 그대로 가져옵니다.
        // 이렇게 해야 화면에 보이는 조준선과 발사 방향이 서로 반대로 어긋나지 않습니다.
        if (aimLineController != null)
        {
            Vector2 aimLineDirection = aimLineController.GetCurrentAimDirection();
            if (aimLineDirection.sqrMagnitude > 0.001f)
            {
                return KeepLaunchDirectionUpward(aimLineDirection);
            }
        }

        if (aimController != null)
        {
            Transform aimTransform = aimController.transform;
            Transform shooterVisual = aimTransform.Find("ShooterVisual");

            if (shooterVisual != null)
            {
                return KeepLaunchDirectionUpward(shooterVisual.up.normalized);
            }

            return KeepLaunchDirectionUpward(aimTransform.up.normalized);
        }

        return Vector2.up;
    }

    // ============================================================
    // 발사 방향이 아래쪽이나 뒤쪽으로 뒤집히지 않게 보정하는 함수입니다.
    //
    // [왜 필요한가?]
    // 조준선/슈터 회전 연결이 순간적으로 반대로 읽히면 버블이 뒤로 날아갈 수 있습니다.
    // 버블슈터에서는 항상 위쪽 경기장 안으로 발사해야 하므로, y가 음수면 방향을 뒤집습니다.
    // ============================================================
    private Vector2 KeepLaunchDirectionUpward(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            return Vector2.up;
        }

        direction = direction.normalized;

        // 아래쪽으로 읽혔다면 반대로 뒤집어서 위쪽으로 보냅니다.
        if (direction.y < 0f)
        {
            direction = -direction;
        }

        return direction.normalized;
    }

    // ============================================================
    // 실제 버블을 발사하는 함수입니다.
    // direction: 버블이 날아갈 방향
    // ============================================================
    private void LaunchBubble(Vector2 direction)
    {
        // 발사 중으로 표시합니다.
        isLaunching = true;
        lastLaunchTime = Time.time;

        // 현재 버블의 SpriteRenderer를 가져옵니다.
        Transform shooterVisual = transform.Find("ShooterVisual");
        Transform parentForBubble = shooterVisual != null ? shooterVisual : transform;
        Transform currentBubbleTransform = parentForBubble.Find("CurrentBubble");
        if (currentBubbleTransform == null)
        {
            isLaunching = false;
            return;
        }

        SpriteRenderer currentRenderer = currentBubbleTransform.GetComponent<SpriteRenderer>();
        if (currentRenderer == null || currentRenderer.sprite == null)
        {
            isLaunching = false;
            return;
        }

        // 발사할 버블 오브젝트를 새로 만듭니다.
        GameObject launchedBubble = new GameObject("LaunchedBubble");

        // 발사된 버블을 Default 레이어(0번)로 설정합니다.
        launchedBubble.layer = 0;

        // 슈터 위치에서 발사합니다.
        launchedBubble.transform.position = currentBubbleTransform.position;

        // 현재 버블의 Sprite와 색을 복사합니다.
        SpriteRenderer launchedRenderer = launchedBubble.AddComponent<SpriteRenderer>();
        launchedRenderer.sprite = currentRenderer.sprite;
        launchedRenderer.color = currentRenderer.color;
        launchedRenderer.sortingOrder = currentRenderer.sortingOrder;

        // 현재 버블의 크기를 복사합니다.
        launchedBubble.transform.localScale = currentBubbleTransform.localScale;

        // Rigidbody2D를 붙여서 물리적으로 움직이게 합니다.
        Rigidbody2D rb = launchedBubble.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // CircleCollider2D를 붙여서 충돌 감지를 합니다.
        CircleCollider2D circleCollider = launchedBubble.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;

        // Physics Material 2D를 만들어서 벽 반사를 설정합니다.
        PhysicsMaterial2D bounceMaterial = new PhysicsMaterial2D("BubbleBounce");
        bounceMaterial.bounciness = bounciness;
        bounceMaterial.friction = 0f;
        circleCollider.sharedMaterial = bounceMaterial;

        // 조준 방향으로 발사 속도를 적용합니다.
        rb.linearVelocity = direction.normalized * launchSpeed;

        // 충돌 감지를 위한 스크립트를 붙입니다.
        BubbleCollisionHandler collisionHandler = launchedBubble.AddComponent<BubbleCollisionHandler>();
        collisionHandler.Initialize(this, stageBubbleLayout);
    }

    // ============================================================
    // 발사된 버블이 멈출 때 호출되는 함수입니다.
    // BubbleCollisionHandler에서 호출합니다.
    // stoppedBubble: 멈춘 버블 오브젝트
    // ============================================================
    public void OnBubbleStopped(GameObject stoppedBubble)
    {
        // 발사 중 상태를 해제합니다.
        isLaunching = false;

        // 멈춘 버블도 다음 발사에서 다른 버블로 감지되어야 합니다.
        // Collider를 지우지 않고, 스테이지 버블과 같은 감지용 Trigger로 바꿉니다.
        CircleCollider2D collider = stoppedBubble.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
            collider.sharedMaterial = null;
        }

        // Bubble_로 시작해야 BubbleCollisionHandler가 스테이지 버블로 알아봅니다.
        stoppedBubble.name = "Bubble_Stopped";

        // Ignore Raycast 레이어(2번)로 바꿔서 조준선 Raycast에는 걸리지 않게 합니다.
        stoppedBubble.layer = 2;

        // 멈춘 뒤에는 더 이상 발사 충돌 감지 스크립트가 작동하면 안 됩니다.
        BubbleCollisionHandler collisionHandler = stoppedBubble.GetComponent<BubbleCollisionHandler>();
        if (collisionHandler != null)
        {
            Object.Destroy(collisionHandler);
        }

        // 멈춘 버블을 StageBubbleLayout의 자식으로 넣습니다.
        if (stageBubbleLayout != null)
        {
            stoppedBubble.transform.SetParent(stageBubbleLayout.transform);
        }

        // [기능 31] 같은 색 찾기
        // 버블이 붙은 뒤, 주변 6방향에 같은 색 버블이 있는지 확인합니다.
        FindSameColorBubbles(stoppedBubble);

        // [기능 32] 같은 색 3개 이상 찾기
        // 새 버블부터 시작해서 같은 색으로 이어진 버블 전체 개수를 셉니다.
        CheckThreeOrMoreSameColorBubbles(stoppedBubble);

        // BubbleSwapController에게 다음 버블을 현재 버블로 바꿔달라고 말합니다.
        // 색 검사를 먼저 끝낸 뒤 다음 버블로 교체합니다.
        if (swapController != null)
        {
            swapController.SwapBubbles();
        }
    }

    // ============================================================
    // [기능 31] 같은 색 찾기
    // 새로 붙은 버블 주변 6방향에 같은 색 버블이 있는지 확인합니다.
    //
    // [왜 필요한가?]
    // 버블슈터에서 같은 색 3개 이상이면 제거해야 합니다.
    // 이 함수는 그 중 "같은 색 찾기" 단계입니다.
    // 지금은 찾기만 하고, 제거는 기능 32~35번에서 만들 예정입니다.
    //
    // [실행 순서]
    // 1. 새로 붙은 버블의 색을 가져옵니다.
    // 2. StageBubbleLayout의 격자 간격으로 6방향 거리를 계산합니다.
    // 3. 각 방향에 Physics2D.OverlapCircleAll로 주변 버블을 찾습니다.
    // 4. 찾은 버블의 색과 비교해서 같은 색이면 리스트에 추가합니다.
    // 5. Debug.Log로 결과를 출력합니다.
    //
    // stoppedBubble: 새로 붙은 버블 오브젝트
    // ============================================================
    private void FindSameColorBubbles(GameObject stoppedBubble)
    {
        // 새로 붙은 버블의 SpriteRenderer를 가져옵니다.
        SpriteRenderer stoppedRenderer = stoppedBubble.GetComponent<SpriteRenderer>();
        if (stoppedRenderer == null)
        {
            return;
        }

        // 새로 붙은 버블의 "실제로 보이는 색"을 기억합니다.
        // Sprite 그림 색과 SpriteRenderer Color 색을 함께 계산해야 합니다.
        Color stoppedColor = GetVisibleBubbleColor(stoppedRenderer);

        // StageBubbleLayout이 없으면 격자 간격을 알 수 없습니다.
        if (stageBubbleLayout == null)
        {
            return;
        }

        // 같은 색 버블을 저장할 리스트입니다.
        // List<GameObject>는 "GameObject 목록"이라는 뜻입니다.
        System.Collections.Generic.List<GameObject> sameColorBubbles = new System.Collections.Generic.List<GameObject>();

        // 실제로 한 칸 거리 안에 있는 이웃 버블만 가져옵니다.
        // 이렇게 해야 멀리 있는 같은 색 버블을 잘못 세지 않습니다.
        System.Collections.Generic.List<GameObject> neighborBubbles = FindAdjacentBubbles(stoppedBubble);

        for (int i = 0; i < neighborBubbles.Count; i++)
        {
            GameObject hitObject = neighborBubbles[i];
            SpriteRenderer hitRenderer = hitObject.GetComponent<SpriteRenderer>();
            if (hitRenderer == null)
            {
                continue;
            }

            Color hitColor = GetVisibleBubbleColor(hitRenderer);
            if (IsSameColor(stoppedColor, hitColor))
            {
                sameColorBubbles.Add(hitObject);
            }
        }

        // 결과를 Debug.Log로 출력합니다.
        // Debug.Log는 Unity Console 창에 메시지를 보여주는 함수입니다.
        int sameColorCount = sameColorBubbles.Count;

        if (sameColorCount > 0)
        {
            // 같은 색 이름을 알아보기 쉽게 출력합니다.
            string colorName = GetColorName(stoppedColor);
            Debug.Log($"[기능 31] 같은 색 {colorName} {sameColorCount + 1}개 연결됨! (새 버블 포함)");
        }
        else
        {
            Debug.Log("[기능 31] 주변에 같은 색 버블이 없습니다.");
        }
    }

    // ============================================================
    // [기능 32] 같은 색 3개 이상 찾기
    // 새로 붙은 버블부터 시작해서, 같은 색으로 이어진 버블 전체를 찾습니다.
    //
    // [왜 필요한가?]
    // 버블슈터에서는 같은 색 버블이 3개 이상 연결되면 나중에 제거해야 합니다.
    // 이번 기능에서는 아직 제거하지 않고, "3개 이상인지 확인"만 합니다.
    //
    // [쉬운 비유]
    // 같은 색 친구 찾기라고 생각하면 됩니다.
    // 새 빨강 버블 옆에 빨강 친구가 있으면 그 친구를 찾고,
    // 그 친구 옆에 또 빨강 친구가 있으면 계속 따라가며 찾습니다.
    //
    // stoppedBubble: 새로 붙은 버블 오브젝트
    // ============================================================
    private void CheckThreeOrMoreSameColorBubbles(GameObject stoppedBubble)
    {
        // 같은 색으로 연결된 버블 목록을 찾습니다.
        System.Collections.Generic.List<GameObject> connectedBubbles = FindConnectedSameColorBubbles(stoppedBubble);

        // 새 버블 색 이름을 Console에 보기 좋게 출력하기 위해 가져옵니다.
        SpriteRenderer stoppedRenderer = stoppedBubble.GetComponent<SpriteRenderer>();
        Color stoppedColor = stoppedRenderer != null ? GetVisibleBubbleColor(stoppedRenderer) : Color.white;
        string colorName = GetColorName(stoppedColor);

        // Count는 리스트 안에 들어 있는 개수입니다.
        int connectedCount = connectedBubbles.Count;

        if (connectedCount >= 3)
        {
            Debug.Log($"[기능 32] 같은 색 3개 이상 발견! 색: {colorName}, 연결 개수: {connectedCount}개");
        }
        else
        {
            Debug.Log($"[기능 32] 같은 색 3개 미만입니다. 색: {colorName}, 연결 개수: {connectedCount}개");
        }
    }

    // ============================================================
    // 같은 색으로 연결된 버블 전체를 찾는 함수입니다.
    //
    // [초보자용 설명]
    // 1. 먼저 새 버블을 확인할 목록(toCheck)에 넣습니다.
    // 2. 확인할 목록에서 하나씩 꺼냅니다.
    // 3. 그 버블 주변 6방향에 같은 색 버블이 있는지 찾습니다.
    // 4. 새로 찾은 같은 색 버블도 확인할 목록에 넣습니다.
    // 5. 더 확인할 버블이 없을 때까지 반복합니다.
    //
    // startBubble: 시작 버블, 보통 새로 붙은 버블입니다.
    // return: 같은 색으로 연결된 모든 버블 목록입니다. 시작 버블도 포함합니다.
    // ============================================================
    private System.Collections.Generic.List<GameObject> FindConnectedSameColorBubbles(GameObject startBubble)
    {
        // 최종 결과 목록입니다. 같은 색으로 연결된 버블들이 여기에 들어갑니다.
        System.Collections.Generic.List<GameObject> connectedBubbles = new System.Collections.Generic.List<GameObject>();

        // 앞으로 확인해야 할 버블 목록입니다.
        System.Collections.Generic.List<GameObject> toCheckBubbles = new System.Collections.Generic.List<GameObject>();

        if (startBubble == null)
        {
            return connectedBubbles;
        }

        SpriteRenderer startRenderer = startBubble.GetComponent<SpriteRenderer>();
        if (startRenderer == null)
        {
            return connectedBubbles;
        }

        // 시작 버블의 실제 보이는 색을 기준 색으로 사용합니다.
        Color targetColor = GetVisibleBubbleColor(startRenderer);

        // 시작 버블도 연결된 버블 1개로 세야 합니다.
        connectedBubbles.Add(startBubble);
        toCheckBubbles.Add(startBubble);

        // 확인할 버블이 남아 있는 동안 계속 반복합니다.
        while (toCheckBubbles.Count > 0)
        {
            // 리스트의 첫 번째 버블을 꺼내서 확인합니다.
            GameObject currentBubble = toCheckBubbles[0];
            toCheckBubbles.RemoveAt(0);

            // 현재 버블 주변 6방향에서 같은 색 이웃을 찾습니다.
            System.Collections.Generic.List<GameObject> sameColorNeighbors = FindSameColorNeighbors(currentBubble, targetColor);

            for (int i = 0; i < sameColorNeighbors.Count; i++)
            {
                GameObject neighborBubble = sameColorNeighbors[i];

                // 이미 연결 목록에 들어간 버블이면 다시 넣지 않습니다.
                // 이렇게 해야 무한 반복을 막을 수 있습니다.
                if (connectedBubbles.Contains(neighborBubble))
                {
                    continue;
                }

                connectedBubbles.Add(neighborBubble);
                toCheckBubbles.Add(neighborBubble);
            }
        }

        return connectedBubbles;
    }

    // ============================================================
    // 한 버블 주변 6방향에서 같은 색 이웃 버블만 찾아주는 함수입니다.
    //
    // bubble: 중심이 되는 버블입니다.
    // targetColor: 찾아야 하는 색입니다.
    // return: 중심 버블 주변에 있는 같은 색 버블 목록입니다.
    // ============================================================
    private System.Collections.Generic.List<GameObject> FindSameColorNeighbors(GameObject bubble, Color targetColor)
    {
        System.Collections.Generic.List<GameObject> neighbors = new System.Collections.Generic.List<GameObject>();

        if (bubble == null || stageBubbleLayout == null)
        {
            return neighbors;
        }

        System.Collections.Generic.List<GameObject> adjacentBubbles = FindAdjacentBubbles(bubble);

        for (int i = 0; i < adjacentBubbles.Count; i++)
        {
            GameObject hitObject = adjacentBubbles[i];
            SpriteRenderer hitRenderer = hitObject.GetComponent<SpriteRenderer>();
            if (hitRenderer == null)
            {
                continue;
            }

            Color hitColor = GetVisibleBubbleColor(hitRenderer);
            if (IsSameColor(targetColor, hitColor))
            {
                neighbors.Add(hitObject);
            }
        }

        return neighbors;
    }

    // ============================================================
    // 중심 버블에 실제로 붙어 있는 이웃 버블만 찾는 함수입니다.
    //
    // [왜 필요한가?]
    // 예전 방식은 6방향 예상 위치마다 큰 원을 검사했습니다.
    // 그러면 실제로 붙어 있지 않은 근처 버블까지 잡혀서 3개 이상으로 잘못 셀 수 있습니다.
    // 이 함수는 중심 버블에서 "한 칸 거리" 안에 있는 버블만 이웃으로 인정합니다.
    // ============================================================
    private System.Collections.Generic.List<GameObject> FindAdjacentBubbles(GameObject centerBubble)
    {
        System.Collections.Generic.List<GameObject> adjacentBubbles = new System.Collections.Generic.List<GameObject>();

        if (centerBubble == null || stageBubbleLayout == null)
        {
            return adjacentBubbles;
        }

        float bubbleSpacing = stageBubbleLayout.GetBubbleSpacing();

        // 이웃 버블 중심 간 거리는 bubbleSpacing 정도입니다.
        // 1.15를 곱해서 아주 작은 위치 오차는 허용합니다.
        float neighborDistance = bubbleSpacing * 1.15f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(centerBubble.transform.position, neighborDistance, ~0);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            GameObject hitObject = hitColliders[i].gameObject;

            if (hitObject == centerBubble)
            {
                continue;
            }

            if (!hitObject.name.StartsWith("Bubble_"))
            {
                continue;
            }

            if (adjacentBubbles.Contains(hitObject))
            {
                continue;
            }

            float distance = Vector2.Distance(centerBubble.transform.position, hitObject.transform.position);

            // 너무 가까운 값은 겹침 오류일 수 있고, 너무 먼 값은 이웃이 아닙니다.
            if (distance > 0.01f && distance <= neighborDistance)
            {
                adjacentBubbles.Add(hitObject);
            }
        }

        return adjacentBubbles;
    }

    // ============================================================
    // SpriteRenderer가 화면에 보여주는 실제 색을 구하는 함수입니다.
    //
    // [왜 필요한가?]
    // Unity에서 버블 색은 두 가지 방식으로 보일 수 있습니다.
    // 1. Sprite 그림 자체가 빨강이고, SpriteRenderer Color는 흰색인 경우
    // 2. Sprite 그림은 흰색이고, SpriteRenderer Color가 빨강인 경우
    //
    // 화면에서는 둘 다 빨강으로 보이지만,
    // SpriteRenderer.color만 비교하면 1번은 흰색, 2번은 빨강이라서 다르게 판단됩니다.
    // 그래서 Sprite 그림 색과 SpriteRenderer Color를 곱해서 "실제로 보이는 색"을 만듭니다.
    // ============================================================
    private Color GetVisibleBubbleColor(SpriteRenderer targetRenderer)
    {
        if (targetRenderer == null)
        {
            return Color.white;
        }

        // SpriteRenderer.color는 그림에 입히는 색입니다.
        Color tintColor = targetRenderer.color;

        // Sprite 그림 자체의 가운데 색을 가져옵니다.
        Color spriteColor = GetSpriteCenterColor(targetRenderer.sprite);

        // 실제 화면 색 = Sprite 그림 색 * SpriteRenderer Color 색입니다.
        Color visibleColor = new Color(
            spriteColor.r * tintColor.r,
            spriteColor.g * tintColor.g,
            spriteColor.b * tintColor.b,
            1f
        );

        return visibleColor;
    }

    // ============================================================
    // Sprite 그림의 가운데 픽셀 색을 가져오는 함수입니다.
    // 가운데 픽셀은 버블 원의 중심이라서 빨강/파랑/노랑 색을 알기 좋습니다.
    //
    // 만약 PNG가 읽기 불가능한 설정이면 Unity가 오류를 낼 수 있습니다.
    // 그럴 때는 안전하게 흰색을 돌려줍니다.
    // ============================================================
    private Color GetSpriteCenterColor(Sprite targetSprite)
    {
        if (targetSprite == null || targetSprite.texture == null)
        {
            return Color.white;
        }

        try
        {
            Rect textureRect = targetSprite.textureRect;
            int centerX = Mathf.RoundToInt(textureRect.x + textureRect.width / 2f);
            int centerY = Mathf.RoundToInt(textureRect.y + textureRect.height / 2f);

            Color centerColor = targetSprite.texture.GetPixel(centerX, centerY);
            centerColor.a = 1f;
            return centerColor;
        }
        catch (UnityException)
        {
            // Texture가 Read/Write 불가능하면 여기로 옵니다.
            // 이 경우에는 SpriteRenderer.color만 사용하게 흰색을 돌려줍니다.
            return Color.white;
        }
    }

    // ============================================================
    // 두 색이 같은지 비교하는 함수입니다.
    //
    // [왜 필요한가?]
    // Color는 소수점으로 이루어져 있어서 완전히 같은지 비교하기 어렵습니다.
    // 그래서 아주 작은 차이(0.01)까지 "같다"고 판단하는 함수를 만듭니다.
    //
    // color1: 첫 번째 색
    // color2: 두 번째 색
    // return: 같으면 true, 다르면 false
    // ============================================================
    private bool IsSameColor(Color color1, Color color2)
    {
        // Mathf.Abs는 "절대값"을 구하는 함수입니다.
        // 두 색의 차이가 0.25보다 작으면 "같은 색"으로 판단합니다.
        // Sprite 그림색과 tint 색을 섞으면 약간의 차이가 생길 수 있어서 넉넉하게 잡습니다.
        float threshold = 0.25f;

        bool r = Mathf.Abs(color1.r - color2.r) < threshold;
        bool g = Mathf.Abs(color1.g - color2.g) < threshold;
        bool b = Mathf.Abs(color1.b - color2.b) < threshold;

        // 빨강, 초록, 파랑 모두 비슷하면 같은 색입니다.
        return r && g && b;
    }

    // ============================================================
    // Color를 이름으로 바꿔주는 함수입니다.
    // Debug.Log에서 "빨강", "파랑", "노랑"으로 보기 쉽게 출력합니다.
    //
    // color: 확인할 색
    // return: 색 이름 문자열
    // ============================================================
    private string GetColorName(Color color)
    {
        // 빨강: R이 크고 G, B가 작은 색
        if (color.r > 0.5f && color.g < 0.5f && color.b < 0.5f)
        {
            return "빨강";
        }

        // 파랑: B가 크고 R, G가 작은 색
        if (color.r < 0.5f && color.g < 0.5f && color.b > 0.5f)
        {
            return "파랑";
        }

        // 노랑: R, G가 크고 B가 작은 색
        if (color.r > 0.5f && color.g > 0.5f && color.b < 0.5f)
        {
            return "노랑";
        }

        // 그 외에는 색 이름 대신 RGB 값을 출력합니다.
        return $"R:{color.r:F2} G:{color.g:F2} B:{color.b:F2}";
    }
}

// ============================================================
// BubbleCollisionHandler는 발사된 버블의 충돌을 감지하는 보조 스크립트입니다.
// 발사된 버블에 자동으로 붙여서, 버블이 다른 버블이나 천장에 닿으면 멈추게 합니다.
//
// [이 스크립트가 하는 일 - 실행 순서]
// 1. Initialize()로 스크립트가 시작됩니다.
// 2. Update()에서 매 프레임 주변 Collider를 감지합니다.
// 3. 스테이지 버블에 닿으면 → FindBestGridPosition()으로 빈 격자 칸을 찾습니다.
// 4. 천장에 닿으면 → TryFindCeilingFallbackPosition()으로 기존 버블 근처 빈칸을 찾습니다.
// 5. 빈칸을 찾으면 → StopBubbleAtPosition()으로 버블을 멈춥니다.
// 6. 못 찾으면 → ClampPositionInsidePlayArea()로 벽/천장 안쪽에 멈춥니다.
// ============================================================
public class BubbleCollisionHandler : MonoBehaviour
{
    // ---- 연결용 변수들 ----

    // BubbleLauncherController의 OnBubbleStopped를 호출하기 위한 연결입니다.
    private BubbleLauncherController launcher;

    // StageBubbleLayout은 격자 간격을 계산하고, 멈춘 버블을 자식으로 넣을 때 사용합니다.
    private StageBubbleLayout stageBubbleLayout;

    // ---- 타이머 변수들 ----

    // 스폰 직후 충돌을 무시하기 위한 타이머입니다.
    // 발사 직후 버블이 슈터 근처 스테이지 버블에 바로 붙지 않게 합니다.
    private float spawnTime;
    private const float IgnoreCollisionAfterSpawn = 0.3f;

    // ---- 감지 관련 변수들 ----

    // 감지 반지름입니다. 버블 크기에 맞게 설정됩니다.
    private float detectionRadius = 0.3f;

    // 바로 전 프레임 위치입니다.
    private Vector3 previousPosition;

    // ---- 조준선 경로 추적 변수들 ----

    // 발사를 시작한 위치입니다.
    private Vector3 launchStartPosition;

    // 발사 방향입니다. (처음 발사할 때의 방향)
    private Vector3 launchDirection;

    // ============================================================
    // Initialize는 BubbleLauncherController에서 호출됩니다.
    // 발사된 버블에 이 스크립트를 붙일 때 한 번만 호출됩니다.
    // ============================================================
    public void Initialize(BubbleLauncherController launcherController, StageBubbleLayout layout)
    {
        launcher = launcherController;
        stageBubbleLayout = layout;
        spawnTime = Time.time;

        // 발사된 버블의 크기에 맞게 감지 반지름을 설정합니다.
        float localScale = transform.localScale.x;
        detectionRadius = localScale * 0.5f;

        // 발사 시작 위치를 기록합니다.
        previousPosition = transform.position;
        launchStartPosition = transform.position;

        // 발사 방향을 기록합니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            launchDirection = rb.linearVelocity.normalized;
        }
        else
        {
            launchDirection = Vector3.up;
        }

        // 처음에는 발사 경로 = 처음 발사 경로입니다.
        // (currentPathDirection과 hasReflected는 제거되었습니다)
    }

    // ============================================================
    // Update는 매 프레임 호출됩니다.
    // 여기서 주변 Collider를 감지해서 버블을 멈출지 결정합니다.
    //
    // [실행 순서]
    // 1. 스폰 직후(0.3초)에는 감지하지 않습니다.
    // 2. 벽에 튕긴 뒤 방향이 바뀌었는지 확인합니다.
    // 3. 주변 Collider를 모두 찾습니다.
    // 4. 스테이지 버블이 있으면 → 격자 위치에 붙입니다.
    // 5. 천장만 있으면 → 기존 버블 근처 빈칸에 붙입니다.
    // 6. 아무것도 없으면 → 다음 프레임을 기다립니다.
    // ============================================================
    private void Update()
    {
        // 스폰 직후에는 감지하지 않습니다.
        if (Time.time - spawnTime < IgnoreCollisionAfterSpawn)
        {
            return;
        }

        // 벽에 튕겨 이동 방향이 바뀌었는지 확인합니다.
        UpdateCurrentPathInfo();

        // 현재 버블 위치에서 감지 반지름 안에 있는 Collider2D를 모두 찾습니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, ~0);

        // 가장 가까운 스테이지 버블을 기억합니다.
        Collider2D nearestBubbleCollider = null;
        float nearestBubbleDistance = float.MaxValue;
        bool touchedCeiling = false;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Collider2D hitCollider = hitColliders[i];

            // 자기 자신은 제외합니다.
            if (hitCollider.gameObject == gameObject)
            {
                continue;
            }

            // 스테이지 버블에 닿았는지 확인합니다.
            // 스테이지 버블 이름은 "Bubble_"으로 시작합니다.
            if (hitCollider.gameObject.name.StartsWith("Bubble_"))
            {
                float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestBubbleDistance)
                {
                    nearestBubbleDistance = distance;
                    nearestBubbleCollider = hitCollider;
                }
            }

            // 천장에 닿았는지 확인합니다.
            if (hitCollider.gameObject.name == "Ceiling")
            {
                touchedCeiling = true;
            }
        }

        // [중요] 스테이지 버블에 붙는 것을 먼저 처리합니다.
        // 버블과 천장을 동시에 감지한 경우, 버블에 붙는 것이 우선입니다.
        // 그래야 윗줄 버블 근처에서 천장 쪽으로 잘못 붙는 일을 줄일 수 있습니다.
        if (nearestBubbleCollider != null)
        {
            HandleBubbleAttachment(nearestBubbleCollider.gameObject);
            return;
        }

        // 천장에만 닿았을 때의 처리입니다.
        if (touchedCeiling)
        {
            HandleCeilingAttachment();
            return;
        }

        // 아무것도 닿지 않았다면 현재 위치를 다음 프레임의 이전 위치로 저장합니다.
        previousPosition = transform.position;
    }

    // ============================================================
    // 벽에 튕겨 이동 방향이 바뀌었는지 확인합니다.
    // 이 함수는 더 이상 사용하지 않습니다.
    // 대신 GetAimedXAtY에서 previousPosition 기반으로 직접 계산합니다.
    // ============================================================
    private void UpdateCurrentPathInfo()
    {
        // 아무것도 하지 않습니다.
    }

    // ============================================================
    // 스테이지 버블에 닿았을 때 호출됩니다.
    // hitBubble: 닿은 스테이지 버블
    //
    // [하는 일]
    // 1. 버블의 속도를 0으로 만듭니다.
    // 2. 격자 간격을 계산합니다.
    // 3. 6방향 빈칸 후보를 만듭니다.
    // 4. 조준선 경로와 가장 가까운 빈칸을 고릅니다.
    // 5. 그 위치에 버블을 놓습니다.
    // ============================================================
    private void HandleBubbleAttachment(GameObject hitBubble)
    {
        // Rigidbody2D를 가져와서 속도를 0으로 만듭니다.
        StopBubblePhysics();

        if (hitBubble == null)
        {
            FinishBubbleStop();
            return;
        }

        StageBubbleLayout layout = hitBubble.GetComponentInParent<StageBubbleLayout>();
        if (layout == null)
        {
            FinishBubbleStop();
            return;
        }

        // 격자 간격을 가져옵니다.
        float bubbleDiameter = layout.GetBubbleDiameter();
        float bubbleSpacing = layout.GetBubbleSpacing();

        // 버블슈터 격자는 벌집 모양 6방향입니다.
        // 세로 간격 = 가로 간격 * 루트3 / 2
        float verticalSpacing = bubbleSpacing * Mathf.Sqrt(3f) / 2f;

        // 6방향 후보 위치를 만듭니다.
        Vector3[] neighborOffsets = CreateSixDirectionOffsets(bubbleSpacing, verticalSpacing);

        // 조준선 경로 기준으로 가장 가까운 빈칸을 찾습니다.
        Vector3 bestPosition = FindBestGridPosition(
            hitBubble.transform.position,
            neighborOffsets,
            bubbleDiameter,
            previousPosition
        );

        // 버블을 격자 위치에 놓습니다.
        transform.position = bestPosition;

        // BubbleLauncherController에게 버블이 멈췄다고 알립니다.
        FinishBubbleStop();
    }

    // ============================================================
    // 천장에 닿았을 때 호출됩니다.
    //
    // [하는 일]
    // 1. 기존 스테이지 버블이 있으면, 그 버블 근처 빈칸에 붙입니다.
    // 2. 기존 스테이지 버블이 없으면, 천장에 붙입니다.
    // ============================================================
    private void HandleCeilingAttachment()
    {
        // 기존 스테이지 버블 근처의 빈칸을 찾습니다.
        if (TryFindCeilingFallbackPosition())
        {
            return;
        }

        // 빈칸을 못 찾으면 안전한 위치에 멈춥니다.
        StopBubblePhysics();
        float bubbleDiameter = stageBubbleLayout != null ? stageBubbleLayout.GetBubbleDiameter() : transform.localScale.x;
        Vector3 safePosition = ClampPositionInsidePlayArea(transform.position, bubbleDiameter);
        transform.position = safePosition;
        FinishBubbleStop();
    }

    // ============================================================
    // 천장에 닿았을 때 기존 스테이지 버블 근처의 빈칸을 찾습니다.
    // return: 빈칸을 찾았으면 true, 못 찾았으면 false
    //
    // [하는 일]
    // 1. 모든 스테이지 버블을 확인합니다.
    // 2. 각 버블 주변 6방향 빈칸을 찾습니다.
    // 3. 위쪽 빈칸은 제외합니다. (새 버블이 위에 붙지 않게)
    // 4. 조준선 X 위치와 가장 가까운 빈칸을 고릅니다.
    // ============================================================
    private bool TryFindCeilingFallbackPosition()
    {
        if (stageBubbleLayout == null)
        {
            return false;
        }

        float bubbleDiameter = stageBubbleLayout.GetBubbleDiameter();
        float bubbleSpacing = stageBubbleLayout.GetBubbleSpacing();
        float verticalSpacing = bubbleSpacing * Mathf.Sqrt(3f) / 2f;
        Vector3[] neighborOffsets = CreateSixDirectionOffsets(bubbleSpacing, verticalSpacing);

        Collider2D[] stageColliders = stageBubbleLayout.GetComponentsInChildren<Collider2D>();

        Vector3 bestPosition = transform.position;
        float bestY = float.MinValue;
        float bestXDistance = float.MaxValue;
        bool foundPosition = false;

        for (int i = 0; i < stageColliders.Length; i++)
        {
            GameObject stageBubble = stageColliders[i].gameObject;

            // 스테이지 버블만 확인합니다.
            if (!stageBubble.name.StartsWith("Bubble_"))
            {
                continue;
            }

            for (int j = 0; j < neighborOffsets.Length; j++)
            {
                Vector3 candidatePosition = stageBubble.transform.position + neighborOffsets[j];

                // 새 버블이 위쪽에 붙지 않게, 기존 버블보다 위인 후보는 제외합니다.
                if (candidatePosition.y > stageBubble.transform.position.y + 0.01f)
                {
                    continue;
                }

                // 이미 버블이 있는 위치는 제외합니다.
                if (IsPositionOccupied(candidatePosition, bubbleDiameter))
                {
                    continue;
                }

                // 벽/천장 밖이면 제외합니다.
                if (!IsPositionInsidePlayArea(candidatePosition, bubbleDiameter))
                {
                    continue;
                }

                // 먼저 가장 위쪽 빈칸을 고릅니다.
                // 같은 높이의 빈칸끼리는 조준선 경로와 가장 가까운 칸을 고릅니다.
                float targetX = GetAimedXAtY(candidatePosition.y);
                float xDistance = Mathf.Abs(candidatePosition.x - targetX);

                if (!foundPosition
                    || candidatePosition.y > bestY + 0.01f
                    || (Mathf.Abs(candidatePosition.y - bestY) < 0.01f && xDistance < bestXDistance))
                {
                    foundPosition = true;
                    bestPosition = candidatePosition;
                    bestY = candidatePosition.y;
                    bestXDistance = xDistance;
                }
            }
        }

        if (!foundPosition)
        {
            // 빈칸을 못 찾으면 안전한 위치에 멈춥니다.
            StopBubblePhysics();
            Vector3 safePosition = ClampPositionInsidePlayArea(transform.position, bubbleDiameter);
            transform.position = safePosition;
            FinishBubbleStop();
            return true;
        }

        StopBubblePhysics();
        transform.position = bestPosition;
        FinishBubbleStop();
        return true;
    }

    // ============================================================
    // 버블슈터 벌집 모양의 6방향 후보를 만듭니다.
    // bubbleSpacing: 버블 사이의 가로 간격
    // verticalSpacing: 버블 사이의 세로 간격
    //
    // [6방향 설명]
    // 벌집 모양에서는 버블이 6방향으로 붙습니다:
    //   ↗ ↗   (왼쪽 위, 오른쪽 위)
    // ← ● →   (왼쪽, 오른쪽)
    //   ↘ ↘   (왼쪽 아래, 오른쪽 아래)
    // ============================================================
    private Vector3[] CreateSixDirectionOffsets(float bubbleSpacing, float verticalSpacing)
    {
        return new[]
        {
            new Vector3(-bubbleSpacing, 0f, 0f),                    // 왼쪽
            new Vector3(bubbleSpacing, 0f, 0f),                     // 오른쪽
            new Vector3(-bubbleSpacing / 2f, verticalSpacing, 0f),  // 왼쪽 위
            new Vector3(bubbleSpacing / 2f, verticalSpacing, 0f),   // 오른쪽 위
            new Vector3(-bubbleSpacing / 2f, -verticalSpacing, 0f), // 왼쪽 아래
            new Vector3(bubbleSpacing / 2f, -verticalSpacing, 0f)   // 오른쪽 아래
        };
    }

    // ============================================================
    // 6방향 후보 중에서 조준선 경로와 가장 가까운 빈칸을 찾습니다.
    // hitBubblePosition: 닿은 스테이지 버블의 위치
    // neighborOffsets: 6방향 후보 위치 오프셋
    // bubbleDiameter: 버블 크기
    // referencePosition: 버블이 이전 프레임에 있던 위치
    //
    // [실행 순서]
    // 1. 6방향 후보를 하나씩 확인합니다.
    // 2. 위쪽 후보는 제외합니다. (겹침 방지)
    // 3. 이미 버블이 있는 후보는 제외합니다.
    // 4. 벽/천장 밖 후보는 제외합니다.
    // 5. 남은 후보 중 조준선 경로와 가장 가까운 칸을 고릅니다.
    // 6. 주변이 모두 차 있으면, 스테이지 전체에서 빈칸을 찾습니다.
    // ============================================================
    private Vector3 FindBestGridPosition(
        Vector3 hitBubblePosition,
        Vector3[] neighborOffsets,
        float bubbleDiameter,
        Vector3 referencePosition)
    {
        Vector3 bestPosition = transform.position;
        float bestScore = float.MaxValue;
        bool foundAny = false;

        for (int i = 0; i < neighborOffsets.Length; i++)
        {
            Vector3 candidatePosition = hitBubblePosition + neighborOffsets[i];

            // 발사 버블은 아래에서 위로 올라옵니다.
            // 맞은 버블보다 위쪽인 후보는 제외합니다.
            if (candidatePosition.y > hitBubblePosition.y + 0.01f)
            {
                continue;
            }

            // 이미 버블이 있는 위치는 제외합니다.
            if (IsPositionOccupied(candidatePosition, bubbleDiameter))
            {
                continue;
            }

            // 벽/천장 밖이면 제외합니다.
            if (!IsPositionInsidePlayArea(candidatePosition, bubbleDiameter))
            {
                continue;
            }

            // 후보 칸 높이에서 조준선이 지나가는 X 위치를 계산합니다.
            float targetX = GetAimedXAtY(candidatePosition.y);
            float xDistance = Mathf.Abs(candidatePosition.x - targetX);

            // 점수를 계산합니다. X 거리가 가까울수록 좋습니다.
            float score = xDistance;

            if (score < bestScore)
            {
                bestScore = score;
                bestPosition = candidatePosition;
                foundAny = true;
            }
        }

        // 빈칸을 찾았으면 반환합니다.
        if (foundAny)
        {
            return bestPosition;
        }

        // 맞은 버블 주변이 모두 차 있으면, 전체 스테이지에서 빈칸을 찾습니다.
        if (TryFindAnyEmptyStagePosition(referencePosition, bubbleDiameter, out Vector3 fallbackPosition))
        {
            return fallbackPosition;
        }

        // 정말로 빈칸이 없으면 현재 위치를 벽 안쪽으로 밀어 넣습니다.
        return ClampPositionInsidePlayArea(bestPosition, bubbleDiameter);
    }

    // ============================================================
    // 스테이지 전체에서 빈 격자 칸을 찾습니다.
    // referencePosition: 기준 위치 (가까운 칸을 고르기 위해)
    // bubbleDiameter: 버블 크기
    // bestPosition: 찾은 빈칸 위치 (결과)
    // return: 빈칸을 찾았으면 true
    //
    // [우선순위]
    // 1. 위쪽 줄을 먼저 고릅니다. (아래로 쌓이지 않게)
    // 2. 같은 높이면 조준선 X 위치와 가까운 칸을 고릅니다.
    // ============================================================
    private bool TryFindAnyEmptyStagePosition(Vector3 referencePosition, float bubbleDiameter, out Vector3 bestPosition)
    {
        bestPosition = transform.position;

        if (stageBubbleLayout == null)
        {
            return false;
        }

        float bubbleSpacing = stageBubbleLayout.GetBubbleSpacing();
        float verticalSpacing = bubbleSpacing * Mathf.Sqrt(3f) / 2f;
        Vector3[] neighborOffsets = CreateSixDirectionOffsets(bubbleSpacing, verticalSpacing);
        Collider2D[] stageColliders = stageBubbleLayout.GetComponentsInChildren<Collider2D>();

        float bestY = float.MinValue;
        float bestXDistance = float.MaxValue;
        bool foundPosition = false;

        for (int i = 0; i < stageColliders.Length; i++)
        {
            GameObject stageBubble = stageColliders[i].gameObject;

            if (!stageBubble.name.StartsWith("Bubble_"))
            {
                continue;
            }

            for (int j = 0; j < neighborOffsets.Length; j++)
            {
                Vector3 candidatePosition = stageBubble.transform.position + neighborOffsets[j];

                // 위쪽 후보는 제외합니다.
                if (candidatePosition.y > stageBubble.transform.position.y + 0.01f)
                {
                    continue;
                }

                // 이미 버블이 있는 위치는 제외합니다.
                if (IsPositionOccupied(candidatePosition, bubbleDiameter))
                {
                    continue;
                }

                // 벽/천장 밖이면 제외합니다.
                if (!IsPositionInsidePlayArea(candidatePosition, bubbleDiameter))
                {
                    continue;
                }

                // 위쪽 줄을 먼저 고릅니다.
                float targetX = GetAimedXAtY(candidatePosition.y);
                float xDistance = Mathf.Abs(candidatePosition.x - targetX);

                if (!foundPosition
                    || candidatePosition.y > bestY + 0.01f
                    || (Mathf.Abs(candidatePosition.y - bestY) < 0.01f && xDistance < bestXDistance))
                {
                    foundPosition = true;
                    bestY = candidatePosition.y;
                    bestXDistance = xDistance;
                    bestPosition = candidatePosition;
                }
            }
        }

        return foundPosition;
    }

    // ============================================================
    // 후보 위치 근처에 이미 다른 버블이 있으면 true를 돌려줍니다.
    // candidatePosition: 확인할 위치
    // bubbleDiameter: 버블 크기 (감지 반지름 계산에 사용)
    // ============================================================
    private bool IsPositionOccupied(Vector3 candidatePosition, float bubbleDiameter)
    {
        // 버블 크기의 35%를 감지 반지름으로 사용합니다.
        float checkRadius = bubbleDiameter * 0.35f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(candidatePosition, checkRadius, ~0);

        for (int i = 0; i < colliders.Length; i++)
        {
            GameObject otherObject = colliders[i].gameObject;

            // 자기 자신은 제외합니다.
            if (otherObject == gameObject)
            {
                continue;
            }

            // Bubble_로 시작하는 이름이면 스테이지 버블입니다.
            if (otherObject.name.StartsWith("Bubble_"))
            {
                return true;
            }
        }

        return false;
    }

    // ============================================================
    // 후보 위치가 LeftWall, RightWall, Ceiling 안쪽인지 확인합니다.
    // candidatePosition: 확인할 위치
    // bubbleDiameter: 버블 크기 (반지름 계산에 사용)
    // ============================================================
    private bool IsPositionInsidePlayArea(Vector3 candidatePosition, float bubbleDiameter)
    {
        if (stageBubbleLayout == null)
        {
            return true;
        }

        if (!stageBubbleLayout.TryGetPlayAreaWorldBounds(out float leftX, out float rightX, out float ceilingY))
        {
            return true;
        }

        float bubbleRadius = bubbleDiameter / 2f;

        // 왼쪽 벽을 넘어가면 안 됩니다.
        if (candidatePosition.x - bubbleRadius < leftX)
        {
            return false;
        }

        // 오른쪽 벽을 넘어가면 안 됩니다.
        if (candidatePosition.x + bubbleRadius > rightX)
        {
            return false;
        }

        // 천장을 넘어가면 안 됩니다.
        if (candidatePosition.y + bubbleRadius > ceilingY)
        {
            return false;
        }

        return true;
    }

    // ============================================================
    // 특정 높이(y)에서 가장 가까운 빈 격자 칸의 x 위치를 반환합니다.
    // targetY: 확인할 높이
    //
    // [방법]
    // 버블의 현재 X 위치를 그대로 반환합니다.
    // FindBestGridPosition에서 이미 6방향 후보를 검사하므로,
    // 여기서는 단순히 "현재 위치의 X"를 기준으로 씁니다.
    // ============================================================
    private float GetAimedXAtY(float targetY)
    {
        return transform.position.x;
    }

    // ============================================================
    // 버블 위치가 벽/천장 밖으로 나가면 안쪽으로 밀어 넣습니다.
    // targetPosition: 원래 위치
    // bubbleDiameter: 버블 크기
    // ============================================================
    private Vector3 ClampPositionInsidePlayArea(Vector3 targetPosition, float bubbleDiameter)
    {
        if (stageBubbleLayout == null)
        {
            return targetPosition;
        }

        if (!stageBubbleLayout.TryGetPlayAreaWorldBounds(out float leftX, out float rightX, out float ceilingY))
        {
            return targetPosition;
        }

        float bubbleRadius = bubbleDiameter / 2f;

        // X 위치를 벽 안쪽으로 제한합니다.
        targetPosition.x = Mathf.Clamp(targetPosition.x, leftX + bubbleRadius, rightX - bubbleRadius);

        // Y 위치를 천장 아래쪽으로 제한합니다.
        targetPosition.y = Mathf.Min(targetPosition.y, ceilingY - bubbleRadius);

        return targetPosition;
    }

    // ============================================================
    // 버블의 물리(Rigidbody2D)를 멈춥니다.
    // 속도를 0으로 만들고, Static으로 바꿔서 더 이상 움직이지 않게 합니다.
    // ============================================================
    private void StopBubblePhysics()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    // ============================================================
    // 버블이 멈춘 뒤 BubbleLauncherController에게 알립니다.
    // ============================================================
    private void FinishBubbleStop()
    {
        if (launcher != null)
        {
            launcher.OnBubbleStopped(gameObject);
        }
    }
}
