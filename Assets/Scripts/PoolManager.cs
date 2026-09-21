using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolObject[] poolables;
    [SerializeField] private Transform enemyBox;

    private List<Stack<PoolObject>> poolStack = new();
    private Coroutine coroutine;

#if UNITY_EDITOR

    private void Reset()
    {
        
    }
#endif

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        poolStack = new List<Stack<PoolObject>>();

        for(int i = 0; i < poolables.Length; i++)
        {
            poolStack.Add(new Stack<PoolObject>());

            for(int j=0; j<10; j++)
            {
                PoolObject pool = Instantiate(poolables[i]);

                pool.SetIndex(i);

                pool.transform.SetParent(enemyBox);

                pool.gameObject.SetActive(false);

                poolStack[i].Push(pool); //^1 or int index?
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

    //public void Push(PoolObject pool)
    //{
    //    pool.gameObject.SetActive(false);

    //    poolStack[pool.index].Push(pool);
    //}

    public void Push(PoolObject pool,float durationTIme)
    {        
        StartCoroutine(Co_Push(pool, durationTIme));        
    }

    private IEnumerator Co_Push(PoolObject pool, float time)
    {
        //yield return new WaitForSeconds(time);

        //pool.gameObject.SetActive(false);

        //poolStack[pool.index].Push(pool);        

        float timer = 0;

        while(timer<time)
        {
            yield return null;

            timer += Time.deltaTime;

            if (!pool.gameObject.activeInHierarchy)
                yield break;
        }

        pool.gameObject.SetActive(false);
        poolStack[pool.index].Push(pool);
    }
}



//public interface IPoolable
//{
//    void SetIndex(int index);
//    void Init();
//}

//public abstract class PoolObject : MonoBehaviour, IPoolable
//{
//    public int index;
//    public virtual void SetIndex(int index)
//    {
//        this.index = index;
//    }       

//    public abstract void Init();
//}