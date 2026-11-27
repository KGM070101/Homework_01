using System.Collections;
using UnityEngine;

public class Summoner_Summon : Summoner_States
{
    private enum Phase
    {
        Aim,
        Summon,
        End
    }
    private Phase phase;

        

    private float aimDuration = 2.0f;
    private float endDelay = 2.0f;
    private float timer;

    private bool isSummoning = false;
    
    public Summoner_Summon(Summoner summoner, Summoner_FSM fsm) : base(summoner, fsm)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        phase = Phase.Aim;
        timer = 0.0f;
        isSummoning = false;

        summoner.Rigidbody2D.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick(float deltaTIme)
    {
        base.Tick(deltaTIme);
        timer += deltaTIme;

        switch(phase)
        {
            case Phase.Aim:
                Tick_Aim();
                break;
            case Phase.Summon:
                Tick_Summon();
                break;
            case Phase.End:
                Tick_End();
                break;
        }
    }

    private void Tick_Aim()
    {
        Vector2 dir =
            (summoner.Player.transform.position - summoner.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;        
        summoner.Rigidbody2D.SetRotation(angle);

        if(timer>=aimDuration)
        {
            phase = Phase.Summon;
            timer = 0.0f;
            return;
        }

        if (summoner.IsKnockbacking)
            return;
        summoner.Rigidbody2D.linearVelocity = Vector2.zero;
    }

    private void Tick_Summon()
    {
        float summonDuration = summoner.SummonCount * 0.45f;

        Vector2 dir =
            (summoner.Player.transform.position - summoner.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        summoner.Rigidbody2D.SetRotation(angle);

        if(timer>=summonDuration)
        {
            phase = Phase.End;
            timer = 0;
            return;
        }

        if(!isSummoning)
        {
            isSummoning = true;
            summoner.StartCoroutine(summoner.Co_SummonSkeleton());
        }

        if (summoner.IsKnockbacking)
            return;

        summoner.Rigidbody2D.linearVelocity = Vector2.zero;                                
    }

    private void Tick_End()
    {
        Vector2 dir =
            (summoner.Player.transform.position - summoner.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;        
        summoner.Rigidbody2D.SetRotation(angle);       

        if(timer>=endDelay)
        {
            fsm.ChangeState(SummonerStateEnum.normal);
            return;
        }

        if (summoner.IsKnockbacking)
            return;
        summoner.Rigidbody2D.linearVelocity = Vector2.zero;

    }    
}
