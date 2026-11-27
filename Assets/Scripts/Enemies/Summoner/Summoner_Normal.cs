using UnityEngine;

public class Summoner_Normal : Summoner_States
{
    private float patternCheckInterval = 0.2f;
    private float patternTimer;
    private float patternChance = 0.5f;
    private float angle;

    private Vector2 direction;    

    public Summoner_Normal(Summoner summoner, Summoner_FSM fsm) : base(summoner, fsm)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick(float deltaTIme)
    {
        base.Tick(deltaTIme);

        //Tracing Player
        {
            direction =
            (summoner.Player.transform.position - summoner.transform.position).normalized;

            float distance =
                Vector2.Distance(summoner.Player.transform.position, summoner.transform.position);

            angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            summoner.Rigidbody2D.SetRotation(angle);

            if (summoner.IsKnockbacking)
            {
                return;
            }               

            if(distance>summoner.StopDistance)
            {
                summoner.Rigidbody2D.linearVelocity = direction * summoner.Speed;

                if(distance>25.0f)
                {
                    summoner.Rigidbody2D.linearVelocity = direction * summoner.Speed * 3.0f;
                }
            }
            else if(distance<7)
            {
                summoner.Rigidbody2D.linearVelocity = -direction * summoner.Speed * 1.5f;
            }
            else
            {
                summoner.Rigidbody2D.linearVelocity = Vector2.zero;

                patternTimer += deltaTIme;
                if (patternTimer >= patternCheckInterval)
                {
                    patternTimer = 0;

                    if (Random.value <= patternChance)
                    {
                        fsm.ChangeState
                            (Random.value < 0.4f ? SummonerStateEnum.patternSummon : SummonerStateEnum.patternShoot);                        
                    }
                }
            }
        }
    }
}
