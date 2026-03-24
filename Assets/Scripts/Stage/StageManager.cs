using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum StageState { Prepare, Battle, StageClear, Victory, Defeat }

/// <summary>
/// 전체 스테이지 흐름 관리.
/// - Prepare: 유닛 배치 대기
/// - Battle: 전투 중 (WaveManager가 웨이브 실행)
/// - StageClear: 이번 스테이지 클리어 → 잠시 후 다음 스테이지 Prepare
/// - Victory: 모든 스테이지 클리어
/// - Defeat: 캐슬 HP 0
///
/// 적이 캐슬에 도달하면: Castle -1HP → (HP 남아있으면) 스테이지 재시도
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("References")]
    public Castle castle;

    [Header("스테이지 목록")]
    public StageData[] stages;

    [Header("스테이지 클리어 후 다음 스테이지까지 대기(초)")]
    public float stageClearDelay = 2f;

    [Header("재시도 전 대기(초)")]
    public float retryDelay = 1.5f;

    public StageState CurrentState { get; private set; } = StageState.Prepare;
    public int CurrentStageIndex { get; private set; } = 0;
    public StageData CurrentStage => (stages != null && CurrentStageIndex < stages.Length)
        ? stages[CurrentStageIndex] : null;

    public UnityEvent<StageState> OnStateChanged;
    /// <summary>(현재 스테이지 번호 1-based, 전체 스테이지 수)</summary>
    public UnityEvent<int, int> OnStageChanged;

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
        Debug.Log($"[StageManager] State → {newState}  (Stage {CurrentStageIndex + 1}/{stages?.Length})");
        OnStateChanged?.Invoke(newState);
    }

    // ─────────────────────────────────────────────
    // 외부 호출 API
    // ─────────────────────────────────────────────

    /// <summary>UI '전투 시작' 버튼에서 호출</summary>
    public void StartBattle()
    {
        if (CurrentState != StageState.Prepare) return;
        OnStageChanged?.Invoke(CurrentStageIndex + 1, stages?.Length ?? 0);
        ChangeState(StageState.Battle);
    }

    /// <summary>WaveManager가 모든 웨이브 완료 후 호출</summary>
    public void OnAllWavesCleared()
    {
        if (CurrentState != StageState.Battle) return;
        ChangeState(StageState.StageClear);
        StartCoroutine(ProceedToNextStage());
    }

    /// <summary>Enemy가 캐슬 Y에 도달했을 때 호출</summary>
    public void OnEnemyBreached(Enemy enemy)
    {
        if (CurrentState != StageState.Battle) return;

        castle.TakeDamage(1);

        // 캐슬이 살아있으면 재시도, 죽었으면 OnCastleDestroyed가 Defeat 처리
        if (castle.CurrentHp > 0)
            StartCoroutine(RetryCurrentStage());
    }

    // ─────────────────────────────────────────────
    // 내부 흐름
    // ─────────────────────────────────────────────

    IEnumerator ProceedToNextStage()
    {
        yield return new WaitForSeconds(stageClearDelay);

        CurrentStageIndex++;
        if (stages == null || CurrentStageIndex >= stages.Length)
        {
            ChangeState(StageState.Victory);
        }
        else
        {
            // 유닛 HP 회복
            ResetAllUnitHp();
            OnStageChanged?.Invoke(CurrentStageIndex + 1, stages.Length);
            ChangeState(StageState.Prepare);
        }
    }

    IEnumerator RetryCurrentStage()
    {
        // 웨이브 코루틴 즉시 중단 (OnAllWavesCleared 오호출 방지)
        WaveManager.Instance?.StopAllWaves();

        // 잔존 적 모두 제거
        KillAllEnemies();

        yield return new WaitForSeconds(retryDelay);

        // 유닛 HP 회복
        ResetAllUnitHp();

        // 같은 스테이지 — 버튼을 눌러야 재시작
        OnStageChanged?.Invoke(CurrentStageIndex + 1, stages?.Length ?? 0);
        ChangeState(StageState.Prepare);
    }

    void OnCastleDestroyed()
    {
        KillAllEnemies();
        ChangeState(StageState.Defeat);
    }

    void KillAllEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
            e.ForceKill();
    }

    void ResetAllUnitHp()
    {
        UnitEntity[] units = FindObjectsByType<UnitEntity>(FindObjectsSortMode.None);
        foreach (var u in units)
            u.ResetHp();
    }
}
