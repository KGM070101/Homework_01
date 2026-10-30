using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolObject[] poolables;
    [SerializeField] private Transform enemyBox;

    private List<Stack<PoolObject>> poolStack = new();
    private readonly List<PoolObject> delayed = new();
    private Coroutine coroutine;
    private float pushTimer;
    private bool canPush;

#if UNITY_EDITOR

    private void Reset()
    {
        
    }
#endif

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        for (int i = delayed.Count - 1; i >= 0; i--)   
        {
            PoolObject pool = delayed[i];

            if (pool == null || !pool.gameObject.activeSelf)
            {
                delayed.RemoveAt(i);                   
                continue;
            }

            if (Time.time >= pool.PushTime)
            {
                delayed.RemoveAt(i);
                PushNow(pool);
            }
        }

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

    public void PushNow(PoolObject pool)
    {
        if (!pool.gameObject.activeSelf)
            return;

        pool.gameObject.SetActive(false);
        poolStack[pool.index].Push(pool);
    }

    public void Push(PoolObject pool,float durationTIme)
    {        
        StartCoroutine(Co_Push(pool, durationTIme));        
    }

    public void PushDelay(PoolObject pool,float delayTime)
    {
        pool.PushTime = Time.time + delayTime;

        if(!delayed.Contains(pool))
        {
            delayed.Add(pool);
        }
      



        
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