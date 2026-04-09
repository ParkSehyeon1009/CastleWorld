using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점 UI 컨트롤러
/// </summary>
public class ShopUI : MonoBehaviour
{
    private const int SingleDrawCost = 500;
    private const int TenDrawCost    = 4500;

    [Header("UI 참조")]
    [SerializeField] TMP_Text goldText;
    [SerializeField] Button closeButton;
    [SerializeField] Button singleDrawButton;
    [SerializeField] Button tenDrawButton;

    [Header("가챠 연결")]
    [SerializeField] GachaResultUI gachaResultUI;

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
        singleDrawButton?.onClick.AddListener(OnSingleDraw);
        tenDrawButton?.onClick.AddListener(OnTenDraw);
    }

    void OnSingleDraw()
    {
        if (GachaSystem.Instance == null || gachaResultUI == null) return;

        if (!PlayerWallet.Instance.Spend(SingleDrawCost))
        {
            Debug.Log("[ShopUI] 골드 부족 (1뽑: 500G)");
            return;
        }

        var result = GachaSystem.Instance.DrawOne();
        if (result != null) PlayerInventory.Instance?.AddUnit(result);
        gachaResultUI.ShowSingleResult(result);
    }

    void OnTenDraw()
    {
        if (GachaSystem.Instance == null || gachaResultUI == null) return;

        if (!PlayerWallet.Instance.Spend(TenDrawCost))
        {
            Debug.Log("[ShopUI] 골드 부족 (10뽑: 4500G)");
            return;
        }

        var results = GachaSystem.Instance.DrawTen();
        PlayerInventory.Instance?.AddUnits(results);
        gachaResultUI.ShowTenResult(results);
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
