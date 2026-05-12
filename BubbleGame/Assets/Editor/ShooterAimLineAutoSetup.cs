using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ShooterAimLineAutoSetup은 ShooterRoot에 조준선 스크립트와 LineRenderer를 자동으로 붙여주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class ShooterAimLineAutoSetup
{
    private const string ShooterRootObjectName = "ShooterRoot";
    private const string ShooterVisualObjectName = "ShooterVisual";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Shooter Aim Line을 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Shooter Aim Line")]
    public static void SetupShooterAimLine()
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

        // ShooterRoot에 조준선 스크립트를 붙입니다.
        ShooterAimLineController aimLineController = shooterRoot.GetComponent<ShooterAimLineController>();

        if (aimLineController == null)
        {
            aimLineController = Undo.AddComponent<ShooterAimLineController>(shooterRoot);
        }

        // ShooterVisual을 방향 기준으로 사용합니다.
        Transform shooterVisual = shooterRoot.transform.Find(ShooterVisualObjectName);

        // LineRenderer를 준비합니다.
        LineRenderer lineRenderer = shooterRoot.GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = Undo.AddComponent<LineRenderer>(shooterRoot);
        }

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedLine = new SerializedObject(aimLineController);
        serializedLine.FindProperty("showAimLine").boolValue = true;
        serializedLine.FindProperty("aimDirectionSource").objectReferenceValue = shooterVisual != null ? shooterVisual : shooterRoot.transform;
        serializedLine.FindProperty("lineStartOffset").floatValue = 0.6f;
        serializedLine.FindProperty("lineLength").floatValue = 5f;
        serializedLine.FindProperty("lineWidth").floatValue = 0.05f;
        serializedLine.FindProperty("lineColor").colorValue = new Color(1f, 1f, 1f, 0.75f);
        serializedLine.FindProperty("sortingOrder").intValue = 20;
        serializedLine.FindProperty("aimLocalDirection").vector2Value = Vector2.up;
        serializedLine.ApplyModifiedPropertiesWithoutUndo();

        // ShooterRoot 위치는 절대 바꾸지 않습니다.
        EditorUtility.SetDirty(aimLineController);
        EditorUtility.SetDirty(lineRenderer);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("슈터 조준선 자동 세팅 완료: ShooterRoot 위치는 유지하고 조준선만 추가했습니다.");
    }
}
