using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolObject[] poolables;

    private List<Stack<PoolObject>> poolStack = new();

    private void Init()
    {
        poolStack = new List<Stack<PoolObject>>();

        foreach(PoolObject poolable in poolables)
        {
            poolStack.Add(new Stack<PoolObject>());

            for(int i=0; i<5; i++)
            {
                PoolObject pool = Instantiate(poolable);

                pool.transform.SetParent(transform);

                pool.gameObject.SetActive(false);

                poolStack[^1].Push(pool);
            }

        }
    }

    public PoolObject Get(int index)
    {
        if(poolStack[index].Count>0)
        {
            PoolObject target= poolStack[index].Pop();

            target.gameObject.SetActive(true);

            target.SetIndex(index);

            target.Init();

            return target;
        }      
        
        PoolObject pool = Instantiate(poolables[index]);

        pool.SetIndex(index);

        pool.Init();

        return pool;        
    }

    public void Push(PoolObject pool)
    {
        pool.gameObject.SetActive(false);

        poolStack[pool.index].Push(pool);
    }
}



public interface IPoolable
{
    void SetIndex(int index);
    void Init();
}

public abstract class PoolObject : MonoBehaviour, IPoolable
{
    public int index;
    public virtual void SetIndex(int index)
    {
        this.index = index;
    }       

    public abstract void Init();
}