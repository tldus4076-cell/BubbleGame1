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

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 필요한 SpriteRenderer를 준비합니다.
        PrepareShooter();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // 슈터 이미지를 적용합니다.
        ApplyShooterVisual();
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
}
