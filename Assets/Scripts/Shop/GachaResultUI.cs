using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 뽑기 결과 패널 — 결과 카드 목록 표시 후 닫기
/// </summary>
public class GachaResultUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] GameObject resultPanel;
    [SerializeField] Transform  cardContainer;   // GridLayoutGroup 권장
    [SerializeField] GameObject unitCardPrefab;
    [SerializeField] Button     closeButton;

    void Awake()
    {
        closeButton?.onClick.AddListener(Hide);
        Hide();
    }

    public void Show(List<UnitData> results, GachaPool pool)
    {
        gameObject.SetActive(true);

        // 기존 카드 제거
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        // 카드 생성
        foreach (var unit in results)
        {
            var card = Instantiate(unitCardPrefab, cardContainer);
            card.GetComponent<UnitCardUI>()?.Setup(unit, pool);
        }

        resultPanel.SetActive(true);
    }

    public void Hide()
    {
        resultPanel.SetActive(false);
        gameObject.SetActive(false);
    }
}
