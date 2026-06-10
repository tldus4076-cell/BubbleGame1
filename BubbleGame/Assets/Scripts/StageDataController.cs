using UnityEngine;

// ============================================================
// StageDataController는 스테이지 이름을 저장하고 화면에 보여주는 스크립트입니다.
//
// [이 스크립트가 필요한 이유]
// 게임에는 Stage 1, Stage 2, Stage 3 같은 이름이 필요합니다.
// 플레이어가 지금 어떤 스테이지를 플레이하고 있는지 알 수 있게 해줍니다.
//
// [어떻게 동작하나요?]
// 1. Inspector에서 스테이지 이름을 직접 입력합니다. (예: "Stage 1")
// 2. TextMeshPro 텍스트 오브젝트를 연결하면 화면에 이름이 표시됩니다.
// 3. TextMeshPro를 연결하지 않으면 Unity Console에 이름이 출력됩니다.
//
// [Inspector에서 연결할 것]
// - Stage Name Text: 화면에 스테이지 이름을 보여줄 TextMeshProUGUI (선택)
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

    [Header("화면 표시 설정")]

    [Tooltip("화면에 스테이지 이름을 보여줄 TextMeshPro 텍스트입니다. 비워두면 Console에만 출력합니다.")]
    [SerializeField] private TMPro.TextMeshProUGUI stageNameText;

    [Tooltip("게임 시작 시 자동으로 스테이지 이름을 표시할지 여부입니다.")]
    [SerializeField] private bool showOnStart = true;

    // ============================================================
    // [실행 흐름]
    //
    // 1. Unity가 이 스크립트가 붙은 오브젝트를 찾습니다.
    // 2. Awake()가 먼저 실행됩니다. (Start보다 빠름)
    // 3. Start()가 실행됩니다. (게임 시작 시 한 번)
    // 4. showOnStart가 true이면 스테이지 이름을 화면에 표시합니다.
    // ============================================================

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    // 지금은 특별히 준비할 것이 없어서 비워둡니다.
    private void Awake()
    {
        // 나중에 여기에 초기화 코드를 넣을 수 있습니다.
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 여기에서 스테이지 이름을 화면에 표시합니다.
    private void Start()
    {
        // showOnStart가 true이면 스테이지 이름을 표시합니다.
        if (showOnStart)
        {
            ShowStageName();
        }
    }

    // ============================================================
    // [공개 함수 - 다른 스크립트에서 호출할 수 있는 함수]
    // ============================================================

    // 스테이지 이름을 화면에 표시하는 함수입니다.
    // Inspector에서 stageName에 입력한 텍스트를 화면에 보여줍니다.
    public void ShowStageName()
    {
        // TextMeshPro 텍스트가 연결되어 있으면 화면에 표시합니다.
        if (stageNameText != null)
        {
            // TextMeshProUGUI의 text 속성에 스테이지 이름을 넣습니다.
            stageNameText.text = stageName;
            Debug.Log($"[기능 41] 스테이지 이름 표시: {stageName}");
        }
        else
        {
            // TextMeshPro가 연결되어 있지 않으면 Console에만 출력합니다.
            Debug.Log($"[기능 41] 스테이지 이름: {stageName} (TextMeshPro가 연결되지 않아 Console에만 출력합니다.)");
        }
    }

    // Inspector에서 스테이지 이름을 바꿀 때 호출할 수 있는 함수입니다.
    // 나중에 스테이지 전환 기능에서 사용할 수 있습니다.
    public void SetStageName(string newName)
    {
        // 새로운 이름을 저장합니다.
        stageName = newName;

        // 화면에 바로 반영합니다.
        ShowStageName();
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
            Debug.LogWarning("[기능 41] 스테이지 번호는 1 이상이어야 합니다. 1로 고쳤습니다.");
        }

        // 새로운 번호를 저장합니다.
        stageNumber = newNumber;

        // 화면에 바로 반영합니다.
        ShowStageName();
    }

    // ============================================================
    // [Inspector 값이 바뀔 때 자동으로 호출되는 함수]
    // ============================================================

    // OnValidate는 Inspector에서 값이 바뀔 때 Unity Editor에서 자동으로 호출됩니다.
    // stageName이나 stageNumber를 Inspector에서 바꾸면 바로 화면에 반영됩니다.
    private void OnValidate()
    {
        // 스테이지 번호가 1보다 작으면 1로 고칩니다.
        if (stageNumber < 1)
        {
            stageNumber = 1;
        }

        // stageName이 비어 있으면 기본값으로 "Stage 1"을 넣습니다.
        if (string.IsNullOrEmpty(stageName))
        {
            stageName = "Stage 1";
            Debug.LogWarning("[기능 41] 스테이지 이름이 비어 있어서 'Stage 1'로 고쳤습니다.");
        }
    }
}
