using System;
using TMPro;
using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    public UnitData Data { get; private set; }
    public int Col { get; private set; }
    public int Row { get; private set; }
    public int CurrentHp { get; private set; }
    public int Level { get; private set; } = 1;

    public event Action<UnitEntity> OnDied;

    private SpriteRenderer _sr;
    private TextMeshPro _levelText;

    public void Init(UnitData data, int col, int row)
    {
        Data = data;
        Col  = col;
        Row  = row;
        Level = 1;
        CurrentHp = GetMaxHp();

        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null && data.icon != null)
            _sr.sprite = data.icon;

        RefreshLevelVisual();
    }

    // ── 스케일된 스탯 (레벨당 +50%) ──────────────────────────
    public int   GetMaxHp()       => Mathf.RoundToInt(Data.maxHp       * (1f + (Level - 1) * 0.5f));
    public int   GetAttackPower() => Mathf.RoundToInt(Data.attackPower * (1f + (Level - 1) * 0.5f));
    public float GetAttackSpeed() => Data.attackSpeed * (1f + (Level - 1) * 0.1f);
    public float GetRange()       => Data.range + (Level - 1) * 0.2f;

    // ── 합성: 레벨 올리기 ─────────────────────────────────────
    /// <summary>드래그 이동 시 그리드 좌표만 갱신 (오브젝트 이동은 별도).</summary>
    public void SetGridPosition(int col, int row)
    {
        Col = col;
        Row = row;
    }

    public void UpgradeLevel()
    {
        if (Level >= 9) return;
        Level++;
        CurrentHp = GetMaxHp();
        RefreshLevelVisual();
        Debug.Log($"[Merge] {Data.unitName} → Lv{Level}");
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
            CurrentHp = GetMaxHp();
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

    // ── 레벨 텍스트 비주얼 ────────────────────────────────────
    void RefreshLevelVisual()
    {
        if (_levelText == null)
        {
            GameObject go = new GameObject("LevelText");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0.3f, 0.35f, 0f);

            _levelText = go.AddComponent<TextMeshPro>();
            _levelText.fontSize       = 2.5f;
            _levelText.fontStyle      = FontStyles.Bold;
            _levelText.alignment      = TextAlignmentOptions.Center;
            _levelText.GetComponent<MeshRenderer>().sortingOrder = 10;
        }

        _levelText.text  = Level >= 2 ? $"Lv{Level}" : "";
        _levelText.color = LevelColor(Level);
    }

    static Color LevelColor(int lv)
    {
        if (lv >= 7) return new Color(1f, 0.6f, 0f);   // 금색 (7~9)
        if (lv >= 4) return new Color(0.6f, 0.3f, 1f); // 보라 (4~6)
        return Color.white;                              // 흰색 (1~3)
    }
}
