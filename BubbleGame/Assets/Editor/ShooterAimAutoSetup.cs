using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ShooterAimAutoSetup은 ShooterRoot에 ShooterAimController를 자동으로 붙여주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class ShooterAimAutoSetup
{
    private const string ShooterRootObjectName = "ShooterRoot";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Shooter Aim을 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Shooter Aim")]
    public static void SetupShooterAim()
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

        // ShooterRoot를 찾습니다.
        GameObject shooterRoot = GameObject.Find(ShooterRootObjectName);

        if (shooterRoot == null)
        {
            Debug.LogWarning("ShooterRoot를 찾지 못했습니다. 먼저 Bubble Shooter > Setup Shooter를 실행해주세요.");
            return;
        }

        // ShooterRoot에 ShooterAimController를 붙입니다.
        ShooterAimController aimController = shooterRoot.GetComponent<ShooterAimController>();

        if (aimController == null)
        {
            aimController = Undo.AddComponent<ShooterAimController>(shooterRoot);
        }

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedAim = new SerializedObject(aimController);
        serializedAim.FindProperty("targetCamera").objectReferenceValue = Camera.main;
        serializedAim.FindProperty("rotationTarget").objectReferenceValue = shooterRoot.transform;
        serializedAim.FindProperty("angleOffset").floatValue = -90f;
        serializedAim.FindProperty("rotateSpeed").floatValue = 0f;
        serializedAim.FindProperty("aimEnabled").boolValue = true;
        serializedAim.FindProperty("useMouseAim").boolValue = true;
        serializedAim.FindProperty("useKeyboardAim").boolValue = true;
        serializedAim.FindProperty("keyboardRotationSpeed").floatValue = 120f;
        serializedAim.FindProperty("keyboardUpAngle").floatValue = 90f;
        serializedAim.FindProperty("useAimLimit").boolValue = true;
        serializedAim.FindProperty("minAimAngle").floatValue = 30f;
        serializedAim.FindProperty("maxAimAngle").floatValue = 150f;
        serializedAim.ApplyModifiedPropertiesWithoutUndo();

        // ShooterRoot 위치는 절대 바꾸지 않습니다. 회전 스크립트만 붙입니다.
        EditorUtility.SetDirty(aimController);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("슈터 조준 자동 세팅 완료: ShooterRoot 위치는 유지하고 마우스 조준 회전만 추가했습니다.");
    }
}
