using UnityEngine;

public class Item_Heal : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {            
            player.Heal(10);
            poolManager.Push(poolObject);
        }
    }

    public void Reset_Item_Heal()
    {

    }
}
