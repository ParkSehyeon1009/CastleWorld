using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점 UI 컨트롤러 — 뽑기 버튼, 골드 표시, 결과 패널 연동
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("뽑기 풀")]
    [SerializeField] GachaPool gachaPool;

    [Header("UI 참조")]
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text poolNameText;
    [SerializeField] TMP_Text poolDescText;

    [SerializeField] Button   singleDrawButton;
    [SerializeField] Button   tenDrawButton;
    [SerializeField] TMP_Text singleCostText;
    [SerializeField] TMP_Text tenCostText;

    [SerializeField] GachaResultUI resultUI;

    void OnEnable()
    {
        RefreshGoldUI();
        RefreshPoolInfo();

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
        singleDrawButton?.onClick.AddListener(OnSingleDraw);
        tenDrawButton?.onClick.AddListener(OnTenDraw);
    }

    void RefreshPoolInfo()
    {
        if (gachaPool == null) return;
        if (poolNameText) poolNameText.text = gachaPool.poolName;
        if (poolDescText)  poolDescText.text  = gachaPool.poolDescription;
        if (singleCostText) singleCostText.text = $"{gachaPool.singleCost:N0} G";
        if (tenCostText)    tenCostText.text    = $"{gachaPool.tenCost:N0} G";
    }

    void RefreshGoldUI()
    {
        if (goldText == null || PlayerWallet.Instance == null) return;
        goldText.text = $"{PlayerWallet.Instance.Gold:N0} G";
    }

    void OnGoldChanged(int newGold)
    {
        if (goldText != null)
            goldText.text = $"{newGold:N0} G";
    }

    void OnSingleDraw()
    {
        if (gachaPool == null) return;

        if (!PlayerWallet.Instance.HasEnough(gachaPool.singleCost))
        {
            Debug.Log("[ShopUI] 골드가 부족합니다.");
            return;
        }

        PlayerWallet.Instance.Spend(gachaPool.singleCost);
        var result = gachaPool.DrawMultiple(1);
        PlayerInventory.Instance?.AddUnits(result);
        resultUI?.Show(result, gachaPool);
    }

    void OnTenDraw()
    {
        if (gachaPool == null) return;

        if (!PlayerWallet.Instance.HasEnough(gachaPool.tenCost))
        {
            Debug.Log("[ShopUI] 골드가 부족합니다.");
            return;
        }

        PlayerWallet.Instance.Spend(gachaPool.tenCost);
        var result = gachaPool.DrawMultiple(10);
        PlayerInventory.Instance?.AddUnits(result);
        resultUI?.Show(result, gachaPool);
    }
}
