using UnityEngine;

/// <summary>
/// 적 한 종류의 스탯 정의 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Defence/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName = "적";
    public Sprite icon;

    [Header("스탯")]
    public int maxHp = 30;
    public float moveSpeed = 2f;

    [Header("캐슬 공격")]
    [Tooltip("캐슬에 도달했을 때 입히는 데미지")]
    public int castleDamage = 1;

    [Header("보상")]
    [Tooltip("처치 시 지급되는 골드")]
    public int goldReward = 10;
}
