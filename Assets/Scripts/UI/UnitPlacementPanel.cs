using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 하단 유닛 배치 패널.
/// PlayerInventory에서 보유 유닛 목록을 읽어 카드 버튼을 동적 생성.
/// </summary>
public class UnitPlacementPanel : MonoBehaviour
{
    [Header("References")]
    public Transform  buttonContainer;   // HorizontalLayoutGroup이 붙은 Content
    public GameObject unitCardPrefab;    // UnitCardButton 프리팹
    public Button     startBattleButton;

    [Header("등록된 유닛 목록")]
    [Tooltip("뽑기 풀에 있는 모든 UnitData를 여기에 등록")]
    public List<UnitData> allUnits = new();

    void Start()
    {
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(OnStartBattle);

        if (StageManager.Instance != null)
            StageManager.Instance.OnStateChanged.AddListener(OnStateChanged);

        RefreshPanel();
    }

    public void RefreshPanel()
    {
        if (buttonContainer == null || unitCardPrefab == null) return;

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        if (PlayerInventory.Instance == null) return;

        foreach (UnitData unit in allUnits)
        {
            int count = PlayerInventory.Instance.GetCount(unit);
            if (count <= 0) continue;

            GameObject btn = Instantiate(unitCardPrefab, buttonContainer);
            UnitCardButton card = btn.GetComponent<UnitCardButton>();
            if (card != null) card.Setup(unit, count);
        }
    }

    void OnStartBattle()
    {
        StageManager.Instance?.StartBattle();
    }

    void OnStateChanged(StageState state)
    {
        // 전투 시작 후 버튼 비활성화, 배치 패널은 유지
        if (startBattleButton != null)
            startBattleButton.interactable = state == StageState.Prepare;
    }
}
