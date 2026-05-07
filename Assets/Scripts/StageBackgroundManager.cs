using UnityEngine;

// StageBackgroundManager는 스테이지 배경 이미지를 화면에 보여주는 스크립트입니다.
// 지금은 Stage 1 배경 1장만 사용합니다.
// 나중에 Stage 2, Stage 3 배경도 쉽게 추가할 수 있게 만들어 둡니다.
public class StageBackgroundManager : MonoBehaviour
{
    // SpriteRenderer는 2D 그림을 화면에 보여주는 컴포넌트입니다.
    // 여기에는 StageBackground 오브젝트의 SpriteRenderer를 연결합니다.
    [Header("배경을 보여줄 SpriteRenderer")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    // Stage 1에서 사용할 배경 이미지입니다.
    // Inspector에서 준비한 Stage 1 Sprite 이미지를 넣습니다.
    [Header("Stage 1 배경 이미지")]
    [SerializeField] private Sprite stage1Background;

    // 나중에 사용할 Stage 2 배경 이미지 자리입니다.
    // 지금은 비워 두어도 됩니다.
    [Header("Stage 2 배경 이미지 - 나중에 사용")]
    [SerializeField] private Sprite stage2Background;

    // 나중에 사용할 Stage 3 배경 이미지 자리입니다.
    // 지금은 비워 두어도 됩니다.
    [Header("Stage 3 배경 이미지 - 나중에 사용")]
    [SerializeField] private Sprite stage3Background;

    // 배경 크기를 화면보다 살짝 크게 만드는 값입니다.
    // 1이면 화면에 딱 맞고, 1.05면 5% 더 크게 보입니다.
    [Header("배경 크기 여유")]
    [SerializeField] private float sizeMultiplier = 1.05f;

    // 배경이 다른 오브젝트보다 뒤에 보이게 하는 순서 값입니다.
    // 숫자가 작을수록 뒤에 보입니다.
    [Header("배경 정렬 순서")]
    [SerializeField] private int sortingOrder = -100;

    // 게임이 시작될 때 한 번 실행됩니다.
    private void Start()
    {
        // 우선 Stage 1 배경을 보여줍니다.
        ShowStage1Background();
    }

    // Stage 1 배경을 보여주는 함수입니다.
    public void ShowStage1Background()
    {
        // Stage 1 배경 이미지를 적용합니다.
        SetBackground(stage1Background);
    }

    // Stage 2 배경을 보여주는 함수입니다.
    // 지금은 사용하지 않지만, 나중에 스테이지 변경 기능에서 사용할 수 있습니다.
    public void ShowStage2Background()
    {
        // Stage 2 배경 이미지가 있으면 적용합니다.
        SetBackground(stage2Background);
    }

    // Stage 3 배경을 보여주는 함수입니다.
    // 지금은 사용하지 않지만, 나중에 스테이지 변경 기능에서 사용할 수 있습니다.
    public void ShowStage3Background()
    {
        // Stage 3 배경 이미지가 있으면 적용합니다.
        SetBackground(stage3Background);
    }

    // 실제로 배경 이미지를 바꾸고 화면 크기에 맞추는 함수입니다.
    private void SetBackground(Sprite backgroundSprite)
    {
        // backgroundRenderer가 연결되지 않았을 때 오류를 보여줍니다.
        if (backgroundRenderer == null)
        {
            Debug.LogError("StageBackgroundManager: Background Renderer가 연결되지 않았습니다.");
            return;
        }

        // 배경 이미지가 연결되지 않았을 때 오류를 보여줍니다.
        if (backgroundSprite == null)
        {
            Debug.LogError("StageBackgroundManager: 배경 이미지 Sprite가 연결되지 않았습니다.");
            return;
        }

        // SpriteRenderer에 배경 이미지를 넣습니다.
        backgroundRenderer.sprite = backgroundSprite;

        // 배경이 다른 오브젝트보다 뒤에 보이게 순서를 정합니다.
        backgroundRenderer.sortingOrder = sortingOrder;

        // 배경을 카메라 화면 크기에 맞춥니다.
        FitBackgroundToCamera();
    }

    // 배경 이미지를 카메라 화면에 꽉 차게 만드는 함수입니다.
    private void FitBackgroundToCamera()
    {
        // Main Camera는 게임 화면을 보여주는 기본 카메라입니다.
        Camera mainCamera = Camera.main;

        // Main Camera가 없으면 오류를 보여줍니다.
        if (mainCamera == null)
        {
            Debug.LogError("StageBackgroundManager: Main Camera를 찾을 수 없습니다. 카메라 Tag를 MainCamera로 설정하세요.");
            return;
        }

        // 배경 이미지가 없으면 더 진행하지 않습니다.
        if (backgroundRenderer.sprite == null)
        {
            Debug.LogError("StageBackgroundManager: SpriteRenderer에 배경 이미지가 없습니다.");
            return;
        }

        // 카메라가 Orthographic인지 확인합니다.
        // Orthographic은 2D 게임에서 보통 쓰는 카메라 방식입니다.
        if (!mainCamera.orthographic)
        {
            Debug.LogWarning("StageBackgroundManager: Main Camera가 Orthographic이 아닙니다. 2D 게임에서는 Orthographic을 추천합니다.");
        }

        // 카메라 화면의 세로 크기를 구합니다.
        float cameraHeight = mainCamera.orthographicSize * 2f;

        // 카메라 화면의 가로 크기를 구합니다.
        // aspect는 화면의 가로세로 비율입니다.
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // 배경 이미지의 원래 가로 크기를 구합니다.
        float spriteWidth = backgroundRenderer.sprite.bounds.size.x;

        // 배경 이미지의 원래 세로 크기를 구합니다.
        float spriteHeight = backgroundRenderer.sprite.bounds.size.y;

        // 화면 가로에 맞추려면 몇 배 키워야 하는지 계산합니다.
        float scaleX = cameraWidth / spriteWidth;

        // 화면 세로에 맞추려면 몇 배 키워야 하는지 계산합니다.
        float scaleY = cameraHeight / spriteHeight;

        // 가로와 세로 중 더 큰 값을 고릅니다.
        // 이렇게 해야 화면에 빈 공간이 생기지 않습니다.
        float finalScale = Mathf.Max(scaleX, scaleY) * sizeMultiplier;

        // 배경 크기를 바꿉니다.
        backgroundRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // 배경을 카메라가 보는 가운데 위치에 둡니다.
        // Z값은 0으로 두고, sortingOrder로 뒤에 보이게 합니다.
        backgroundRenderer.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0f);
    }
}
