using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// WallBoundsAutoSetup은 벽 충돌 영역을 자동으로 만들어주는 Editor 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class WallBoundsAutoSetup
{
    private const string WallsRootObjectName = "WallsRoot";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Walls를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Walls")]
    public static void SetupWalls()
    {
        // Play 중일 때는 Scene을 자동 수정하지 않습니다.
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.IsValid())
        {
            return;
        }

        WallBoundsController wallBoundsController = FindOrCreateWallsRoot();

        SerializedObject serializedWalls = new SerializedObject(wallBoundsController);
        serializedWalls.FindProperty("targetCamera").objectReferenceValue = Camera.main;
        serializedWalls.FindProperty("wallThickness").floatValue = 0.5f;
        serializedWalls.FindProperty("extraHeight").floatValue = 2f;
        serializedWalls.FindProperty("autoSetupOnStart").boolValue = true;
        serializedWalls.FindProperty("showDebugVisuals").boolValue = true;
        serializedWalls.FindProperty("debugColor").colorValue = new Color(1f, 0f, 0f, 0.25f);
        serializedWalls.ApplyModifiedPropertiesWithoutUndo();

        wallBoundsController.SetupWalls();

        EditorUtility.SetDirty(wallBoundsController);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("벽 자동 세팅 완료: LeftWall, RightWall, Ceiling Collider를 만들었습니다.");
    }

    // WallsRoot와 WallBoundsController를 찾거나 만드는 함수입니다.
    private static WallBoundsController FindOrCreateWallsRoot()
    {
        GameObject wallsRoot = GameObject.Find(WallsRootObjectName);

        if (wallsRoot == null)
        {
            wallsRoot = new GameObject(WallsRootObjectName);
            Undo.RegisterCreatedObjectUndo(wallsRoot, "Create Walls Root");
        }

        WallBoundsController wallBoundsController = wallsRoot.GetComponent<WallBoundsController>();

        if (wallBoundsController == null)
        {
            wallBoundsController = Undo.AddComponent<WallBoundsController>(wallsRoot);
        }

        return wallBoundsController;
    }
}
