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

        if (distance > data[1].StopDistance)
        {
            rigidbody2D.linearVelocity = direction * data[1].MoveSpeed;
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

        player.TakeDamage(data[1].Power);

        Enemy_Punch();
    }
}
