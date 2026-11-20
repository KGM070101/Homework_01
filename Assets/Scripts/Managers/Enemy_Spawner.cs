using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public static Enemy_Spawner Instance;

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

    [SerializeField]
    private TextMeshProUGUI enemyCountText;

    [SerializeField]
    private PoolManager poolManager;

    [SerializeField]
    private Boss_0 boss0;

    private Player player;

    public int enemyCount;

    private Coroutine coroutine;

    private void Awake()
    {
        if(Instance!=null)
        {
            DestroyImmediate(gameObject);
        }

        Instance = this;

        player = FindFirstObjectByType<Player>();   
    }

    private void Start()
    {
        enemyCountText.text = "" + enemyCount;
    }

    private void OnEnable()
    {
        GameManager.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        GameManager.OnEnemyDied -= HandleEnemyDied;
    }

    private void Update()
    {
        //Debug.Log(enemyCount);
    }    

    private void HandleEnemyDied(float xp)
    {
        RemoveEnemyCount();
    }

    public void SpawnEnemies(float statMultiplier)
    {
        float randomTrigger = Random.Range(0, 99);
        int index = randomTrigger < 30 ? 1 : 0;

        PoolObject pool = poolManager.Get(index);

        pool.transform.SetParent(enemyBox);
        pool.transform.position = GetRandomPos();
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<IWaveScalable>().ApplyWaveScale(statMultiplier);

        AddEnemyCount();
    }

    public void SpawnEnemiesVer2(float statMultiplier)
    {
        float randomTrigger = Random.Range(0, 99);
        int index = randomTrigger < 99 ? 7 : 0;

        PoolObject pool = poolManager.Get(index);

        pool.transform.SetParent(enemyBox);
        pool.transform.position = GetRandomPos();
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<IWaveScalable>().ApplyWaveScale(statMultiplier);

        AddEnemyCount();
    }

    public void SpawnBoss0()
    {
        Boss_0 boss = Instantiate(boss0, GetRandomPos(), Quaternion.identity, enemyBox);

        AddEnemyCount();
    }


    public Vector3 GetRandomPos() //두 원의 반지름 차에서 적 생성
    {
        // 랜덤 방향 계산
        Vector3 randomDir = Random.insideUnitCircle.normalized;

        // 랜덤 거리 계산
        float dist = Random.Range(_minRadius, _maxRadius);

        Vector3 randomPoint = transform.position + randomDir * dist;

        return randomPoint;        
    }    

    private void AddEnemyCount()
    {
        enemyCount++;
        enemyCountText.text = "" + enemyCount;

        if (enemyCount >= 200)
            player.Dead();            
    }

    public void RemoveEnemyCount()
    {
        enemyCount--;
        enemyCountText.text = "" + enemyCount;

        if (enemyCount <= 0)
            enemyCount = 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _minRadius);
        Gizmos.DrawWireSphere(transform.position, _maxRadius);        
    }
#endif
}
