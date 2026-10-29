using UnityEngine;

public class Boss_0 : Character
{
    private Player player;
    private Boss0_FSM fsm;
   
    private float hp;
    public float Hp => hp;

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
        fsm = new Boss0_FSM(player, this);        

        hp = data[1].MaxHp;
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
}
