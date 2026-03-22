using System;
using UnityEngine;

/// <summary>
/// 웨이포인트를 따라 이동하다 캐슬 도달 시 데미지를 입힘.
/// HP가 0이 되면 골드를 지급하고 소멸.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyData data { get; private set; }

    public int CurrentHp { get; private set; }

    /// <summary>사망 시 호출 (isDead: true = 처치, false = 캐슬 도달)</summary>
    public event Action<Enemy, bool> OnDead;

    // 웨이포인트 이동 상태
    private int _waypointIndex = 0;
    private bool _arrived = false;

    public void Init(EnemyData enemyData)
    {
        data = enemyData;
        CurrentHp = data.maxHp;
        _waypointIndex = 0;
        _arrived = false;

        // 첫 번째 웨이포인트 방향으로 즉시 이동 시작
        if (WaypointPath.Instance != null && WaypointPath.Instance.Count > 0)
            transform.position = WaypointPath.Instance.GetWaypoint(0);
    }

    void Update()
    {
        if (_arrived) return;
        if (WaypointPath.Instance == null) return;

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (_waypointIndex >= WaypointPath.Instance.Count)
        {
            ReachCastle();
            return;
        }

        Vector3 target = WaypointPath.Instance.GetWaypoint(_waypointIndex);
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * data.moveSpeed * Time.deltaTime;

        // 도착 판정
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            _waypointIndex++;
            if (_waypointIndex >= WaypointPath.Instance.Count)
                ReachCastle();
        }
    }

    void ReachCastle()
    {
        if (_arrived) return;
        _arrived = true;

        Castle castle = FindFirstObjectByType<Castle>();
        if (castle != null)
            castle.TakeDamage(data.castleDamage);

        OnDead?.Invoke(this, false);
        Destroy(gameObject);
    }

    /// <summary>유닛이나 투사체에서 호출</summary>
    public void TakeDamage(int damage)
    {
        if (_arrived || CurrentHp <= 0) return;

        CurrentHp -= damage;
        if (CurrentHp <= 0)
            Die();
    }

    void Die()
    {
        if (_arrived) return;
        _arrived = true;

        // 골드 지급
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.Earn(data.goldReward);

        OnDead?.Invoke(this, true);
        Destroy(gameObject);
    }
}
