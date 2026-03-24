using UnityEngine;

/// <summary>
/// 유닛 자동 전투 컴포넌트.
/// 사거리(UnitData.range) 안의 가장 가까운 적을 attackSpeed 주기로 공격.
/// UnitEntity와 같은 오브젝트에 부착.
/// </summary>
[RequireComponent(typeof(UnitEntity))]
public class UnitCombat : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;

    private UnitEntity _entity;
    private float      _attackTimer;

    void Awake()
    {
        _entity = GetComponent<UnitEntity>();
    }

    void Update()
    {
        // 전투 중일 때만 공격
        if (StageManager.Instance == null ||
            StageManager.Instance.CurrentState != StageState.Battle)
            return;

        if (_entity.Data == null) return;

        _attackTimer += Time.deltaTime;

        float cooldown = _entity.GetAttackSpeed() > 0f
            ? 1f / _entity.GetAttackSpeed()
            : 1f;

        if (_attackTimer < cooldown) return;

        Enemy target = FindNearestEnemy();
        if (target == null) return;

        FireAt(target);
        _attackTimer = 0f;
    }

    Enemy FindNearestEnemy()
    {
        float range = _entity.GetRange();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        Enemy   nearest     = null;
        float   nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e == null || e.CurrentHp <= 0) continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest     = e;
            }
        }

        return nearest;
    }

    void FireAt(Enemy target)
    {
        if (projectilePrefab == null) return;

        GameObject go   = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(target, _entity.GetAttackPower());
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_entity == null) _entity = GetComponent<UnitEntity>();
        if (_entity?.Data == null) return;
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _entity.GetRange());
    }
#endif
}
