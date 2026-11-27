using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Summoner : Character, IDamageable, IKnockbackable, IWaveScalable
{
    [SerializeField] private Transform enemyBox;
    [SerializeField] private Transform firePoint;
    [SerializeField] public int SummonCount = 5;
    public Transform FirePoint => firePoint;
    public Transform EnemyBox => enemyBox;

    private Player player;
    public Player Player => player;

    private EnemyHpBarRotation enemyHpBar;

    private PoolManager poolManager;
    public PoolManager PoolManager => poolManager;
    public Rigidbody2D Rigidbody2D => rigidbody2D;

    private Summoner_FSM fsm;
  
    private Vector2 randomXpAmount = new Vector2(5.0f, 8.0f);
    private Vector2 facingDir;
    public Vector2 FacingDir => facingDir;

    private float hp;
    public float Hp => hp;

    private float maxHp;
    public float MaxHp => maxHp;

    private float power;
    public float Power => power;

    private float speed;
    public float Speed => speed;

    private float stopDistance;
    public float StopDistance => stopDistance;

    private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    private float knockbackDuration;
    public float WaveMultiplier { get; private set; } = 1f;

    private float minRadius = 2;
    private float maxRadius = 3;
    

    private bool isKnockbacking = false;
    public bool IsKnockbacking => isKnockbacking;

    private bool isDying = false;

    private Color damageColor = new Color(0.7f, 0.3f, 0.3f);
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();

        player = FindFirstObjectByType<Player>();
        enemyHpBar = GetComponentInChildren<EnemyHpBarRotation>();
        poolManager = FindAnyObjectByType<PoolManager>();
        fsm = new Summoner_FSM(this);

        maxHp = data[0].MaxHp;
        hp = data[0].MaxHp;
        power = data[0].Power;
        speed = data[0].MoveSpeed;
        stopDistance = data[0].StopDistance;
        attackSpeed = data[0].AttackSpeed;

        originalColor = spriteRenderer.color;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Init()
    {
        isDying = false;
        isKnockbacking = false;
        maxHp = data[0].MaxHp;
        hp = maxHp;

        rigidbody2D.linearVelocity = Vector2.zero;

        enemyHpBar.SetCurrentHp(1);

        boxCollider2D.enabled = true;

        spriteRenderer.DOKill();
        spriteRenderer.color = originalColor;
    }

    protected override void Update()
    {
        base.Update();

        fsm.Tick(Time.deltaTime);

        // 적 넉백 타이머
        {
            knockbackDuration -= Time.deltaTime;
            if (knockbackDuration <= 0)
            {
                isKnockbacking = false;
            }
        }

        //시선 방향
        {
            facingDir = (player.transform.position - transform.position);
        }
    }

    public void ApplyWaveScale(float multifly)
    {
        WaveMultiplier = multifly;

        maxHp = data[0].MaxHp * multifly;
        hp = maxHp;
        power = data[0].Power * multifly;
        speed = data[0].MoveSpeed * multifly;
    }

    public void Damage(float damage)
    {
        if (isDying)
            return;

        hp -= damage;
        float currentHpRation = hp / maxHp;

        enemyHpBar.SetCurrentHp(currentHpRation);

        if (hp <= 0)
        {
            StartCoroutine(Co_Dead());
        }

        StartCoroutine(Co_DamageColor());
    }

    private IEnumerator Co_DamageColor()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(0.05f);

        spriteRenderer.color = originalColor;
    }

    private IEnumerator Co_Dead()
    {
        isDying = true;
        hp = 0;

        spriteRenderer.DOFade(0.0f, 0.1f);
        boxCollider2D.enabled = false;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.DOKill();

        float randomXp = Random.Range(randomXpAmount.x, randomXpAmount.y);

        GameManager.EnemyDied(randomXp);

        poolManager.PushNow(this);
    }

    public void Knockback(Vector2 direction, float duration, float force)
    {
        if (isDying)
            return;

        isKnockbacking = true;

        knockbackDuration = duration;

        rigidbody2D.linearVelocity = direction.normalized * force;
    }

    public void Heal(float healAmount)
    {
        hp += healAmount;
        float currentHpRation = hp / maxHp;

        enemyHpBar.SetCurrentHp(currentHpRation);

        if (hp >= maxHp)
            hp = maxHp;
    }

    private Vector3 GetRandomPos()
    {
        Vector3 randomDir = Random.insideUnitCircle.normalized;

        float dist = Random.Range(minRadius, maxRadius);

        Vector3 randomPoint = transform.position + randomDir * dist;

        return randomPoint;
    }

    public IEnumerator Co_SummonSkeleton()
    {
        for (int i = 0; i < SummonCount; i++)
        {
            PoolObject fx = poolManager.Get(13);

            Vector3 position = GetRandomPos();

            fx.transform.position = position;

            yield return new WaitForSeconds(0.15f);

            PoolObject skeleton = poolManager.Get(9);


            skeleton.transform.SetParent(enemyBox);
            skeleton.transform.position = position;
            skeleton.transform.rotation = Quaternion.identity;

            GameManager.EnemySpawn();

            skeleton.GetComponent<IWaveScalable>().ApplyWaveScale(WaveMultiplier);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
