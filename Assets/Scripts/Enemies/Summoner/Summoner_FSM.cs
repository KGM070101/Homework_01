using System.Collections.Generic;
using UnityEngine;

public enum SummonerStateEnum
{
    normal,
    patternShoot,
    patternSummon
}

public class Summoner_FSM
{
    public Summoner_States currentState;

    private Summoner_Normal normal;
    private Summoner_Shoot shoot;
    private Summoner_Summon summon;

    private Dictionary<SummonerStateEnum, Summoner_States> states = new();

    public Summoner_FSM(Summoner summoner)
    {
        normal = new Summoner_Normal(summoner, this);
        shoot = new Summoner_Shoot(summoner, this);
        summon = new Summoner_Summon(summoner, this);

        states.Add(SummonerStateEnum.normal, normal);
        states.Add(SummonerStateEnum.patternShoot, shoot);
        states.Add(SummonerStateEnum.patternSummon, summon);

        ChangeState(SummonerStateEnum.normal);
    }

    public void ChangeState(SummonerStateEnum stateEnum)
    {
        if (!states.TryGetValue(stateEnum, out Summoner_States state))
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
