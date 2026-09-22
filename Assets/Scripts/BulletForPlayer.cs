using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletForPlayer : MonoBehaviour
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

    private bool hitted = false;
    //private Enemy enemy;
    //private Enemy2 enemy2;

    private float angle;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        poolManager = FindAnyObjectByType<PoolManager>();
        poolObject = GetComponent<PoolObject>();
        player = FindAnyObjectByType<Player>();
        //enemy = FindAnyObjectByType<Enemy>();
        //enemy2 = FindAnyObjectByType<Enemy2>();
    }

    public void Shoot(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle+90);

        //Destroy(gameObject, durationTime);
        if(hitted==false)
        {
            poolManager.Push(poolObject, durationTime);
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {                
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if(player.isInBurstFireUltState==false)
            {
                hitted = true;
                Enemy enemy = collision.GetComponentInParent<Enemy>();
                enemy.Damage(player.power);
                player.UltStack += 1;
                poolManager.Push(poolObject, 0);
            }
            else
            {
                hitted = true;
                Enemy enemy = collision.GetComponentInParent<Enemy>();                
                enemy.Damage(player.power * 1.2f);
            }
        }
        if (collision.gameObject.CompareTag("Enemy2"))
        {
            if (player.isInBurstFireUltState == false)
            {
                hitted = true;
                Enemy2 enemy2 = collision.GetComponentInParent<Enemy2>();
                enemy2.Damage(player.power);
                player.UltStack += 1;
                poolManager.Push(poolObject, 0);
            }
            else
            {
                hitted = true;
                Enemy2 enemy2 = collision.GetComponentInParent<Enemy2>();
                enemy2.Damage(player.power * 1.2f);
            }
        }
    }
}
