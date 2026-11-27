using Mono.Cecil;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Drone : SkillRunTIme
{
    [SerializeField] private float wanderRadius = 3.0f;
    [SerializeField] private Vector2 wanderInterval = new Vector2(2.0f, 5.0f);
    [SerializeField] private float delayTime = 0.5f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float detectRadius = 15.0f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 720.0f;

    private Rigidbody2D rigidbody2D;
    private PoolManager poolManager;
    private Transform target;
    private Vector2 dir;
    private Vector2 offset;
    private Vector2 velocity;
    private float timer;
    private float fireTimer;

    private void Awake()
    {
        rigidbody2D=GetComponent<Rigidbody2D>();
        poolManager = FindFirstObjectByType<PoolManager>();
    }

    public override void Configure(Player player)
    {
        base.Configure(player);

        transform.position = player.transform.position;
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            ChangeOffset();

        Vector2 targetPos = (Vector2)player.transform.position + offset;
                
        transform.position = 
            Vector2.SmoothDamp(transform.position, targetPos, ref velocity, delayTime, maxSpeed);

        target = FindNeardestEnemy();

        if (target)
        {
            dir=(target.transform.position - transform.position).normalized;            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float finalAngle =
                Mathf.MoveTowardsAngle(rigidbody2D.rotation, angle, turnSpeed * Time.deltaTime);
           
            rigidbody2D.SetRotation(finalAngle);

            TryAttack();
        }
        else
        {
            Vector2 facingDir = targetPos - (Vector2)transform.position;
            Vector2 norFacingDir = facingDir.normalized;
            float facingAngle = Mathf.Atan2(norFacingDir.y, norFacingDir.x) * Mathf.Rad2Deg;

            float finalFacingAngle=
                Mathf.MoveTowardsAngle(rigidbody2D.rotation, facingAngle, turnSpeed * Time.deltaTime);
           
            rigidbody2D.SetRotation(finalFacingAngle);
        }
    }

    private void ChangeOffset()
    {
        offset = Random.insideUnitCircle * wanderRadius;
        timer = Random.Range(wanderInterval.x, wanderInterval.y);
    }

    private Transform FindNeardestEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(transform.position, detectRadius);

        Transform nearestEnemy = null;
        float nearestEnemyDistance = float.MaxValue;

        foreach(Collider2D enemy in enemies)
        {
            IDamageable target=
                enemy.GetComponentInParent<IDamageable>();

            if (target is null)
                continue;

            Transform enemyTransform = ((Component)target).transform;

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

    private void TryAttack()
    {
        if (Time.time < fireTimer)
            return;

        fireTimer = Time.time + attackSpeed;

        GetBullet();
    }

    private void GetBullet()
    {
        PoolObject pool = poolManager.Get(3);

        pool.transform.SetParent(poolManager.BulletBox);
        pool.transform.position = firePoint.position;
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<BulletForPlayer>()?.Shoot(dir, player.power);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
#endif
}
