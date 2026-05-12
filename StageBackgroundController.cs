using UnityEngine;

// StageBackgroundController는 스테이지 배경 이미지를 화면에 꽉 차게 보여주는 스크립트입니다.
// 이 스크립트는 빈 GameObject에 붙이고, 배경 이미지는 Inspector에서 연결합니다.
public class StageBackgroundController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("배경을 맞출 기준 카메라입니다. 비워두면 Main Camera를 자동으로 찾습니다.")]
    [SerializeField] private Camera targetCamera;

    [Header("Stage 1 배경 설정")]
    [Tooltip("Stage 1에서 사용할 배경 Sprite를 여기에 넣어주세요.")]
    [SerializeField] private Sprite stage1BackgroundSprite;

    [Header("배경 오브젝트 설정")]
    [Tooltip("배경을 보여줄 SpriteRenderer입니다. 비워두면 자동으로 만들어줍니다.")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Tooltip("배경이 다른 오브젝트보다 뒤에 보이도록 하는 순서입니다. 숫자가 작을수록 뒤에 보입니다.")]
    [SerializeField] private int backgroundOrderInLayer = -100;

    [Tooltip("카메라보다 살짝 앞쪽에 배경을 놓기 위한 Z 위치입니다. 2D에서는 보통 0보다 큰 값을 사용합니다.")]
    [SerializeField] private float backgroundZPosition = 10f;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    // 필요한 오브젝트를 미리 준비할 때 사용합니다.
    private void Awake()
    {
        // 카메라가 Inspector에 연결되지 않았다면 Main Camera를 자동으로 찾습니다.
        FindTargetCameraIfNeeded();

        // 배경을 보여줄 SpriteRenderer가 없다면 자동으로 만듭니다.
        CreateBackgroundRendererIfNeeded();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기에서 Stage 1 배경을 실제로 화면에 적용합니다.
    private void Start()
    {
        // 이번 기능은 Stage 1 배경만 사용합니다.
        ApplyStage1Background();
    }

    // LateUpdate는 매 프레임의 마지막에 호출됩니다.
    // 카메라 위치가 움직여도 배경이 항상 카메라 뒤에 고정되어 보이게 합니다.
    private void LateUpdate()
    {
        // 카메라와 배경이 준비되어 있을 때만 위치와 크기를 다시 맞춥니다.
        if (targetCamera != null && backgroundRenderer != null)
        {
            // 카메라가 보는 위치에 배경을 따라오게 합니다.
            MoveBackgroundToCamera();

            // 화면 크기에 맞춰 배경 크기를 다시 맞춥니다.
            ResizeBackgroundToCameraView();
        }
    }

    // Inspector에서 카메라를 넣지 않았을 때 자동으로 Main Camera를 찾는 함수입니다.
    private void FindTargetCameraIfNeeded()
    {
        // targetCamera가 비어 있으면 자동 찾기를 시도합니다.
        if (targetCamera == null)
        {
            // Camera.main은 태그가 MainCamera인 카메라를 찾습니다.
            targetCamera = Camera.main;
        }
    }

    // 배경 SpriteRenderer가 없을 때 자동으로 만들어주는 함수입니다.
    private void CreateBackgroundRendererIfNeeded()
    {
        // backgroundRenderer가 이미 연결되어 있으면 새로 만들 필요가 없습니다.
        if (backgroundRenderer != null)
        {
            return;
        }

        // 배경 전용 GameObject를 새로 만듭니다.
        GameObject backgroundObject = new GameObject("Stage 1 Background");

        // 이 스크립트가 붙은 오브젝트의 자식으로 넣어서 Hierarchy를 깔끔하게 만듭니다.
        backgroundObject.transform.SetParent(transform);

        // 새 오브젝트에 SpriteRenderer를 붙입니다.
        backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
    }

    // Stage 1 배경 Sprite를 SpriteRenderer에 넣는 함수입니다.
    private void ApplyStage1Background()
    {
        // 배경 이미지가 연결되지 않았다면 오류를 보여주고 멈춥니다.
        if (stage1BackgroundSprite == null)
        {
            Debug.LogError("Stage 1 배경 Sprite가 비어 있습니다. Inspector에서 Stage 1 배경 이미지를 연결해주세요.");
            return;
        }

        // SpriteRenderer가 없다면 오류를 보여주고 멈춥니다.
        if (backgroundRenderer == null)
        {
            Debug.LogError("배경 SpriteRenderer가 없습니다. Background Renderer를 연결하거나 자동 생성되게 해주세요.");
            return;
        }

        // SpriteRenderer에 Stage 1 배경 이미지를 넣습니다.
        backgroundRenderer.sprite = stage1BackgroundSprite;

        // 배경이 다른 Sprite보다 뒤에 보이도록 순서를 낮게 설정합니다.
        backgroundRenderer.sortingOrder = backgroundOrderInLayer;

        // 배경 위치와 크기를 카메라에 맞춥니다.
        MoveBackgroundToCamera();
        ResizeBackgroundToCameraView();
    }

    // 배경을 카메라가 보는 위치에 맞추는 함수입니다.
    private void MoveBackgroundToCamera()
    {
        // 카메라가 없으면 위치를 맞출 수 없으므로 멈춥니다.
        if (targetCamera == null)
        {
            return;
        }

        // 카메라의 X, Y 위치를 가져옵니다.
        Vector3 cameraPosition = targetCamera.transform.position;

        // 2D 배경은 카메라 중앙에 놓고, Z 위치만 따로 지정합니다.
        backgroundRenderer.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, backgroundZPosition);
    }

    // 배경 이미지를 카메라 화면 크기에 맞게 키우는 함수입니다.
    private void ResizeBackgroundToCameraView()
    {
        // 카메라나 배경 이미지가 없으면 크기를 계산할 수 없으므로 멈춥니다.
        if (targetCamera == null || backgroundRenderer == null || backgroundRenderer.sprite == null)
        {
            return;
        }

        // Orthographic Camera는 2D에서 많이 쓰는 카메라입니다.
        // orthographicSize는 카메라 화면 높이의 절반 크기입니다.
        float cameraHeight = targetCamera.orthographicSize * 2f;

        // 화면 너비는 화면 높이에 카메라 비율(aspect)을 곱해서 구합니다.
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // Sprite의 실제 월드 크기를 가져옵니다.
        float spriteWidth = backgroundRenderer.sprite.bounds.size.x;
        float spriteHeight = backgroundRenderer.sprite.bounds.size.y;

        // 0으로 나누면 오류가 나기 때문에 크기가 0이면 멈춥니다.
        if (spriteWidth <= 0f || spriteHeight <= 0f)
        {
            return;
        }

        // 카메라 너비에 맞추려면 Sprite를 몇 배 키워야 하는지 계산합니다.
        float widthScale = cameraWidth / spriteWidth;

        // 카메라 높이에 맞추려면 Sprite를 몇 배 키워야 하는지 계산합니다.
        float heightScale = cameraHeight / spriteHeight;

        // 둘 중 더 큰 값을 사용해야 화면에 빈 공간이 생기지 않습니다.
        float finalScale = Mathf.Max(widthScale, heightScale);

        // 계산한 크기를 배경에 적용합니다.
        backgroundRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
