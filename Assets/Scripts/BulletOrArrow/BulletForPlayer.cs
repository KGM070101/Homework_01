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

    private float damage;

    private new Rigidbody2D rigidbody2D;

    private PoolManager poolManager;
    private PoolObject poolObject;
    private Player player;

    private Vector2 knockbackDir;

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
        if (hitted)
            return;

        IDamageable target = 
            collision.GetComponentInParent<IDamageable>();
        IKnockbackable targetToKnockback =
            collision.GetComponentInParent<IKnockbackable>();
        
        if (target is null)
            return;

        if(player.isInBurstFireUltState)
        {
            target.Damage(damage*1.2f);
            return;
        }
        else if(player.isInShotgunUltState)
        {
            if(targetToKnockback!=null)
            {                
                targetToKnockback.Knockback(knockbackDir, 0.2f, 10.0f);
            }

            hitted = true;
            target.Damage(damage*1.2f);                        
            player.Heal(1);
            poolManager.PushNow(poolObject);
            return;
        }

        hitted = true;
        target.Damage(damage);
        player.UltStack += 1;
        poolManager.PushNow(poolObject);        
    }

    public override void Init()
    {
        hitted = false;
        rigidbody2D.linearVelocity = Vector2.zero;
        poolManager.PushDelay(poolObject, durationTime);
    }

    public void Shoot(Vector2 direction, float damage, float lifeTime = -1.0f)
    {
        this.damage = damage;

        Vector2 normalizedDirection = direction.normalized;

        knockbackDir = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2
            (normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle+90);

        //Destroy(gameObject, durationTime);
        float life = lifeTime > 0.0f ? lifeTime : durationTime;
        poolManager.PushDelay(poolObject, life);               
    }    
}
