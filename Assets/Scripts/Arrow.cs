using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : PoolObject
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
    private Player player;
    private PoolManager poolManager;
    private PoolObject poolObject;

    private float angle;
    private float arrowSpeed;
    private float damage;
    private float ultGain;
    private bool isUltArrow;
    private int hitCount;
    public Transform firstEnemy;
    private List<Transform> hittedEnemy = new List<Transform>();
    
    public Vector2 Dir;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<Player>();
        poolObject = GetComponent<PoolObject>();
        poolManager = FindAnyObjectByType<PoolManager>();
    }

    private void Update()
    {
        //Debug.Log()
    }

    public override void Init()
    {
        rigidbody2D.linearVelocity = Vector2.zero;
        poolManager.PushDelay(poolObject, durationTime);
    }

    public void Shoot(Vector2 direction,float speed,float chargeRatio)
    {
        hitCount = 0;
        firstEnemy = null;

        hittedEnemy.Clear();

        Dir = direction.normalized;
        arrowSpeed = speed;
              
        rigidbody2D.linearVelocity = Dir * speed;

        angle = 
            Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle-90);

        damage = player.power * 10 * chargeRatio;
        ultGain = 10 * chargeRatio;
        isUltArrow = player.isInUltState == true ? true : false;

        poolManager.PushDelay(poolObject, durationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target =
            collision.GetComponentInParent<IDamageable>();

        if (target is null)
            return;

        Transform root = ((Component)target).transform;
        if (hittedEnemy.Contains(root))
            return;

        hittedEnemy.Add(root);

        if(!isUltArrow)
        {
            target.Damage(damage);
            player.UltStack += ultGain;
            poolManager.PushNow(poolObject);
            return;
        }

        target.Damage(damage * 1.2f);
        hitCount++;
        
        Transform nextTarget = 
            hitCount >= homingMaxCount ? null : FindNearestEnemy();

        if (nextTarget is null)
        {
            poolManager.PushNow(poolObject);
            return;
        }

        Retarget(nextTarget);        
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] enemyColliders = 
            Physics2D.OverlapCircleAll(transform.position, homingSearchRange);

        Transform nearestEnemy = null;
        float nearestEnemyDistance=float.MaxValue;

        foreach(Collider2D enemyCollider in enemyColliders)
        {
            IDamageable target = 
                enemyCollider.GetComponentInParent<IDamageable>();

            if (target is null)
                continue;

            Transform enemyTransform = ((Component)target).transform;        

            if(hittedEnemy.Contains(enemyTransform))
            {
                continue;
            }

            float distance = 
                (enemyTransform.position - transform.position).sqrMagnitude;

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
        Vector2 nextdirection = 
            ((Vector2)target.position - rigidbody2D.position).normalized;

        Dir = nextdirection;
        
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
