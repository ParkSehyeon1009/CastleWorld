using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 웨이브 순서대로 적을 스폰하고, 모든 적이 처리되면 다음 웨이브로 진행.
/// StageManager.StartBattle() 호출 시 자동으로 시작됨.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("웨이브 설정")]
    public WaveData[] waves;

    [Header("Enemy Prefab")]
    [Tooltip("Enemy 컴포넌트가 붙은 기본 프리팹")]
    public GameObject enemyPrefab;

    [Header("스폰 위치")]
    public Transform spawnPoint;

    // 이벤트
    public UnityEvent<int, int> OnWaveStart;   // (현재 웨이브, 전체 웨이브)
    public UnityEvent<int, int> OnWaveEnd;
    public UnityEvent OnAllWavesCleared;

    private int _currentWaveIndex = 0;
    private int _aliveEnemyCount = 0;
    private bool _running = false;

    public int CurrentWave => _currentWaveIndex + 1;
    public int TotalWaves => waves != null ? waves.Length : 0;

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
            StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        if (_running) yield break;
        _running = true;
        _currentWaveIndex = 0;

        while (_currentWaveIndex < waves.Length)
        {
            yield return StartCoroutine(RunSingleWave(waves[_currentWaveIndex]));
            _currentWaveIndex++;

            if (_currentWaveIndex < waves.Length)
                yield return new WaitForSeconds(waves[_currentWaveIndex - 1].delayAfterWave);
        }

        _running = false;
        OnAllWavesCleared?.Invoke();
        StageManager.Instance?.OnAllWavesCleared();
    }

    IEnumerator RunSingleWave(WaveData waveData)
    {
        OnWaveStart?.Invoke(CurrentWave, TotalWaves);
        Debug.Log($"[WaveManager] Wave {CurrentWave}/{TotalWaves} 시작");

        // 이번 웨이브 총 적 수 계산
        int total = 0;
        foreach (var g in waveData.enemyGroups) total += g.count;
        _aliveEnemyCount = total;

        // 각 그룹 순서대로 스폰
        foreach (var group in waveData.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyData);
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }

        // 모든 적이 처리될 때까지 대기
        yield return new WaitUntil(() => _aliveEnemyCount <= 0);

        OnWaveEnd?.Invoke(CurrentWave, TotalWaves);
        Debug.Log($"[WaveManager] Wave {CurrentWave}/{TotalWaves} 종료");
    }

    void SpawnEnemy(EnemyData data)
    {
        if (enemyPrefab == null || data == null) return;

        Vector3 pos = spawnPoint != null
            ? spawnPoint.position
            : WaypointPath.Instance?.GetWaypoint(0) ?? Vector3.zero;

        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.Init(data);
        enemy.OnDead += OnEnemyDead;
    }

    void OnEnemyDead(Enemy enemy, bool wasKilled)
    {
        _aliveEnemyCount = Mathf.Max(0, _aliveEnemyCount - 1);
    }
}
