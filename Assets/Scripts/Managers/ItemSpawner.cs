using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private float _minRadius;

    [SerializeField]
    private float _maxRadius;
    
    [SerializeField]
    private Transform itemBox;

    private PoolManager poolManager;

    private Vector2 randomSpawnTrigger;
    private Vector2 randomSpawnInterval = new Vector2(10, 15);

    private Coroutine coroutine;

    private void Awake()
    {
        poolManager = FindAnyObjectByType<PoolManager>();
    }

    private void Start()
    {
        coroutine = StartCoroutine(Co_SpawnItem());
    }

    private void Spawn_Item_Heal()
    {
        PoolObject heal = poolManager.Get(2);

        heal.transform.SetParent(itemBox);
        heal.transform.position = GetRandomPos();
        heal.transform.rotation = Quaternion.identity;
    }

    private Vector3 GetRandomPos()
    {
        Vector3 randomDir = Random.insideUnitCircle.normalized;

        float distance = Random.Range(_minRadius, _maxRadius);

        Vector3 randomPoint = transform.position + randomDir * distance;

        return randomPoint;
    }

    private IEnumerator Co_SpawnItem()
    {
        while(true)
        {
            Spawn_Item_Heal();

            float randomInterval = 
                Random.Range(randomSpawnInterval.x, randomSpawnInterval.y);

            yield return new WaitForSeconds(randomInterval);            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _minRadius);
        Gizmos.DrawWireSphere(transform.position, _maxRadius);
    }
}
