using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FastEnemy : Character,IDamageable,IKnockbackable,IWaveScalable
{
    private Player player;
    private EnemyHpBarRotation enemyHpBar;
    private PoolManager poolManager;

    private Color damageColor = new Color(0.7f, 0.3f, 0.3f);
    private Color originalColor;

    private Vector2 randomXpAmount = new Vector2(1.5f, 3.0f);

    private float hp;
    private float maxHp;
    private float speed;
    private float power;
    private float EnemyTimer;
    private float knockbackDuration;

    private bool isDying;
    private bool isKnockbacking = false;

    protected override void Awake()
    {
        base.Awake();

        player=FindFirstObjectByType<Player>();
        enemyHpBar=GetComponentInChildren<EnemyHpBarRotation>();
        poolManager=FindAnyObjectByType<PoolManager>();

        originalColor = spriteRenderer.color;

        hp = data[0].MaxHp;
        maxHp = data[0].MaxHp;
        speed = data[0].MoveSpeed;
        power = data[0].Power;
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (player.isDead)
            return;

        TracePlayer();
    }

    public void ApplyWaveScale(float multifly)
    {
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

        if(hp<=0)
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

        PoolObject dieEffect = poolManager.Get(10);

        dieEffect.transform.position = transform.position;

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

    private void TracePlayer()
    {         
        Vector2 direction =
            (player.transform.position - transform.position).normalized;

        float distance =
            (Vector2.Distance(player.transform.position, transform.position));

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rigidbody2D.SetRotation(angle);

        if (isKnockbacking)
        {
            return;
        }

        if (distance > data[0].StopDistance)
        {
            if (isKnockbacking == false)
            {
                rigidbody2D.linearVelocity = direction * speed;
                if (distance > 25.0f)
                {
                    rigidbody2D.linearVelocity = direction * speed * 3.0f;
                }
            }
        }
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < EnemyTimer)
        {
            return;
        }

        EnemyTimer = Time.time + data[0].AttackSpeed;

        player.TakeDamage(power);        
    }
}
