using System;
using UnityEngine;

/// <summary>
/// 웨이브 1개의 구성 정의 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NewWaveData", menuName = "Defence/Wave Data")]
public class WaveData : ScriptableObject
{
    [Serializable]
    public class EnemyGroup
    {
        public EnemyData enemyData;
        [Tooltip("이 그룹에서 스폰할 적 수")]
        public int count = 5;
        [Tooltip("각 적 사이 스폰 간격(초)")]
        public float spawnInterval = 0.8f;
    }

    [Header("웨이브 구성")]
    public EnemyGroup[] enemyGroups;

    [Header("다음 웨이브까지 대기 시간(초)")]
    public float delayAfterWave = 3f;
}
