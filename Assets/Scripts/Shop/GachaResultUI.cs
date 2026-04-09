using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GachaResultPanel 컨트롤러.
/// SPUM 프리팹을 GachaPreviewCapture로 캡처해 RawImage에 표시.
/// spumPrefab 없을 때 icon(Sprite)으로 폴백.
/// </summary>
public class GachaResultUI : MonoBehaviour
{
    [Header("컨테이너")]
    [SerializeField] GameObject singleDrawContainer;
    [SerializeField] GameObject tenDrawContainer;

    [Header("1뽑 - 카드 CharacterImage (RawImage)")]
    [SerializeField] RawImage singleCharacterImage;

    [Header("10뽑 - 카드 CharacterImage 0~9 (RawImage)")]
    [SerializeField] RawImage[] tenCharacterImages;

    [Header("닫기 버튼 (선택)")]
    [SerializeField] Button closeButton;

    private readonly List<RenderTexture> _snapshots = new();

    void Start()
    {
        closeButton?.onClick.AddListener(Hide);
    }

    // ────────────────────────────────────────────
    // Public API
    // ────────────────────────────────────────────

    public void ShowSingleResult(UnitData unit)
    {
        singleDrawContainer?.SetActive(true);
        tenDrawContainer?.SetActive(false);
        gameObject.SetActive(true);

        ReleaseSnapshots();
        StartCoroutine(CaptureSingle(unit));
    }

    public void ShowTenResult(List<UnitData> units)
    {
        singleDrawContainer?.SetActive(false);
        tenDrawContainer?.SetActive(true);
        gameObject.SetActive(true);

        ReleaseSnapshots();
        StartCoroutine(CaptureTen(units));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // ────────────────────────────────────────────
    // Capture coroutines
    // ────────────────────────────────────────────

    IEnumerator CaptureSingle(UnitData unit)
    {
        if (singleCharacterImage == null) yield break;

        if (unit?.spumPrefab != null && GachaPreviewCapture.Instance != null)
        {
            yield return StartCoroutine(
                GachaPreviewCapture.Instance.CaptureUnit(unit.spumPrefab, rt =>
                {
                    singleCharacterImage.texture = rt;
                    if (rt != null) _snapshots.Add(rt);
                })
            );
        }
        else if (unit?.icon != null)
        {
            singleCharacterImage.texture = unit.icon.texture;
        }
    }

    IEnumerator CaptureTen(List<UnitData> units)
    {
        for (int i = 0; i < tenCharacterImages.Length && i < units.Count; i++)
        {
            if (tenCharacterImages[i] == null) continue;

            int idx  = i;
            var unit = units[idx];

            if (unit?.spumPrefab != null && GachaPreviewCapture.Instance != null)
            {
                yield return StartCoroutine(
                    GachaPreviewCapture.Instance.CaptureUnit(unit.spumPrefab, rt =>
                    {
                        tenCharacterImages[idx].texture = rt;
                        if (rt != null) _snapshots.Add(rt);
                    })
                );
            }
            else if (unit?.icon != null)
            {
                tenCharacterImages[idx].texture = unit.icon.texture;
            }
        }
    }

    // ────────────────────────────────────────────
    // Memory management
    // ────────────────────────────────────────────

    void ReleaseSnapshots()
    {
        foreach (var rt in _snapshots)
            if (rt != null) rt.Release();
        _snapshots.Clear();
    }

    void OnDestroy() => ReleaseSnapshots();
}
