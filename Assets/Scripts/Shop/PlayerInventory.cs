using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 플레이어가 뽑은 유닛 목록 관리 싱글톤
/// (현재는 메모리 보관 — 필요 시 JSON 저장 확장 가능)
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // 유닛 이름 → 보유 수량
    private Dictionary<string, int> _ownedUnits = new();

    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddUnit(UnitData unit)
    {
        if (unit == null) return;
        _ownedUnits.TryGetValue(unit.unitName, out int current);
        _ownedUnits[unit.unitName] = current + 1;
        OnInventoryChanged?.Invoke();
    }

    public void AddUnits(List<UnitData> units)
    {
        foreach (var u in units) AddUnit(u);
    }

    public int GetCount(UnitData unit)
    {
        if (unit == null) return 0;
        return _ownedUnits.TryGetValue(unit.unitName, out int c) ? c : 0;
    }

    public Dictionary<string, int> GetAll() => _ownedUnits;
}
