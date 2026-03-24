using System;
using UnityEngine;

/// <summary>
/// 위에서 아래로 내려오며 유닛을 공격합니다.
/// 유닛이 사거리 안에 있으면 멈추고 공격, 유닛 사망 시 계속 이동.
/// 캐슬 Y 이하 도달 시 StageManager.OnEnemyBreached() 호출.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyData data { get; private set; }
    public int CurrentHp { get; private set; }

    /// <summary>사망 시 (wasKilled: true=처치, false=캐슬 도달)</summary>
    public event Action<Enemy, bool> OnDead;

    private bool _arrived = false;
    private UnitEntity _currentTarget = null;
    private float _attackTimer = 0f;

    public void Init(EnemyData enemyData)
    {
        data = enemyData;
        CurrentHp = data.maxHp;
        _arrived = false;
        _currentTarget = null;
        _attackTimer = 0f;
    }

    void Update()
    {
        if (_arrived) return;
        if (data == null) return;

        _currentTarget = FindNearestUnit();

        if (_currentTarget != null)
        {
            // 목표 유닛 X 위치로 수평 정렬
            float targetX = _currentTarget.transform.position.x;
            float dx = targetX - transform.position.x;
            if (Mathf.Abs(dx) > 0.05f)
            {
                float step = data.moveSpeed * Time.deltaTime;
                transform.position += new Vector3(Mathf.Sign(dx) * Mathf.Min(Mathf.Abs(dx), step), 0f);
            }

            // 공격 쿨다운
            _attackTimer += Time.deltaTime;
            float cooldown = data.attackSpeed > 0f ? 1f / data.attackSpeed : 1f;
            if (_attackTimer >= cooldown)
            {
                _attackTimer = 0f;
                _currentTarget.TakeDamage(data.attackDamage);
            }
        }
        else
        {
            // 아래로 이동
            transform.position += Vector3.down * data.moveSpeed * Time.deltaTime;

            // 캐슬 도달 판정
            Castle castle = StageManager.Instance?.castle;
            if (castle != null && transform.position.y <= castle.transform.position.y)
                ReachCastle();
        }
    }

    UnitEntity FindNearestUnit()
    {
        UnitEntity[] all = FindObjectsByType<UnitEntity>(FindObjectsSortMode.None);
        UnitEntity nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var u in all)
        {
            if (u.CurrentHp <= 0) continue;
            float dist = Vector2.Distance(transform.position, u.transform.position);
            if (dist <= data.attackRange && dist < nearestDist)
            {
                nearestDist = dist;
                nearest = u;
            }
        }
        return nearest;
    }

    void ReachCastle()
    {
        if (_arrived) return;
        _arrived = true;
        StageManager.Instance?.OnEnemyBreached(this);
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

        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.Earn(data.goldReward);

        OnDead?.Invoke(this, true);
        Destroy(gameObject);
    }

    /// <summary>StageManager가 재시도 시 강제 제거</summary>
    public void ForceKill()
    {
        if (_arrived) return;
        _arrived = true;
        OnDead?.Invoke(this, false);
        Destroy(gameObject);
    }
}
