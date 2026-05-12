using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ScoreAutoSetup은 Unity Editor에서 점수 UI를 자동으로 만들어주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class ScoreAutoSetup
{
    private const string CanvasObjectName = "GameCanvas";
    private const string ScoreTextObjectName = "ScoreText";
    private const string ScoreControllerObjectName = "ScoreController";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Score Text를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Score Text")]
    public static void SetupScoreText()
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

        // Canvas를 찾거나 새로 만듭니다.
        Canvas canvas = FindOrCreateCanvas();

        // ScoreText를 찾거나 새로 만듭니다.
        Text scoreText = FindOrCreateScoreText(canvas.transform);

        // ScoreText에 그림자를 붙여 밝은 배경에서도 보이게 합니다.
        AddShadowIfNeeded(scoreText.gameObject);

        // ScoreText에 글자 스타일 전용 스크립트를 붙입니다.
        Component scoreStyleController = AddScoreUIStyleControllerIfNeeded(scoreText.gameObject);

        // ScoreController를 찾거나 새로 만듭니다.
        ScoreController scoreController = FindOrCreateScoreController();

        // ScoreController의 private 변수에 ScoreText와 기본 점수를 연결합니다.
        SerializedObject serializedScore = new SerializedObject(scoreController);
        serializedScore.FindProperty("scoreText").objectReferenceValue = scoreText;
        serializedScore.FindProperty("currentScore").intValue = 0;
        serializedScore.FindProperty("showNumberOnly").boolValue = true;
        serializedScore.ApplyModifiedPropertiesWithoutUndo();

        // 화면 글자를 즉시 0으로 맞춥니다.
        scoreText.text = "0";

        // Scene이 바뀌었다고 Unity에 알려주고 저장합니다.
        EditorUtility.SetDirty(scoreController);
        EditorUtility.SetDirty(scoreText);
        if (scoreStyleController != null)
        {
            EditorUtility.SetDirty(scoreStyleController);
        }
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("점수 숫자 자동 세팅 완료: ScoreText와 ScoreController를 준비했습니다.");
    }

    // Canvas를 찾거나 새로 만드는 함수입니다.
    private static Canvas FindOrCreateCanvas()
    {
        GameObject canvasObject = GameObject.Find(CanvasObjectName);

        if (canvasObject == null)
        {
            canvasObject = new GameObject(CanvasObjectName);
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Game Canvas");
        }

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = Undo.AddComponent<Canvas>(canvasObject);
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = Undo.AddComponent<CanvasScaler>(canvasObject);
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        if (canvasObject.GetComponent<GraphicRaycaster>() == null)
        {
            Undo.AddComponent<GraphicRaycaster>(canvasObject);
        }

        return canvas;
    }

    // ScoreText를 찾거나 새로 만드는 함수입니다.
    private static Text FindOrCreateScoreText(Transform canvasTransform)
    {
        Transform existingScoreText = canvasTransform.Find(ScoreTextObjectName);

        if (existingScoreText != null)
        {
            Text existingText = existingScoreText.GetComponent<Text>();
            if (existingText != null)
            {
                // 기존 ScoreText가 있으면 위치와 크기는 건드리지 않고 글자 스타일만 보정합니다.
                ConfigureScoreTextVisualOnly(existingText);
                return existingText;
            }
        }

        GameObject scoreTextObject = new GameObject(ScoreTextObjectName);
        Undo.RegisterCreatedObjectUndo(scoreTextObject, "Create Score Text");
        scoreTextObject.transform.SetParent(canvasTransform, false);

        Text scoreText = Undo.AddComponent<Text>(scoreTextObject);
        ConfigureNewScoreText(scoreText);

        return scoreText;
    }

    // 새로 만든 ScoreText의 기본 위치와 모양을 설정합니다.
    private static void ConfigureNewScoreText(Text scoreText)
    {
        ConfigureScoreTextVisualOnly(scoreText);

        RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(-120f, -80f);
        rectTransform.sizeDelta = new Vector2(220f, 100f);
        rectTransform.localScale = Vector3.one;
    }

    // ScoreText의 위치와 크기는 건드리지 않고 글자 모양만 설정합니다.
    private static void ConfigureScoreTextVisualOnly(Text scoreText)
    {
        scoreText.fontSize = 64;
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.color = Color.white;
        scoreText.raycastTarget = false;

        if (scoreText.font == null)
        {
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }

    // ScoreText에 그림자를 붙이는 함수입니다.
    private static void AddShadowIfNeeded(GameObject scoreTextObject)
    {
        Shadow shadow = scoreTextObject.GetComponent<Shadow>();

        if (shadow == null)
        {
            shadow = Undo.AddComponent<Shadow>(scoreTextObject);
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        shadow.effectDistance = new Vector2(3f, -3f);
        shadow.useGraphicAlpha = true;
    }

    // ScoreController 오브젝트를 찾거나 새로 만드는 함수입니다.
    private static ScoreController FindOrCreateScoreController()
    {
        GameObject scoreControllerObject = GameObject.Find(ScoreControllerObjectName);

        if (scoreControllerObject == null)
        {
            scoreControllerObject = new GameObject(ScoreControllerObjectName);
            Undo.RegisterCreatedObjectUndo(scoreControllerObject, "Create Score Controller");
        }

        ScoreController scoreController = scoreControllerObject.GetComponent<ScoreController>();
        if (scoreController == null)
        {
            scoreController = Undo.AddComponent<ScoreController>(scoreControllerObject);
        }

        return scoreController;
    }

    // ScoreUIStyleController를 ScoreText에 붙이는 함수입니다.
    private static Component AddScoreUIStyleControllerIfNeeded(GameObject scoreTextObject)
    {
        Type styleType = Type.GetType("ScoreUIStyleController, Assembly-CSharp");

        if (styleType == null)
        {
            Debug.LogWarning("ScoreUIStyleController 스크립트를 아직 찾지 못했습니다. Unity 컴파일이 끝난 뒤 Bubble Shooter > Setup Score Text를 다시 눌러주세요.");
            return null;
        }

        Component styleController = scoreTextObject.GetComponent(styleType);

        if (styleController == null)
        {
            styleController = Undo.AddComponent(scoreTextObject, styleType);
        }

        return styleController;
    }
}
