using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// StageManager로부터 StageData를 받아 웨이브 순서대로 적을 스폰.
/// 적은 그리드 위쪽 랜덤 열(column) 위치에서 스폰됩니다.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("스폰 설정")]
    [Tooltip("그리드 상단으로부터 얼마나 위에 스폰할지 (월드 단위)")]
    public float spawnYOffset = 4f;

    [Header("폴백 웨이브 (StageData 미설정 시 사용)")]
    [Tooltip("StageManager에 stages가 없을 때 직접 이 웨이브 목록을 사용합니다")]
    public WaveData[] waves;

    // 이벤트
    public UnityEvent<int, int> OnWaveStart;   // (현재 웨이브, 전체 웨이브)
    public UnityEvent<int, int> OnWaveEnd;

    private int _currentWaveIndex = 0;
    private int _aliveEnemyCount = 0;
    private bool _running = false;
    private List<Enemy> _activeEnemies = new();

    public int CurrentWave => _currentWaveIndex + 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (StageManager.Instance != null)
            StageManager.Instance.OnStateChanged.AddListener(OnStateChanged);
    }

    void OnStateChanged(StageState state)
    {
        if (state == StageState.Battle)
        {
            StageData stage = StageManager.Instance?.CurrentStage;
            if (stage != null)
            {
                StartCoroutine(RunStage(stage));
            }
            else if (waves != null && waves.Length > 0)
            {
                // StageData 미설정 시 폴백: 직접 할당된 waves 사용
                StartCoroutine(RunWaveArray(waves));
            }
            else
            {
                Debug.LogWarning("[WaveManager] 실행할 웨이브가 없습니다. StageManager에 StageData를 설정하거나 WaveManager의 waves 필드를 채우세요.");
            }
        }
    }

    // ─────────────────────────────────────────────
    // 웨이브 실행
    // ─────────────────────────────────────────────

    IEnumerator RunStage(StageData stageData)
    {
        yield return StartCoroutine(RunWaveArray(stageData.waves));
    }

    IEnumerator RunWaveArray(WaveData[] waveArray)
    {
        if (_running) yield break;
        _running = true;
        _currentWaveIndex = 0;
        _activeEnemies.Clear();

        if (waveArray == null || waveArray.Length == 0)
        {
            _running = false;
            StageManager.Instance?.OnAllWavesCleared();
            yield break;
        }

        while (_currentWaveIndex < waveArray.Length)
        {
            yield return StartCoroutine(RunSingleWave(waveArray[_currentWaveIndex]));
            _currentWaveIndex++;

            if (_currentWaveIndex < waveArray.Length)
                yield return new WaitForSeconds(waveArray[_currentWaveIndex - 1].delayAfterWave);
        }

        _running = false;
        StageManager.Instance?.OnAllWavesCleared();
    }

    IEnumerator RunSingleWave(WaveData waveData)
    {
        int total = 0;
        foreach (var g in waveData.enemyGroups) total += g.count;
        _aliveEnemyCount = total;

        OnWaveStart?.Invoke(CurrentWave, 0);
        Debug.Log($"[WaveManager] Wave {CurrentWave} 시작 (적 {total}마리)");

        foreach (var group in waveData.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyData);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        // 모든 적 처리 대기
        yield return new WaitUntil(() => _aliveEnemyCount <= 0);

        OnWaveEnd?.Invoke(CurrentWave, 0);
        Debug.Log($"[WaveManager] Wave {CurrentWave} 종료");
    }

    // ─────────────────────────────────────────────
    // 스폰
    // ─────────────────────────────────────────────

    void SpawnEnemy(EnemyData data)
    {
        if (enemyPrefab == null || data == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.Init(data);
        enemy.OnDead += OnEnemyDead;
        _activeEnemies.Add(enemy);
    }

    Vector3 GetSpawnPosition()
    {
        GridManager grid = GridManager.Instance;
        if (grid == null) return new Vector3(0f, spawnYOffset, 0f);

        // 그리드 열(column) 중 랜덤 하나의 X 위치
        int randomCol = Random.Range(0, grid.columns);
        float spawnX = grid.GetSlotWorldPos(randomCol, 0).x;

        // 그리드 상단 Y + 오프셋
        float topRowY = grid.GetSlotWorldPos(0, grid.rows - 1).y;
        float spawnY = topRowY + spawnYOffset;

        return new Vector3(spawnX, spawnY, 0f);
    }

    void OnEnemyDead(Enemy enemy, bool wasKilled)
    {
        _activeEnemies.Remove(enemy);
        _aliveEnemyCount = Mathf.Max(0, _aliveEnemyCount - 1);
    }

    // ─────────────────────────────────────────────
    // 재시도 시 리셋 (StageManager가 RetryCurrentStage 처리하므로 자동 대기)
    // ─────────────────────────────────────────────

    public void StopAllWaves()
    {
        StopAllCoroutines();
        _running = false;
        _aliveEnemyCount = 0;
        _activeEnemies.Clear();
    }
}
