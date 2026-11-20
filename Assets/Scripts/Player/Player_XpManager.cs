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
        level += 1;

        OnLevelUp?.Invoke((int)level);
        requireXp += 10;
        xp = 0;
        hp += data[0].MaxHp * 0.05f;
        maxHP += data[0].MaxHp * 0.05f;
        power += data[0].Power*0.05f;

        IndicatePlayerHp();
    }
}
