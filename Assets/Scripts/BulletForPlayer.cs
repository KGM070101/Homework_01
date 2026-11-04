using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletForPlayer : PoolObject
{
    [SerializeField]
    private float speed = 12.0f;

    [SerializeField]
    public float durationTime = 3.0f;
    public float DurationTIme => durationTime;

    private new Rigidbody2D rigidbody2D;

    private PoolManager poolManager;
    private PoolObject poolObject;
    private Player player;

    //private Enemy enemy;
    //private Enemy2 enemy2;

    private float angle;

    private bool hitted=false;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        poolManager = FindAnyObjectByType<PoolManager>();
        poolObject = GetComponent<PoolObject>();
        player = FindAnyObjectByType<Player>();
        //enemy = FindAnyObjectByType<Enemy>();
        //enemy2 = FindAnyObjectByType<Enemy2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hitted)
                return;

            hitted = true;

            if (player.isInBurstFireUltState == false)
            {
                Enemy enemy = collision.GetComponentInParent<Enemy>();
                enemy.Damage(player.power);
                player.UltStack += 1;
                poolManager.PushNow(poolObject);
            }
            else
            {
                Enemy enemy = collision.GetComponentInParent<Enemy>();
                enemy.Damage(player.power * 1.2f);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy2"))
        {
            if (hitted)
                return;

            hitted = true;

            if (player.isInBurstFireUltState == false)
            {
                Enemy2 enemy2 = collision.GetComponentInParent<Enemy2>();
                enemy2.Damage(player.power);
                player.UltStack += 1;
                poolManager.PushNow(poolObject);
            }
            else
            {
                Enemy2 enemy2 = collision.GetComponentInParent<Enemy2>();
                enemy2.Damage(player.power * 1.2f);
            }
        }
        else if (collision.gameObject.gameObject.CompareTag("Boss0"))
        {
            if (hitted)
                return;

            hitted = true;

            if (player.isInBurstFireUltState == false)
            {
                Boss_0 boss = collision.gameObject.GetComponentInParent<Boss_0>();
                boss.Damage(player.power);
                player.UltStack += 1;
                poolManager.PushNow(poolObject);
            }
            else
            {
                Boss_0 boss = collision.gameObject.GetComponentInParent<Boss_0>();
                boss.Damage(player.power * 1.2f);
            }
        }        
    }

    public override void Init()
    {
        hitted = false;
        rigidbody2D.linearVelocity = Vector2.zero;
        poolManager.PushDelay(poolObject, durationTime);
    }

    public void Shoot(Vector2 direction, float lifeTime = -1.0f)
    {
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2
            (normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle+90);

        //Destroy(gameObject, durationTime);
        float life = lifeTime > 0.0f ? lifeTime : durationTime;
        poolManager.PushDelay(poolObject, life);               
    }    
}
