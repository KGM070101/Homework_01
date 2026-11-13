using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : PoolObject
{
    [SerializeField]
    private float durationTime = 3.0f;
   
    [SerializeField]
    private float homingSearchRange = 20.0f;

    [SerializeField]
    private float homingMaxCount = 5.0f;

    [SerializeField]
    private float turnSpeed = 1080.0f;

    private Transform target;
    private bool consumed;

    private new Rigidbody2D rigidbody2D;
    private Player player;
    private PoolManager poolManager;

    private float angle;
    private float arrowSpeed;
    private float damage;
    private float ultGain;
    private bool isUltArrow;
    private int hitCount;
    private List<Transform> hittedEnemy = new List<Transform>();
    
    public Vector2 Dir;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = FindAnyObjectByType<Player>();
        poolManager = FindAnyObjectByType<PoolManager>();
    }

    private void Update()
    {

    }

    public override void Init()
    {
        rigidbody2D.linearVelocity = Vector2.zero;
        poolManager.PushDelay(this, durationTime);
        hitCount = 0;
        hittedEnemy.Clear();
        target = null;
        consumed = false;
        isUltArrow = false;      
    }

    public void Shoot(Vector2 direction,float speed,float chargeRatio)
    {
        Dir = direction.normalized;
        arrowSpeed = speed;
              
        rigidbody2D.linearVelocity = Dir * speed;

        angle = 
            Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle-90);

        damage = player.power * 10 * chargeRatio;
        ultGain = 10 * chargeRatio;
        isUltArrow = player.isInUltState == true ? true : false;

        poolManager.PushDelay(this, durationTime);
    }

    private void FixedUpdate()
    {
        if (!isUltArrow || target is null)
            return;

        if(!target.gameObject.activeInHierarchy)
        {
            target = FindNearestEnemy();
            if (target is null)
                return;
        }

        Vector2 to = (Vector2)target.position - rigidbody2D.position;
        if (to.sqrMagnitude < 0.0001f)
            return;

        float current = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        float wanted = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;

        float next = 
            Mathf.MoveTowardsAngle(current, wanted, turnSpeed * Time.fixedDeltaTime);

        float rad = next * Mathf.Deg2Rad;
        Dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        rigidbody2D.linearVelocity = Dir * arrowSpeed;
        transform.rotation = Quaternion.Euler(0f, 0f, next - 90f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (consumed)
            return;

        IDamageable damageable =
            collision.GetComponentInParent<IDamageable>();

        if (damageable is null)
            return;

        Transform root = ((Component)damageable).transform;
        if (hittedEnemy.Contains(root))
            return;

        hittedEnemy.Add(root);

        if(!isUltArrow)
        {
            consumed = true;
            damageable.Damage(damage);
            player.UltStack += ultGain;
            poolManager.PushNow(this);
            return;
        }

        damageable.Damage(damage * 1.2f);
        hitCount++;

        target = hitCount >= homingMaxCount ? null : FindNearestEnemy();
       
        if (target is null)
        {
            consumed = true;
            poolManager.PushNow(this);            
        }             
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

    //private void Retarget(Transform target)
    //{
    //    Vector2 nextdirection = 
    //        ((Vector2)target.position - rigidbody2D.position).normalized;

    //    Dir = nextdirection;
        
    //    rigidbody2D.linearVelocity = Dir * arrowSpeed;
    //    angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90f);       
    //}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, homingSearchRange);
    }
#endif
}
