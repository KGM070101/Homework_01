using UnityEngine;

public abstract class Summoner_States
{
    protected Player player;
    protected Summoner summoner;
    protected Summoner_FSM fsm;

    public Summoner_States(Summoner summoner,Summoner_FSM fsm)
    {
        this.summoner = summoner;
        this.fsm = fsm;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Tick(float deltaTIme)
    {

    }
}
