using UnityEngine;
using UnityEngine.UI;

// ScoreController는 현재 점수를 저장하고 화면에 보여주는 스크립트입니다.
// 이번 기능 9번에서는 처음 점수 0을 보여주는 것까지만 담당합니다.
public class ScoreController : MonoBehaviour
{
    [Header("점수 값 설정")]
    [Tooltip("현재 점수입니다. 게임 시작 시 0으로 시작합니다.")]
    [SerializeField] private int currentScore = 0;

    [Header("점수 글자 연결")]
    [Tooltip("GameCanvas 안에 있는 ScoreText UI Text를 여기에 연결합니다.")]
    [SerializeField] private Text scoreText;

    [Header("점수 표시 설정")]
    [Tooltip("체크하면 숫자만 보이고, 체크를 끄면 SCORE 0처럼 보입니다.")]
    [SerializeField] private bool showNumberOnly = true;

    // Awake는 게임이 시작될 때 Start보다 먼저 한 번 호출됩니다.
    private void Awake()
    {
        // 점수가 음수가 되지 않게 고칩니다.
        currentScore = Mathf.Max(currentScore, 0);

        // 시작하자마자 화면 글자를 현재 점수로 맞춥니다.
        UpdateScoreText();
    }

    // Start는 게임이 시작될 때 한 번 호출됩니다.
    private void Start()
    {
        // Play를 눌렀을 때도 점수 글자가 확실히 갱신되게 합니다.
        UpdateScoreText();
    }

    // 점수를 더하는 함수입니다.
    // 기능 10번에서 버블 제거 점수와 연결할 수 있도록 미리 준비합니다.
    public void AddScore(int amount)
    {
        // 음수가 들어와도 점수가 0 아래로 내려가지 않게 합니다.
        currentScore += amount;
        currentScore = Mathf.Max(currentScore, 0);

        // 점수가 바뀌었으니 화면 글자도 다시 바꿉니다.
        UpdateScoreText();
    }

    // 점수를 0으로 되돌리는 함수입니다.
    // 나중에 다시하기나 새 스테이지 시작 때 사용할 수 있습니다.
    public void ResetScore()
    {
        // 현재 점수를 0으로 만듭니다.
        currentScore = 0;

        // 화면 글자도 0으로 바꿉니다.
        UpdateScoreText();
    }

    // 현재 점수를 다른 스크립트가 읽을 수 있게 해주는 함수입니다.
    public int GetCurrentScore()
    {
        // 현재 점수를 돌려줍니다.
        return currentScore;
    }

    // 화면에 보이는 점수 글자를 바꾸는 함수입니다.
    public void UpdateScoreText()
    {
        // ScoreText가 연결되지 않았으면 글자를 바꿀 수 없으므로 멈춥니다.
        if (scoreText == null)
        {
            return;
        }

        // 숫자만 보이게 할지, SCORE 글자까지 같이 보이게 할지 선택합니다.
        if (showNumberOnly)
        {
            scoreText.text = currentScore.ToString();
        }
        else
        {
            scoreText.text = "SCORE " + currentScore;
        }
    }
}
