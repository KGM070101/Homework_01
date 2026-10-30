using Unity.VisualScripting;
using UnityEngine;

public class Boss0_Pattern1 : Boss0_States
{   private enum Phase
    {
        Dash,
        Shoot,
        End
    }
    private Phase phase;

    private BulletForBoss0 bulletPrefab;

    //Phase.Dash
    private float dashDuration = 0.5f;
    private float dashSpeedMultifly = 3.0f;

    //Phase.Shoot
    private int spinCycleCount = 5;
    private float spinSpeed = 240f;
    private float fireInterval = 0.5f;
    private int bulletCount = 8;
    private float spawnRadius = 1.0f;

    //Phase.End
    private float endDelay = 1.0f;

    private float timer;
    private float fireTImer;
    private float spinAngle;
    private float totalSpin;
    private Vector2 dashDirection;

    private PoolManager poolManager;

    public Boss0_Pattern1(Player player, Boss_0 boss, Boss0_FSM fsm, Boss0_Normal normalState) : base(player, boss, fsm, normalState)
    {

    }

    public override void Enter()
    {
        base.Enter();

        phase = Phase.Dash;
        timer = 0.0f;
        fireTImer = 0.0f;
        totalSpin = 0.0f;

        dashDirection = (player.transform.position - boss.transform.position).normalized;
        spinAngle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
        boss.Rigidbody2D.SetRotation(spinAngle);

        if (poolManager == null)
            poolManager = Object.FindAnyObjectByType<PoolManager>();
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
            case Phase.Dash:
                Tick_Dash();
                break;
            case Phase.Shoot:
                Tick_Shoot(Time.deltaTime);
                break;
            case Phase.End:
                Tick_End();
                break;
        }
    }

    private void Tick_Dash()
    {
        boss.Rigidbody2D.linearVelocity = dashDirection * boss.Speed * dashSpeedMultifly;

        if(timer>=dashDuration)
        {
            boss.Rigidbody2D.linearVelocity = Vector2.zero;
            phase = Phase.Shoot;
            timer = 0.0f;
        }
    }

    private void Tick_Shoot(float deltaTIme)
    {
        float delta = spinSpeed * deltaTIme;
        spinAngle += delta;
        totalSpin += Mathf.Abs(delta);
        boss.Rigidbody2D.SetRotation(spinAngle);

        fireTImer += deltaTIme;

        if(fireTImer>=fireInterval)
        {
            fireTImer = 0.0f;
            Fire();
        }

        if(totalSpin>=spinCycleCount*360f)
        {
            phase = Phase.End;
            timer = 0.0f;
        }
    }

    private void Fire()
    {
        float step = 360.0f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = spinAngle + step * i;
            float radian = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            Vector2 spawnPos = (Vector2)boss.transform.position + dir * spawnRadius;

            
        }
    }

    private void Tick_End()
    {
        if (timer >= endDelay)
            fsm.ChangeState(StateEnum.normal);
    }
}
