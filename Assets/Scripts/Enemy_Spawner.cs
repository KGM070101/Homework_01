using System.Collections;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField]
    private float _minRadius;

    [SerializeField]
    private float _maxRadius;

    [SerializeField]
    private float spawnInterval=2;

    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private Enemy2 enemy2Prefab;

    [SerializeField]
    private Transform enemyBox;
   
    public int enemyCount;

    private Vector2 randomSpawnTrigger = new Vector2(0f, 99f);

    private Coroutine coroutine;

    private void Start()
    {
        coroutine = StartCoroutine(Co_SpawnEnemy());
    }

    private void Update()
    {
        //Debug.Log(enemyCount);
    }
    private void SpawnEnemy()
    {
        Enemy enemy = 
            Instantiate(enemyPrefab, GetRandomPos(),Quaternion.identity,enemyBox);
    }

    private void SpawnEnemy2()
    {
        Enemy2 enemy2 =
            Instantiate(enemy2Prefab, GetRandomPos(), Quaternion.identity, enemyBox);
    }


    private Vector3 GetRandomPos() //두 원의 반지름 차에서 적 생성
    {
        // 랜덤 방향 계산
        Vector3 randomDir = Random.insideUnitCircle.normalized;

        // 랜덤 거리 계산
        float dist = Random.Range(_minRadius, _maxRadius);

        Vector3 randomPoint = transform.position + randomDir * dist;

        return randomPoint;        
    }

    private IEnumerator Co_SpawnEnemy()
    {        
        while(true)
        {
            float randomTrigger = Random.Range(randomSpawnTrigger.x, randomSpawnTrigger.y);

            yield return new WaitForSeconds(spawnInterval);

            if (randomTrigger<20)
            {
                SpawnEnemy2();
                enemyCount++;
            }
            else
            {
                SpawnEnemy();
                enemyCount++;
            }                       
        }
       
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _minRadius);
        Gizmos.DrawWireSphere(transform.position, _maxRadius);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position, new Vector3(30, 30));
        //Gizmos.DrawWireCube(transform.position, new Vector3(35, 35));
    }
#endif
}
