using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int Index;                   // 웨이브 번호
    public float InitTime;              // 웨이브 시작 시간
    public float[] EnemyRates;          // 적 생성 확률
    public float SpawnSpan;             // 적 생성 간격 시간

    public int GetRandomEnemyIndex()
    {

        float total = 0;

        foreach (float elem in EnemyRates)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < EnemyRates.Length; i++)
        {
            if (randomPoint < EnemyRates[i])
            {
                return i;
            }
            else
            {
                randomPoint -= EnemyRates[i];
            }
        }
        return EnemyRates.Length - 1;
    }
}

[CreateAssetMenu(fileName = "StageData", menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    public WaveData[] WaveDatas;


    public WaveData GetCurrentWaveData(float playTime)
    {
        return null;
    }

    
}
