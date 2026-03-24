using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 테스트용 부트스트랩 — GameScene 직접 실행 시 필요한 싱글톤 데이터를 초기화.
/// 빌드 시에는 MainScene에서 진입하므로 이 오브젝트는 비활성화하거나 삭제.
/// </summary>
public class TestBootstrap : MonoBehaviour
{

[Header("자동 전투 시작 (테스트용)")]
    public bool autoStartBattle = false;
    public float autoStartDelay = 2f;

    [Header("테스트용 지급 유닛 목록")]
    public List<UnitData> testUnits = new();

    [Tooltip("각 유닛 지급 수량")]
    public int countPerUnit = 3;

    void Awake()
    {
        // PlayerInventory, PlayerWallet이 아직 없으면 이 오브젝트에 붙여서 생성
        if (PlayerInventory.Instance == null)
            gameObject.AddComponent<PlayerInventory>();

        if (PlayerWallet.Instance == null)
            gameObject.AddComponent<PlayerWallet>();
    }

void Start()
    {
        foreach (var unit in testUnits)
            for (int i = 0; i < countPerUnit; i++)
                PlayerInventory.Instance.AddUnit(unit);

        UnitPlacementPanel panel = FindFirstObjectByType<UnitPlacementPanel>();
        panel?.RefreshPanel();

        Debug.Log($"[TestBootstrap] 유닛 {testUnits.Count}종 x{countPerUnit}개 지급 완료");

        if (autoStartBattle)
            StartCoroutine(AutoStartAfterDelay());
    }


IEnumerator AutoStartAfterDelay()
    {
        yield return new WaitForSeconds(autoStartDelay);
        Debug.Log("[TestBootstrap] 자동 전투 시작!");
        StageManager.Instance?.StartBattle();
    }
}
