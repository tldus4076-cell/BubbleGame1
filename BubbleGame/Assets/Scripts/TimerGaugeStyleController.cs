using UnityEngine;
using UnityEngine.UI;

// TimerGaugeStyleController는 TimerGaugeFill의 색, 외곽선, 그림자만 담당하는 스크립트입니다.
// 게이지가 줄어드는 기능은 TimerGaugeController가 담당하므로 여기서는 건드리지 않습니다.
// TimerGaugeFill의 위치와 크기도 이 스크립트에서는 절대 바꾸지 않습니다.
[ExecuteAlways]
public class TimerGaugeStyleController : MonoBehaviour
{
    [Header("게이지 색 설정")]
    [Tooltip("TimerGaugeFill의 기본 색입니다. 노란색/주황색 계열을 추천합니다.")]
    [SerializeField] private Color gaugeColor = new Color(1f, 0.72f, 0.05f, 1f);

    [Header("외곽선 설정")]
    [Tooltip("게이지에 외곽선을 사용할지 정합니다. 밝은 배경에서 잘 보이게 도와줍니다.")]
    [SerializeField] private bool useOutline = true;

    [Tooltip("외곽선 색입니다. 어두운 갈색을 추천합니다.")]
    [SerializeField] private Color outlineColor = new Color(0.38f, 0.19f, 0.03f, 1f);

    [Tooltip("외곽선 두께입니다. 숫자가 커질수록 두꺼워집니다.")]
    [SerializeField] private Vector2 outlineDistance = new Vector2(2f, -2f);

    [Header("그림자 설정")]
    [Tooltip("게이지에 그림자를 사용할지 정합니다. 배경과 분리되어 보이게 도와줍니다.")]
    [SerializeField] private bool useShadow = true;

    [Tooltip("그림자 색입니다. 검은색 반투명을 추천합니다.")]
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.55f);

    [Tooltip("그림자가 게이지에서 얼마나 떨어져 보일지 정합니다.")]
    [SerializeField] private Vector2 shadowDistance = new Vector2(3f, -3f);

    // Image는 TimerGaugeFill의 그림과 색을 담당합니다.
    private Image gaugeImage;

    // Outline은 외곽선을 담당합니다.
    private Outline outline;

    // Shadow는 그림자를 담당합니다.
    private Shadow shadow;

    // OnEnable은 오브젝트나 스크립트가 켜질 때 호출됩니다.
    private void OnEnable()
    {
        // 스크립트가 켜질 때 스타일을 적용합니다.
        ApplyStyle();
    }

    // OnValidate는 Inspector 값이 바뀔 때 Unity Editor에서 호출됩니다.
    private void OnValidate()
    {
        // 색이나 그림자 값을 바꾸면 Scene 창에서도 바로 보이게 적용합니다.
        ApplyStyle();
    }

    // 게이지 스타일을 한 번에 적용하는 함수입니다.
    public void ApplyStyle()
    {
        // 필요한 컴포넌트를 먼저 찾습니다.
        FindComponentsIfNeeded();

        // 게이지 색을 적용합니다.
        ApplyGaugeColor();

        // 외곽선을 적용합니다.
        ApplyOutline();

        // 그림자를 적용합니다.
        ApplyShadow();
    }

    // 필요한 컴포넌트를 찾는 함수입니다.
    private void FindComponentsIfNeeded()
    {
        // Image가 비어 있으면 이 오브젝트에서 찾습니다.
        if (gaugeImage == null)
        {
            gaugeImage = GetComponent<Image>();
        }

        // Outline이 비어 있으면 이 오브젝트에서 찾습니다.
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }

        // Shadow가 비어 있으면 진짜 Shadow 컴포넌트만 찾습니다.
        // Outline도 Shadow의 한 종류라서 구분해서 찾아야 합니다.
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
    }

    // 게이지 색을 적용하는 함수입니다.
    private void ApplyGaugeColor()
    {
        // Image가 없으면 색을 바꿀 수 없으므로 멈춥니다.
        if (gaugeImage == null)
        {
            return;
        }

        // Image 색만 바꾸고 위치와 크기는 바꾸지 않습니다.
        gaugeImage.color = gaugeColor;
    }

    // 외곽선을 적용하는 함수입니다.
    private void ApplyOutline()
    {
        // 외곽선을 사용하지 않으면 꺼둡니다.
        if (!useOutline)
        {
            if (outline != null)
            {
                outline.enabled = false;
            }

            return;
        }

        // Outline이 없으면 새로 붙입니다.
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // 외곽선 색과 두께를 적용합니다.
        outline.enabled = true;
        outline.effectColor = outlineColor;
        outline.effectDistance = outlineDistance;
        outline.useGraphicAlpha = true;
    }

    // 그림자를 적용하는 함수입니다.
    private void ApplyShadow()
    {
        // 그림자를 사용하지 않으면 꺼둡니다.
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
            shadow = gameObject.AddComponent<Shadow>();
        }

        // 그림자 색과 거리를 적용합니다.
        shadow.enabled = true;
        shadow.effectColor = shadowColor;
        shadow.effectDistance = shadowDistance;
        shadow.useGraphicAlpha = true;
    }
}
