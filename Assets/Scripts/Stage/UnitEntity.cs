using System;
using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    public UnitData Data { get; private set; }
    public int Col { get; private set; }
    public int Row { get; private set; }
    public int CurrentHp { get; private set; }

    public event Action<UnitEntity> OnDied;

    private SpriteRenderer _sr;

    public void Init(UnitData data, int col, int row)
    {
        Data = data;
        Col  = col;
        Row  = row;
        CurrentHp = data.maxHp;

        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null && data.icon != null)
            _sr.sprite = data.icon;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentHp <= 0) return;
        CurrentHp -= damage;
        if (CurrentHp <= 0)
            Die();
    }

    public void ResetHp()
    {
        if (Data != null)
            CurrentHp = Data.maxHp;
    }

    void Die()
    {
        OnDied?.Invoke(this);
        Remove();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        GridManager.Instance?.RemoveUnit(Col, Row);
    }
}
