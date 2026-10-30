using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Boss_0 : Character
{
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform sprite;

    [SerializeField]
    private Image dashRange;
    public GameObject BulletPrefab => bulletPrefab;

    private Player player;
    public Player Player => player;

    private Boss0_FSM fsm;
    private Boss0_Normal normalState;
    private EnemyHpBarRotation enemyHpBar;  
   
    private float hp;
    public float Hp => hp;

    private float maxHp;

    private float speed;
    public float Speed => speed;

    private float power;
    public float Power => power;

    private float stopDistance;
    public float StopDistance => stopDistance;

    public Rigidbody2D Rigidbody2D => rigidbody2D;

    protected override void Awake()
    {
        base.Awake();

        player = FindFirstObjectByType<Player>();        
        fsm = new Boss0_FSM(this);
        enemyHpBar = GetComponentInChildren<EnemyHpBarRotation>();
        //dashRange = GetComponentInChildren<Image>();

        hp = data[1].MaxHp;
        maxHp = data[1].MaxHp;
        speed = data[1].MoveSpeed;
        power = data[1].Power;
        stopDistance = data[1].StopDistance;

        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Boss_0"), LayerMask.NameToLayer("Enemy"), true);           
        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Boss_0"), LayerMask.NameToLayer("Enemy2"), true);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        fsm.Tick(Time.deltaTime);

        //Debug
        {
            Debug.Log(fsm.currentState);
        }        
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(power);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if(collision.gameObject.CompareTag("BulletForPlayer"))
        {
            if(!player.isInShotgunUltState)
            return;

            player.Heal(1f);
        }
        else if(collision.gameObject.CompareTag("Arrow"))
        {
            if(!player.isInArrowUltState)
            {
                Damage(player.power*10*player.arrow_ChargeRation);
                player.UltStack+=(10*player.arrow_ChargeRation);
                Destroy(collision.gameObject);
            }
            else
            {
                Damage((player.power*10*player.arrow_ChargeRation)*1.2f);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();        
    }    

    public void Damage(float damage)
    {
        hp -= damage;
        float currentHpRation = hp / maxHp;

        if (hp<=0)
        {
            Die();
        }

        enemyHpBar.SetCurrentHp(currentHpRation);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void DashReady(float duration)
    {
        sprite.DOKill();
        sprite.transform.DOShakePosition(duration, 0.1f, 50, 180, snapping: false, fadeOut: false);
    }

    public void OnDashRange(float timer,float duration)
    {
        dashRange.fillAmount = timer / duration;
    }

    public void CancelDashRange()
    {
        dashRange.fillAmount = 0.0f;
    }
}
