using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점 UI 컨트롤러
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] TMP_Text goldText;
    [SerializeField] Button closeButton;
    [SerializeField] Button singleDrawButton;
    [SerializeField] Button tenDrawButton;

    void OnEnable()
    {
        RefreshGoldUI();

        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnGoldChanged += OnGoldChanged;
    }

    void OnDisable()
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnGoldChanged -= OnGoldChanged;
    }

    void Start()
    {
        if (closeButton == null)
            closeButton = transform.Find("CloseButton")?.GetComponent<Button>();
        if (singleDrawButton == null)
            singleDrawButton = transform.Find("SingleDrawButton")?.GetComponent<Button>();
        if (tenDrawButton == null)
            tenDrawButton = transform.Find("TenDrawButton")?.GetComponent<Button>();

        closeButton?.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void RefreshGoldUI()
    {
        if (goldText == null || PlayerWallet.Instance == null) return;
        goldText.text = $"{PlayerWallet.Instance.Gold:N0}";
    }

    void OnGoldChanged(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold:N0}";
    }
}
