using UnityEngine;

// ShooterController는 슈터 이미지를 화면에 보여주고 정렬 순서를 관리하는 스크립트입니다.
// 슈터 위치는 사용자가 Scene 창에서 직접 조절합니다.
// 조준, 회전, 발사는 나중 기능에서 만듭니다.
public class ShooterController : MonoBehaviour
{
    [Header("슈터 이미지 설정")]
    [Tooltip("슈터로 사용할 Sprite입니다. 비워두면 임시 흰색 사각형 Sprite를 사용합니다.")]
    [SerializeField] private Sprite shooterSprite;

    [Tooltip("ShooterVisual에 붙어 있는 SpriteRenderer입니다. 비워두면 자동으로 찾거나 만듭니다.")]
    [SerializeField] private SpriteRenderer shooterRenderer;

    [Header("정렬 설정")]
    [Tooltip("슈터가 배경보다 앞에 보이게 하는 정렬 순서입니다. 배경이 -100이면 슈터는 10 정도가 좋습니다.")]
    [SerializeField] private int sortingOrder = 10;

    [Header("격자 발사 설정")]
    [Tooltip("새 격자 방식으로 버블을 발사할지 정합니다. 기존 물리 발사 스크립트를 함께 쓰면 꺼두세요.")]
    [SerializeField] private bool useGridTargetShooting = false;

    [Tooltip("조준 방향을 알려주는 AimController입니다.")]
    [SerializeField] private AimController aimController;

    [Tooltip("버블 칸을 관리하는 BubbleGridManager입니다.")]
    [SerializeField] private BubbleGridManager gridManager;

    [Tooltip("버블이 출발하는 위치입니다. 비워두면 ShooterRoot 위치에서 발사합니다.")]
    [SerializeField] private Transform firePoint;

    [Tooltip("BubbleNextController가 없을 때 사용할 예비 색입니다. 보통은 흰색으로 둡니다.")]
    [SerializeField] private Color bubbleColor = Color.white;

    [Tooltip("버블이 target cell까지 이동하는 속도입니다.")]
    [SerializeField] private float projectileMoveSpeed = 10f;

    [Tooltip("발사된 버블의 크기입니다. GridManager의 Cell Spacing과 맞추면 좋습니다.")]
    [SerializeField] private float projectileScale = 0.45f;

    [Header("현재/다음 버블 표시 설정")]
    [Tooltip("체크하면 슈터 중앙 동그라미에 현재 발사될 버블을 보여줍니다.")]
    [SerializeField] private bool showBubblePreviews = true;

    [Tooltip("다음 버블을 보여주는 BubbleNextController입니다. 연결하면 작은 다음 버블이 실제 다음 발사 색이 됩니다.")]
    [SerializeField] private BubbleNextController nextBubbleController;

    [Tooltip("현재 버블 표시 위치입니다. ShooterVisual 중심 기준으로 X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 currentBubblePreviewOffset = new Vector2(0f, 0f);

    [Tooltip("현재 버블 표시 크기입니다.")]
    [SerializeField] private float currentBubblePreviewScale = 0.25f;

    private Sprite currentProjectileSprite;
    private SpriteRenderer currentBubblePreviewRenderer;
    private BubbleNextController subscribedNextBubbleController;

    private void OnEnable()
    {
        SubscribeToNextBubbleEvent();
    }

    private void OnDisable()
    {
        UnsubscribeFromNextBubbleEvent();
    }

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 필요한 SpriteRenderer를 준비합니다.
        PrepareShooter();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        SubscribeToNextBubbleEvent();

        // 슈터 이미지를 적용합니다.
        ApplyShooterVisual();
        PrepareBubblePreviews();
        PrepareProjectileQueue();
        ApplyBubblePreviews();
    }

    private void Update()
    {
        if (!useGridTargetShooting)
        {
            return;
        }

        // BubbleNextController와 아직 동기화 안 됐으면 매 프레임 시도합니다.
        if (currentProjectileSprite == null)
        {
            PrepareProjectileQueue();
        }

        if (Input.GetMouseButtonDown(0))
        {
            FireBubbleToTargetCell();
        }

        ApplyBubblePreviews();
    }

    // 슈터에 필요한 기본 준비를 하는 함수입니다.
    private void PrepareShooter()
    {
        // SpriteRenderer를 찾거나 만듭니다.
        FindOrCreateShooterRenderer();

        // 슈터 이미지를 적용합니다.
        ApplyShooterVisual();
    }

    // ShooterVisual과 SpriteRenderer를 찾거나 만드는 함수입니다.
    private void FindOrCreateShooterRenderer()
    {
        // 이미 연결되어 있으면 새로 만들 필요가 없습니다.
        if (shooterRenderer != null)
        {
            return;
        }

        // 자식에서 SpriteRenderer를 먼저 찾아봅니다.
        shooterRenderer = GetComponentInChildren<SpriteRenderer>();

        if (shooterRenderer != null)
        {
            return;
        }

        // 없으면 ShooterVisual 자식 오브젝트를 만듭니다.
        GameObject visualObject = new GameObject("ShooterVisual");
        visualObject.transform.SetParent(transform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localRotation = Quaternion.identity;
        visualObject.transform.localScale = Vector3.one;

        // ShooterVisual에 SpriteRenderer를 붙입니다.
        shooterRenderer = visualObject.AddComponent<SpriteRenderer>();
    }

    // 슈터 Sprite와 정렬 순서를 적용하는 함수입니다.
    private void ApplyShooterVisual()
    {
        if (shooterRenderer == null)
        {
            return;
        }

        // Sprite가 연결되어 있으면 그 Sprite를 사용합니다.
        if (shooterSprite != null)
        {
            shooterRenderer.sprite = shooterSprite;
        }
        else
        {
            // Sprite가 없으면 임시 흰색 사각형 Sprite를 만들어 사용합니다.
            shooterRenderer.sprite = CreateTemporaryShooterSprite();
            Debug.LogWarning("Shooter Sprite가 비어 있어서 임시 흰색 사각형 Sprite를 사용합니다. 나중에 Inspector에서 실제 슈터 Sprite를 연결해주세요.");
        }

        // 슈터가 배경보다 앞에 보이도록 정렬 순서를 설정합니다.
        shooterRenderer.sortingOrder = sortingOrder;
    }

    // 임시 흰색 사각형 Sprite를 만드는 함수입니다.
    private Sprite CreateTemporaryShooterSprite()
    {
        // 작은 흰색 Texture를 만듭니다.
        Texture2D texture = new Texture2D(32, 32);

        // 모든 픽셀을 흰색으로 채웁니다.
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }

        // 픽셀 변경을 적용합니다.
        texture.Apply();

        // Texture를 Sprite로 바꿉니다.
        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        return Sprite.Create(texture, rect, pivot, 32f);
    }

    // ============================================================
    // 새 구조: 조준선이 먼저 target cell을 정하고, 버블은 그 칸으로 이동합니다.
    // ============================================================
    private void FireBubbleToTargetCell()
    {
        PrepareGridShootingReferences();
        PrepareProjectileQueue();

        if (aimController == null || gridManager == null)
        {
            Debug.LogWarning("AimController 또는 BubbleGridManager가 연결되지 않아서 발사할 수 없습니다.");
            return;
        }

        if (currentProjectileSprite == null)
        {
            Debug.LogWarning("다음 버블 Sprite가 아직 준비되지 않아서 발사할 수 없습니다. BubbleNextController의 Bubble Sprites를 확인해주세요.");
            return;
        }

        Transform safeFirePoint = firePoint != null ? firePoint : transform;
        Vector3[] aimLinePoints = aimController.GetCurrentAimLinePoints();

        bool foundTarget = gridManager.TryFindTargetSlotOnAimPath(aimLinePoints, out BubbleSlot targetSlot);
        if (!foundTarget)
        {
            Debug.Log("dotted line 경로에 들어갈 수 있는 빈 target cell을 찾지 못했습니다. dotted line이 빈칸을 정확히 지나가게 조준해주세요.");
            return;
        }

        GameObject projectileObject = CreateProjectileObject(safeFirePoint.position);
        BubbleProjectile projectile = projectileObject.GetComponent<BubbleProjectile>();
        if (projectile == null)
        {
            projectile = projectileObject.AddComponent<BubbleProjectile>();
        }

        projectile.LaunchToCell(gridManager, targetSlot, projectileMoveSpeed, aimLinePoints);

        MoveNextBubbleToCurrentBubble();
        ApplyBubblePreviews();
    }

    private void PrepareGridShootingReferences()
    {
        // 외부 참조는 Inspector의 [SerializeField] 칸에서 직접 연결합니다.
        // 이름으로 오브젝트를 찾지 않아야 색 동기화와 발사 연결이 안정적입니다.
        SubscribeToNextBubbleEvent();
    }

    private GameObject CreateProjectileObject(Vector3 startPosition)
    {
        GameObject projectileObject = new GameObject("GridBubbleProjectile");
        projectileObject.transform.position = startPosition;

        SpriteRenderer spriteRenderer = projectileObject.AddComponent<SpriteRenderer>();

        // 항상 currentProjectileSprite를 사용합니다.
        // BubbleNextController와 동기화된 색이므로 표시 색과 항상 같습니다.
        spriteRenderer.sprite = currentProjectileSprite;
        spriteRenderer.color = GetProjectileTintColor();
        spriteRenderer.sortingOrder = 50;

        projectileObject.transform.localScale = Vector3.one * projectileScale;
        return projectileObject;
    }

    private void PrepareBubblePreviews()
    {
        if (!showBubblePreviews)
        {
            return;
        }

        Transform previewAnchor = GetPreviewAnchor();

        // 예전에 FirePoint 옆에 만들었던 현재 버블 표시 오브젝트만 제거합니다.
        // NextBubble은 BubbleNextController가 관리하므로 여기서 지우면 안 됩니다.
        RemovePreviewObject("CurrentBubble", firePoint);

        if (currentBubblePreviewRenderer == null)
        {
            currentBubblePreviewRenderer = FindOrCreatePreviewRenderer("ShooterCenterBubblePreview", previewAnchor);
        }
    }

    private SpriteRenderer FindOrCreatePreviewRenderer(string objectName, Transform parent)
    {
        Transform foundTransform = parent.Find(objectName);
        if (foundTransform == null)
        {
            foundTransform = transform.Find(objectName);
        }

        if (foundTransform == null)
        {
            GameObject previewObject = new GameObject(objectName);
            previewObject.transform.SetParent(parent, false);
            foundTransform = previewObject.transform;
        }

        SpriteRenderer renderer = foundTransform.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = foundTransform.gameObject.AddComponent<SpriteRenderer>();
        }

        renderer.sortingOrder = 60;
        return renderer;
    }

    private void PrepareProjectileQueue()
    {
        // 현재 발사 버블 Sprite가 아직 없으면 BubbleNextController에서 가져옵니다.
        // BubbleNextController가 아직 Sprite를 고르지 않았으면 아무것도 하지 않습니다.
        // 다음 프레임 ApplyBubblePreviews에서 다시 시도합니다.
        if (currentProjectileSprite != null)
        {
            return;
        }

        if (nextBubbleController != null && nextBubbleController.GetSelectedNextBubbleSprite() != null)
        {
            // BubbleNextController가 보여주는 작은 버블을 현재 발사 버블로 가져옵니다.
            // 여기서는 새 버블을 다시 뽑지 않습니다.
            // 그래야 지금 화면에 보이는 작은 버블 색과 실제 발사 색이 같습니다.
            currentProjectileSprite = nextBubbleController.GetSelectedNextBubbleSprite();
        }
    }

    private void MoveNextBubbleToCurrentBubble()
    {
        // BubbleNextController가 보여주던 작은 버블을 다음 현재 버블로 사용합니다.
        // 이렇게 해야 화면에 보이는 "다음 버블"과 실제 다음 발사 색이 같아집니다.
        if (nextBubbleController != null && nextBubbleController.GetSelectedNextBubbleSprite() != null)
        {
            nextBubbleController.SelectNewNextBubble();
            currentProjectileSprite = nextBubbleController.GetSelectedNextBubbleSprite();
            return;
        }

        // BubbleNextController가 아직 준비 안 됐으면 기다립니다.
        // 자기만의 랜덤을 뽑지 않습니다. 그래야 색이 같습니다.
        currentProjectileSprite = null;
    }

    private void SubscribeToNextBubbleEvent()
    {
        if (subscribedNextBubbleController == nextBubbleController)
        {
            return;
        }

        UnsubscribeFromNextBubbleEvent();

        if (nextBubbleController == null)
        {
            return;
        }

        subscribedNextBubbleController = nextBubbleController;
        subscribedNextBubbleController.NextBubbleSpriteChanged += OnNextBubbleSpriteChanged;
    }

    private void UnsubscribeFromNextBubbleEvent()
    {
        if (subscribedNextBubbleController == null)
        {
            return;
        }

        subscribedNextBubbleController.NextBubbleSpriteChanged -= OnNextBubbleSpriteChanged;
        subscribedNextBubbleController = null;
    }

    private void OnNextBubbleSpriteChanged(Sprite nextSprite)
    {
        // 작은 버블 Sprite가 바뀌는 순간, 실제 발사 Sprite도 같은 것으로 맞춥니다.
        currentProjectileSprite = nextSprite;
    }

    private Color GetProjectileTintColor()
    {
        if (nextBubbleController != null)
        {
            return nextBubbleController.GetBubbleTintColor();
        }

        return bubbleColor;
    }

    private void ApplyBubblePreviews()
    {
        if (!showBubblePreviews)
        {
            SetPreviewEnabled(currentBubblePreviewRenderer, false);
            return;
        }

        PrepareGridShootingReferences();
        PrepareBubblePreviews();
        PrepareProjectileQueue();

        ApplyPreviewRenderer(currentBubblePreviewRenderer, currentProjectileSprite, currentBubblePreviewOffset, currentBubblePreviewScale);
    }

    private void ApplyPreviewRenderer(SpriteRenderer renderer, Sprite sprite, Vector2 offset, float scale)
    {
        if (renderer == null)
        {
            return;
        }

        Transform safeFirePoint = GetPreviewAnchor();
        renderer.enabled = true;
        renderer.sprite = sprite;
        renderer.color = GetProjectileTintColor();
        renderer.transform.position = safeFirePoint.position + new Vector3(offset.x, offset.y, 0f);
        renderer.transform.localScale = Vector3.one * scale;
    }

    private void SetPreviewEnabled(SpriteRenderer renderer, bool enabled)
    {
        if (renderer != null)
        {
            renderer.enabled = enabled;
        }
    }

    private void RemovePreviewObject(string objectName, Transform parent)
    {
        Transform foundTransform = parent != null ? parent.Find(objectName) : null;
        if (foundTransform == null)
        {
            foundTransform = transform.Find(objectName);
        }

        if (foundTransform == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(foundTransform.gameObject);
        }
        else
        {
            DestroyImmediate(foundTransform.gameObject);
        }
    }

    private Transform GetPreviewAnchor()
    {
        // 슈터 중앙 동그라미는 보통 ShooterVisual의 중심입니다.
        if (shooterRenderer != null)
        {
            return shooterRenderer.transform;
        }

        return transform;
    }
}
