using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Boss0_States 
{
    protected Player player;
    protected Boss_0 boss;
    protected Boss0_FSM fsm;
    protected Boss0_Normal normalState;

    public Boss0_States(Player player,Boss_0 boss, Boss0_FSM fsm,Boss0_Normal normalState)
    {
        this.player = player;
        this.boss = boss;
        this.fsm = fsm;
        this.normalState = normalState;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Tick(float deltaTime)
    {

    }
}
