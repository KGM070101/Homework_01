using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public int index;
    public float PushTime;
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public virtual void Init()
    {
      
    }
}
