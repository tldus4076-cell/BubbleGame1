using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ShooterAutoSetup은 Unity Editor에서 슈터 오브젝트를 자동으로 만들어주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class ShooterAutoSetup
{
    private const string ShooterRootObjectName = "ShooterRoot";
    private const string ShooterVisualObjectName = "ShooterVisual";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Shooter를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Shooter")]
    public static void SetupShooter()
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

        // ShooterRoot와 필요한 컴포넌트를 준비합니다.
        ShooterController shooterController = FindOrCreateShooter();

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedShooter = new SerializedObject(shooterController);
        serializedShooter.FindProperty("sortingOrder").intValue = 10;
        serializedShooter.ApplyModifiedPropertiesWithoutUndo();

        // Scene이 바뀌었다고 Unity에 알려주고 저장합니다.
        EditorUtility.SetDirty(shooterController);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("슈터 자동 세팅 완료: ShooterRoot와 ShooterVisual을 준비했습니다. 위치는 Scene 창에서 직접 조절해주세요.");
    }

    // ShooterRoot와 ShooterController를 찾거나 만드는 함수입니다.
    private static ShooterController FindOrCreateShooter()
    {
        GameObject shooterRoot = GameObject.Find(ShooterRootObjectName);

        if (shooterRoot == null)
        {
            shooterRoot = new GameObject(ShooterRootObjectName);
            Undo.RegisterCreatedObjectUndo(shooterRoot, "Create Shooter Root");
        }

        ShooterController shooterController = shooterRoot.GetComponent<ShooterController>();

        if (shooterController == null)
        {
            shooterController = Undo.AddComponent<ShooterController>(shooterRoot);
        }

        SpriteRenderer shooterRenderer = FindOrCreateShooterVisual(shooterRoot.transform);

        SerializedObject serializedShooter = new SerializedObject(shooterController);
        serializedShooter.FindProperty("shooterRenderer").objectReferenceValue = shooterRenderer;
        serializedShooter.ApplyModifiedPropertiesWithoutUndo();

        return shooterController;
    }

    // ShooterVisual과 SpriteRenderer를 찾거나 만드는 함수입니다.
    private static SpriteRenderer FindOrCreateShooterVisual(Transform shooterRootTransform)
    {
        Transform existingVisual = shooterRootTransform.Find(ShooterVisualObjectName);

        if (existingVisual != null)
        {
            SpriteRenderer existingRenderer = existingVisual.GetComponent<SpriteRenderer>();

            if (existingRenderer != null)
            {
                return existingRenderer;
            }
        }

        GameObject visualObject = new GameObject(ShooterVisualObjectName);
        Undo.RegisterCreatedObjectUndo(visualObject, "Create Shooter Visual");
        visualObject.transform.SetParent(shooterRootTransform);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localRotation = Quaternion.identity;
        visualObject.transform.localScale = Vector3.one;

        SpriteRenderer shooterRenderer = Undo.AddComponent<SpriteRenderer>(visualObject);
        shooterRenderer.sortingOrder = 10;

        return shooterRenderer;
    }
}
