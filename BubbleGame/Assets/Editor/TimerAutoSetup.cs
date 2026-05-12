using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TimerAutoSetup은 Unity Editor에서 타이머 UI를 자동으로 만들어주는 도구입니다.
// 이 파일은 Assets/Editor 폴더 안에 있으므로 게임 빌드에는 포함되지 않습니다.
public static class TimerAutoSetup
{
    // 자동으로 만들 오브젝트 이름들입니다.
    private const string CanvasObjectName = "GameCanvas";
    private const string TimerTextObjectName = "TimerText";
    private const string TimerControllerObjectName = "TimerController";

    // Unity 위쪽 메뉴에서 Bubble Shooter > Setup Timer Text를 누르면 실행됩니다.
    [MenuItem("Bubble Shooter/Setup Timer Text")]
    public static void SetupTimerText()
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

        // TimerText를 찾거나 새로 만듭니다.
        Text timerText = FindOrCreateTimerText(canvas.transform);

        // TimerText에 위치/크기/색/그림자를 맞추는 스크립트를 붙입니다.
        // 문자열로 찾으면 Unity 프로젝트 파일 갱신이 늦어도 Editor 컴파일 문제가 줄어듭니다.
        Component timerUIPositioner = AddTimerUIPositionerIfNeeded(timerText.gameObject);

        // TimerController 오브젝트를 찾거나 새로 만듭니다.
        TimerController timerController = FindOrCreateTimerController();

        // TimerController의 private 변수에 TimerText와 기본 시간을 연결합니다.
        SerializedObject serializedTimer = new SerializedObject(timerController);
        serializedTimer.FindProperty("timerText").objectReferenceValue = timerText;
        serializedTimer.FindProperty("startTime").floatValue = 60f;
        serializedTimer.FindProperty("currentTime").floatValue = 60f;
        serializedTimer.FindProperty("startOnPlay").boolValue = true;
        serializedTimer.ApplyModifiedPropertiesWithoutUndo();

        // Scene이 바뀌었다고 Unity에 알려주고 저장합니다.
        EditorUtility.SetDirty(timerController);
        if (timerUIPositioner != null)
        {
            EditorUtility.SetDirty(timerUIPositioner);
        }
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("타이머 숫자 자동 세팅 완료: GameCanvas, TimerText, TimerController를 준비했습니다.");
    }

    // Canvas를 찾거나 새로 만드는 함수입니다.
    private static Canvas FindOrCreateCanvas()
    {
        // 이름으로 Canvas 오브젝트를 찾습니다.
        GameObject canvasObject = GameObject.Find(CanvasObjectName);

        // 없으면 새로 만듭니다.
        if (canvasObject == null)
        {
            canvasObject = new GameObject(CanvasObjectName);
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Game Canvas");
        }

        // Canvas 컴포넌트를 가져오거나 추가합니다.
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = Undo.AddComponent<Canvas>(canvasObject);
        }

        // UI가 화면 위에 보이도록 Screen Space - Overlay로 설정합니다.
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // 화면 크기에 따라 UI 크기가 자연스럽게 맞춰지도록 CanvasScaler를 추가합니다.
        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = Undo.AddComponent<CanvasScaler>(canvasObject);
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        // UI 클릭 처리를 위한 GraphicRaycaster를 추가합니다.
        if (canvasObject.GetComponent<GraphicRaycaster>() == null)
        {
            Undo.AddComponent<GraphicRaycaster>(canvasObject);
        }

        return canvas;
    }

    // TimerText를 찾거나 새로 만드는 함수입니다.
    private static Text FindOrCreateTimerText(Transform canvasTransform)
    {
        // Canvas 아래에서 TimerText를 찾습니다.
        Transform existingTimerText = canvasTransform.Find(TimerTextObjectName);

        // 이미 있으면 그 TextMeshPro 컴포넌트를 사용합니다.
        if (existingTimerText != null)
        {
            Text existingText = existingTimerText.GetComponent<Text>();
            if (existingText != null)
            {
                // 이미 TimerText가 있더라도 위치와 크기가 잘못되어 있을 수 있으므로 다시 보기 좋게 고칩니다.
                ConfigureTimerText(existingText);
                return existingText;
            }
        }

        // TimerText 오브젝트를 새로 만듭니다.
        GameObject timerTextObject = new GameObject(TimerTextObjectName);
        Undo.RegisterCreatedObjectUndo(timerTextObject, "Create Timer Text");
        timerTextObject.transform.SetParent(canvasTransform, false);

        // Text는 Canvas 안에서 사용하는 기본 UI 글자 컴포넌트입니다.
        // TextMeshPro 필수 리소스가 없어도 동작해서 초보자가 세팅하기 쉽습니다.
        Text timerText = Undo.AddComponent<Text>(timerTextObject);

        // 새로 만든 TimerText의 글자 크기와 위치를 설정합니다.
        ConfigureTimerText(timerText);

        return timerText;
    }

    // TimerText가 화면 위쪽 중앙에 잘 보이도록 설정하는 함수입니다.
    private static void ConfigureTimerText(Text timerText)
    {
        // 처음 보일 글자입니다.
        timerText.text = "1:00";

        // 글자 크기를 크게 설정합니다.
        timerText.fontSize = 96;

        // 글자를 가운데 정렬합니다.
        timerText.alignment = TextAnchor.MiddleCenter;

        // 흰색으로 표시합니다.
        timerText.color = Color.white;

        // Unity 기본 폰트를 사용합니다.
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // RectTransform은 UI의 위치와 크기를 담당합니다.
        RectTransform rectTransform = timerText.GetComponent<RectTransform>();

        // 화면 위쪽 가운데를 기준점으로 사용합니다.
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);

        // 위쪽에서 80만큼 아래에 놓습니다.
        rectTransform.anchoredPosition = new Vector2(0f, -80f);

        // 글자가 들어갈 UI 박스 크기입니다.
        rectTransform.sizeDelta = new Vector2(300f, 120f);
    }

    // TimerController 오브젝트를 찾거나 새로 만드는 함수입니다.
    private static TimerController FindOrCreateTimerController()
    {
        // 이름으로 TimerController 오브젝트를 찾습니다.
        GameObject timerControllerObject = GameObject.Find(TimerControllerObjectName);

        // 없으면 새로 만듭니다.
        if (timerControllerObject == null)
        {
            timerControllerObject = new GameObject(TimerControllerObjectName);
            Undo.RegisterCreatedObjectUndo(timerControllerObject, "Create Timer Controller");
        }

        // TimerController 컴포넌트를 가져오거나 추가합니다.
        TimerController timerController = timerControllerObject.GetComponent<TimerController>();
        if (timerController == null)
        {
            timerController = Undo.AddComponent<TimerController>(timerControllerObject);
        }

        return timerController;
    }

    // TimerUIPositioner를 TimerText에 붙이는 함수입니다.
    private static Component AddTimerUIPositionerIfNeeded(GameObject timerTextObject)
    {
        // TimerUIPositioner 타입을 이름으로 찾습니다.
        Type positionerType = Type.GetType("TimerUIPositioner, Assembly-CSharp");

        // 타입을 찾지 못하면 안내 메시지만 보여주고 멈춥니다.
        if (positionerType == null)
        {
            Debug.LogWarning("TimerUIPositioner 스크립트를 아직 찾지 못했습니다. Unity 컴파일이 끝난 뒤 Bubble Shooter > Setup Timer Text를 다시 눌러주세요.");
            return null;
        }

        // 이미 붙어 있는지 확인합니다.
        Component positioner = timerTextObject.GetComponent(positionerType);

        // 없으면 새로 붙입니다.
        if (positioner == null)
        {
            positioner = Undo.AddComponent(timerTextObject, positionerType);
        }

        return positioner;
    }
}
