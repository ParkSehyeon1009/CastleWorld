using UnityEngine;

/// <summary>
/// 그리드에 배치된 유닛 인스턴스.
/// 슬롯 위치를 기억하고, 제거 시 GridManager에서 슬롯을 반환.
/// </summary>
public class UnitEntity : MonoBehaviour
{
    public UnitData Data { get; private set; }
    public int Col { get; private set; }
    public int Row { get; private set; }

    private SpriteRenderer _sr;

    public void Init(UnitData data, int col, int row)
    {
        Data = data;
        Col  = col;
        Row  = row;

        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null && data.icon != null)
            _sr.sprite = data.icon;
    }

    /// <summary>유닛 제거 (판매/사망 등) — 슬롯도 자동 반환</summary>
    public void Remove()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        GridManager.Instance?.RemoveUnit(Col, Row);
    }
}
