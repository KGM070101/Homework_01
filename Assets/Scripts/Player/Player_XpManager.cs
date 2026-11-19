using System;
using UnityEngine;

public partial class Player
{
    [Header("- Xp/Level")]
    [SerializeField]
    private float level = 1;

    public event Action<int> OnLevelUp;

    private float xp;
    private float requireXp = 20;
    private float xpMultiply = 1.0f;

    public void GetXp(float xpAmount)
    {
        xp += xpAmount*xpMultiply;
    }

    private void LevelUp()
    {
        //if (level >= 10)
        //    return;

        level += 1;

        OnLevelUp?.Invoke((int)level);
        requireXp *= 1.2f;
        xp = 0;
        hp += 10;
        maxHP += 10;
        power *= 1.1f;

        IndicatePlayerHp();
    }
}
