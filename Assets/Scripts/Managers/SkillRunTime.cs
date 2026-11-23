using UnityEngine;

public class SkillRunTIme : MonoBehaviour
{
    protected Player player;

    public virtual void Configure(Player player)
    {
        this.player = player;
    }
}
