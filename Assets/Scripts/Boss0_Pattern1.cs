using UnityEngine;

public class Boss0_Pattern1 : Boss0_States
{   
    public Boss0_Pattern1(Player player, Boss_0 boss, Boss0_FSM fsm) : base(player, boss, fsm)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Enter();
    }

    public override void Tick(float deltaTIme)
    {
        base.Enter();
    }
}
