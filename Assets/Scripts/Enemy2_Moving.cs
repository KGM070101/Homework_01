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
            }
        }
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;
            TryAttack();

        }
    }

    private void TryAttack()
    {
        if (Time.time < EnemyTimer)
        {
            return;
        }

        EnemyTimer = Time.time + data[2].AttackSpeed;

        BulletForEnemy2 bullet =
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity, bulletBox);
        bullet.Shoot(facingDir);

        WeaponBounce();
            
        //player.TakeDamage(power);        
    }
}
