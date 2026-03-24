using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 유닛을 그리드에 배치하는 유틸리티.
/// FieldGachaManager 등에서 PlaceUnitAt()을 호출해 사용.
/// </summary>
public class UnitPlacementManager : MonoBehaviour
{
    public static UnitPlacementManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("UnitEntity + UnitCombat + UnitDragHandler 가 붙은 기본 유닛 프리팹")]
    public GameObject unitPrefab;

    public UnityEvent<UnitData, int, int> OnUnitPlaced; // (data, col, row)

    void Awake() => Instance = this;

    /// <summary>
    /// 지정 슬롯에 유닛을 즉시 배치.
    /// 슬롯이 비어 있어야 한다 (호출 측에서 확인).
    /// </summary>
    public void PlaceUnitAt(UnitData data, int col, int row)
    {
        if (data == null) return;
        if (!GridManager.Instance.IsEmpty(col, row))
        {
            Debug.LogWarning($"[UnitPlacement] 슬롯 ({col},{row}) 이미 사용 중");
            return;
        }

        GridManager.Instance.PlaceUnit(col, row, data);

        Vector3 pos = GridManager.Instance.GetSlotWorldPos(col, row);
        GameObject go = Instantiate(unitPrefab, pos, Quaternion.identity);

        UnitEntity entity = go.GetComponent<UnitEntity>();
        entity.Init(data, col, row);
        GridManager.Instance.RegisterEntity(col, row, entity);

        // 드래그 핸들러가 프리팹에 없으면 자동 추가
        if (go.GetComponent<UnitDragHandler>() == null)
            go.AddComponent<UnitDragHandler>();

        OnUnitPlaced?.Invoke(data, col, row);
        Debug.Log($"[UnitPlacement] {data.unitName} 배치 → ({col},{row})");
    }

    // ── 하위 호환용 스텁 (UnitCardButton 등에서 참조 중) ──────────
    [System.Obsolete("드래그앤드롭 방식으로 교체됨. 사용 중단 예정.")]
    public void BeginPlacement(UnitData data)
    {
        Debug.Log("[UnitPlacement] BeginPlacement는 더 이상 사용되지 않습니다. 드래그앤드롭을 사용하세요.");
    }
}
