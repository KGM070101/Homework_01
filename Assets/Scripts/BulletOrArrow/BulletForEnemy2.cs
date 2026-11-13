using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletForEnemy2 : PoolObject
{
    [SerializeField]
    private float speed = 12.0f;

    [SerializeField]
    public float durationTime = 3.0f;

    private new Rigidbody2D rigidbody2D;
    private PoolManager poolManager;

    private float angle;
    private float damage;

    private bool hitted = false;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        poolManager = FindAnyObjectByType<PoolManager>();
    }

    public override void Init()
    {
        hitted = false;
        rigidbody2D.linearVelocity = Vector2.zero;
    }

    public void Shoot(Vector2 direction, float damage)
    {
        this.damage = damage;

        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle + 90);

        poolManager.PushDelay(this, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (hitted)
                return;

            Player player = collision.GetComponentInParent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            hitted = true;
            poolManager.PushNow(this);
        }
    }
}
