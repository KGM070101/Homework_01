using UnityEngine;

public partial class Player
{
    [Header("- Xp/Level")]
    [SerializeField]
    private float level = 1;

    private float xp;
    private float requireXp = 20;       

    public void GetXp(float xpAmount)
    {
        xp += xpAmount;
    }

    private void LevelUp()
    {
        level += 1;
        requireXp *= 1.4f;
        xp = 0;
        hp += 10;
        maxHP += 10;
        power *= 1.1f;
    }
}
