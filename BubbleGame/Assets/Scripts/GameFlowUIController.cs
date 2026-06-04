using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class GameFlowUIController : MonoBehaviour
{
    [Header("Injected Gameplay References")]
    [SerializeField] private TimerController timerController;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private BubbleLauncherController launcherController;
    [SerializeField] private ShooterAimController aimController;
    [SerializeField] private ShooterAimLineController aimLineController;

    [Header("Injected UI References")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private Text gameOverMessageText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button restartToStartButton;
    [SerializeField] private Button quitButton;

    private bool gameStarted;
    private bool gameOverShown;

    private void Awake()
    {
        ShowStartScreen();
    }

    private void OnEnable()
    {
        if (timerController != null)
        {
            timerController.TimeUp += HandleTimeUp;
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartStage);
        }

        if (restartToStartButton != null)
        {
            restartToStartButton.onClick.AddListener(ShowStartScreen);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void OnDisable()
    {
        if (timerController != null)
        {
            timerController.TimeUp -= HandleTimeUp;
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RestartStage);
        }

        if (restartToStartButton != null)
        {
            restartToStartButton.onClick.RemoveListener(ShowStartScreen);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }
    }

    private void ShowStartScreen()
    {
        gameStarted = false;
        gameOverShown = false;
        Time.timeScale = 0f;
        SetGameplayInputEnabled(false);

        if (timerController != null)
        {
            timerController.ResetTimer();
            timerController.StopTimer();
        }

        if (scoreController != null)
        {
            scoreController.ResetScore();
        }

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        gameOverShown = false;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        SetGameplayInputEnabled(true);

        if (timerController != null)
        {
            timerController.ResetTimer();
            timerController.StartTimer();
        }
    }

    private void ShowGameOverScreen()
    {
        gameOverShown = true;
        Time.timeScale = 0f;
        SetGameplayInputEnabled(false);

        int score = scoreController != null ? scoreController.GetCurrentScore() : 0;

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = score.ToString();
        }

        if (gameOverMessageText != null)
        {
            gameOverMessageText.text = score >= 100 ? "멋진 슈팅이에요!" : "다음엔 더 높이 터뜨려봐요!";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void HandleTimeUp()
    {
        if (!gameStarted || gameOverShown)
        {
            return;
        }

        ShowGameOverScreen();
    }

    private void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    private void SetGameplayInputEnabled(bool isEnabled)
    {
        if (launcherController != null)
        {
            launcherController.enabled = isEnabled;
        }

        if (aimController != null)
        {
            aimController.enabled = isEnabled;
        }

        if (aimLineController != null)
        {
            aimLineController.enabled = isEnabled;
        }
    }
}
