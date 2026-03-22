using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 유닛 배치 모드 관리.
/// BeginPlacement(data) 호출 → 그리드 셀 클릭/탭 → 골드 차감 후 유닛 배치.
/// 우클릭 또는 CancelPlacement()로 취소.
/// </summary>
public class UnitPlacementManager : MonoBehaviour
{
    public static UnitPlacementManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("UnitEntity 컴포넌트가 붙은 기본 유닛 프리팹")]
    public GameObject unitPrefab;

    public bool IsPlacing { get; private set; }
    private UnitData _pendingUnit;

    // 이벤트
    public UnityEvent<UnitData> OnPlacementStarted;
    public UnityEvent           OnPlacementCancelled;
    public UnityEvent<UnitData, int, int> OnUnitPlaced; // (data, col, row)

    void Awake() => Instance = this;

    /// <summary>배치 모드 시작. UI 버튼에서 호출.</summary>
    public void BeginPlacement(UnitData data)
    {
        if (data == null) return;
        _pendingUnit = data;
        IsPlacing = true;
        OnPlacementStarted?.Invoke(data);
        Debug.Log($"[UnitPlacement] 배치 모드 시작: {data.unitName}");
    }

    /// <summary>배치 모드 취소.</summary>
    public void CancelPlacement()
    {
        _pendingUnit = null;
        IsPlacing = false;
        OnPlacementCancelled?.Invoke();
        Debug.Log("[UnitPlacement] 배치 취소");
    }

    void Update()
    {
        if (!IsPlacing) return;

        // 우클릭 → 취소
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            return;
        }

        bool clicked = Input.GetMouseButtonDown(0);

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            clicked = true;
#endif

        if (!clicked) return;

        // UI 위를 클릭한 경우 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 screenPos = Input.mousePosition;
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;
#endif

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        if (!GridManager.Instance.WorldToSlot(worldPos, out int col, out int row))
        {
            Debug.Log("[UnitPlacement] 그리드 범위 밖");
            return;
        }

        if (!GridManager.Instance.IsEmpty(col, row))
        {
            Debug.Log($"[UnitPlacement] 슬롯 ({col},{row}) 이미 사용 중");
            return;
        }

        TryPlace(col, row);
    }

    void TryPlace(int col, int row)
    {
        if (_pendingUnit == null) return;

        // 골드 차감
        if (PlayerWallet.Instance != null && !PlayerWallet.Instance.Spend(_pendingUnit.cost))
        {
            Debug.Log($"[UnitPlacement] 골드 부족 (필요: {_pendingUnit.cost})");
            return;
        }

        // 그리드 데이터 등록
        GridManager.Instance.PlaceUnit(col, row, _pendingUnit);

        // 유닛 오브젝트 생성
        Vector3 pos = GridManager.Instance.GetSlotWorldPos(col, row);
        GameObject go = Instantiate(unitPrefab, pos, Quaternion.identity);
        UnitEntity entity = go.GetComponent<UnitEntity>();
        entity.Init(_pendingUnit, col, row);

        OnUnitPlaced?.Invoke(_pendingUnit, col, row);
        Debug.Log($"[UnitPlacement] {_pendingUnit.unitName} 배치 완료 → ({col},{row})");

        // 배치 후 모드 유지 (연속 배치 가능)
        // 같은 유닛을 계속 놓으려면 이 상태 유지, 1회만이면 아래 주석 해제:
        // IsPlacing = false; _pendingUnit = null;
    }
}
