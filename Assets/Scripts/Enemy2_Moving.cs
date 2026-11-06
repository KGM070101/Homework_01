using UnityEngine;

public partial class Enemy2
{
    private float EnemyTimer;

    private void TracePlayer()
    {
        //if (playerPos == null|| hp <= 0)
        //{
        //    return;
        //}        

        facingDir = (player.transform.position - transform.position);

        Vector2 direction =
            (player.transform.position - transform.position).normalized;

        float distance =
            (Vector2.Distance(player.transform.position, transform.position));

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rigidbody2D.SetRotation(angle);

        if (isKnockbacking)
        {
            return;
        }

        if (distance > data[2].StopDistance)
        {
            if (isKnockbacking == false)
            {
                rigidbody2D.linearVelocity = direction * speed;
                if (distance > 25.0f)
                {
                    rigidbody2D.linearVelocity = direction * speed * 3.0f;
                }
            }
        }
        else if(distance<10 )
        {
            rigidbody2D.linearVelocity = -direction * speed*1.5f;
        }
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;
            TryAttack();

        }
    }

    private void GetBullet()
    {
        PoolObject pool = poolManager.Get(6);

        pool.transform.SetParent(poolManager.BulletBox);
        pool.transform.position = firePoint.position;
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<BulletForEnemy2>()?.Shoot(facingDir, power);
    }

    private void TryAttack()
    {
        if (Time.time < EnemyTimer)
        {
            return;
        }

        EnemyTimer = Time.time + data[2].AttackSpeed;

        GetBullet();

        WeaponBounce();                          
    }
}
