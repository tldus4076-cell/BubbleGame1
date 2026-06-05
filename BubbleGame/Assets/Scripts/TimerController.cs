using UnityEngine;
using UnityEngine.UI;
using System;

// TimerController는 화면에 남은 시간을 숫자로 보여주는 스크립트입니다.
// 이 스크립트는 배경 기능과 분리되어 있어서 StageBackgroundController를 건드리지 않습니다.
public class TimerController : MonoBehaviour
{
    public event Action TimeUp;

    [Header("타이머 시간 설정")]
    [Tooltip("게임을 시작할 때 타이머가 몇 초부터 시작할지 정합니다. 예: 4분은 240초입니다.")]
    [SerializeField] private float startTime = 60f;

    [Tooltip("현재 남은 시간입니다. Play 중에 줄어드는 값을 확인할 수 있습니다.")]
    [SerializeField] private float currentTime;

    [Header("타이머 글자 연결")]
    [Tooltip("Canvas 안에 있는 TimerText UI Text 오브젝트를 여기에 연결합니다.")]
    [SerializeField] private Text timerText;

    [Header("타이머 동작 설정")]
    [Tooltip("체크되어 있으면 Play를 누를 때 자동으로 타이머가 시작됩니다.")]
    [SerializeField] private bool startOnPlay = true;

    // 타이머가 지금 움직이는 중인지 기억하는 변수입니다.
    private bool isRunning;
    private bool timeUpRaised;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    // 여기서는 현재 시간을 시작 시간으로 준비합니다.
    private void Awake()
    {
        // currentTime을 startTime과 같은 값으로 맞춥니다.
        ResetTimer();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    // 자동 시작 옵션이 켜져 있으면 타이머를 움직이게 합니다.
    private void Start()
    {
        // startOnPlay가 true이면 게임 시작과 동시에 시간이 줄어듭니다.
        if (startOnPlay)
        {
            // 타이머를 움직이는 상태로 바꿉니다.
            isRunning = true;
        }

        // 시작하자마자 화면 글자를 한 번 갱신합니다.
        UpdateTimerText();
    }

    // Update는 게임이 실행되는 동안 매 프레임 호출됩니다.
    // 여기서 시간이 조금씩 줄어들게 만듭니다.
    private void Update()
    {
        // 타이머가 멈춘 상태라면 아무것도 하지 않습니다.
        if (!isRunning)
        {
            return;
        }

        // 시간이 이미 0이면 더 줄이지 않고 멈춥니다.
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateTimerText();
            RaiseTimeUp();
            return;
        }

        // Time.deltaTime은 이전 프레임부터 지금 프레임까지 흐른 시간입니다.
        // 이 값을 빼면 실제 시간처럼 자연스럽게 줄어듭니다.
        currentTime -= Time.deltaTime;

        // Mathf.Max는 둘 중 더 큰 값을 고릅니다.
        // currentTime이 0보다 작아지지 않도록 막습니다.
        currentTime = Mathf.Max(currentTime, 0f);

        // 줄어든 시간을 화면 글자에 다시 표시합니다.
        UpdateTimerText();

        if (currentTime <= 0f)
        {
            isRunning = false;
            RaiseTimeUp();
        }
    }

    // 타이머를 처음 상태로 되돌리는 함수입니다.
    // 나중에 다시하기 버튼이나 새 스테이지 시작 때 사용할 수 있습니다.
    public void ResetTimer()
    {
        // 시작 시간이 음수이면 이상하므로 0 이상으로 고쳐줍니다.
        startTime = Mathf.Max(startTime, 0f);

        // 현재 시간을 시작 시간으로 되돌립니다.
        currentTime = startTime;
        timeUpRaised = false;

        // 화면 글자도 현재 시간에 맞게 바꿉니다.
        UpdateTimerText();
    }

    // 나중에 Stage별 시간을 넣기 위한 함수입니다.
    // 예: Stage 1은 90초, Stage 2는 75초, Stage 3은 60초처럼 바꿀 수 있습니다.
    public void SetStartTime(float seconds)
    {
        // seconds가 음수로 들어오면 0으로 고쳐줍니다.
        startTime = Mathf.Max(seconds, 0f);

        // 새 시작 시간에 맞게 현재 시간도 다시 맞춥니다.
        ResetTimer();
    }

    // 타이머를 다시 움직이게 하는 함수입니다.
    // 나중에 일시정지나 재시작 기능을 만들 때 사용할 수 있습니다.
    public void StartTimer()
    {
        // 남은 시간이 있을 때만 타이머를 시작합니다.
        if (currentTime > 0f)
        {
            isRunning = true;
        }
    }

    // 타이머를 잠시 멈추는 함수입니다.
    // 기능 5번에서는 꼭 필요하지 않지만, 나중에 일시정지 기능에 사용할 수 있습니다.
    public void StopTimer()
    {
        // 타이머를 멈춘 상태로 바꿉니다.
        isRunning = false;
    }

    // 현재 남은 시간을 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    // TimerGaugeController가 이 값을 읽어서 게이지 길이를 계산합니다.
    public float GetCurrentTime()
    {
        // 현재 남은 시간을 돌려줍니다.
        return currentTime;
    }

    // 시작 시간을 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    // 예: 시작 시간이 240초이고 현재 시간이 120초면 게이지는 절반이 됩니다.
    public float GetStartTime()
    {
        // 처음 시작 시간을 돌려줍니다.
        return startTime;
    }

    private void RaiseTimeUp()
    {
        if (timeUpRaised)
        {
            return;
        }

        timeUpRaised = true;
        TimeUp?.Invoke();
    }

    // 화면에 보이는 타이머 숫자를 바꾸는 함수입니다.
    private void UpdateTimerText()
    {
        // timerText가 연결되지 않았으면 글자를 바꿀 수 없으므로 멈춥니다.
        if (timerText == null)
        {
            return;
        }

        // CeilToInt는 소수점이 있는 시간을 올림해서 정수로 바꿉니다.
        // 예: 239.2초는 240초로 보여서 4:00처럼 표시됩니다.
        int displayTime = Mathf.CeilToInt(currentTime);

        // 전체 초를 분으로 바꿉니다.
        // 예: 240초 / 60 = 4분입니다.
        int minutes = displayTime / 60;

        // 전체 초에서 60으로 나누고 남은 값을 초로 사용합니다.
        // 예: 239초 % 60 = 59초입니다.
        int seconds = displayTime % 60;

        // 화면에 분:초 형식으로 표시합니다.
        // seconds:00은 초가 5일 때 05처럼 두 자리로 보이게 합니다.
        timerText.text = minutes + ":" + seconds.ToString("00");
    }
}
