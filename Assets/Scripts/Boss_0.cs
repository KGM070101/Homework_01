using UnityEngine;

public class Boss_0 : Character
{
    private Player player;

    private float hp;
    private float speed;
    private float power;
    private float stopDistance;

    protected override void Awake()
    {
        base.Awake();

        player = FindFirstObjectByType<Player>();

        hp = data[1].MaxHp;
        speed = data[1].MoveSpeed;
        power = data[1].Power;
        stopDistance = data[1].StopDistance;

        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Boss_0"), LayerMask.NameToLayer("Enemy"), true);           
        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Boss_0"), LayerMask.NameToLayer("Enemy2"), true);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(power);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        TracePlayer();
    }

    private void TracePlayer()
    {           
        Vector2 direction =
            (player.transform.position - transform.position).normalized;

        float distance =
            (Vector2.Distance(player.transform.position, transform.position));

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rigidbody2D.SetRotation(angle);
        
        if (distance > stopDistance)
        {
            rigidbody2D.linearVelocity = direction * speed;

            if (distance > 25.0f)
            {
                rigidbody2D.linearVelocity = direction * speed * 3.0f;
            }            
        }
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;            
        }
    }
}
