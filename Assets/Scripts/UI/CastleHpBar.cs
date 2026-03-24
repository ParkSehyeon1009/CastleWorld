using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 캐슬 HP 바 — Castle.OnHpChanged 이벤트 수신
/// </summary>
public class CastleHpBar : MonoBehaviour
{
    [Header("References")]
    public Castle castle;
    public Image  fillImage;
    public TextMeshProUGUI hpText;

    void Start()
    {
        if (castle == null)
            castle = FindFirstObjectByType<Castle>();

        if (castle != null)
            UpdateBar(castle.CurrentHp, castle.maxHp);
    }

    void Update()
    {
        if (castle != null)
            UpdateBar(castle.CurrentHp, castle.maxHp);
    }

    void UpdateBar(int current, int max)
    {
        if (fillImage != null)
            fillImage.fillAmount = max > 0 ? (float)current / max : 0f;

        if (hpText != null)
            hpText.text = $"{current} / {max}";
    }
}
