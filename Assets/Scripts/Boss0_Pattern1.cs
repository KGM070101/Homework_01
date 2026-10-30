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
    private float dashDuration = 0.7f;
    private float dashSpeedMultifly = 4.0f;

    //Phase.Shoot
    private int spinCycleCount = 5;
    private float spinSpeed = 280f;
    private float fireInterval = 0.4f;
    private int bulletCount = 5;
    private float spawnRadius = 1.0f;

    //Phase.End
    private float endDelay = 1.0f;

    private float timer;
    private float fireTImer;
    private float spinAngle;
    private float totalSpin;
    private Vector2 dashDirection;

    private PoolManager poolManager;
        
    public Boss0_Pattern1(Boss_0 boss, Boss0_FSM fsm) : base(boss, fsm)
    {
    }

    public override void Enter()
    {
        base.Enter();

        phase = Phase.Dash;
        timer = 0.0f;
        fireTImer = 0.0f;
        totalSpin = 0.0f;

        dashDirection = (boss.Player.transform.position - boss.transform.position).normalized;
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
        float distance=Vector2.Distance(boss.Player.transform.position, boss.transform.position);

        boss.Rigidbody2D.linearVelocity = dashDirection * boss.Speed * dashSpeedMultifly;
        boss.Rigidbody2D.freezeRotation=true;

        if(timer>=dashDuration||distance<3.0f)
        {
            boss.Rigidbody2D.linearVelocity = Vector2.zero;
            boss.Rigidbody2D.freezeRotation=false;
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

            GameObject obj=
            UnityEngine.Object.Instantiate(boss.BulletPrefab,spawnPos,Quaternion.identity);
            obj.GetComponent<BulletForBoss0>().Shoot(dir);
        }
    }

    private void Tick_End()
    {
        dashDirection = (boss.Player.transform.position - boss.transform.position).normalized;
        float targetAngle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;

        float finalAngle =
            Mathf.MoveTowardsAngle(boss.Rigidbody2D.rotation, targetAngle, 120f * Time.deltaTime);

        boss.Rigidbody2D.SetRotation(finalAngle);

        if (timer >= endDelay)
            fsm.ChangeState(StateEnum.normal);
    }
}
