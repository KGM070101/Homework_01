using UnityEngine;

public class Explosion : PoolObject
{
    [SerializeField] private float lifeTime = 0.7f;

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
