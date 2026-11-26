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

    private float minRadius = 4;
    private float maxRadius = 6;
    private int SummonCount = 3;       

    private float aimDuration = 0.5f;
    private float endDelay = 0.5f;
    private float timer;
    
    public Summoner_Summon(Summoner summoner, Summoner_FSM fsm) : base(summoner, fsm)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        phase = Phase.Aim;
        timer = 0.0f;

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
            (summoner.Player.transform.position = summoner.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        summoner.Rigidbody2D.SetRotation(angle);

        if(timer>=aimDuration)
        {
            phase = Phase.Summon;
            timer = 0.0f;
        }
    }

    private void Tick_Summon()
    {
        SummonSkeleton();
        phase = Phase.End;
        timer = 0;
    }

    private void Tick_End()
    {
        if(timer>=endDelay)
        {
            fsm.ChangeState(SummonerStateEnum.normal);
        }
    }

    private Vector3 GetRandomPos()
    {
        Vector3 randomDir = Random.insideUnitCircle.normalized;

        float dist = Random.Range(minRadius, maxRadius);

        Vector3 randomPoint = summoner.transform.position + randomDir * dist;

        return randomPoint;
    }

    private void SummonSkeleton()
    {
        for(int i=0; i<SummonCount; i++)
        {
            PoolObject skeleton = summoner.PoolManager.Get(9);

            skeleton.transform.SetParent(summoner.EnemyBox);
            skeleton.transform.position = GetRandomPos();
            skeleton.transform.rotation = Quaternion.identity;

            skeleton.GetComponent<IWaveScalable>().ApplyWaveScale(summoner.WaveMultiplier);
        }        
    }
}
