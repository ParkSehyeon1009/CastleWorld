using UnityEngine;

/// <summary>
/// 유닛 한 종류의 정보를 담는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewUnit", menuName = "Defence/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("기본 정보")]
    public string unitName = "유닛";
    [TextArea(2, 4)]
    public string description = "";
    public Sprite icon;

    [Header("등급")]
    public UnitGrade grade = UnitGrade.Common;

    [Header("뽑기 가중치 (높을수록 잘 나옴)")]
    [Range(0.1f, 100f)]
    public float weight = 60f;

    [Header("스탯 (확장용)")]
    public int attackPower = 10;
    public float attackSpeed = 1f;
    public float range = 3f;
    public int cost = 100;  // 배치 비용
}

public enum UnitGrade
{
    Common    = 0,
    Rare      = 1,
    Epic      = 2,
    Legendary = 3,
}
