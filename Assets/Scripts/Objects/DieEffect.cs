using UnityEngine;

public class DieEffect : PoolObject
{
    [SerializeField] private float lifeTime = 0.11f;

    private PoolManager poolManager;

    private void Awake()
    {
        poolManager = FindFirstObjectByType<PoolManager>();
    }

    public override void Init()
    {
        poolManager.PushDelay(this,lifeTime);
    }
}
