using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 뽑기 결과 카드 1장 — 아이콘 / 이름 / 등급 표시
/// </summary>
public class UnitCardUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] Image  cardBackground;
    [SerializeField] Image  unitIcon;
    [SerializeField] TMP_Text unitNameText;
    [SerializeField] TMP_Text gradeText;

    static readonly string[] GradeLabels = { "Common", "Rare", "Epic", "Legendary" };

    public void Setup(UnitData unit, GachaPool pool)
    {
        if (unit == null) return;

        unitNameText.text = unit.unitName;
        gradeText.text    = GradeLabels[(int)unit.grade];

        Color bg = pool.GetGradeColor(unit.grade);
        cardBackground.color = bg;

        // 아이콘이 있으면 표시, 없으면 색상만
        unitIcon.sprite  = unit.icon;
        unitIcon.enabled = unit.icon != null;

        // 전설 등급 강조 효과 (약간 커지게)
        float scale = unit.grade == UnitGrade.Legendary ? 1.1f : 1f;
        transform.localScale = Vector3.one * scale;
    }
}
