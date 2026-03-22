using System;
using UnityEngine;

/// <summary>
/// 플레이어 재화(골드) 관리 싱글톤 — PlayerPrefs로 저장/로드
/// </summary>
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    private const string GoldKey = "PlayerGold";
    private const int DefaultGold = 90000;

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        Gold = PlayerPrefs.GetInt(GoldKey, DefaultGold);
    }

    void Save()
    {
        PlayerPrefs.SetInt(GoldKey, Gold);
        PlayerPrefs.Save();
    }

    public bool HasEnough(int amount) => Gold >= amount;

    /// <summary>골드 차감. 성공 여부 반환.</summary>
    public bool Spend(int amount)
    {
        if (!HasEnough(amount)) return false;
        Gold -= amount;
        Save();
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void Earn(int amount)
    {
        Gold += amount;
        Save();
        OnGoldChanged?.Invoke(Gold);
    }

    /// <summary>디버그용 골드 초기화</summary>
    [ContextMenu("골드 초기화 (3000)")]
    public void ResetGold()
    {
        Gold = DefaultGold;
        Save();
        OnGoldChanged?.Invoke(Gold);
    }
}
