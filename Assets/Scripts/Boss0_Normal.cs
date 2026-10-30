using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss0_Normal : Boss0_States
{
    
    private float patternCheckInterval = 1.0f;
    private float patternTImer;
    private float patternChance = 0.4f;

    private bool canUsePattern;
    public bool CanUsePattern => canUsePattern;
    
    private Vector2 direction;
    public Vector2 Direction => direction;

    private float angle;
    public float Angle => angle;

    
    public Boss0_Normal(Player player, Boss_0 boss, Boss0_FSM fsm, Boss0_Normal normalState) : base(player, boss, fsm, normalState)
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

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        //Tracing Player
        {
            direction =
            (player.transform.position - boss.transform.position).normalized;

            float distance =
                (Vector2.Distance(player.transform.position, boss.transform.position));

            angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            boss.Rigidbody2D.SetRotation(angle);

            if (distance > boss.StopDistance)
            {
                //canUsePattern = false;

                boss.Rigidbody2D.linearVelocity = direction * boss.Speed;

                if (distance > 25.0f)
                {
                    boss.Rigidbody2D.linearVelocity = direction * boss.Speed * 3.0f;
                }
            }
            else
            {
                //canUsePattern = true;

                boss.Rigidbody2D.linearVelocity = Vector2.zero;                

                patternTImer += deltaTime;
                if(patternTImer>=patternCheckInterval)
                {
                    patternTImer = 0;

                    if(Random.value<=patternChance)
                    {
                        fsm.ChangeState
                            (Random.value < 0.5f ? StateEnum.pattern0 : StateEnum.pattern1);

                        //fsm.ChangeState(StateEnum.pattern0);
                    }
                }
            }
        }
       
    }
}
