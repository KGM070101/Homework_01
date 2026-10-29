using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss0_Normal : Boss0_States
{   
    public Boss0_Normal(Player player, Boss_0 boss, Boss0_FSM fsm) : base(player, boss, fsm)
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

    public override void Tick(float deltaTime)
    {
        base.Enter();

        Vector2 direction =
            (player.transform.position - boss.transform.position).normalized;

        float distance =
            (Vector2.Distance(player.transform.position, boss.transform.position));

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        boss.Rigidbody2D.SetRotation(angle);

        if (distance > boss.StopDistance)
        {
            boss.Rigidbody2D.linearVelocity = direction * boss.Speed;

            if (distance > 25.0f)
            {
                boss.Rigidbody2D.linearVelocity = direction * boss.Speed * 3.0f;
            }
        }
        else
        {
            boss.Rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}
