using UnityEngine;

public partial class Enemy
{
    private float EnemyTimer;
    private void TracePlayer()
    {
        //if (playerPos == null|| hp <= 0)
        //{
        //    return;
        //}        

        Vector2 direction =
            (player.transform.position - transform.position).normalized;

        float distance =
            (Vector2.Distance(player.transform.position, transform.position));
             
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rigidbody2D.SetRotation(angle);

        if(isKnockbacking)
        {
            return;
        }    

        if (distance > data[1].StopDistance)
        {
            if(isKnockbacking==false)
            {
                rigidbody2D.linearVelocity = direction * speed;
                if(distance>25.0f)
                {
                    rigidbody2D.linearVelocity = direction * speed * 3.0f;
                }
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

        EnemyTimer = Time.time + data[1].AttackSpeed;

        player.TakeDamage(power);

        Enemy_Punch();
    }
}
