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

        // 멈춘 버블의 Rigidbody2D를 제거합니다.
        Rigidbody2D rb = stoppedBubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Object.Destroy(rb);
        }

        // 멈춘 버블의 Collider를 제거합니다.
        // 나중에 매칭/제거 기능에서 다시 설정할 수 있습니다.
        CircleCollider2D collider = stoppedBubble.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }

        // 멈춘 버블을 StageBubbleLayout의 자식으로 넣습니다.
        // 이렇게 하면 나중에 매칭/제거 기능에서 스테이지 버블과 함께 처리할 수 있습니다.
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
// OnCollisionEnter2D는 "2D 물리 충돌이 시작될 때" Unity가 자동으로 호출합니다.
public class BubbleCollisionHandler : MonoBehaviour
{
    // BubbleLauncherController의 OnBubbleStopped를 호출하기 위한 연결입니다.
    private BubbleLauncherController launcher;

    // StageBubbleLayout은 멈춘 버블을 자식으로 넣을 때 사용합니다.
    private StageBubbleLayout stageBubbleLayout;

    // 초기화 함수입니다. BubbleLauncherController에서 호출합니다.
    public void Initialize(BubbleLauncherController launcherController, StageBubbleLayout layout)
    {
        launcher = launcherController;
        stageBubbleLayout = layout;
    }

    // OnCollisionEnter2D는 2D 물리 충돌이 시작될 때 Unity가 자동으로 호출합니다.
    // Collision2D는 "충돌 정보"입니다.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 이름을 가져옵니다.
        string collidedName = collision.gameObject.name;

        // 천장에 닿았는지 확인합니다.
        if (collidedName == "Ceiling")
        {
            StopBubble();
            return;
        }

        // 스테이지 버블에 닿았는지 확인합니다.
        // 스테이지 버블의 이름은 "Bubble_0_0", "Bubble_0_1" 같은 형식입니다.
        if (collidedName.StartsWith("Bubble_"))
        {
            StopBubble();
            return;
        }
    }

    // 버블을 멈추는 함수입니다.
    private void StopBubble()
    {
        // Rigidbody2D를 가져와서 속도를 0으로 만듭니다.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // BubbleLauncherController에게 버블이 멈췄다고 알립니다.
        if (launcher != null)
        {
            launcher.OnBubbleStopped(gameObject);
        }
    }
}
