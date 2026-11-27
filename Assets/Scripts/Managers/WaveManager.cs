using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private int wave = 0;

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

    [SerializeField]
    private TextMeshProUGUI waveText;

    private Enemy_Spawner enemy_Spawner;
    private Player player;

    public event Action<int> OnWaveStart;
    public event Action<int> OnWaveClear;

    private void Awake()
    {
        player=FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        enemy_Spawner = Enemy_Spawner.Instance;
        StartCoroutine(Co_WaveLoop());
    }

    private IEnumerator Co_WaveLoop()
    {
        while(!player.isDead)
        {
            yield return new WaitForSeconds(restTime);

            wave++;
            IndicateWaveText(wave);
            OnWaveStart?.Invoke(wave);

            if(wave%10!=0&&wave<=10)
            {
                int enemyCount = baseEnemyCount + enemyAddPerWave * (wave - 1);
                float multiplier = 1.0f + enemyStatAddPerWave * (wave - 1);

                for (int i = 0; i < enemyCount; i++)
                {
                    enemy_Spawner.SpawnEnemies(multiplier);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
            else if(wave%10!=0&&wave<=99)
            {
                int enemyCount = baseEnemyCount + enemyAddPerWave * (wave - 1);
                float multiplier = 1.0f + enemyStatAddPerWave * (wave - 1);

                for (int i = 0; i < enemyCount; i++)
                {
                    if (UnityEngine.Random.value < Mathf.Max(0.9f - 0.04f * (wave - 11), 0.5f))
                        enemy_Spawner.SpawnEnemies(multiplier);
                    else
                        enemy_Spawner.SpawnEnemiesVer2(multiplier);

                    yield return new WaitForSeconds(spawnInterval);
                }                
            }
            else
            {
                Debug.Log(enemy_Spawner);
                enemy_Spawner.SpawnBoss0();
            }

            yield return new WaitUntil(() => enemy_Spawner.enemyCount <= 0);
        }
    }

    private void IndicateWaveText(int wave)
    {        
        waveText.text = "" + wave;
    }
}
