using TMPro;
using UnityEngine;

/// <summary>
/// 상단 HUD: 웨이브 정보 + 골드 표시
/// </summary>
public class BattleHUD : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI goldText;

    void Start()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveStart.AddListener(OnWaveStart);

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageChanged.AddListener(OnStageChanged);
            int total = StageManager.Instance.stages != null ? StageManager.Instance.stages.Length : 0;
            UpdateWave(StageManager.Instance.CurrentStageIndex + 1, total);
        }

        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnGoldChanged += OnGoldChanged;
            UpdateGold(PlayerWallet.Instance.Gold);
        }
    }

    void OnWaveStart(int current, int total) { /* 웨이브 정보는 스테이지로 대체 */ }

    void OnStageChanged(int stageNum, int totalStages) => UpdateWave(stageNum, totalStages);

    void OnGoldChanged(int gold) => UpdateGold(gold);

    void UpdateWave(int current, int total)
    {
        if (waveText != null)
            waveText.text = $"Stage  {current} / {total}";
    }

    void UpdateGold(int gold)
    {
        if (goldText != null)
            goldText.text = $"Gold  {gold}";
    }

    void OnDestroy()
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnGoldChanged -= OnGoldChanged;
    }
}
