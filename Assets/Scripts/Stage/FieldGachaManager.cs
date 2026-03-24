using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 씬 내 유닛 뽑기 버튼 관리.
/// 인벤토리에서 랜덤 유닛 1개를 꺼내 그리드의 첫 번째 빈 슬롯에 배치.
/// 비용: baseCost(50G) + 뽑은 횟수 × costIncrement(10G)
/// </summary>
public class FieldGachaManager : MonoBehaviour
{
    public static FieldGachaManager Instance { get; private set; }

    [Header("뽑기 비용")]
    public int baseCost      = 50;
    public int costIncrement = 10;

    [Header("UI")]
    public Button         drawButton;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI inventoryCountText; // (선택) 남은 인벤토리 수 표시

    private int _drawCount = 0;

    public int CurrentCost => baseCost + _drawCount * costIncrement;

    void Awake() => Instance = this;

    void Start()
    {
        drawButton?.onClick.AddListener(OnDraw);

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += RefreshUI;
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnGoldChanged += _ => RefreshUI();

        RefreshUI();
    }

    void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= RefreshUI;
    }

    void OnDraw()
    {
        // ① 인벤토리 확인
        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.HasAny())
        {
            Debug.Log("[FieldGacha] 인벤토리에 유닛이 없습니다.");
            return;
        }

        // ② 골드 확인
        if (PlayerWallet.Instance == null || !PlayerWallet.Instance.HasEnough(CurrentCost))
        {
            Debug.Log($"[FieldGacha] 골드 부족 (필요: {CurrentCost}G)");
            return;
        }

        // ③ 빈 슬롯 확인
        if (!GridManager.Instance.FindFirstEmptySlot(out int col, out int row))
        {
            Debug.Log("[FieldGacha] 그리드에 빈 슬롯이 없습니다.");
            return;
        }

        // ④ 인벤토리에서 유닛 꺼내기
        UnitData unit = PlayerInventory.Instance.DrawRandom();
        if (unit == null) return;

        // ⑤ 골드 차감 & 배치
        PlayerWallet.Instance.Spend(CurrentCost);
        UnitPlacementManager.Instance.PlaceUnitAt(unit, col, row);

        _drawCount++;
        Debug.Log($"[FieldGacha] {unit.unitName} 소환! 다음 비용: {CurrentCost}G");
        RefreshUI();
    }

    void RefreshUI()
    {
        if (costText != null)
            costText.text = $"{CurrentCost} G";

        if (inventoryCountText != null && PlayerInventory.Instance != null)
        {
            int total = 0;
            foreach (var kv in PlayerInventory.Instance.GetAll())
                total += kv.Value;
            inventoryCountText.text = $"보유: {total}";
        }

        if (drawButton != null)
        {
            bool canDraw = PlayerInventory.Instance != null && PlayerInventory.Instance.HasAny()
                        && PlayerWallet.Instance    != null && PlayerWallet.Instance.HasEnough(CurrentCost)
                        && GridManager.Instance     != null && GridManager.Instance.FindFirstEmptySlot(out _, out _);
            drawButton.interactable = canDraw;
        }
    }
}
