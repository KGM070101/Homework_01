using UnityEngine;

public class Boss_0 : Character
{
    [SerializeField]
    private GameObject bulletPrefab;
    public GameObject BulletPrefab => bulletPrefab;

    private Player player;
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
        fsm = new Boss0_FSM(player, this,normalState);
        enemyHpBar = GetComponentInChildren<EnemyHpBarRotation>();

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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();        
    }    

    public void Damge(float damage)
    {
        hp -= damage;
        float currentHpRation = hp / maxHp;

        enemyHpBar.SetCurrentHp(currentHpRation);
    }
}
