using UnityEngine;

public class BulletForBoss0 : PoolObject
{
    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private float durationTime = 2.5f;

    private new Rigidbody2D rigidbody2D;
    private PoolManager poolManager;
    private PoolObject poolObject;

    private float angle;
    private float damage;

    private bool hitted = false;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();        
        poolObject = GetComponent<PoolObject>();
        poolManager = FindAnyObjectByType<PoolManager>();
    }

    public override void Init()
    {
        hitted = false;
        rigidbody2D.linearVelocity = Vector2.zero;
        poolManager.PushDelay(poolObject, durationTime);
    }

    public void Shoot(Vector2 direction,float damage)
    {
        this.damage = damage;

        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);        

        poolManager.PushDelay(poolObject, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (hitted)
                return;            

            Player player = collision.GetComponentInParent<Player>();

            if(player!=null)
            {
                player.TakeDamage(damage);
            }

            hitted = true;
            poolManager.PushNow(poolObject);           
        }
    }
}
