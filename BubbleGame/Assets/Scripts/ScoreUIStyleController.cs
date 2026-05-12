using UnityEngine;
using UnityEngine.UI;

// ScoreUIStyleController는 ScoreText의 글자 색, 크기, 그림자, 외곽선만 담당하는 스크립트입니다.
// 점수 값은 ScoreController가 담당하므로 이 스크립트는 Text 값을 바꾸지 않습니다.
// ScoreText의 위치와 크기도 직접 옮긴 값을 유지해야 하므로 이 스크립트에서는 바꾸지 않습니다.
[ExecuteAlways]
public class ScoreUIStyleController : MonoBehaviour
{
    [Header("글자 설정")]
    [Tooltip("점수 글자 크기입니다.")]
    [SerializeField] private int fontSize = 64;

    [Tooltip("점수 글자 색입니다. 밝은 배경에서는 흰색을 추천합니다.")]
    [SerializeField] private Color textColor = Color.white;

    [Header("그림자 설정")]
    [Tooltip("점수 글자에 그림자를 사용할지 정합니다.")]
    [SerializeField] private bool useShadow = true;

    [Tooltip("그림자 색입니다. 검은색 반투명을 추천합니다.")]
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.7f);

    [Tooltip("그림자가 글자에서 얼마나 떨어져 보일지 정합니다.")]
    [SerializeField] private Vector2 shadowDistance = new Vector2(3f, -3f);

    [Header("외곽선 설정")]
    [Tooltip("점수 글자에 외곽선을 사용할지 정합니다.")]
    [SerializeField] private bool useOutline = false;

    [Tooltip("외곽선 색입니다. 검은색이나 어두운 갈색을 추천합니다.")]
    [SerializeField] private Color outlineColor = new Color(0f, 0f, 0f, 0.85f);

    [Tooltip("외곽선 두께입니다. 숫자가 커질수록 두꺼워집니다.")]
    [SerializeField] private Vector2 outlineDistance = new Vector2(2f, -2f);

    // Text는 Unity 기본 UI 글자를 담당합니다.
    private Text scoreText;

    // Shadow는 글자 그림자를 담당합니다.
    private Shadow shadow;

    // Outline은 글자 외곽선을 담당합니다.
    private Outline outline;

    // OnEnable은 오브젝트나 스크립트가 켜질 때 호출됩니다.
    private void OnEnable()
    {
        ApplyStyle();
    }

    // OnValidate는 Inspector 값이 바뀔 때 Unity Editor에서 호출됩니다.
    private void OnValidate()
    {
        ApplyStyle();
    }

    // 점수 글자 스타일을 적용하는 함수입니다.
    public void ApplyStyle()
    {
        FindComponentsIfNeeded();
        ApplyTextStyle();
        ApplyShadowStyle();
        ApplyOutlineStyle();
    }

    // 필요한 컴포넌트를 찾는 함수입니다.
    private void FindComponentsIfNeeded()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<Text>();
        }

        if (shadow == null)
        {
            Shadow[] shadows = GetComponents<Shadow>();

            for (int i = 0; i < shadows.Length; i++)
            {
                if (shadows[i].GetType() == typeof(Shadow))
                {
                    shadow = shadows[i];
                    break;
                }
            }
        }

        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
    }

    // 글자 크기와 색을 적용하는 함수입니다.
    private void ApplyTextStyle()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.fontSize = fontSize;
        scoreText.color = textColor;
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.raycastTarget = false;

        if (scoreText.font == null)
        {
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }

    // 그림자 설정을 적용하는 함수입니다.
    private void ApplyShadowStyle()
    {
        if (!useShadow)
        {
            if (shadow != null)
            {
                shadow.enabled = false;
            }

            return;
        }

        if (shadow == null)
        {
            shadow = gameObject.AddComponent<Shadow>();
        }

        shadow.enabled = true;
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowDistance;
        shadow.useGraphicAlpha = true;
    }

    // 외곽선 설정을 적용하는 함수입니다.
    private void ApplyOutlineStyle()
    {
        if (!useOutline)
        {
            if (outline != null)
            {
                outline.enabled = false;
            }

            return;
        }

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        outline.enabled = true;
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;
        outline.useGraphicAlpha = true;
    }
}
