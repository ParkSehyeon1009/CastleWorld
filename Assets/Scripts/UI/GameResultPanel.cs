using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 승리/패배 결과 패널. StageManager.OnStateChanged 수신.
/// </summary>
public class GameResultPanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject          panel;
    public TextMeshProUGUI     resultText;

void Start()
    {
        if (panel != null) panel.SetActive(false);

        // RetryButton / QuitButton을 자식에서 자동 탐색해 연결
        var retry = transform.Find("RetryButton");
        if (retry != null) retry.GetComponent<UnityEngine.UI.Button>()?.onClick.AddListener(OnRetry);

        var quit = transform.Find("QuitButton");
        if (quit != null) quit.GetComponent<UnityEngine.UI.Button>()?.onClick.AddListener(OnQuit);

        if (StageManager.Instance != null)
            StageManager.Instance.OnStateChanged.AddListener(OnStateChanged);
    }

    void OnStateChanged(StageState state)
    {
        if (state != StageState.Victory && state != StageState.Defeat) return;

        if (panel != null) panel.SetActive(true);

        if (resultText != null)
            resultText.text = state == StageState.Victory
                ? "Victory!"
                : "Defeat...";
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuit()
    {
        SceneManager.LoadScene("MainScene");
    }
}
