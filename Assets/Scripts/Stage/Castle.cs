using UnityEngine;
using UnityEngine.Events;

public class Castle : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 3;

    public int CurrentHp { get; private set; }

    public UnityEvent<int, int> OnHpChanged; // (current, max)
    public UnityEvent OnDestroyed;

    void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHp <= 0) return;
        CurrentHp = Mathf.Max(0, CurrentHp - damage);
        OnHpChanged?.Invoke(CurrentHp, maxHp);
        if (CurrentHp <= 0)
            OnDestroyed?.Invoke();
    }
}
