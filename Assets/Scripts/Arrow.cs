using Unity.Mathematics;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float durationTime = 3.0f;

    [SerializeField]
    private Arrow arrowPrefab;

    [SerializeField]
    private Transform bulletBox;

    private new Rigidbody2D rigidbody2D;
    private Enemy enemy;
    private Enemy2 enemy2;
    private Player player;

    private float angle;
    public Transform EndPos;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        enemy = FindAnyObjectByType<Enemy>(FindObjectsInactive.Include);
        enemy2 = FindAnyObjectByType<Enemy2>(FindObjectsInactive.Include);
        player = FindAnyObjectByType<Player>();
    }
    
    public void Shoot(Vector2 direction,float speed)
    {
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = 
            Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle-90);

        Destroy(gameObject, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy")||
            collision.gameObject.CompareTag("Enemy2"))
        {
            EndPos.position = transform.position;

            if(player.isInArrowUltState==true)
            {
                //float arrowSpreadAngle = 360f;
                float arrowCount = 10f;

                //float startAngle = arrowSpreadAngle * 0.5f;
                //float angleStep =
                //    arrowSpreadAngle / (arrowCount - 1);


                for (int i=0; i<arrowCount; i++)
                {
                    float angle = 360f / arrowCount * i;
                    Vector2 dir = 
                        new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), 
                        Mathf.Sin(angle * Mathf.Deg2Rad));

                    Arrow arrow =
                        Instantiate(arrowPrefab,
                        EndPos.position,
                        Quaternion.identity,
                        bulletBox);

                    arrow.Shoot(dir, 60);
                }
            }
        }
    }
}
