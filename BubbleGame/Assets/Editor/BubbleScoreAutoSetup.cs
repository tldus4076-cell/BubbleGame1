using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// BubbleScoreAutoSetup은 BubbleScoreManager 오브젝트를 자동으로 만들어주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class BubbleScoreAutoSetup
{
    private const string BubbleScoreManagerObjectName = "BubbleScoreManager";
    private const string ScoreControllerObjectName = "ScoreController";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Bubble Score Manager를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Bubble Score Manager")]
    public static void SetupBubbleScoreManager()
    {
        // Play 중일 때는 Scene을 자동 수정하지 않습니다.
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        // 현재 열려 있는 Scene을 가져옵니다.
        Scene activeScene = SceneManager.GetActiveScene();

        // Scene이 올바르지 않으면 멈춥니다.
        if (!activeScene.IsValid())
        {
            return;
        }

        // BubbleScoreManager를 찾거나 새로 만듭니다.
        BubbleScoreManager bubbleScoreManager = FindOrCreateBubbleScoreManager();

        // ScoreController를 찾습니다.
        ScoreController scoreController = FindScoreController();

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedManager = new SerializedObject(bubbleScoreManager);
        serializedManager.FindProperty("scoreController").objectReferenceValue = scoreController;
        serializedManager.FindProperty("scorePerBubble").intValue = 10;
        serializedManager.FindProperty("useKeyboardTest").boolValue = true;
        serializedManager.FindProperty("testRemovedBubbleCount").intValue = 3;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        // Scene이 바뀌었다고 Unity에 알려주고 저장합니다.
        EditorUtility.SetDirty(bubbleScoreManager);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("버블 점수 매니저 자동 세팅 완료: Play 중 T 키를 누르면 3개 제거 테스트로 점수가 올라갑니다.");
    }

    // BubbleScoreManager 오브젝트를 찾거나 새로 만드는 함수입니다.
    private static BubbleScoreManager FindOrCreateBubbleScoreManager()
    {
        GameObject managerObject = GameObject.Find(BubbleScoreManagerObjectName);

        if (managerObject == null)
        {
            managerObject = new GameObject(BubbleScoreManagerObjectName);
            Undo.RegisterCreatedObjectUndo(managerObject, "Create Bubble Score Manager");
        }

        BubbleScoreManager bubbleScoreManager = managerObject.GetComponent<BubbleScoreManager>();

        if (bubbleScoreManager == null)
        {
            bubbleScoreManager = Undo.AddComponent<BubbleScoreManager>(managerObject);
        }

        return bubbleScoreManager;
    }

    // ScoreController를 찾는 함수입니다.
    private static ScoreController FindScoreController()
    {
        GameObject scoreControllerObject = GameObject.Find(ScoreControllerObjectName);

        if (scoreControllerObject == null)
        {
            Debug.LogWarning("ScoreController 오브젝트를 찾지 못했습니다. 먼저 Bubble Shooter > Setup Score Text를 실행해주세요.");
            return null;
        }

        ScoreController scoreController = scoreControllerObject.GetComponent<ScoreController>();

        if (scoreController == null)
        {
            Debug.LogWarning("ScoreController 컴포넌트를 찾지 못했습니다. ScoreController 오브젝트에 ScoreController.cs가 붙어 있는지 확인해주세요.");
        }

        return scoreController;
    }
}
