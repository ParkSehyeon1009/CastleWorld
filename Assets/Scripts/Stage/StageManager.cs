using UnityEngine;
using UnityEngine.Events;

public enum StageState { Prepare, Battle, Victory, Defeat }

/// <summary>
/// 스테이지 전체 흐름 관리 (준비 → 전투 → 승리/패배)
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("References")]
    public Castle castle;

    [Header("Stage Info")]
    public int stageNumber = 1;

    public StageState CurrentState { get; private set; } = StageState.Prepare;

    public UnityEvent<StageState> OnStateChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (castle != null)
            castle.OnDestroyed.AddListener(OnCastleDestroyed);

        ChangeState(StageState.Prepare);
    }

    public void ChangeState(StageState newState)
    {
        CurrentState = newState;
        Debug.Log($"[StageManager] State → {newState}");
        OnStateChanged?.Invoke(newState);
    }

    /// <summary>UI의 '전투 시작' 버튼에서 호출</summary>
    public void StartBattle()
    {
        if (CurrentState != StageState.Prepare) return;
        ChangeState(StageState.Battle);
    }

    /// <summary>WaveManager가 모든 웨이브 완료 후 호출</summary>
    public void OnAllWavesCleared()
    {
        if (CurrentState != StageState.Battle) return;
        ChangeState(StageState.Victory);
    }

    void OnCastleDestroyed()
    {
        ChangeState(StageState.Defeat);
    }
}
