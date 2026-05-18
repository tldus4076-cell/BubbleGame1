using UnityEngine;

// BubbleLauncherController는 "버블 발사"를 담당하는 스크립트입니다.
// 마우스 왼쪽 버튼을 클릭하면 현재 버블을 조준 방향으로 발사합니다.
// 발사된 버블은 벽에 닿으면 반사되고, 천장이나 스테이지 버블에 닿으면 멈춥니다.
// 이 스크립트는 ShooterRoot 오브젝트에 붙여서 사용합니다.
public class BubbleLauncherController : MonoBehaviour
{
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

    // BubbleCurrentController는 "현재 발사할 버블"을 보여주는 스크립트입니다.
    private BubbleCurrentController currentController;

    // BubbleSwapController는 "다음 버블을 현재 버블로 바꾸는 로직"을 담당하는 스크립트입니다.
    private BubbleSwapController swapController;

    // ShooterAimController는 "슈터 조준 방향"을 담당하는 스크립트입니다.
    private ShooterAimController aimController;

    // StageBubbleLayout은 "스테이지 버블 배치"를 담당하는 스크립트입니다.
    // 멈춘 버블을 스테이지 버블 자식으로 넣을 때 사용합니다.
    private StageBubbleLayout stageBubbleLayout;

    // 지금 발사된 버블이 날아가는 중인지 확인하는 변수입니다.
    // 한 번에 하나만 발사해야 하기 때문에 사용합니다.
    private bool isLaunching = false;

    // 발사 후 다시 발사할 수 있을 때까지 기다리는 시간입니다.
    // 이렇게 해야 버블이 멈춘 직후에 바로 새 버블이 발사되지 않습니다.
    private float launchCooldown = 0.2f;

    // 마지막으로 발사한 시간을 기록합니다.
    private float lastLaunchTime = -1f;

    // Awake는 Start보다 먼저 한 번 호출됩니다.
    // 여기서는 필요한 스크립트 연결을 준비합니다.
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

        // StageBubbleLayout은 WallsRoot에 붙어 있습니다.
        stageBubbleLayout = FindFirstObjectByType<StageBubbleLayout>();
    }

    // Update는 매 프레임 호출됩니다.
    // 여기서는 마우스 입력을 확인해서 버블을 발사합니다.
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

    // 버블을 발사하려고 시도하는 함수입니다.
    private void TryLaunchBubble()
    {
        // 이미 발사된 버블이 날아가는 중이면 새 버블을 발사하지 않습니다.
        // 이렇게 해야 한 번에 하나만 발사됩니다.
        if (isLaunching)
        {
            return;
        }

        // 마지막 발사 후 쿨다운 시간이 지나지 않았으면 발사하지 않습니다.
        // 이렇게 해야 버블이 멈춘 직후에 바로 새 버블이 발사되지 않습니다.
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

    // 조준 방향을 가져오는 함수입니다.
    private Vector2 GetLaunchDirection()
    {
        // ShooterAimController가 있으면 조준 방향을 가져옵니다.
        if (aimController != null)
        {
            // ShooterVisual의 회전 방향을 기준으로 앞쪽 방향을 구합니다.
            Transform aimTransform = aimController.transform;
            Transform shooterVisual = aimTransform.Find("ShooterVisual");

            if (shooterVisual != null)
            {
                // ShooterVisual의 위쪽 방향이 조준 방향입니다.
                return shooterVisual.up.normalized;
            }

            // ShooterVisual이 없으면 aimTransform의 위쪽 방향을 사용합니다.
            return aimTransform.up.normalized;
        }

        // ShooterAimController가 없으면 위쪽으로 발사합니다.
        return Vector2.up;
    }

    // 실제 버블을 발사하는 함수입니다.
    private void LaunchBubble(Vector2 direction)
    {
        // 발사 중으로 표시합니다.
        isLaunching = true;

        // 발사 시간을 기록합니다.
        lastLaunchTime = Time.time;

        // 현재 버블의 SpriteRenderer를 가져옵니다.
        // CurrentBubble은 ShooterVisual의 자식이므로, ShooterVisual 아래에서 찾습니다.
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
        // 이 오브젝트는 ShooterRoot와 완전히 별개입니다.
        GameObject launchedBubble = new GameObject("LaunchedBubble");

        // 발사된 버블을 Default 레이어(0번)로 설정합니다.
        // 이렇게 해야 스테이지 버블(레이어 2번)과 충돌 감지가 됩니다.
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

        // 중력을 0으로 설정합니다. 버블은 중력에 떨어지지 않습니다.
        rb.gravityScale = 0f;

        // 마찰력을 0으로 설정합니다. 버블이 느려지지 않게 합니다.
        rb.linearDamping = 0f;

        // 회전을 막습니다. 버블이 회전하지 않게 합니다.
        rb.freezeRotation = true;

        // 연속 충돌 감지를 설정합니다.
        // 이렇게 해야 빠른 속도로 날아가는 버블이 스테이지 버블을 통과하지 않습니다.
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // CircleCollider2D를 붙여서 충돌 감지를 합니다.
        CircleCollider2D circleCollider = launchedBubble.AddComponent<CircleCollider2D>();

        // Collider 크기를 Sprite에 맞게 자동으로 조절합니다.
        if (launchedRenderer.sprite != null)
        {
            float spriteWidth = launchedRenderer.sprite.bounds.size.x;
            float localScale = launchedBubble.transform.localScale.x;
            circleCollider.radius = (spriteWidth / 2f) * localScale;
        }

        // Physics Material 2D를 만들어서 벽 반사를 설정합니다.
        PhysicsMaterial2D bounceMaterial = new PhysicsMaterial2D("BubbleBounce");
        bounceMaterial.bounciness = bounciness;
        bounceMaterial.friction = 0f;
        circleCollider.sharedMaterial = bounceMaterial;

        // 조준 방향으로 발사 속도를 적용합니다.
        // 이 속도는 새로 만든 launchedBubble에만 적용됩니다.
        rb.linearVelocity = direction.normalized * launchSpeed;

        // 충돌 감지를 위한 스크립트를 붙입니다.
        BubbleCollisionHandler collisionHandler = launchedBubble.AddComponent<BubbleCollisionHandler>();
        collisionHandler.Initialize(this, stageBubbleLayout);
    }

    // 발사된 버블이 멈출 때 호출되는 함수입니다.
    // BubbleCollisionHandler에서 호출합니다.
    public void OnBubbleStopped(GameObject stoppedBubble)
    {
        // 발사 중 상태를 해제합니다.
        isLaunching = false;

        // 쿨다운 시간을 다시 기록합니다.
        lastLaunchTime = Time.time;

        // 멈춘 버블의 Collider를 제거합니다.
        CircleCollider2D collider = stoppedBubble.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }

        // 멈춘 버블을 StageBubbleLayout의 자식으로 넣습니다.
        if (stageBubbleLayout != null)
        {
            stoppedBubble.transform.SetParent(stageBubbleLayout.transform);
        }

        // BubbleSwapController에게 다음 버블을 현재 버블로 바꿔달라고 말합니다.
        if (swapController != null)
        {
            swapController.SwapBubbles();
        }
    }
}

// BubbleCollisionHandler는 발사된 버블의 충돌을 감지하는 보조 스크립트입니다.
public class BubbleCollisionHandler : MonoBehaviour
{
    // BubbleLauncherController의 OnBubbleStopped를 호출하기 위한 연결입니다.
    private BubbleLauncherController launcher;

    // StageBubbleLayout은 멈춘 버블을 자식으로 넣을 때 사용합니다.
    private StageBubbleLayout stageBubbleLayout;

    // 스폰 직후 충돌을 무시하기 위한 타이머입니다.
    private float spawnTime;
    private const float IgnoreCollisionAfterSpawn = 0.3f;

    // 감지 반지름입니다. 버블 크기와 비슷하게 설정합니다.
    private float detectionRadius = 0.3f;

    // 초기화 함수입니다.
    public void Initialize(BubbleLauncherController launcherController, StageBubbleLayout layout)
    {
        launcher = launcherController;
        stageBubbleLayout = layout;
        spawnTime = Time.time;

        // 발사된 버블의 크기에 맞게 감지 반지름을 설정합니다.
        float localScale = transform.localScale.x;
        detectionRadius = localScale * 0.5f;
    }

    // Update는 매 프레임 호출됩니다.
    // 여기서 Physics2D.OverlapCircle로 스테이지 버블을 감지합니다.
    private void Update()
    {
        // 스폰 직후에는 감지하지 않습니다.
        if (Time.time - spawnTime < IgnoreCollisionAfterSpawn)
        {
            return;
        }

        // 현재 버블 위치에서 감지 반지름 안에 있는 Collider2D를 찾습니다.
        // ~0은 "모든 레이어를 감지한다"는 뜻입니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, ~0);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Collider2D hitCollider = hitColliders[i];

            // 스테이지 버블에 닿았는지 확인합니다.
            if (hitCollider.gameObject.name.StartsWith("Bubble_"))
            {
                StopBubble(hitCollider.gameObject);
                return;
            }

            // 천장에 닿았는지 확인합니다.
            if (hitCollider.gameObject.name == "Ceiling")
            {
                StopBubble(null);
                return;
            }
        }
    }

    // 버블을 멈추는 함수입니다.
    private void StopBubble(GameObject hitBubble)
    {
        // Rigidbody2D를 가져와서 속도를 0으로 만듭니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 충돌한 스테이지 버블이 있으면 아래쪽에 배치합니다.
        if (hitBubble != null)
        {
            StageBubbleLayout layout = hitBubble.GetComponentInParent<StageBubbleLayout>();
            if (layout != null)
            {
                float bubbleDiameter = layout.GetBubbleDiameter();
                Vector3 snapPosition = hitBubble.transform.position;
                snapPosition.y -= bubbleDiameter;
                transform.position = snapPosition;
            }
        }

        // BubbleLauncherController에게 버블이 멈췄다고 알립니다.
        if (launcher != null)
        {
            launcher.OnBubbleStopped(gameObject);
        }
    }
}
