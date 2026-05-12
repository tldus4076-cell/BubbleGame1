using UnityEngine;
using UnityEngine.UI;

// TimerUIPositioner는 TimerText의 위치, 크기, 색, 그림자를 보기 좋게 맞추는 스크립트입니다.
// 타이머 시간이 줄어드는 기능은 TimerController가 담당하고, 이 스크립트는 UI 모양만 담당합니다.
// 직접 Scene 창에서 TimerText를 옮길 수 있도록 Play 시작 때 자동 위치 보정은 하지 않습니다.
public class TimerUIPositioner : MonoBehaviour
{
    [Header("위치 설정")]
    [Tooltip("TimerText를 화면 위쪽 중앙에서 얼마나 옮길지 정합니다. X는 좌우, Y는 위아래입니다.")]
    [SerializeField] private Vector2 anchoredPosition = new Vector2(0f, -80f);

    [Tooltip("TimerText 박스의 크기입니다. 글자가 잘리지 않으면 됩니다.")]
    [SerializeField] private Vector2 size = new Vector2(320f, 120f);

    [Header("글자 설정")]
    [Tooltip("타이머 글자 크기입니다.")]
    [SerializeField] private int fontSize = 96;

    [Tooltip("타이머 글자 색입니다. 밝은 배경 위에서는 흰색을 추천합니다.")]
    [SerializeField] private Color textColor = Color.white;

    [Header("그림자 설정")]
    [Tooltip("타이머 글자 뒤에 그림자를 사용할지 정합니다.")]
    [SerializeField] private bool useShadow = true;

    [Tooltip("그림자 색입니다. 검은색 반투명을 추천합니다.")]
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.75f);

    [Tooltip("그림자가 글자에서 얼마나 떨어져 보일지 정합니다.")]
    [SerializeField] private Vector2 shadowDistance = new Vector2(3f, -3f);

    // RectTransform은 UI의 위치와 크기를 담당합니다.
    private RectTransform rectTransform;

    // Text는 Unity 기본 UI 글자를 담당합니다.
    private Text timerText;

    // Shadow는 Unity 기본 UI 그림자를 담당합니다.
    private Shadow shadow;

    // TimerText의 위치, 크기, 글자 색, 그림자를 한 번에 적용하는 함수입니다.
    // 이 함수는 Bubble Shooter > Setup Timer Text 메뉴를 눌렀을 때만 사용합니다.
    // 평소에는 직접 옮긴 TimerText 위치를 덮어쓰지 않습니다.
    public void ApplyUISettings()
    {
        // 필요한 컴포넌트를 먼저 찾습니다.
        FindComponentsIfNeeded();

        // 위치와 크기를 맞춥니다.
        ApplyPositionAndSize();

        // 글자 모양을 맞춥니다.
        ApplyTextStyle();

        // 그림자를 맞춥니다.
        ApplyShadowStyle();
    }

    // 필요한 컴포넌트를 자동으로 찾는 함수입니다.
    private void FindComponentsIfNeeded()
    {
        // RectTransform이 비어 있으면 이 오브젝트에서 찾습니다.
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // Text가 비어 있으면 이 오브젝트에서 찾습니다.
        if (timerText == null)
        {
            timerText = GetComponent<Text>();
        }
    }

    // 위치와 크기를 적용하는 함수입니다.
    private void ApplyPositionAndSize()
    {
        // RectTransform이 없으면 UI 위치를 바꿀 수 없으므로 멈춥니다.
        if (rectTransform == null)
        {
            return;
        }

        // 화면 위쪽 중앙을 기준점으로 사용합니다.
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);

        // Inspector에서 정한 위치와 크기를 적용합니다.
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        rectTransform.localScale = Vector3.one;
    }

    // 글자 크기, 색, 정렬을 적용하는 함수입니다.
    private void ApplyTextStyle()
    {
        // Text가 없으면 글자 모양을 바꿀 수 없으므로 멈춥니다.
        if (timerText == null)
        {
            return;
        }

        // 글자가 잘 보이도록 크게 만들고 가운데 정렬합니다.
        timerText.fontSize = fontSize;
        timerText.color = textColor;
        timerText.alignment = TextAnchor.MiddleCenter;
        timerText.raycastTarget = false;

        // 기본 폰트가 없을 때 Unity 기본 폰트를 넣습니다.
        if (timerText.font == null)
        {
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }

    // 그림자를 적용하는 함수입니다.
    private void ApplyShadowStyle()
    {
        // 그림자를 사용하지 않으면 Shadow 컴포넌트를 꺼둡니다.
        if (!useShadow)
        {
            if (shadow != null)
            {
                shadow.enabled = false;
            }

            return;
        }

        // Shadow가 없으면 새로 붙입니다.
        if (shadow == null)
        {
            shadow = GetComponent<Shadow>();
        }

        if (shadow == null)
        {
            shadow = gameObject.AddComponent<Shadow>();
        }

        // 그림자 색과 거리를 적용합니다.
        shadow.enabled = true;
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowDistance;
        shadow.useGraphicAlpha = true;
    }
}
