using UnityEngine;

// ============================================================
// StageDataController는 스테이지 데이터를 저장하고,
// 게임 시작 시 스테이지 이름을 잠깐 보여준 뒤 자동으로 숨기고,
// 스테이지 번호에 맞는 배경 이미지를 자동으로 바꾸는 스크립트입니다.
//
// [이 스크립트가 필요한 이유]
// 게임에는 Stage 1, Stage 2, Stage 3 같은 이름이 필요합니다.
// 플레이어가 지금 어떤 스테이지를 플레이하고 있는지 알 수 있게 해줍니다.
// 또한 스테이지마다 배경 이미지가 다르기 때문에,
// 스테이지 번호에 맞는 배경을 자동으로 바꿔줘야 합니다.
//
// [어떻게 동작하나요?]
// 1. Inspector에서 스테이지 이름과 번호를 입력합니다.
// 2. Inspector에서 StageBackgroundController를 연결합니다.
// 3. 게임 시작 시 스테이지 이름이 화면에 잠깐 보입니다.
// 4. hideDelay초 뒤에 이름이 자동으로 사라집니다.
// 5. StageBackgroundController에 연결된 배경이 자동으로 바뀝니다.
//
// [Inspector에서 연결할 것]
// - Stage Name Text: 화면에 스테이지 이름을 보여줄 TextMeshProUGUI (선택)
// - Background Controller: 배경 이미지를 바꿔주는 StageBackgroundController (선택)
// ============================================================
public class StageDataController : MonoBehaviour
{
    // ============================================================
    // [Inspector에 보이는 변수들]
    // ============================================================

    [Header("스테이지 데이터 설정")]

    [Tooltip("스테이지 이름입니다. Inspector에서 'Stage 1', 'Stage 2' 등으로 직접 입력하세요.")]
    [SerializeField] private string stageName = "Stage 1";

    [Tooltip("스테이지 번호입니다. Stage 1이면 1, Stage 2이면 2를 입력하세요.")]
    [SerializeField] private int stageNumber = 1;

    [Header("배경 이미지 설정")]

    [Tooltip("이 스테이지에서 사용할 배경 Sprite입니다. Inspector에서 배경 이미지를 넣어주세요.")]
    [SerializeField] private Sprite stageBackground;

    [Tooltip("배경 이미지를 자동으로 바꿔주는 StageBackgroundController입니다. Inspector에서 연결해주세요.")]
    [SerializeField] private StageBackgroundController backgroundController;

    [Header("버블 색 설정")]

    [Tooltip("이 스테이지에서 사용할 버블 Sprite 목록입니다. 빨강, 파랑, 노랑 순서로 넣는 것을 추천합니다.")]
    [SerializeField] private Sprite[] stageBubbleSprites;

    [Tooltip("이 스테이지에서 사용할 색 종류 수입니다. Stage 1은 3색, Stage 2는 4색, Stage 3은 5색을 추천합니다.")]
    [SerializeField] private int colorCount = 3;

    [Header("시작 배치 설정")]

    [Tooltip("시작 시 채울 줄 수입니다. Stage 1은 4줄, Stage 2는 5줄, Stage 3은 6줄을 추천합니다.")]
    [SerializeField] private int startRows = 4;

    [Tooltip("한 줄의 칸 수입니다. Stage 1은 6칸을 추천합니다.")]
    [SerializeField] private int startCols = 6;

    [Tooltip("시작 버블 배치를 숫자로 저장합니다. 0=빨강, 1=파랑, 2=노랑입니다. 비어 있으면 기본 배치를 사용합니다.")]
    [SerializeField] private int[] startBubblePattern;

    [Header("제한 샷 수 설정")]

    [Tooltip("이 스테이지의 제한 샷 수입니다. Stage 1은 25발, Stage 2는 22발, Stage 3은 18발을 추천합니다.")]
    [SerializeField] private int maxShotCount = 25;

    [Header("제한 시간 설정")]

    [Tooltip("이 스테이지의 제한 시간입니다. 초 단위입니다. Stage 1은 90초, Stage 2는 75초, Stage 3은 60초를 추천합니다.")]
    [SerializeField] private float timeLimit = 90f;

    [Header("장애물 설정")]

    [Tooltip("이 스테이지에 장애물이 있는지 여부입니다. 체크하면 장애물이 있는 스테이지가 됩니다.")]
    [SerializeField] private bool hasObstacles = false;

    [Header("화면 표시 설정")]

    [Tooltip("화면에 스테이지 이름을 보여줄 TextMeshPro 텍스트입니다. 비워두면 Console에만 출력합니다.")]
    [SerializeField] private TMPro.TextMeshProUGUI stageNameText;

    [Tooltip("게임 시작 시 자동으로 스테이지 이름을 표시할지 여부입니다.")]
    [SerializeField] private bool showOnStart = true;

    [Tooltip("스테이지 이름이 몇 초 뒤에 사라질지 정합니다. 1.5이면 1.5초 뒤에 사라집니다.")]
    [SerializeField] private float hideDelay = 1.5f;

    // ============================================================
    // [내부 변수 - Inspector에 보이지 않는 변수들]
    // ============================================================

    // 지금 실행 중인 숨김 Coroutine을 기억하는 변수입니다.
    // Coroutine이란? "시간을 기다렸다가 실행하는 기능"이라고 생각하면 됩니다.
    private Coroutine hideCoroutine;

    // ============================================================
    // [실행 흐름]
    //
    // 1. Unity가 이 스크립트가 붙은 오브젝트를 찾습니다.
    // 2. Awake()가 먼저 실행됩니다. (Start보다 빠름)
    // 3. Start()가 실행됩니다. (게임 시작 시 한 번)
    // 4. showOnStart가 true이면 ShowStageInfo()를 호출합니다.
    // 5. ShowStageInfo()에서:
    //    a. 스테이지 이름을 화면에 표시합니다.
    //    b. StageBackgroundController에 배경을 자동으로 바꿉니다.
    // 6. hideDelay초 뒤에 자동으로 이름이 사라집니다.
    // ============================================================

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 나중에 여기에 초기화 코드를 넣을 수 있습니다.
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기에서 스테이지 정보를 화면에 표시하고 배경을 바꿉니다.
    private void Start()
    {
        // showOnStart가 true이면 스테이지 정보를 표시합니다.
        if (showOnStart)
        {
            ShowStageInfo();
        }
    }

    // ============================================================
    // [공개 함수 - 다른 스크립트에서 호출할 수 있는 함수]
    // ============================================================

    // 스테이지 이름과 배경을 함께 처리하는 함수입니다.
    // 기존 ShowStageName()을 ShowStageInfo()로 이름을 바꾸고,
    // 배경 이미지도 함께 자동으로 바꾸도록 만들었습니다.
    //
    // [이 함수가 하는 일]
    // 1. 스테이지 이름을 화면에 잠깐 보여줍니다.
    // 2. hideDelay초 뒤에 자동으로 이름을 숨깁니다.
    // 3. StageBackgroundController가 연결되어 있으면 배경을 자동으로 바꿉니다.
    // 4. 이 스테이지에서 사용할 버블 색 정보도 Console에 출력합니다.
    public void ShowStageInfo()
    {
        // ============================================================
        // 1단계: 스테이지 이름을 화면에 표시합니다.
        // ============================================================
        ShowStageNameOnScreen();

        // ============================================================
        // 2단계: 배경 이미지를 자동으로 바꿉니다.
        // ============================================================
        ApplyBackground();

        // ============================================================
        // 3단계: 기능 43 테스트용으로 버블 색 정보를 Console에 출력합니다.
        // ============================================================
        LogStageBubbleColorData();

        // ============================================================
        // 4단계: 기능 44 테스트용으로 배치 설정을 Console에 출력합니다.
        // ============================================================
        LogStageLayoutData();

        // ============================================================
        // 5단계: 기능 45 테스트용으로 제한 샷 수를 Console에 출력합니다.
        // ============================================================
        LogStageShotLimitData();

        // ============================================================
        // 6단계: 기능 46 테스트용으로 제한 시간을 Console에 출력합니다.
        // ============================================================
        LogStageTimeLimitData();

        // ============================================================
        // 7단계: 기능 47 테스트용으로 장애물 유무를 Console에 출력합니다.
        // ============================================================
        LogStageObstacleData();
    }

    // 스테이지 이름을 화면에서 숨기는 함수입니다.
    // hideDelay초 뒤에 자동으로 호출됩니다.
    public void HideStageName()
    {
        // TextMeshPro 텍스트가 연결되어 있으면 숨깁니다.
        if (stageNameText != null)
        {
            // SetActive(false)는 "이 오브젝트를 꺼줘"라는 뜻입니다.
            // 꺼진 오브젝트는 화면에 보이지 않습니다.
            stageNameText.gameObject.SetActive(false);

            Debug.Log("[기능 42] 스테이지 이름 숨김");
        }
    }

    // Inspector에서 스테이지 이름과 배경을 바꿀 때 호출할 수 있는 함수입니다.
    // 나중에 스테이지 전환 기능에서 사용할 수 있습니다.
    public void SetStageInfo(string newName)
    {
        // 새로운 이름을 저장합니다.
        stageName = newName;

        // 화면에 바로 반영합니다. (잠깐 보여주고 다시 사라짐 + 배경 변경)
        ShowStageInfo();
    }

    // 다른 스크립트에서 현재 스테이지 이름을 가져갈 때 사용하는 함수입니다.
    public string GetStageName()
    {
        // 저장된 스테이지 이름을 돌려줍니다.
        return stageName;
    }

    // 다른 스크립트에서 스테이지 번호를 가져갈 때 사용하는 함수입니다.
    public int GetStageNumber()
    {
        // 저장된 스테이지 번호를 돌려줍니다.
        return stageNumber;
    }

    // Inspector에서 스테이지 번호를 바꿀 때 호출할 수 있는 함수입니다.
    public void SetStageNumber(int newNumber)
    {
        // 1보다 작은 번호는 1로 고칩니다.
        if (newNumber < 1)
        {
            newNumber = 1;
            Debug.LogWarning("[기능 43] 스테이지 번호는 1 이상이어야 합니다. 1로 고쳤습니다.");
        }

        // 새로운 번호를 저장합니다.
        stageNumber = newNumber;

        // 화면에 바로 반영합니다. (이름 + 배경)
        ShowStageInfo();
    }

    // 다른 스크립트에서 이 스테이지의 버블 Sprite 목록을 가져갈 때 사용하는 함수입니다.
    // 예: BubbleNextController가 다음 버블 Sprite를 고를 때 사용할 수 있습니다.
    public Sprite[] GetStageBubbleSprites()
    {
        // Inspector에 연결된 버블 Sprite 배열을 돌려줍니다.
        return stageBubbleSprites;
    }

    // 다른 스크립트에서 이 스테이지의 색 종류 수를 가져갈 때 사용하는 함수입니다.
    // 예: Stage 1은 3색, Stage 2는 4색, Stage 3은 5색입니다.
    public int GetColorCount()
    {
        // Inspector에 입력한 색 종류 수를 돌려줍니다.
        return colorCount;
    }

    // 다른 스크립트에서 이 스테이지의 시작 버블 배치 패턴을 가져갈 때 사용하는 함수입니다.
    // 예: 0,0,1,1,2,2이면 빨강,빨강,파랑,파랑,노랑,노랑 순서로 배치됩니다.
    public int[] GetStartBubblePattern()
    {
        // Inspector에 입력한 배치 패턴 배열을 돌려줍니다.
        return startBubblePattern;
    }

    // 다른 스크립트에서 이 스테이지의 시작 줄 수를 가져갈 때 사용하는 함수입니다.
    public int GetStartRows()
    {
        // Inspector에 입력한 줄 수를 돌려줍니다.
        return startRows;
    }

    // 다른 스크립트에서 이 스테이지의 시작 칸 수를 가져갈 때 사용하는 함수입니다.
    public int GetStartCols()
    {
        // Inspector에 입력한 칸 수를 돌려줍니다.
        return startCols;
    }

    // 다른 스크립트에서 이 스테이지의 제한 샷 수를 가져갈 때 사용하는 함수입니다.
    // 예: Stage 1은 25발, Stage 2는 22발, Stage 3은 18발입니다.
    public int GetMaxShotCount()
    {
        // Inspector에 입력한 제한 샷 수를 돌려줍니다.
        return maxShotCount;
    }

    // 다른 스크립트에서 이 스테이지의 제한 시간을 가져갈 때 사용하는 함수입니다.
    // 예: Stage 1은 90초, Stage 2는 75초, Stage 3은 60초입니다.
    public float GetTimeLimit()
    {
        // Inspector에 입력한 제한 시간을 돌려줍니다.
        return timeLimit;
    }

    // 다른 스크립트에서 이 스테이지에 장애물이 있는지 확인할 때 사용하는 함수입니다.
    // true면 장애물이 있는 스테이지, false면 장애물이 없는 스테이지입니다.
    public bool HasObstacles()
    {
        // Inspector에서 체크한 장애물 유무를 돌려줍니다.
        return hasObstacles;
    }

    // ============================================================
    // [내부 함수 - 이 스크립트 안에서만 사용하는 함수]
    // ============================================================

    // 스테이지 이름을 화면에 표시하는 함수입니다.
    // ShowStageInfo()에서 호출됩니다.
    private void ShowStageNameOnScreen()
    {
        // TextMeshPro 텍스트가 연결되어 있으면 화면에 표시합니다.
        if (stageNameText != null)
        {
            // 혹시 숨김 Coroutine이 이전에 실행 중이었다면 멈춥니다.
            StopHideCoroutine();

            // 텍스트 오브젝트를 다시 보이게 합니다.
            stageNameText.gameObject.SetActive(true);

            // TextMeshProUGUI의 text 속성에 스테이지 이름을 넣습니다.
            stageNameText.text = stageName;

            // hideDelay초 뒤에 자동으로 숨기는 Coroutine을 시작합니다.
            hideCoroutine = StartCoroutine(HideStageNameAfterDelay());

            Debug.Log($"[기능 42] 스테이지 이름 표시: {stageName} ({hideDelay}초 뒤 사라짐)");
        }
        else
        {
            // TextMeshPro가 연결되어 있지 않으면 Console에만 출력합니다.
            Debug.Log($"[기능 42] 스테이지 이름: {stageName} (TextMeshPro가 연결되지 않아 Console에만 출력합니다.)");
        }
    }

    // 배경 이미지를 자동으로 바꾸는 함수입니다.
    // ShowStageInfo()에서 호출됩니다.
    //
    // [이 함수가 하는 일]
    // 1. backgroundController가 Inspector에 연결되어 있는지 확인합니다.
    // 2. 연결되어 있으면 SetStageBackground(stageNumber)를 호출합니다.
    // 3. 연결되어 있지 않으면 Console에 로그만 출력합니다.
    private void ApplyBackground()
    {
        // StageBackgroundController가 Inspector에 연결되어 있으면 배경을 바꿉니다.
        if (backgroundController != null)
        {
            // StageBackgroundController의 SetStageBackground() 함수를 호출합니다.
            // 이 함수는 Inspector에 연결된 배경 Sprite 중 stageNumber에 맞는 것을 화면에 적용합니다.
            backgroundController.SetStageBackground(stageNumber);

            Debug.Log($"[기능 42] 배경 이미지 변경: Stage {stageNumber}");
        }
        else
        {
            // StageBackgroundController가 연결되어 있지 않으면 Console에만 출력합니다.
            Debug.Log($"[기능 42] 배경 이미지: Stage {stageNumber} (StageBackgroundController가 연결되지 않아 Console에만 출력합니다.)");
        }
    }

    // 기능 43 테스트용 로그를 출력하는 함수입니다.
    // 이 함수는 ShowStageInfo()에서 호출됩니다.
    // Inspector에 연결한 버블 Sprite 개수와 colorCount가 맞는지 확인할 수 있습니다.
    private void LogStageBubbleColorData()
    {
        // stageBubbleSprites가 비어 있으면 0개로 봅니다.
        int spriteSlotCount = stageBubbleSprites != null ? stageBubbleSprites.Length : 0;

        // 배열 칸은 있지만 실제 Sprite가 연결되지 않은 칸도 있을 수 있으므로,
        // null이 아닌 Sprite만 따로 세어줍니다.
        int connectedSpriteCount = 0;
        for (int i = 0; i < spriteSlotCount; i++)
        {
            if (stageBubbleSprites[i] != null)
            {
                connectedSpriteCount++;
            }
        }

        Debug.Log($"[기능 43] Stage {stageNumber} 색 설정: 사용할 색 {colorCount}개, 연결된 버블 Sprite {connectedSpriteCount}/{spriteSlotCount}개");
    }

    // 기능 44 테스트용 로그를 출력하는 함수입니다.
    // 이 함수는 ShowStageInfo()에서 호출됩니다.
    // Inspector에 입력한 배치 설정이 맞는지 확인할 수 있습니다.
    private void LogStageLayoutData()
    {
        // startBubblePattern이 비어 있으면 0개로 봅니다.
        int patternCount = startBubblePattern != null ? startBubblePattern.Length : 0;

        Debug.Log($"[기능 44] Stage {stageNumber} 배치 설정: {startRows}줄 x {startCols}칸, 패턴 {patternCount}개");
    }

    // 기능 45 테스트용 로그를 출력하는 함수입니다.
    // 이 함수는 ShowStageInfo()에서 호출됩니다.
    // Inspector에 입력한 제한 샷 수가 맞는지 확인할 수 있습니다.
    private void LogStageShotLimitData()
    {
        Debug.Log($"[기능 45] Stage {stageNumber} 제한 샷 수: {maxShotCount}발");
    }

    // 기능 46 테스트용 로그를 출력하는 함수입니다.
    // 이 함수는 ShowStageInfo()에서 호출됩니다.
    // Inspector에 입력한 제한 시간이 맞는지 확인할 수 있습니다.
    private void LogStageTimeLimitData()
    {
        Debug.Log($"[기능 46] Stage {stageNumber} 제한 시간: {timeLimit}초");
    }

    // 기능 47 테스트용 로그를 출력하는 함수입니다.
    // 이 함수는 ShowStageInfo()에서 호출됩니다.
    // Inspector에서 체크한 장애물 유무가 맞는지 확인할 수 있습니다.
    private void LogStageObstacleData()
    {
        // hasObstacles가 true이면 "있음", false이면 "없음"을 표시합니다.
        string obstacleText = hasObstacles ? "있음" : "없음";

        Debug.Log($"[기능 47] Stage {stageNumber} 장애물: {obstacleText}");
    }

    // hideDelay초 뒤에 HideStageName()을 실행하는 Coroutine입니다.
    private System.Collections.IEnumerator HideStageNameAfterDelay()
    {
        // hideDelay초 동안 기다립니다.
        yield return new WaitForSeconds(hideDelay);

        // 기다린 뒤에 글자를 숨깁니다.
        HideStageName();
    }

    // 이전에 실행 중이던 숨김 Coroutine을 멈추는 함수입니다.
    private void StopHideCoroutine()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    // ============================================================
    // [Inspector 값이 바뀔 때 자동으로 호출되는 함수]
    // ============================================================

    // OnValidate는 Inspector에서 값이 바뀔 때 Unity Editor에서 자동으로 호출됩니다.
    private void OnValidate()
    {
        // stageName이 비어 있으면 기본값으로 "Stage 1"을 넣습니다.
        if (string.IsNullOrEmpty(stageName))
        {
            stageName = "Stage 1";
            Debug.LogWarning("[기능 42] 스테이지 이름이 비어 있어서 'Stage 1'로 고쳤습니다.");
        }

        // hideDelay가 0보다 작으면 0으로 고칩니다.
        if (hideDelay < 0f)
        {
            hideDelay = 0f;
        }

        // Inspector에서 stageNumber를 바꾸면 배경도 즉시 바뀌게 합니다.
        ApplyBackground();
    }
}
