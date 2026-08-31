using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolEnum
{
    Enemy1,
    Enemy2,
    Sound
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolSet[] poolSets;
    private Dictionary<PoolEnum, PoolObject> poolSetDic = new();

    private Dictionary<PoolEnum, Stack<PoolObject>> poolStackDic = new();
    
    private void Init()
    {
        foreach (PoolSet set in poolSets)
        {
            poolSetDic.Add(set.PoolEnum, set.PoolObject);
        }

        foreach (PoolSet poolSet in poolSets)
        {
            poolStackDic.Add(poolSet.PoolEnum, new Stack<PoolObject>());

            for(int i=0; i<5; i++)
            {
                PoolObject pool = Instantiate(poolSet.PoolObject);

                pool.transform.SetParent(transform);

                pool.gameObject.SetActive(false);

                poolStackDic[poolSet.PoolEnum].Push(pool);
            }

        }
    }

    public PoolObject Get(PoolEnum poolEnum)
    {
        if(poolStackDic[poolEnum].Count>0)
        {
            PoolObject target= poolStackDic[poolEnum].Pop();

            target.gameObject.SetActive(true);

            target.SetEnum(poolEnum);

            target.Init();

            return target;
        }      
        
        PoolObject pool = Instantiate(poolSetDic[poolEnum]);

        pool.SetEnum(poolEnum);

        pool.Init();

        return pool;        
    }

    public void Push(PoolObject pool)
    {
        pool.gameObject.SetActive(false);

        poolStackDic[pool.poolEnum].Push(pool);
    }
}



public interface IPoolable
{
    void SetEnum(PoolEnum poolEnum);
    void Init();
}

public abstract class PoolObject : MonoBehaviour, IPoolable
{
    public PoolEnum poolEnum;
    public virtual void SetEnum(PoolEnum poolEnum)
    {
        this.poolEnum = poolEnum;
    }       

    public abstract void Init();
}

[Serializable]
public class PoolSet
{
    public PoolEnum PoolEnum;
    public PoolObject PoolObject;
}