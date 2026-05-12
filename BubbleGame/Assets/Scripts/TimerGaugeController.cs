using UnityEngine;
using UnityEngine.UI;

// TimerGaugeController는 남은 시간에 맞춰 타이머 게이지를 줄이는 스크립트입니다.
// 타이머 숫자는 TimerController가 담당하고, 이 스크립트는 게이지만 담당합니다.
public class TimerGaugeController : MonoBehaviour
{
    [Header("타이머 연결")]
    [Tooltip("시간을 계산하는 TimerController를 여기에 연결합니다.")]
    [SerializeField] private TimerController timerController;

    [Header("게이지 이미지 연결")]
    [Tooltip("실제로 줄어드는 TimerGaugeFill Image를 여기에 연결합니다.")]
    [SerializeField] private Image gaugeFillImage;

    [Header("자동 찾기 설정")]
    [Tooltip("비어 있는 연결이 있으면 시작할 때 자동으로 찾아봅니다.")]
    [SerializeField] private bool findReferencesOnStart = true;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 자동 찾기 옵션이 켜져 있으면 필요한 연결을 찾아봅니다.
        if (findReferencesOnStart)
        {
            FindReferencesIfNeeded();
        }
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // 시작하자마자 게이지를 한 번 맞춥니다.
        UpdateGauge();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    private void Update()
    {
        // 매 프레임 남은 시간에 맞춰 게이지를 줄입니다.
        UpdateGauge();
    }

    // 필요한 연결이 비어 있으면 자동으로 찾아주는 함수입니다.
    private void FindReferencesIfNeeded()
    {
        // TimerController가 비어 있으면 Scene에서 하나를 찾습니다.
        if (timerController == null)
        {
            timerController = FindFirstObjectByType<TimerController>();
        }

        // 게이지 Image가 비어 있으면 먼저 이름으로 TimerGaugeFill을 찾습니다.
        // 수동 세팅할 때 TimerGaugeController와 TimerGaugeFill이 부모/자식 관계가 아닐 수 있기 때문입니다.
        if (gaugeFillImage == null)
        {
            GameObject gaugeFillObject = GameObject.Find("TimerGaugeFill");

            if (gaugeFillObject != null)
            {
                gaugeFillImage = gaugeFillObject.GetComponent<Image>();
            }
        }

        // 그래도 비어 있으면 이 오브젝트나 자식에서 Image를 찾아봅니다.
        if (gaugeFillImage == null)
        {
            gaugeFillImage = GetComponentInChildren<Image>();
        }

        // 게이지 Image를 찾았다면 Filled 방식으로 강제 설정합니다.
        // Inspector에서 Image Type 설정을 빼먹어도 게이지가 줄어들게 하기 위한 안전장치입니다.
        if (gaugeFillImage != null)
        {
            SetupGaugeFillImage();
        }
    }

    // 남은 시간 비율을 계산해서 게이지에 적용하는 함수입니다.
    private void UpdateGauge()
    {
        // 실행 중 연결이 빠져 있으면 다시 찾아봅니다.
        if (timerController == null || gaugeFillImage == null)
        {
            FindReferencesIfNeeded();
        }

        // 연결이 비어 있으면 계산할 수 없으므로 멈춥니다.
        if (timerController == null || gaugeFillImage == null)
        {
            return;
        }

        // 시작 시간을 가져옵니다.
        float startTime = timerController.GetStartTime();

        // 시작 시간이 0이면 0으로 나누는 오류가 생기므로 게이지를 비우고 멈춥니다.
        if (startTime <= 0f)
        {
            gaugeFillImage.fillAmount = 0f;
            return;
        }

        // 현재 남은 시간을 가져옵니다.
        float currentTime = timerController.GetCurrentTime();

        // 남은 시간 비율을 계산합니다.
        // 예: 현재 30초 / 시작 60초 = 0.5, 즉 게이지 절반입니다.
        float timeRatio = currentTime / startTime;

        // Clamp01은 값을 0과 1 사이로 막아줍니다.
        // 1보다 크면 1, 0보다 작으면 0으로 고칩니다.
        timeRatio = Mathf.Clamp01(timeRatio);

        // Image의 fillAmount에 비율을 넣으면 게이지가 줄어듭니다.
        gaugeFillImage.fillAmount = timeRatio;
    }

    // TimerGaugeFill Image가 fillAmount로 줄어들 수 있게 설정하는 함수입니다.
    private void SetupGaugeFillImage()
    {
        // Filled 타입이어야 fillAmount 값에 따라 이미지가 줄어드는 모습이 보입니다.
        gaugeFillImage.type = Image.Type.Filled;

        // Horizontal은 가로 방향으로 채우고 줄이는 방식입니다.
        gaugeFillImage.fillMethod = Image.FillMethod.Horizontal;

        // 0은 Left입니다. 왼쪽에서 시작해서 오른쪽으로 차 있는 게이지가 됩니다.
        gaugeFillImage.fillOrigin = 0;
    }
}
