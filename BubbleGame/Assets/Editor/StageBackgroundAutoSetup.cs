using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// StageBackgroundAutoSetup은 Unity Editor에서 Stage 배경을 자동으로 연결해주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있어야 하며, 게임 빌드에는 포함되지 않습니다.
[InitializeOnLoad]
public static class StageBackgroundAutoSetup
{
    // Stage 1 배경으로 사용할 이미지 경로입니다.
    private const string Stage1SpritePath = "Assets/Image/1.png";

    // Stage 2 배경으로 사용할 이미지 경로입니다.
    // 파일이 아직 없어도 오류가 나지 않게 선택적으로만 연결합니다.
    private const string Stage2SpritePath = "Assets/Image/2.png";

    // Stage 3 배경으로 사용할 이미지 경로입니다.
    // 파일이 아직 없어도 오류가 나지 않게 선택적으로만 연결합니다.
    private const string Stage3SpritePath = "Assets/Image/3.png";

    // 배경 기능을 담당하는 오브젝트 이름입니다.
    private const string ControllerObjectName = "StageBackgroundController";

    // Unity Editor가 스크립트를 다시 읽을 때 자동으로 호출됩니다.
    static StageBackgroundAutoSetup()
    {
        // Unity가 완전히 준비된 다음 실행되도록 살짝 늦게 호출합니다.
        EditorApplication.delayCall += SetupStage1BackgroundIfNeeded;
    }

    // 메뉴에서도 직접 실행할 수 있게 만듭니다.
    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Stage 1 Background를 누르면 됩니다.
    [MenuItem("Bubble Shooter/Setup Stage 1 Background")]
    public static void SetupStage1BackgroundIfNeeded()
    {
        // Play 중일 때는 Scene을 자동 수정하지 않습니다.
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        // 현재 열려 있는 Scene을 가져옵니다.
        Scene activeScene = SceneManager.GetActiveScene();

        // Scene이 없거나 수정할 수 없는 상태라면 멈춥니다.
        if (!activeScene.IsValid())
        {
            return;
        }

        // Assets/Image/1.png에서 Stage 1 Sprite를 찾습니다.
        Sprite stage1Sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Stage1SpritePath);

        // Assets/Image/2.png에서 Stage 2 Sprite를 찾습니다.
        // 아직 Stage 2 이미지가 없으면 null이 들어갑니다.
        Sprite stage2Sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Stage2SpritePath);

        // Assets/Image/3.png에서 Stage 3 Sprite를 찾습니다.
        // 아직 Stage 3 이미지가 없으면 null이 들어갑니다.
        Sprite stage3Sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Stage3SpritePath);

        // Sprite를 찾지 못하면 오류를 알려주고 멈춥니다.
        if (stage1Sprite == null)
        {
            Debug.LogWarning("Stage 1 배경 Sprite를 찾지 못했습니다. Assets/Image/1.png의 Texture Type을 Sprite (2D and UI)로 바꾼 뒤 Apply를 눌러주세요.");
            return;
        }

        // Scene 안에서 StageBackgroundController 오브젝트를 찾습니다.
        GameObject controllerObject = GameObject.Find(ControllerObjectName);

        // 없다면 새로 만듭니다.
        if (controllerObject == null)
        {
            controllerObject = new GameObject(ControllerObjectName);
            Undo.RegisterCreatedObjectUndo(controllerObject, "Create Stage Background Controller");
        }

        // 오브젝트에 StageBackgroundController 스크립트가 붙어 있는지 확인합니다.
        StageBackgroundController controller = controllerObject.GetComponent<StageBackgroundController>();

        // 없다면 붙입니다.
        if (controller == null)
        {
            controller = Undo.AddComponent<StageBackgroundController>(controllerObject);
        }

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedController = new SerializedObject(controller);

        // Main Camera를 찾아서 targetCamera에 넣습니다.
        Camera mainCamera = Camera.main;
        serializedController.FindProperty("targetCamera").objectReferenceValue = mainCamera;

        // Assets/Image/1.png Sprite를 stage1BackgroundSprite에 넣습니다.
        serializedController.FindProperty("stage1BackgroundSprite").objectReferenceValue = stage1Sprite;

        // Assets/Image/2.png Sprite가 있다면 stage2BackgroundSprite에 넣습니다.
        // 없다면 기존에 사용자가 Inspector에서 넣어둔 값을 건드리지 않습니다.
        if (stage2Sprite != null)
        {
            serializedController.FindProperty("stage2BackgroundSprite").objectReferenceValue = stage2Sprite;
        }

        // Assets/Image/3.png Sprite가 있다면 stage3BackgroundSprite에 넣습니다.
        // 없다면 기존에 사용자가 Inspector에서 넣어둔 값을 건드리지 않습니다.
        if (stage3Sprite != null)
        {
            serializedController.FindProperty("stage3BackgroundSprite").objectReferenceValue = stage3Sprite;
        }

        // 배경이 뒤에 보이도록 Order in Layer를 낮게 설정합니다.
        serializedController.FindProperty("backgroundOrderInLayer").intValue = -100;

        // 일반적인 2D 카메라에서 보이도록 Z 위치를 10으로 설정합니다.
        serializedController.FindProperty("backgroundZPosition").floatValue = 10f;

        // 처음에는 Stage 1 배경이 보이도록 현재 Stage 번호를 1로 설정합니다.
        serializedController.FindProperty("currentStageNumber").intValue = 1;

        // 변경한 Inspector 값을 실제 오브젝트에 적용합니다.
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        // 배경 오브젝트를 만들고 Scene/Game 화면에 맞게 크기를 조절합니다.
        controller.RefreshBackground();

        // Scene이 바뀌었다고 Unity에 알려줍니다.
        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(activeScene);

        // 자동 저장합니다. 기능 하나 완성 후 저장하라는 조건을 지키기 위한 부분입니다.
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("Stage 배경 자동 세팅 완료: Stage 1 배경은 화면에 표시하고, Stage 2/Stage 3 배경 칸은 나중에 사용할 수 있게 준비했습니다.");
    }
}
