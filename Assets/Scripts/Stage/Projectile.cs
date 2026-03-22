using UnityEngine;

/// <summary>
/// 유닛이 발사한 투사체. 타겟 Enemy를 추적하다 명중 시 데미지 적용.
/// </summary>
public class Projectile : MonoBehaviour
{
    public float speed = 8f;

    private Enemy _target;
    private int   _damage;

    public void Init(Enemy target, int damage)
    {
        _target = target;
        _damage = damage;
    }

    void Update()
    {
        // 타겟이 이미 죽었거나 사라진 경우
        if (_target == null || _target.CurrentHp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (_target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // 명중 판정
        if (Vector3.Distance(transform.position, _target.transform.position) < 0.15f)
        {
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
