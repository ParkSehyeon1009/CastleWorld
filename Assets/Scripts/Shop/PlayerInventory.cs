using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 플레이어가 뽑은 유닛 목록 관리 싱글톤
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // 유닛 이름 → 보유 수량
    private Dictionary<string, int>      _ownedUnits  = new();
    // 유닛 이름 → UnitData 역참조 (랜덤 뽑기에 사용)
    private Dictionary<string, UnitData> _unitLookup  = new();

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
        _unitLookup[unit.unitName] = unit;
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

    public bool HasAny()
    {
        foreach (var kv in _ownedUnits)
            if (kv.Value > 0) return true;
        return false;
    }

    /// <summary>
    /// 인벤토리에서 랜덤 유닛 1개를 꺼낸다 (수량 1 감소).
    /// 꺼낸 UnitData를 반환, 없으면 null.
    /// </summary>
    public UnitData DrawRandom()
    {
        var available = new List<string>();
        foreach (var kv in _ownedUnits)
            if (kv.Value > 0 && _unitLookup.ContainsKey(kv.Key))
                available.Add(kv.Key);

        if (available.Count == 0) return null;

        string key = available[UnityEngine.Random.Range(0, available.Count)];
        _ownedUnits[key]--;
        if (_ownedUnits[key] <= 0)
            _ownedUnits.Remove(key);

        UnitData unit = _unitLookup[key];
        OnInventoryChanged?.Invoke();
        return unit;
    }

    public Dictionary<string, int> GetAll() => _ownedUnits;
}
