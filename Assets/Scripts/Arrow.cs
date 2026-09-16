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

    [SerializeField]
    private float homingSearchRange = 20.0f;

    [SerializeField]
    private float homingMaxCount = 5.0f;

    private new Rigidbody2D rigidbody2D;
    private Enemy enemy;
    private Enemy2 enemy2;
    private Player player;

    private float angle;
    private float arrowSpeed;
    private int hitCount;
    public Transform firstEnemy;

    public Vector2 Dir;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        enemy = FindAnyObjectByType<Enemy>(FindObjectsInactive.Include);
        enemy2 = FindAnyObjectByType<Enemy2>(FindObjectsInactive.Include);
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        //Debug.Log()
    }

    public void Shoot(Vector2 direction,float speed)
    {
        hitCount = 0;
        firstEnemy = null;

        Dir = direction.normalized;
        arrowSpeed = speed;
       
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = 
            Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle-90);

        Destroy(gameObject, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("충돌");

        if (!player.isInUltState)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy")||
            collision.gameObject.CompareTag("Enemy2"))
        {
            //Debug.Log("충돌");

            firstEnemy = collision.transform;

            hitCount++;
            
            if(hitCount>=homingMaxCount)
            {
                Destroy(gameObject);
                return;
            }

            Transform nextTarget = FindNearestEnemy(firstEnemy);

            //Debug.Log(nextTarget);

            if(nextTarget==null)
            {
                Destroy(gameObject);
                return;
            }

            Retarget(nextTarget);
        }
    }

    private Transform FindNearestEnemy(Transform firstEnemy)
    {
        Collider2D[] enemyColliders = 
            Physics2D.OverlapCircleAll(transform.position, homingSearchRange);

        Transform nearestEnemy = null;
        float nearestEnemyDistance=float.MaxValue;

        foreach(Collider2D enemyCollider in enemyColliders)
        {
            if(!enemyCollider.CompareTag("Enemy")&&
                !enemyCollider.CompareTag("Enemy2"))
            {
                continue;
            }

            Transform enemyTransform = enemyCollider.transform;

            if(enemyTransform==firstEnemy)
            {
                continue;
            }

            float distance = (enemyTransform.position - transform.position).sqrMagnitude;

            if(distance<nearestEnemyDistance)
            {
                nearestEnemyDistance = distance;
                nearestEnemy = enemyTransform;
            }
        }

        return nearestEnemy;
    }

    private void Retarget(Transform target)
    {
        Vector2 nextdirection = ((Vector2)target.position - rigidbody2D.position).normalized;

        Dir = nextdirection;

        //rigidbody2D.position += Dir * 0.2f;

        rigidbody2D.linearVelocity = Dir * arrowSpeed;

        angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90f);       
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, homingSearchRange);
    }
#endif
}
