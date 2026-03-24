using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 필드 위 유닛 드래그앤드롭.
/// - 빈 슬롯에 드롭   → 이동
/// - 같은 유닛 + 같은 레벨 슬롯에 드롭 → 합성 (Lv+1, 최대 9)
/// - 그 외            → 원위치 복귀
///
/// 유닛 프리팹에 Collider2D가 없으면 BoxCollider2D를 자동 추가.
/// </summary>
[RequireComponent(typeof(UnitEntity))]
public class UnitDragHandler : MonoBehaviour
{
    private UnitEntity     _entity;
    private SpriteRenderer _sr;

    private bool    _isDragging;
    private Vector3 _dragOffset;
    private Vector3 _originalWorldPos;
    private int     _originalCol;
    private int     _originalRow;
    private int     _originalSortOrder;

    void Awake()
    {
        _entity = GetComponent<UnitEntity>();
        _sr     = GetComponent<SpriteRenderer>();

        if (GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * 0.85f;
        }
    }

    // ── 마우스/터치 입력 ───────────────────────────────────────────

    void OnMouseDown()
    {
        if (IsPointerOverUI()) return;

        _isDragging       = true;
        _originalCol      = _entity.Col;
        _originalRow      = _entity.Row;
        _originalWorldPos = transform.position;

        Vector3 mouseWorld = GetMouseWorldPos();
        _dragOffset = transform.position - mouseWorld;

        if (_sr != null)
        {
            _originalSortOrder = _sr.sortingOrder;
            _sr.sortingOrder   = 50;
        }

        // 그리드에서 임시 해제 (다른 유닛이 이 슬롯을 빈 칸으로 인식하지 않도록)
        GridManager.Instance.RemoveUnit(_originalCol, _originalRow);
        _entity.SetGridPosition(-1, -1); // OnDestroy 중복 제거 방지
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        transform.position = GetMouseWorldPos() + _dragOffset;
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;
        _isDragging = false;

        if (_sr != null) _sr.sortingOrder = _originalSortOrder;

        Vector3 dropPos = GetMouseWorldPos();

        if (GridManager.Instance.WorldToSlot(dropPos, out int col, out int row))
        {
            if (GridManager.Instance.IsEmpty(col, row))
            {
                MoveTo(col, row);
                return;
            }

            UnitEntity target = GridManager.Instance.GetEntity(col, row);
            if (CanMerge(target))
            {
                target.UpgradeLevel();
                Destroy(gameObject);
                return;
            }
        }

        SnapBack();
    }

    // ── 이동 / 합성 / 복귀 ────────────────────────────────────────

    void MoveTo(int col, int row)
    {
        _entity.SetGridPosition(col, row);
        GridManager.Instance.PlaceUnit(col, row, _entity.Data);
        GridManager.Instance.RegisterEntity(col, row, _entity);
        transform.position = GridManager.Instance.GetSlotWorldPos(col, row);
        Debug.Log($"[Drag] {_entity.Data.unitName} 이동 → ({col},{row})");
    }

    void SnapBack()
    {
        _entity.SetGridPosition(_originalCol, _originalRow);
        GridManager.Instance.PlaceUnit(_originalCol, _originalRow, _entity.Data);
        GridManager.Instance.RegisterEntity(_originalCol, _originalRow, _entity);
        transform.position = _originalWorldPos;
    }

    bool CanMerge(UnitEntity target)
    {
        return target != null
            && target != _entity
            && target.Data    == _entity.Data
            && target.Level   == _entity.Level
            && _entity.Level   < 9;
    }

    // ── 유틸리티 ──────────────────────────────────────────────────

    static Vector3 GetMouseWorldPos()
    {
        Vector3 screenPos = Input.mousePosition;
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;
#endif
        Vector3 world = Camera.main.ScreenToWorldPoint(screenPos);
        world.z = 0f;
        return world;
    }

    static bool IsPointerOverUI()
    {
        var es = EventSystem.current;
        if (es == null) return false;
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            return es.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        return es.IsPointerOverGameObject();
    }
}
