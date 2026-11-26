using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Summoner : Character, IDamageable, IKnockbackable, IWaveScalable
{
    [SerializeField] private Transform enemyBox;
    public Transform EnemyBox => enemyBox;

    private Player player;
    public Player Player => player;

    private EnemyHpBarRotation enemyHpBar;

    private PoolManager poolManager;
    public PoolManager PoolManager => poolManager;
    public Rigidbody2D Rigidbody2D => rigidbody2D;
  
    private Vector2 randomXpAmount = new Vector2(5.0f, 8.0f);

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

    private float knockbackDuration;
    public float WaveMultiplier { get; private set; } = 1f;

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

        maxHp = data[0].MaxHp;
        hp = data[0].MaxHp;
        power = data[0].Power;
        speed = data[0].MoveSpeed;
        stopDistance = data[0].StopDistance;

        originalColor = spriteRenderer.color;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Init()
    {
        isDying = false;
        maxHp = data[0].MaxHp;
        hp = maxHp;

        enemyHpBar.SetCurrentHp(1);

        boxCollider2D.enabled = true;

        spriteRenderer.DOKill();
        spriteRenderer.color = originalColor;
    }

    protected override void Update()
    {
        base.Update();

        // 적 넉백 타이머
        {
            knockbackDuration -= Time.deltaTime;
            if (knockbackDuration <= 0)
            {
                isKnockbacking = false;
            }
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

        spriteRenderer.DOFade(0.0f, 0.5f);
        boxCollider2D.enabled = false;

        yield return new WaitForSeconds(0.5f);

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
}
