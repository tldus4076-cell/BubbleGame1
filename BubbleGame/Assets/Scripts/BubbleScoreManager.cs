using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// BubbleScoreManager는 제거된 버블 개수에 맞춰 점수를 올리는 스크립트입니다.
// 실제 버블 제거 기능은 아직 만들지 않고, 점수 계산 준비만 합니다.
public class BubbleScoreManager : MonoBehaviour
{
    [Header("점수 컨트롤러 연결")]
    [Tooltip("현재 점수를 저장하고 화면에 보여주는 ScoreController를 연결합니다.")]
    [SerializeField] private ScoreController scoreController;

    [Header("버블 점수 설정")]
    [Tooltip("버블 1개가 제거될 때 몇 점을 줄지 정합니다.")]
    [SerializeField] private int scorePerBubble = 10;

    [Header("테스트 설정")]
    [Tooltip("체크하면 Play 중 T 키로 3개 버블 제거 점수 테스트를 할 수 있습니다.")]
    [SerializeField] private bool useKeyboardTest = true;

    [Tooltip("테스트 키를 눌렀을 때 제거된 것으로 처리할 버블 개수입니다.")]
    [SerializeField] private int testRemovedBubbleCount = 3;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // ScoreController가 연결되지 않았다면 자동으로 찾아봅니다.
        FindScoreControllerIfNeeded();

        // 점수 설정 값이 이상하지 않게 정리합니다.
        ClampSettings();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    private void Update()
    {
        // 테스트 키 입력을 확인합니다.
        HandleKeyboardTest();
    }

    // 제거된 버블 개수에 맞춰 점수를 올리는 함수입니다.
    // 나중에 실제 버블 제거 기능에서 이 함수를 호출하면 됩니다.
    public void AddBubbleScore(int removedBubbleCount)
    {
        // ScoreController가 비어 있으면 자동으로 한 번 더 찾아봅니다.
        FindScoreControllerIfNeeded();

        // 제거 개수가 0 이하이면 점수를 올리지 않습니다.
        if (removedBubbleCount <= 0)
        {
            return;
        }

        // ScoreController가 없으면 점수를 올릴 수 없으므로 멈춥니다.
        if (scoreController == null)
        {
            Debug.LogWarning("ScoreController가 연결되지 않아 점수를 올릴 수 없습니다.");
            return;
        }

        // 버블 1개당 점수가 음수가 되지 않도록 막습니다.
        scorePerBubble = Mathf.Max(scorePerBubble, 0);

        // 제거된 개수 x 버블 1개당 점수로 총 점수를 계산합니다.
        int addScore = removedBubbleCount * scorePerBubble;

        // ScoreController의 AddScore 함수를 사용해서 점수를 올립니다.
        scoreController.AddScore(addScore);
    }

    // 테스트용: 버블 1개 제거 점수를 올립니다.
    // 실제 게임 로직이 아니라 기능 확인용입니다.
    public void AddTestScoreForOneBubble()
    {
        AddBubbleScore(1);
    }

    // 테스트용: 버블 3개 제거 점수를 올립니다.
    // 실제 게임 로직이 아니라 기능 확인용입니다.
    public void AddTestScoreForThreeBubbles()
    {
        AddBubbleScore(3);
    }

    // ScoreController가 비어 있으면 Scene에서 자동으로 찾는 함수입니다.
    private void FindScoreControllerIfNeeded()
    {
        // 이미 연결되어 있으면 다시 찾을 필요가 없습니다.
        if (scoreController != null)
        {
            return;
        }

        // Scene 안에서 ScoreController를 찾아 연결합니다.
        scoreController = FindFirstObjectByType<ScoreController>();
    }

    // Inspector 값이 이상하지 않게 정리하는 함수입니다.
    private void ClampSettings()
    {
        // 버블 1개당 점수는 0보다 작을 수 없습니다.
        scorePerBubble = Mathf.Max(scorePerBubble, 0);

        // 테스트 제거 개수는 0보다 작을 수 없습니다.
        testRemovedBubbleCount = Mathf.Max(testRemovedBubbleCount, 0);
    }

    // 테스트 키보드 입력을 처리하는 함수입니다.
    private void HandleKeyboardTest()
    {
        // 테스트 기능을 꺼두면 아무것도 하지 않습니다.
        if (!useKeyboardTest)
        {
            return;
        }

        // T 키를 누르면 테스트용으로 버블 여러 개가 제거된 것처럼 점수를 올립니다.
        if (IsTestKeyPressed())
        {
            AddBubbleScore(testRemovedBubbleCount);
        }
    }

    // 현재 프로젝트 입력 설정에 맞춰 테스트 키가 눌렸는지 확인하는 함수입니다.
    private bool IsTestKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        // 새 Input System을 사용하는 프로젝트에서는 Keyboard.current로 키 입력을 읽습니다.
        return Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        // 예전 Input Manager를 사용하는 프로젝트에서는 Input.GetKeyDown을 사용합니다.
        return Input.GetKeyDown(KeyCode.T);
#else
        // 입력 시스템이 꺼져 있으면 테스트 키를 사용할 수 없습니다.
        return false;
#endif
    }
}
