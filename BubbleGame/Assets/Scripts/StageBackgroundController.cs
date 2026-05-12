using UnityEngine;

[ExecuteAlways]
// StageBackgroundController는 스테이지 배경 이미지를 화면에 꽉 차게 보여주는 스크립트입니다.
// ExecuteAlways는 Play를 누르지 않은 Scene 창에서도 코드가 동작하게 해줍니다.
// 이 스크립트는 빈 GameObject에 붙이고, 배경 이미지는 Inspector에서 연결합니다.
public class StageBackgroundController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("배경을 맞출 기준 카메라입니다. 비워두면 Main Camera를 자동으로 찾습니다.")]
    [SerializeField] private Camera targetCamera;

    [Header("Stage 1 배경 설정")]
    [Tooltip("Stage 1에서 사용할 배경 Sprite를 여기에 넣어주세요.")]
    [SerializeField] private Sprite stage1BackgroundSprite;

    [Header("Stage 2 배경 설정")]
    [Tooltip("Stage 2에서 사용할 배경 Sprite를 여기에 넣어주세요. 이번 기능에서는 연결만 준비하고 아직 화면에는 보여주지 않습니다.")]
    [SerializeField] private Sprite stage2BackgroundSprite;

    [Header("Stage 3 배경 설정")]
    [Tooltip("Stage 3에서 사용할 배경 Sprite를 여기에 넣어주세요. 이번 기능에서는 연결만 준비하고 아직 화면에는 보여주지 않습니다.")]
    [SerializeField] private Sprite stage3BackgroundSprite;

    [Header("현재 스테이지 테스트 설정")]
    [Tooltip("Scene 창과 Game 창에서 테스트할 현재 Stage 번호입니다. 슬라이더를 움직여 1, 2, 3 중 하나를 선택합니다.")]
    [Range(1, 3)]
    [SerializeField] private int currentStageNumber = 1;

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
        // 배경을 준비하고 화면에 맞춥니다.
        RefreshBackground();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기에서 Stage 1 배경을 실제로 화면에 적용합니다.
    private void Start()
    {
        // 배경을 준비하고 화면에 맞춥니다.
        RefreshBackground();
    }

    // OnEnable은 오브젝트나 스크립트가 켜질 때 호출됩니다.
    // Scene 창에서도 배경이 바로 보이게 다시 맞춥니다.
    private void OnEnable()
    {
        // 배경을 준비하고 화면에 맞춥니다.
        RefreshBackground();
    }

    // OnValidate는 Inspector 값이 바뀔 때 Unity Editor에서 호출됩니다.
    // 배경 이미지를 바꾸면 바로 Scene 창에 반영되게 합니다.
    private void OnValidate()
    {
        // Inspector에서 실수로 0, 4 같은 숫자를 넣어도 1~3 사이로 고쳐줍니다.
        currentStageNumber = Mathf.Clamp(currentStageNumber, 1, 3);

        // 배경을 준비하고 화면에 맞춥니다.
        RefreshBackground();
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

        // 이미 자식 오브젝트에 SpriteRenderer가 있다면 그것을 사용합니다.
        // 이렇게 하면 같은 배경 오브젝트가 여러 개 생기는 실수를 줄일 수 있습니다.
        backgroundRenderer = GetComponentInChildren<SpriteRenderer>();

        // 자식에서 SpriteRenderer를 찾았다면 새로 만들 필요가 없습니다.
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

    // 선택한 Stage 번호에 맞는 배경 Sprite를 SpriteRenderer에 넣는 함수입니다.
    public void SetStageBackground(int stageNumber)
    {
        // Inspector나 다른 스크립트에서 잘못된 숫자를 넣어도 1~3 사이로 고쳐줍니다.
        currentStageNumber = Mathf.Clamp(stageNumber, 1, 3);

        // 배경 이미지가 실제로 들어갈 SpriteRenderer가 없다면 오류를 보여주고 멈춥니다.
        if (backgroundRenderer == null)
        {
            Debug.LogError("배경 SpriteRenderer가 없습니다. Background Renderer를 연결하거나 자동 생성되게 해주세요.");
            return;
        }

        // 현재 Stage 번호에 맞는 Sprite를 고릅니다.
        Sprite selectedSprite = GetBackgroundSpriteByStage(currentStageNumber);

        // 선택된 Sprite가 없다면 Stage 1 배경을 대신 사용합니다.
        if (selectedSprite == null)
        {
            Debug.LogWarning("선택한 Stage 배경 Sprite가 비어 있습니다. Stage 1 배경을 대신 사용합니다.");
            selectedSprite = stage1BackgroundSprite;
        }

        // Stage 1 배경도 없다면 보여줄 이미지가 없으므로 오류를 보여주고 멈춥니다.
        if (selectedSprite == null)
        {
            Debug.LogError("Stage 1 배경 Sprite가 비어 있습니다. 최소한 Stage 1 배경 이미지는 Inspector에서 연결해주세요.");
            return;
        }

        // SpriteRenderer에 선택된 배경 이미지를 넣습니다.
        backgroundRenderer.sprite = selectedSprite;

        // 배경이 다른 Sprite보다 뒤에 보이도록 순서를 낮게 설정합니다.
        backgroundRenderer.sortingOrder = backgroundOrderInLayer;

        // 배경 위치와 크기를 카메라에 맞춥니다.
        MoveBackgroundToCamera();
        ResizeBackgroundToCameraView();
    }

    // Stage 번호를 보고 어떤 배경 Sprite를 사용할지 골라주는 함수입니다.
    private Sprite GetBackgroundSpriteByStage(int stageNumber)
    {
        // Stage 번호가 1이면 Stage 1 배경을 돌려줍니다.
        if (stageNumber == 1)
        {
            return stage1BackgroundSprite;
        }

        // Stage 번호가 2이면 Stage 2 배경을 돌려줍니다.
        if (stageNumber == 2)
        {
            return stage2BackgroundSprite;
        }

        // Stage 번호가 3이면 Stage 3 배경을 돌려줍니다.
        if (stageNumber == 3)
        {
            return stage3BackgroundSprite;
        }

        // 혹시 1, 2, 3이 아닌 값이 들어오면 Stage 1 배경을 돌려줍니다.
        Debug.LogWarning("Stage 번호는 1, 2, 3만 사용할 수 있습니다. Stage 1 배경을 사용합니다.");
        return stage1BackgroundSprite;
    }

    // 배경 준비 과정을 한 번에 실행하는 함수입니다.
    // 다른 함수들이 같은 순서로 배경을 다시 맞출 수 있게 모아둔 함수입니다.
    public void RefreshBackground()
    {
        // 카메라가 Inspector에 연결되지 않았다면 Main Camera를 자동으로 찾습니다.
        FindTargetCameraIfNeeded();

        // 배경을 보여줄 SpriteRenderer가 없다면 자동으로 만듭니다.
        CreateBackgroundRendererIfNeeded();

        // 현재 Stage 번호에 맞는 배경을 사용합니다.
        SetStageBackground(currentStageNumber);
    }

    // Stage 2 배경 Sprite가 Inspector에 연결되어 있는지 확인하는 함수입니다.
    // 아직 Stage 2로 바꾸지는 않고, 나중에 기능 4번에서 사용할 준비만 확인합니다.
    public bool HasStage2Background()
    {
        // stage2BackgroundSprite가 비어 있지 않으면 true를 돌려줍니다.
        // true는 "준비됨", false는 "아직 비어 있음"이라는 뜻입니다.
        return stage2BackgroundSprite != null;
    }

    // Stage 2 배경 Sprite를 다른 스크립트가 나중에 가져갈 수 있게 해주는 함수입니다.
    // 기능 4번에서 스테이지 전환을 만들 때 이 함수를 사용할 수 있습니다.
    public Sprite GetStage2BackgroundSprite()
    {
        // Inspector에 연결된 Stage 2 배경 Sprite를 돌려줍니다.
        return stage2BackgroundSprite;
    }

    // Stage 3 배경 Sprite가 Inspector에 연결되어 있는지 확인하는 함수입니다.
    // 아직 Stage 3로 바꾸지는 않고, 나중에 기능 4번에서 사용할 준비만 확인합니다.
    public bool HasStage3Background()
    {
        // stage3BackgroundSprite가 비어 있지 않으면 true를 돌려줍니다.
        // true는 "준비됨", false는 "아직 비어 있음"이라는 뜻입니다.
        return stage3BackgroundSprite != null;
    }

    // Stage 3 배경 Sprite를 다른 스크립트가 나중에 가져갈 수 있게 해주는 함수입니다.
    // 기능 4번에서 스테이지 전환을 만들 때 이 함수를 사용할 수 있습니다.
    public Sprite GetStage3BackgroundSprite()
    {
        // Inspector에 연결된 Stage 3 배경 Sprite를 돌려줍니다.
        return stage3BackgroundSprite;
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
