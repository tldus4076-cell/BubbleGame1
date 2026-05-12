using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TimerGaugeAutoSetup은 Unity Editor에서 타이머 게이지 UI를 자동으로 만들어주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class TimerGaugeAutoSetup
{
    // 자동으로 찾거나 만들 오브젝트 이름입니다.
    private const string CanvasObjectName = "GameCanvas";
    private const string GaugeBackgroundObjectName = "TimerGaugeBackground";
    private const string GaugeFillObjectName = "TimerGaugeFill";
    private const string GaugeControllerObjectName = "TimerGaugeController";
    private const string TimerControllerObjectName = "TimerController";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Timer Gauge를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Timer Gauge")]
    public static void SetupTimerGauge()
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

        // 게이지 배경과 채워지는 이미지를 만들거나 찾습니다.
        Image gaugeBackgroundImage = FindOrCreateGaugeBackground(canvas.transform);
        Image gaugeFillImage = FindOrCreateGaugeFill(gaugeBackgroundImage.transform);

        // TimerGaugeController를 만들거나 찾습니다.
        TimerGaugeController gaugeController = FindOrCreateGaugeController();

        // TimerController를 찾습니다.
        TimerController timerController = FindTimerController();

        // private 변수도 Inspector에 저장된 값으로 안전하게 넣기 위해 SerializedObject를 사용합니다.
        SerializedObject serializedGauge = new SerializedObject(gaugeController);
        serializedGauge.FindProperty("timerController").objectReferenceValue = timerController;
        serializedGauge.FindProperty("gaugeFillImage").objectReferenceValue = gaugeFillImage;
        serializedGauge.FindProperty("findReferencesOnStart").boolValue = true;
        serializedGauge.ApplyModifiedPropertiesWithoutUndo();

        // Scene이 바뀌었다고 Unity에 알려주고 저장합니다.
        EditorUtility.SetDirty(gaugeController);
        EditorUtility.SetDirty(gaugeBackgroundImage);
        EditorUtility.SetDirty(gaugeFillImage);
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("타이머 게이지 자동 세팅 완료: TimerGaugeBackground, TimerGaugeFill, TimerGaugeController를 준비했습니다.");
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

    // 게이지 배경 이미지를 찾거나 만드는 함수입니다.
    private static Image FindOrCreateGaugeBackground(Transform canvasTransform)
    {
        Transform existingGauge = canvasTransform.Find(GaugeBackgroundObjectName);

        if (existingGauge != null)
        {
            Image existingImage = existingGauge.GetComponent<Image>();
            if (existingImage != null)
            {
                return existingImage;
            }
        }

        GameObject gaugeObject = new GameObject(GaugeBackgroundObjectName);
        Undo.RegisterCreatedObjectUndo(gaugeObject, "Create Timer Gauge Background");
        gaugeObject.transform.SetParent(canvasTransform, false);

        Image gaugeBackgroundImage = Undo.AddComponent<Image>(gaugeObject);
        gaugeBackgroundImage.sprite = GetDefaultUISprite();
        gaugeBackgroundImage.color = new Color(0f, 0f, 0f, 0.45f);

        RectTransform rectTransform = gaugeBackgroundImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0f, -150f);
        rectTransform.sizeDelta = new Vector2(360f, 32f);

        return gaugeBackgroundImage;
    }

    // 실제로 줄어드는 게이지 이미지를 찾거나 만드는 함수입니다.
    private static Image FindOrCreateGaugeFill(Transform gaugeBackgroundTransform)
    {
        Transform existingFill = gaugeBackgroundTransform.Find(GaugeFillObjectName);

        if (existingFill != null)
        {
            Image existingImage = existingFill.GetComponent<Image>();
            if (existingImage != null)
            {
                ConfigureGaugeFill(existingImage);
                return existingImage;
            }
        }

        GameObject fillObject = new GameObject(GaugeFillObjectName);
        Undo.RegisterCreatedObjectUndo(fillObject, "Create Timer Gauge Fill");
        fillObject.transform.SetParent(gaugeBackgroundTransform, false);

        Image gaugeFillImage = Undo.AddComponent<Image>(fillObject);
        ConfigureGaugeFill(gaugeFillImage);

        return gaugeFillImage;
    }

    // TimerGaugeFill Image 설정을 적용하는 함수입니다.
    private static void ConfigureGaugeFill(Image gaugeFillImage)
    {
        gaugeFillImage.color = new Color(0.2f, 0.9f, 0.25f, 1f);
        gaugeFillImage.sprite = GetDefaultUISprite();
        gaugeFillImage.type = Image.Type.Filled;
        gaugeFillImage.fillMethod = Image.FillMethod.Horizontal;
        gaugeFillImage.fillOrigin = 0;
        gaugeFillImage.fillAmount = 1f;

        RectTransform rectTransform = gaugeFillImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    // TimerGaugeController 오브젝트를 찾거나 만드는 함수입니다.
    private static TimerGaugeController FindOrCreateGaugeController()
    {
        GameObject controllerObject = GameObject.Find(GaugeControllerObjectName);

        if (controllerObject == null)
        {
            controllerObject = new GameObject(GaugeControllerObjectName);
            Undo.RegisterCreatedObjectUndo(controllerObject, "Create Timer Gauge Controller");
        }

        TimerGaugeController gaugeController = controllerObject.GetComponent<TimerGaugeController>();
        if (gaugeController == null)
        {
            gaugeController = Undo.AddComponent<TimerGaugeController>(controllerObject);
        }

        return gaugeController;
    }

    // TimerController를 찾는 함수입니다.
    private static TimerController FindTimerController()
    {
        GameObject timerControllerObject = GameObject.Find(TimerControllerObjectName);

        if (timerControllerObject == null)
        {
            Debug.LogWarning("TimerController 오브젝트를 찾지 못했습니다. 먼저 Bubble Shooter > Setup Timer Text를 실행해주세요.");
            return null;
        }

        TimerController timerController = timerControllerObject.GetComponent<TimerController>();

        if (timerController == null)
        {
            Debug.LogWarning("TimerController 컴포넌트를 찾지 못했습니다. TimerController 오브젝트에 TimerController.cs가 붙어 있는지 확인해주세요.");
        }

        return timerController;
    }

    // Unity 기본 UI Sprite를 가져오는 함수입니다.
    // Image의 Source Image가 비어 있으면 Image Type, Fill Method 같은 설정이 보이지 않을 수 있습니다.
    private static Sprite GetDefaultUISprite()
    {
        // Unity가 기본으로 가지고 있는 UI Sprite를 가져옵니다.
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }
}
