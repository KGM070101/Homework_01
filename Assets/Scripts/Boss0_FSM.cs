using System.Collections.Generic;
using UnityEngine;

public enum StateEnum
{
    normal,
    pattern0,
    pattern1
}

public class Boss0_FSM 
{
    public Boss0_States currentState;

    private Boss0_Normal normal;
    private Boss0_Pattern0 pattern0;
    private Boss0_Pattern1 pattern1;

    private Dictionary<StateEnum, Boss0_States> states = new();

    public Boss0_FSM(Player player,Boss_0 boss,Boss0_Normal normalState)
    {
        normal = new Boss0_Normal(player,boss,this,normalState);
        pattern0 = new Boss0_Pattern0(player, boss, this, normalState);
        pattern1 = new Boss0_Pattern1(player, boss, this, normalState);

        states.Add(StateEnum.normal, normal);
        states.Add(StateEnum.pattern0, pattern0);
        states.Add(StateEnum.pattern1, pattern1);

        ChangeState(StateEnum.normal);
    }
    
    public void ChangeState(StateEnum stateEnum)
    {
        if (!states.TryGetValue(stateEnum, out Boss0_States state))
            return;

        if (state is null)
            return;

        if (currentState == state)
            return;

        currentState?.Exit();

        currentState = state;
        currentState.Enter();
    }

    public void Tick(float deltaTime)
    {
        currentState?.Tick(deltaTime);
    }
}
