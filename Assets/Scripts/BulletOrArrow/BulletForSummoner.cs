using UnityEngine;

public class BulletForSummoner : PoolObject
{
    [SerializeField] private GameObject orb_0;
    [SerializeField] private GameObject orb_1;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private float period = 1.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float durationTime = 5.0f;

    private Rigidbody2D rigidBody2D;
    private PoolManager poolManager;

    private float angle;
    private float timer = 0;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        poolManager = FindFirstObjectByType<PoolManager>();
    }

    public override void Init()
    {       
        timer = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Cos(timer * Mathf.PI * 2 / period);

        orb_0.transform.localPosition = new Vector3(0, t * radius, 0);
        orb_1.transform.localPosition = new Vector3(0, t * -radius, 0);
    }

    public void Shoot(Vector2 direction)
    {
        Vector2 norDir = direction.normalized;

        rigidBody2D.linearVelocity = norDir * speed;

        angle = Mathf.Atan2(norDir.y, norDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        poolManager.PushDelay(this, durationTime);
    }    
}
