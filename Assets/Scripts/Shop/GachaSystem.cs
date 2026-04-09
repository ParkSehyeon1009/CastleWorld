using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가챠 뽑기 코어 로직 싱글톤
/// UnitData 배열을 weight 기반으로 가중치 랜덤 선택
/// </summary>
public class GachaSystem : MonoBehaviour
{
    public static GachaSystem Instance { get; private set; }

    [Header("뽑기 풀 (UnitData 배열)")]
    [SerializeField] UnitData[] pool;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public UnitData DrawOne()
    {
        return WeightedRandom();
    }

    public List<UnitData> DrawTen()
    {
        var results = new List<UnitData>(10);
        for (int i = 0; i < 10; i++)
            results.Add(WeightedRandom());
        return results;
    }

    UnitData WeightedRandom()
    {
        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning("[GachaSystem] 뽑기 풀이 비어있습니다.");
            return null;
        }

        float total = 0f;
        foreach (var u in pool)
            if (u != null) total += u.weight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var u in pool)
        {
            if (u == null) continue;
            cumulative += u.weight;
            if (roll < cumulative) return u;
        }
        return pool[pool.Length - 1];
    }
}
