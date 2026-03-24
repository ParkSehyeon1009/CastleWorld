using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 하단 배치 패널 — 전투 시작 버튼 관리.
/// 유닛 배치는 FieldGachaManager(뽑기)와 UnitDragHandler(드래그)로 처리.
/// </summary>
public class UnitPlacementPanel : MonoBehaviour
{
    [Header("References")]
    public Button startBattleButton;

    void Start()
    {
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(OnStartBattle);

        if (StageManager.Instance != null)
            StageManager.Instance.OnStateChanged.AddListener(OnStateChanged);
    }

    void OnStartBattle()
    {
        StageManager.Instance?.StartBattle();
    }

    void OnStateChanged(StageState state)
    {
        if (startBattleButton != null)
            startBattleButton.interactable = state == StageState.Prepare;
    }
}
