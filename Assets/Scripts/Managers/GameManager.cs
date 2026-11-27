using System;
using UnityEngine;

public static class GameManager 
{
    public static event Action<float> OnEnemyDied;
    public static event Action OnEnemySpawn;
    public static event Action<bool> OnCardOpenChanged;

    public static void EnemyDied(float xp)
    {
        OnEnemyDied?.Invoke(xp);
    }

    public static void EnemySpawn()
    {
        OnEnemySpawn?.Invoke();
    }

    public static void CardOpenChanged(bool open) => OnCardOpenChanged?.Invoke(open);
}
