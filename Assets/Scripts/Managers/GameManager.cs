using System;
using UnityEngine;

public static class GameManager 
{
    public static event Action<float> OnEnemyDied;

    public static void EnemyDied(float xp)
    {
        OnEnemyDied?.Invoke(xp);
    }
}
