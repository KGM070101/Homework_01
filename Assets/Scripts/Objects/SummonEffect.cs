using UnityEngine;

public class SummonEffect : PoolObject
{
    [SerializeField] private float lifeTime;

    private PoolManager poolManager;

    private void Awake()
    {
        poolManager=FindFirstObjectByType<PoolManager>();
    }

    public override void Init()
    {
        poolManager.PushDelay(this, lifeTime);
    }
}
