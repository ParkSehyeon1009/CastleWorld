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
        {
            WaveManager.Instance.OnWaveStart.AddListener(OnWaveStart);
            UpdateWave(0, WaveManager.Instance.TotalWaves);
        }

        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.OnGoldChanged += OnGoldChanged;
            UpdateGold(PlayerWallet.Instance.Gold);
        }
    }

    void OnWaveStart(int current, int total) => UpdateWave(current, total);

    void OnGoldChanged(int gold) => UpdateGold(gold);

    void UpdateWave(int current, int total)
    {
        if (waveText != null)
            waveText.text = $"Wave  {current} / {total}";
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
