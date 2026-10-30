using UnityEngine;

public class Boss0_Pattern0 : Boss0_States
{    
    private enum Phase
    {
        Aim,
        Dash,
        End
    }

    private Phase phase;

    private float aimDuration = 2.0f;
    private float dashDuration = 1.2f;
    private float endDelay = 1.0f;

    private float spinAngle;
    private float spinSpeed = 1440.0f;

    private float timer;
    private Vector2 dashDirection;       

    public Boss0_Pattern0(Boss_0 boss, Boss0_FSM fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        base.Enter();

        phase = Phase.Aim;                
        timer = 0.0f;
        spinAngle = 0.0f;
        
        boss.DashReady(aimDuration);
        
        boss.Rigidbody2D.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick(float deltaTIme)
    {
        base.Tick(deltaTIme);
        timer += deltaTIme;

        //if (!normalState.CanUsePattern)
        //    return;

        switch(phase)
        {
            case Phase.Aim:
                Tick_Aim();
                break;
            case Phase.Dash:
                Tick_Dash();
                break;
            case Phase.End:
                Tick_End();
                break;
        }
    }

    private void Tick_Aim()
    {
        boss.OnDashRange(timer, aimDuration);

        Vector2 dir = (boss.Player.transform.position - boss.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        boss.Rigidbody2D.SetRotation(angle);

        if(timer>=aimDuration)
        {
            dashDirection = dir;
            phase = Phase.Dash;
            timer = 0f;
        }
    }

    private void Tick_Dash()
    {
        boss.CancelDashRange();

        boss.Rigidbody2D.linearVelocity = dashDirection * boss.Speed * 6f;

        spinAngle += spinSpeed * Time.deltaTime;
        boss.Rigidbody2D.SetRotation(spinAngle);
        

        if(timer>=dashDuration)
        {
            boss.Rigidbody2D.linearVelocity = Vector2.zero;
            phase = Phase.End;
            timer = 0f;
        }
    }

    private void Tick_End()
    {
        Vector2 dir = (boss.Player.transform.position - boss.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        boss.Rigidbody2D.SetRotation(angle);

        if(timer>=endDelay)
        {
            fsm.ChangeState(StateEnum.normal);
        }
    }
}
