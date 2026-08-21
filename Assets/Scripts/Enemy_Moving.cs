using UnityEngine;

public partial class Enemy
{
    private void TracePlayer()
    {
        if (player == null|| hp <= 0)
        {
            return;
        }        

        Vector2 direction =
            (player.position - transform.position).normalized;

        float distance =
            (Vector2.Distance(player.position, transform.position));

        if (distance > data[1].StopDistance)
        {
            rigidbody2D.linearVelocity = direction * data[1].MoveSpeed;
        }        
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}
