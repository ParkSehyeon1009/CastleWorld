using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 뽑기 풀 설정 ScriptableObject — 등록된 유닛 목록에서 가중치 기반으로 뽑기
/// </summary>
[CreateAssetMenu(fileName = "NewGachaPool", menuName = "Defence/Gacha Pool")]
public class GachaPool : ScriptableObject
{
    [Header("풀 정보")]
    public string poolName = "기본 뽑기";
    [TextArea(2, 3)]
    public string poolDescription = "다양한 유닛을 뽑아보세요!";
    public Sprite banner;

    [Header("비용 (골드)")]
    public int singleCost = 300;
    public int tenCost    = 2700;  // 10회 시 10% 할인

    [Header("유닛 목록")]
    public List<UnitData> units = new();

    [Header("등급별 색상 (인스펙터 미리보기용)")]
    public Color commonColor    = new Color(0.8f, 0.8f, 0.8f);
    public Color rareColor      = new Color(0.3f, 0.6f, 1f);
    public Color epicColor      = new Color(0.7f, 0.3f, 1f);
    public Color legendaryColor = new Color(1f, 0.8f, 0.1f);

    /// <summary>유닛 1개 뽑기 (가중치 기반)</summary>
    public UnitData Draw()
    {
        if (units == null || units.Count == 0) return null;

        float total = 0f;
        foreach (var u in units)
            if (u != null) total += u.weight;

        float r = Random.Range(0f, total);
        float acc = 0f;
        foreach (var u in units)
        {
            if (u == null) continue;
            acc += u.weight;
            if (r <= acc) return u;
        }
        return units[^1];
    }

    /// <summary>유닛 n개 뽑기</summary>
    public List<UnitData> DrawMultiple(int count)
    {
        var results = new List<UnitData>(count);
        for (int i = 0; i < count; i++)
            results.Add(Draw());
        return results;
    }

    public Color GetGradeColor(UnitGrade grade) => grade switch
    {
        UnitGrade.Rare      => rareColor,
        UnitGrade.Epic      => epicColor,
        UnitGrade.Legendary => legendaryColor,
        _                   => commonColor,
    };
}
