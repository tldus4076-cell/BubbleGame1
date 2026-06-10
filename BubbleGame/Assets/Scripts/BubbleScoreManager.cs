using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// ============================================================
// BubbleScoreManager는 버블이 제거될 때 점수를 올리는 스크립트입니다.
//
// [어디에 붙이나요?]
// 씬 안의 아무 오브젝트에 붙여도 됩니다.
// 보통 BubbleScoreManager라는 빈 오브젝트를 만들어서 거기 붙입니다.
//
// [Inspector에서 연결할 것]
// 1. Score Controller: 점수를 저장하고 화면에 보여주는 ScoreController
// 2. Grid Manager: 버블 제거 이벤트를 보내주는 BubbleGridManager
//
// [동작 순서]
// 1. BubbleGridManager가 같은 색 3개 이상을 제거합니다.
// 2. BubbleGridManager가 MatchedBubblesRemoved 이벤트를 발생시킵니다.
// 3. BubbleScoreManager가 이 이벤트를 받고 OnBubblesRemoved()를 실행합니다.
// 4. OnBubblesRemoved()가 AddBubbleScore()를 호출합니다.
// 5. AddBubbleScore()가 점수 규칙대로 점수를 계산해서 올립니다.
//
// [점수 규칙]
// - 1개당 기본 점수: 10점
// - 3개 제거: 30점 (10 x 3, 보너스 없음)
// - 4개 제거: 50점 (10 x 4 = 40 + 4개 보너스 10점)
// - 5개 이상 제거: 80점 (10 x 5 = 50 + 5개+ 보너스 30점)
// ============================================================
public class BubbleScoreManager : MonoBehaviour
{
    [Header("점수 컨트롤러 연결")]
    [Tooltip("현재 점수를 저장하고 화면에 보여주는 ScoreController를 연결합니다.")]
    [SerializeField] private ScoreController scoreController;

    [Header("격자 매니저 연결")]
    [Tooltip("버블 제거 이벤트를 보내주는 BubbleGridManager를 연결합니다.")]
    [SerializeField] private BubbleGridManager gridManager;

    [Header("기본 점수 설정")]
    [Tooltip("버블 1개당 기본 점수입니다. 보통 10점입니다.")]
    [SerializeField] private int scorePerBubble = 10;

    [Header("보너스 점수 설정")]
    [Tooltip("같은 색 4개 제거 시 추가 보너스 점수입니다. 기본 10점.")]
    [SerializeField] private int bonusForFourBubbles = 10;

    [Tooltip("같은 색 5개 이상 제거 시 추가 보너스 점수입니다. 기본 30점.")]
    [SerializeField] private int bonusForFiveOrMoreBubbles = 30;

    [Header("테스트 설정")]
    [Tooltip("체크하면 Play 중 T 키로 테스트 점수를 올릴 수 있습니다.")]
    [SerializeField] private bool useKeyboardTest = true;

    [Tooltip("테스트 키를 눌렀을 때 점수 계산에 사용할 제거 개수입니다.")]
    [SerializeField] private int testRemovedBubbleCount = 3;

    // ============================================================
    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    // ============================================================
    private void Awake()
    {
        // 점수 설정 값이 이상하지 않게 정리합니다.
        ClampSettings();
    }

    // ============================================================
    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기서 BubbleGridManager의 이벤트를 구독합니다.
    // ============================================================
    private void Start()
    {
        SubscribeToGridManagerEvent();
    }

    // ============================================================
    // OnDestroy는 오브젝트가 삭제될 때 Unity가 자동으로 호출합니다.
    // 여기서 이벤트 구독을 해제합니다.
    // 해제하지 않으면 메모리 문제가 생길 수 있습니다.
    // ============================================================
    private void OnDestroy()
    {
        UnsubscribeFromGridManagerEvent();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    private void Update()
    {
        HandleKeyboardTest();
    }

    // ============================================================
    // BubbleGridManager의 이벤트를 구독합니다.
    // 구독(subscribe)이란? "버블이 제거되면 나한테도 알려줘"라고 등록하는 것입니다.
    // += 연산자로 구독하고, -= 연산자로 해제합니다.
    // ============================================================
    private void SubscribeToGridManagerEvent()
    {
        if (gridManager == null)
        {
            Debug.LogWarning("BubbleScoreManager: Grid Manager가 Inspector에서 연결되지 않아서 점수 이벤트를 구독할 수 없습니다.");
            return;
        }

        // [기능 35] 버블 제거 시 점수 이벤트를 구독합니다.
        gridManager.MatchedBubblesRemoved += OnBubblesRemoved;

        // [기능 40] 떨어진 버블 점수 이벤트를 구독합니다.
        gridManager.FloatingBubblesDropped += OnBubblesDropped;
    }

    // ============================================================
    // BubbleGridManager의 이벤트 구독을 해제합니다.
    // OnDestroy에서 호출됩니다.
    // ============================================================
    private void UnsubscribeFromGridManagerEvent()
    {
        if (gridManager == null)
        {
            return;
        }

        gridManager.MatchedBubblesRemoved -= OnBubblesRemoved;
        gridManager.FloatingBubblesDropped -= OnBubblesDropped;
    }

    // ============================================================
    // BubbleGridManager가 버블을 제거했을 때 이벤트로 자동으로 호출되는 함수입니다.
    // 이 함수는 직접 호출하지 마세요. 이벤트가 자동으로 호출합니다.
    // ============================================================
    private void OnBubblesRemoved(int removedCount)
    {
        AddBubbleScore(removedCount);
    }

    // ============================================================
    // [기능 40] 떨어진 버블이 있을 때 이벤트로 자동으로 호출되는 함수입니다.
    // 떨어진 버블 수만큼 점수를 올립니다.
    // ============================================================
    private void OnBubblesDropped(int droppedCount)
    {
        AddBubbleScore(droppedCount);
    }

    // ============================================================
    // 제거된 버블 개수에 맞춰 점수를 올리는 함수입니다.
    // 점수 규칙:
    // - 1개당 기본 점수(scorePerBubble) 적용
    // - 4개일 때 bonusForFourBubbles 보너스 추가
    // - 5개 이상일 때 bonusForFiveOrMoreBubbles 보너스 추가
    // ============================================================
    public void AddBubbleScore(int removedBubbleCount)
    {
        if (removedBubbleCount <= 0)
        {
            return;
        }

        if (scoreController == null)
        {
            Debug.LogWarning("ScoreController가 연결되지 않아 점수를 올릴 수 없습니다.");
            return;
        }

        // 점수 규칙대로 점수를 계산합니다.
        int addScore = CalculateScore(removedBubbleCount);

        // ScoreController의 AddScore 함수를 사용해서 점수를 올립니다.
        scoreController.AddScore(addScore);

        Debug.Log($"[기능 35] 점수 증가! 제거: {removedBubbleCount}개, 점수: +{addScore}점");
    }

    // ============================================================
    // 점수 규칙에 따라 점수를 계산하는 함수입니다.
    // - 1~3개: 1개당 기본 점수
    // - 4개: 1개당 기본 점수 x 4 + 4개 보너스
    // - 5개 이상: 1개당 기본 점수 x 5 + 5개+ 보너스
    // ============================================================
    private int CalculateScore(int removedBubbleCount)
    {
        // 점수가 음수가 되지 않도록 막습니다.
        scorePerBubble = Mathf.Max(scorePerBubble, 0);
        bonusForFourBubbles = Mathf.Max(bonusForFourBubbles, 0);
        bonusForFiveOrMoreBubbles = Mathf.Max(bonusForFiveOrMoreBubbles, 0);

        if (removedBubbleCount >= 5)
        {
            // 5개 이상: 기본 점수 x 5 + 5개+ 보너스
            return (scorePerBubble * 5) + bonusForFiveOrMoreBubbles;
        }

        if (removedBubbleCount == 4)
        {
            // 4개: 기본 점수 x 4 + 4개 보너스
            return (scorePerBubble * 4) + bonusForFourBubbles;
        }

        // 1~3개: 기본 점수 x 제거 개수
        return scorePerBubble * removedBubbleCount;
    }

    // 테스트용: 버블 1개 제거 점수를 올립니다.
    public void AddTestScoreForOneBubble()
    {
        AddBubbleScore(1);
    }

    // 테스트용: 버블 3개 제거 점수를 올립니다.
    public void AddTestScoreForThreeBubbles()
    {
        AddBubbleScore(3);
    }

    // Inspector 값이 이상하지 않게 정리하는 함수입니다.
    private void ClampSettings()
    {
        // 모든 점수 값은 0보다 작을 수 없습니다.
        scorePerBubble = Mathf.Max(scorePerBubble, 0);
        bonusForFourBubbles = Mathf.Max(bonusForFourBubbles, 0);
        bonusForFiveOrMoreBubbles = Mathf.Max(bonusForFiveOrMoreBubbles, 0);
        testRemovedBubbleCount = Mathf.Max(testRemovedBubbleCount, 0);
    }

    private void HandleKeyboardTest()
    {
        if (!useKeyboardTest)
        {
            return;
        }

        if (IsTestKeyPressed())
        {
            AddBubbleScore(testRemovedBubbleCount);
        }
    }

    private bool IsTestKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.T);
#else
        return false;
#endif
    }
}
