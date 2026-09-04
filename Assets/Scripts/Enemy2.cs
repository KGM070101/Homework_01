using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class Enemy2 : Character
{
    [Header("- Enemy2")]
    [SerializeField]
    private float hp;

    [SerializeField]
    private GameObject Weapon;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private BulletForEnemy2 bulletPrefab;

    [SerializeField]
    private Transform bulletBox;
    
    private Player player;
    private Enemy_Spawner enemy_Spawner;
    private PoolManager poolManager;
    private PoolObject poolObject;

    private Vector3 originalPos;
    private Vector2 randomXpAmount = new Vector2(1.0f, 2.0f);
    private Vector2 facingDir;
    private Vector2 originalWeaponSize;

    private Color damageColor = new Color(0.7f, 0.3f, 0.3f);
    private Color originalColor;
    private Color originalWeaponColor;

    private float knockbackDuration = 0.2f;
    public float power;
    private float speed;
    private int deadTrigger = 0;

    private bool isKnockbacking = false;

    private DOTween doTween;

    private Coroutine coroutine;

    protected override void Awake()
    {
        base.Awake();

        hp = data[2].MaxHp;
        power = data[2].Power;
        speed = data[2].MoveSpeed;

        originalWeaponSize = enemy2_Weapon.transform.localScale;

        player = FindFirstObjectByType<Player>();
        enemy_Spawner = FindFirstObjectByType<Enemy_Spawner>();
        poolManager = FindAnyObjectByType<PoolManager>();
        poolObject = GetComponent<PoolObject>();

        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Enemy2"), LayerMask.NameToLayer("Enemy2"),true);
        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Enemy2"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Enemy2"), LayerMask.NameToLayer("RedLine"),true);

        GameObject bulletBox = GameObject.FindGameObjectWithTag("BulletBox");
    }

    protected override void Start()
    {
        base.Start();

        originalColor = spriteRenderer.color;
        originalWeaponColor = enemy2_Weapon.spriteRenderer.color;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);


    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.CompareTag("BulletForPlayer"))
        {
            if (collision.gameObject.CompareTag("BulletForPlayer"))
            {
                if (player.isInBurstFireUltState == false &&
                    player.isInShotgunUltState == false)
                {
                    Damage(player.power);
                    player.UltStack += 1;
                    Destroy(collision.gameObject);
                }
                if (player.isInBurstFireUltState == true)
                {
                    Damage(player.power * 1.2f);
                }
                if (player.isInShotgunUltState == true)
                {
                    Vector2 direction =
                    (transform.position - player.transform.position).normalized;

                    Damage(player.power * 1.2f);
                    player.Heal(1f);

                    knockbackDuration = 0.2f;
                    isKnockbacking = true;

                    rigidbody2D.linearVelocity = Vector2.zero;

                    rigidbody2D.AddForce(direction * 10.0f, ForceMode2D.Impulse);

                    coroutine = StartCoroutine(Co_Knockback());

                    Destroy(collision.gameObject);

                }

            }
        }
    }

    private IEnumerator Co_Knockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        isKnockbacking = false;
    }

    protected override void Update()
    {
        base.Update();

        if (gameObject)
        {
            coroutine = StartCoroutine(Co_Dead());
        }
        else
        {
            return;
        }


        if (hp <= 0)
        {
            originalPos = transform.position;
        }

        //적 넉백 타이머
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

        if (player.isDead == false)
        {
            TracePlayer();
        }
        else
        {
            return;
        }
    }

    private void Damage(float damage)
    {
        hp -= damage;
        coroutine = StartCoroutine(Co_DamageColor());
    }

    private IEnumerator Co_DamageColor()
    {
        spriteRenderer.color = damageColor;
        enemy2_Weapon.spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(0.05f);

        spriteRenderer.color = originalColor;
        enemy2_Weapon.spriteRenderer.color = originalWeaponColor;
    }

    private void GiveXp()
    {
        float randomXp = Random.Range(randomXpAmount.x, randomXpAmount.y);
        player.GetXp(randomXp);
    }

    private IEnumerator Co_Dead()
    {
        if (hp <= 0)
        {
            hp = 0;


            spriteRenderer.DOFade(0f, 0.5f);
            enemy2_Weapon.spriteRenderer.DOFade(0f, 0.5f);
            boxCollider2D.enabled = false;


            yield return new WaitForSeconds(0.5f);

            spriteRenderer.DOKill();
            enemy2_Weapon.spriteRenderer.DOKill();
            GiveXp();
            deadTrigger++;
            if(deadTrigger==1)
            {
                enemy_Spawner.enemyCount--;
            }            
            poolManager.Push(poolObject);
        }
    }

    public void ResetEnemy()
    {
        hp = data[1].MaxHp;

        deadTrigger = 0;

        boxCollider2D.enabled = true;

        spriteRenderer.DOKill();
        enemy2_Weapon.spriteRenderer.DOKill();

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;

        Color WeaponColor = enemy2_Weapon.spriteRenderer.color;
        WeaponColor.a = 1f;
        enemy2_Weapon.spriteRenderer.color = WeaponColor;       
    }
}
