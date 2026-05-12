using UnityEngine;

// WallBoundsController는 화면 왼쪽, 오른쪽, 위쪽에 충돌 벽을 만들어주는 스크립트입니다.
// 나중에 조준선 반사, 버블 벽 반사, 천장 붙기 기능에서 사용할 준비용입니다.
public class WallBoundsController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("벽 위치를 계산할 기준 카메라입니다. 비워두면 Main Camera를 자동으로 찾습니다.")]
    [SerializeField] private Camera targetCamera;

    [Header("벽 크기 설정")]
    [Tooltip("벽 두께입니다. 값이 클수록 벽이 두꺼워집니다.")]
    [SerializeField] private float wallThickness = 0.5f;

    [Tooltip("왼쪽/오른쪽 벽을 화면보다 위아래로 얼마나 더 길게 만들지 정합니다.")]
    [SerializeField] private float extraHeight = 2f;

    [Header("자동 설정")]
    [Tooltip("체크되어 있으면 Play 시작 때 카메라 화면에 맞춰 벽 위치를 자동으로 다시 맞춥니다.")]
    [SerializeField] private bool autoSetupOnStart = true;

    [Header("디버그 표시")]
    [Tooltip("체크되어 있으면 Scene/Game 창에서 벽 위치를 반투명 색으로 볼 수 있습니다.")]
    [SerializeField] private bool showDebugVisuals = true;

    [Tooltip("디버그 벽 색입니다. 실제 게임에서는 꺼도 됩니다.")]
    [SerializeField] private Color debugColor = new Color(1f, 0f, 0f, 0.25f);

    private const string LeftWallName = "LeftWall";
    private const string RightWallName = "RightWall";
    private const string CeilingName = "Ceiling";

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // 자동 설정이 켜져 있으면 Play 시작 때 벽을 화면 크기에 맞춥니다.
        if (autoSetupOnStart)
        {
            SetupWalls();
        }
    }

    // Inspector 컴포넌트 메뉴에서 직접 실행할 수 있는 함수입니다.
    [ContextMenu("벽 다시 맞추기")]
    public void SetupWalls()
    {
        // 카메라를 찾습니다.
        FindTargetCameraIfNeeded();

        // 카메라가 없으면 벽 위치를 계산할 수 없습니다.
        if (targetCamera == null)
        {
            Debug.LogWarning("벽 위치를 계산할 카메라가 없습니다. Target Camera를 연결해주세요.");
            return;
        }

        // 2D에서는 Orthographic 카메라를 기준으로 화면 크기를 계산합니다.
        if (!targetCamera.orthographic)
        {
            Debug.LogWarning("Main Camera가 Orthographic이 아닙니다. 2D 프로젝트에서는 Orthographic 카메라를 추천합니다.");
        }

        // 값이 너무 작거나 음수가 되지 않게 안전하게 보정합니다.
        wallThickness = Mathf.Max(wallThickness, 0.01f);
        extraHeight = Mathf.Max(extraHeight, 0f);

        // 카메라가 보는 화면 높이와 너비를 계산합니다.
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // 카메라 중심 위치입니다.
        Vector3 cameraPosition = targetCamera.transform.position;

        // 화면 가장자리 위치를 계산합니다.
        float leftX = cameraPosition.x - cameraWidth * 0.5f;
        float rightX = cameraPosition.x + cameraWidth * 0.5f;
        float topY = cameraPosition.y + cameraHeight * 0.5f;
        float centerY = cameraPosition.y;

        // 벽 높이와 천장 너비를 정합니다.
        float wallHeight = cameraHeight + extraHeight;
        float ceilingWidth = cameraWidth + wallThickness * 2f;

        // 왼쪽 벽을 만들고 위치/크기를 맞춥니다.
        GameObject leftWall = FindOrCreateWall(LeftWallName);
        SetupWallTransform(leftWall, new Vector2(leftX - wallThickness * 0.5f, centerY), new Vector2(wallThickness, wallHeight));

        // 오른쪽 벽을 만들고 위치/크기를 맞춥니다.
        GameObject rightWall = FindOrCreateWall(RightWallName);
        SetupWallTransform(rightWall, new Vector2(rightX + wallThickness * 0.5f, centerY), new Vector2(wallThickness, wallHeight));

        // 천장을 만들고 위치/크기를 맞춥니다.
        GameObject ceiling = FindOrCreateWall(CeilingName);
        SetupWallTransform(ceiling, new Vector2(cameraPosition.x, topY + wallThickness * 0.5f), new Vector2(ceilingWidth, wallThickness));
    }

    // 카메라가 비어 있으면 Main Camera를 찾는 함수입니다.
    private void FindTargetCameraIfNeeded()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    // 이름으로 벽 오브젝트를 찾거나 새로 만드는 함수입니다.
    private GameObject FindOrCreateWall(string wallName)
    {
        Transform existingWall = transform.Find(wallName);

        if (existingWall != null)
        {
            return existingWall.gameObject;
        }

        GameObject wallObject = new GameObject(wallName);
        wallObject.transform.SetParent(transform);

        // 충돌을 위한 BoxCollider2D를 붙입니다.
        wallObject.AddComponent<BoxCollider2D>();

        // 디버그 표시를 위한 SpriteRenderer를 붙입니다.
        SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateDebugSprite();
        spriteRenderer.sortingOrder = 100;

        return wallObject;
    }

    // 벽 위치, 크기, Collider 크기를 맞추는 함수입니다.
    private void SetupWallTransform(GameObject wallObject, Vector2 position, Vector2 size)
    {
        wallObject.transform.localPosition = new Vector3(position.x, position.y, 0f);
        wallObject.transform.localRotation = Quaternion.identity;
        wallObject.transform.localScale = new Vector3(size.x, size.y, 1f);

        BoxCollider2D boxCollider = wallObject.GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            boxCollider = wallObject.AddComponent<BoxCollider2D>();
        }

        // Sprite 크기를 1x1로 만들었으므로 Collider도 1x1로 두고 Transform Scale로 크기를 조절합니다.
        boxCollider.size = Vector2.one;
        boxCollider.offset = Vector2.zero;

        SpriteRenderer spriteRenderer = wallObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDebugSprite();
        }

        spriteRenderer.color = debugColor;
        spriteRenderer.enabled = showDebugVisuals;
    }

    // 디버그용 1x1 흰색 Sprite를 만드는 함수입니다.
    private Sprite CreateDebugSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
