using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private int wave = 1;

    [SerializeField]
    private int baseEnemyCount = 15;

    [SerializeField]
    private int enemyAddPerWave = 5;

    [SerializeField]
    private float enemyStatAddPerWave = 0.05f;

    [SerializeField]
    private float restTime = 5.0f;

    [SerializeField]
    private float spawnInterval = 1.0f;

    private Enemy_Spawner enemy_Spawner;
    private Player player;

    public event Action<int> OnWaveStart;

    private void Awake()
    {
        enemy_Spawner = FindFirstObjectByType<Enemy_Spawner>();
        player=FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        StartCoroutine(Co_WaveLoop());
    }

    private IEnumerator Co_WaveLoop()
    {
        while(!player.isDead)
        {
            yield return new WaitForSeconds(restTime);

            wave++;
            OnWaveStart?.Invoke(wave);

            int enemyCount = baseEnemyCount + enemyAddPerWave * (wave - 1);
            float multiplier = 1.0f + enemyStatAddPerWave * (wave - 1);

            for (int i = 0; i < enemyCount; i++)
            {
                enemy_Spawner.SpawnEnemies(multiplier);
                yield return new WaitForSeconds(spawnInterval);
            }    



            yield return new WaitUntil(() => enemy_Spawner.enemyCount <= 0);
        }
    }
}
