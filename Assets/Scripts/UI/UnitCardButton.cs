using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유닛 배치 패널의 개별 유닛 카드 버튼.
/// UnitPlacementPanel이 동적으로 생성해 Setup()을 호출함.
/// </summary>
public class UnitCardButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Image               iconImage;
    public TextMeshProUGUI     nameText;
    public TextMeshProUGUI     costText;
    public TextMeshProUGUI     countText;

    private UnitData _data;

    public void Setup(UnitData data, int count)
    {
        _data = data;

        if (iconImage  != null && data.icon != null) iconImage.sprite = data.icon;
        if (nameText   != null) nameText.text  = data.unitName;
        if (costText   != null) costText.text  = $"{data.cost} G";
        if (countText  != null) countText.text = $"x{count}";

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        UnitPlacementManager.Instance?.BeginPlacement(_data);
    }
}
