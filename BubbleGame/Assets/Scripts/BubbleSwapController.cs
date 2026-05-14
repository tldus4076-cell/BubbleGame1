using UnityEngine;

// BubbleSwapController는 "다음 버블을 현재 버블로 바꾸는 로직"을 담당하는 스크립트입니다.
// 나중에 기능 21번(버블 발사)에서 버블을 쏜 뒤에 SwapBubbles()를 호출하면 됩니다.
// 이번 기능 20번에서는 바꾸기 로직만 만들고, 테스트용 키보드 입력으로 확인합니다.
// 이 스크립트는 ShooterRoot 오브젝트에 붙여서 사용합니다.
public class BubbleSwapController : MonoBehaviour
{
    [Header("테스트 설정")]
    [Tooltip("체크하면 Space 키를 누를 때마다 다음 버블이 현재 버블로 바뀝니다.")]
    [SerializeField] private bool useTestKey = true;

    // BubbleCurrentController는 "현재 발사할 버블"을 보여주는 스크립트입니다.
    // 같은 ShooterRoot에 붙어 있어서 GetComponent로 가져옵니다.
    // GetComponent는 "이 오브젝트에 붙어 있는 컴포넌트를 가져온다"는 뜻입니다.
    private BubbleCurrentController currentController;

    // BubbleNextController는 "다음에 나올 버블"을 보여주는 스크립트입니다.
    private BubbleNextController nextController;

    // Awake는 Start보다 먼저 한 번 호출됩니다.
    // 여기서는 필요한 스크립트 연결을 준비합니다.
    private void Awake()
    {
        // ShooterRoot에 붙어 있는 BubbleCurrentController를 찾습니다.
        currentController = GetComponent<BubbleCurrentController>();

        // ShooterRoot에 붙어 있는 BubbleNextController를 찾습니다.
        nextController = GetComponent<BubbleNextController>();
    }

    // Update는 매 프레임 호출됩니다.
    // 여기서는 테스트용 키 입력을 확인합니다.
    private void Update()
    {
        // 테스트 키 기능이 꺼져 있으면 아무것도 하지 않습니다.
        if (!useTestKey)
        {
            return;
        }

        // Space 키를 누르면 버블을 바꿉니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapBubbles();
        }
    }

    // 다음 버블을 현재 버블로 바꾸는 함수입니다.
    // 나중에 기능 21번(버블 발사)에서 버블을 쏜 뒤에 이 함수를 호출하면 됩니다.
    // public으로 만든 이유는 다른 스크립트에서 호출할 수 있게 하기 위해서입니다.
    public void SwapBubbles()
    {
        // 1단계: 다음 버블에 표시된 Sprite와 색 인덱스를 가져옵니다.
        Sprite nextSprite = null;
        int nextColorIndex = -1;

        if (nextController != null)
        {
            nextSprite = nextController.GetSelectedNextBubbleSprite();
            nextColorIndex = nextController.GetSelectedColorIndex();
        }

        // 2단계: 가져온 다음 버블 Sprite와 색 인덱스를 현재 버블로 전달합니다.
        // 이렇게 하면 아까 옆에 보이던 다음 버블이 정확히 현재 버블이 됩니다.
        if (currentController != null)
        {
            currentController.SetNextBubble(nextSprite, nextColorIndex);
        }

        // 3단계: 새로운 다음 버블을 랜덤으로 선택합니다.
        if (nextController != null)
        {
            nextController.SelectNewNextBubble();
        }
    }
}
