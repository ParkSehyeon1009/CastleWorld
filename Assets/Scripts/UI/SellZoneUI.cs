using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 유닛 드래그 중 하단에 나타나는 판매 구역 UI.
/// UnitDragHandler가 드래그 시작 시 Show(), 종료 시 Hide() 호출.
/// IsOverSellZone(screenPos)으로 드롭 위치 판정.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class SellZoneUI : MonoBehaviour
{
    public static SellZoneUI Instance { get; private set; }

    public const int SellPrice = 50;

    private RectTransform   _rect;
    private Image           _bg;
    private TextMeshProUGUI _label;

    private static readonly Color NormalColor    = new Color(0.15f, 0.15f, 0.20f, 0.88f);
    private static readonly Color HighlightColor = new Color(0.85f, 0.22f, 0.22f, 0.95f);

    void Awake()
    {
        Instance  = this;
        _rect     = GetComponent<RectTransform>();
        _bg       = GetComponent<Image>();
        _bg.color = NormalColor;

        BuildLabel();
        gameObject.SetActive(false);
    }

    void BuildLabel()
    {
        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(transform, false);

        _label           = textGo.AddComponent<TextMeshProUGUI>();
        _label.text      = "🗑  판매  +50G";
        _label.fontSize  = 18f;
        _label.alignment = TextAlignmentOptions.Center;
        _label.color     = Color.white;

        var r        = _label.GetComponent<RectTransform>();
        r.anchorMin  = Vector2.zero;
        r.anchorMax  = Vector2.one;
        r.offsetMin  = Vector2.zero;
        r.offsetMax  = Vector2.zero;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetHighlight(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetHighlight(bool on)
    {
        _bg.color   = on ? HighlightColor : NormalColor;
        _label.text = on ? "🗑  여기에 놓으면 판매!" : "🗑  판매  +50G";
    }

    public bool IsOverSellZone(Vector2 screenPos)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint(_rect, screenPos, cam);
    }
}
