using UnityEngine;

/// <summary>
/// 스테이지 1개의 구성을 정의하는 ScriptableObject.
/// Defence > Stage Data 메뉴로 생성.
/// </summary>
[CreateAssetMenu(fileName = "NewStageData", menuName = "Defence/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("스테이지 정보")]
    public string stageName = "Stage 1";

    [Header("웨이브 목록")]
    public WaveData[] waves;
}
