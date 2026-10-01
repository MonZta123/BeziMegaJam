using UnityEngine;

/// <summary>
/// Stores player-selected difficulty values across scenes and application launches.
/// </summary>
public static class DifficultyOptions
{
    private const string PlayerHealthKey = "Difficulty.PlayerHealth";
    private const string BossHealthPrefix = "Difficulty.BossHealth.";
    private const string OrdersToWinKey = "Difficulty.OrdersToWin";
    private const string OrderTimerKey = "Difficulty.OrderTimerSeconds";
    private const string OrdersToLoseKey = "Difficulty.OrdersToLose";

    public const string TopBunBossId = "TopBunBossBehaviour";
    public const string PattyBossId = "PattyBossBehaviour";
    public const string BottomBunBossId = "BottomBunBossBehaviour";

    public const int DefaultPlayerHealth = 5;
    public const int DefaultTopBunBossHealth = 10;
    public const int DefaultPattyBossHealth = 5;
    public const int DefaultBottomBunBossHealth = 5;
    public const int DefaultOrdersToWin = 3;
    public const int DefaultOrderTimerSeconds = 180;
    public const int DefaultOrdersToLose = 3;

    /// <summary>Gets the saved player health or the project's default.</summary>
    public static int PlayerHealth => PlayerPrefs.GetInt(PlayerHealthKey, DefaultPlayerHealth);

    /// <summary>Gets the saved orders-to-win target or the project's default.</summary>
    public static int OrdersToWin => PlayerPrefs.GetInt(OrdersToWinKey, DefaultOrdersToWin);

    /// <summary>Gets the saved timer for each order in seconds or the project's default.</summary>
    public static int OrderTimerSeconds => PlayerPrefs.GetInt(OrderTimerKey, DefaultOrderTimerSeconds);

    /// <summary>Gets the saved failed-order limit or the project's default.</summary>
    public static int OrdersToLose => PlayerPrefs.GetInt(OrdersToLoseKey, DefaultOrdersToLose);

    /// <summary>Gets saved health for a boss, falling back to its prefab-configured default.</summary>
    public static int GetBossHealth(string bossId, int defaultValue)
    {
        return PlayerPrefs.GetInt(BossHealthPrefix + bossId, defaultValue);
    }

    /// <summary>Saves and flushes the difficulty settings entered in the main menu.</summary>
    public static void Save(
        int playerHealth,
        int topBunBossHealth,
        int pattyBossHealth,
        int bottomBunBossHealth,
        int ordersToWin,
        int orderTimerSeconds,
        int ordersToLose)
    {
        PlayerPrefs.SetInt(PlayerHealthKey, Mathf.Clamp(playerHealth, 1, 999));
        PlayerPrefs.SetInt(BossHealthPrefix + TopBunBossId, Mathf.Clamp(topBunBossHealth, 1, 999));
        PlayerPrefs.SetInt(BossHealthPrefix + PattyBossId, Mathf.Clamp(pattyBossHealth, 1, 999));
        PlayerPrefs.SetInt(BossHealthPrefix + BottomBunBossId, Mathf.Clamp(bottomBunBossHealth, 1, 999));
        PlayerPrefs.SetInt(OrdersToWinKey, Mathf.Clamp(ordersToWin, 1, 999));
        PlayerPrefs.SetInt(OrderTimerKey, Mathf.Clamp(orderTimerSeconds, 1, 3600));
        PlayerPrefs.SetInt(OrdersToLoseKey, Mathf.Clamp(ordersToLose, 1, 999));
        PlayerPrefs.Save();
    }
}
