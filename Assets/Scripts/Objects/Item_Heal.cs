using UnityEngine;

public class Item_Heal : PoolObject
{
    private PoolManager poolManager;
    private PoolObject poolObject;
    private Player player;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        poolManager = FindFirstObjectByType<PoolManager>();
        poolObject = GetComponent<PoolObject>();
    }

    public override void Init()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {            
            player.Heal(10);
            poolManager.PushNow(poolObject);
        }
    }    
}
