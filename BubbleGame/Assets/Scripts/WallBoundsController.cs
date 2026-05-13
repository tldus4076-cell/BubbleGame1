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

    [Header("디버그 표시")]
    [Tooltip("체크되어 있으면 Scene/Game 창에서 벽 위치를 반투명 색으로 볼 수 있습니다.")]
    [SerializeField] private bool showDebugVisuals = true;

    [Tooltip("디버그 벽 색입니다. 실제 게임에서는 꺼도 됩니다.")]
    [SerializeField] private Color debugColor = new Color(1f, 0f, 0f, 0.25f);

    // Inspector 컴포넌트 메뉴에서 직접 실행할 수 있는 함수입니다.
    // 컴포넌트 오른쪽 메뉴를 누르거나 컴포넌트 이름에서 오른쪽 클릭하면 나옵니다.
    [ContextMenu("벽 다시 맞추기")]
    public void SetupWalls()
    {
        FindTargetCameraIfNeeded();

        if (targetCamera == null)
        {
            Debug.LogWarning("벽 위치를 계산할 카메라가 없습니다. Target Camera를 연결해주세요.");
            return;
        }

        if (!targetCamera.orthographic)
        {
            Debug.LogWarning("Main Camera가 Orthographic이 아닙니다. 2D 프로젝트에서는 Orthographic 카메라를 추천합니다.");
        }

        wallThickness = Mathf.Max(wallThickness, 0.01f);
        extraHeight = Mathf.Max(extraHeight, 0f);

        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        Vector3 cameraPosition = targetCamera.transform.position;

        float leftX = cameraPosition.x - cameraWidth * 0.5f;
        float rightX = cameraPosition.x + cameraWidth * 0.5f;
        float topY = cameraPosition.y + cameraHeight * 0.5f;
        float centerY = cameraPosition.y;

        float wallHeight = cameraHeight + extraHeight;
        float ceilingWidth = cameraWidth + wallThickness * 2f;

        GameObject leftWall = FindOrCreateWall("LeftWall");
        SetupWallTransform(leftWall, new Vector2(leftX - wallThickness * 0.5f, centerY), new Vector2(wallThickness, wallHeight));

        GameObject rightWall = FindOrCreateWall("RightWall");
        SetupWallTransform(rightWall, new Vector2(rightX + wallThickness * 0.5f, centerY), new Vector2(wallThickness, wallHeight));

        GameObject ceiling = FindOrCreateWall("Ceiling");
        SetupWallTransform(ceiling, new Vector2(cameraPosition.x, topY + wallThickness * 0.5f), new Vector2(ceilingWidth, wallThickness));
    }

    private void FindTargetCameraIfNeeded()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private GameObject FindOrCreateWall(string wallName)
    {
        Transform existingWall = transform.Find(wallName);
        if (existingWall != null) return existingWall.gameObject;

        GameObject wallObject = new GameObject(wallName);
        wallObject.transform.SetParent(transform);
        wallObject.AddComponent<BoxCollider2D>();

        SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateDebugSprite();
        spriteRenderer.sortingOrder = 100;

        return wallObject;
    }

    private void SetupWallTransform(GameObject wallObject, Vector2 position, Vector2 size)
    {
        wallObject.transform.localPosition = new Vector3(position.x, position.y, 0f);
        wallObject.transform.localRotation = Quaternion.identity;
        wallObject.transform.localScale = new Vector3(size.x, size.y, 1f);

        BoxCollider2D boxCollider = wallObject.GetComponent<BoxCollider2D>();
        if (boxCollider == null) boxCollider = wallObject.AddComponent<BoxCollider2D>();

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

    private Sprite CreateDebugSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
