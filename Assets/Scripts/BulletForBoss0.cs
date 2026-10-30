using UnityEngine;

public class BulletForBoss0 : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private float durationTime = 10.0f;

    private new Rigidbody2D rigidbody2D;
    private Boss_0 boss;

    private float angle;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boss = FindFirstObjectByType<Boss_0>();
    }

    public void Shoot(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

        Destroy(gameObject, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponentInParent<Player>();

            if(player!=null)
            {
                player.TakeDamage(boss.Power);
            }

            Destroy(gameObject);
        }
    }
}
