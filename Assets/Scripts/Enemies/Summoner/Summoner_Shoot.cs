using UnityEngine;

public class Summoner_Shoot : Summoner_States
{
    private enum Phase
    {
        Shoot,
        End
    }
    private Phase phase;

    private float timer;
    private float endDelay = 1.0f;    
    private float fireDuration = 6.0f;
    private float fireTimer;

    public Summoner_Shoot(Summoner summoner, Summoner_FSM fsm) : base(summoner, fsm)
    {

    }

    public override void Enter()
    {
        base.Enter();
        fireTimer = 0.0f;

        phase = Phase.Shoot;
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
            case Phase.Shoot:
                Tick_Shoot();
                break;
            case Phase.End:
                Tick_End();
                break;
        }
    }

    private void Tick_Shoot()
    {
        Vector2 dir =
            summoner.FacingDir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        summoner.Rigidbody2D.linearVelocity = Vector2.zero;
        summoner.Rigidbody2D.SetRotation(angle);

        if (timer >= fireDuration)
        {
            phase = Phase.End;
            timer = 0.0f;
            return;
        }

        if (Time.time<fireTimer)
            return;

        fireTimer = Time.time + summoner.AttackSpeed;

        GetBullet();       
    }

    private void Tick_End()
    {
        Vector2 dir =
            summoner.FacingDir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;       
        summoner.Rigidbody2D.SetRotation(angle);       

        if (timer >= endDelay)
        {
            fsm.ChangeState(SummonerStateEnum.normal);
            return;
        }

        if (summoner.IsKnockbacking)
            return;

        summoner.Rigidbody2D.linearVelocity = Vector2.zero;
    }

    private void GetBullet()
    {
        PoolObject pool = summoner.PoolManager.Get(11);

        pool.transform.SetParent(summoner.PoolManager.BulletBox);
        pool.transform.position = summoner.FirePoint.position;
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<BulletForSummoner>()?.Shoot(summoner.FacingDir);
    }
}
