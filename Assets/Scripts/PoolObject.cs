using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public int index;
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void Init()
    {
        Enemy enemy = GetComponent<Enemy>();
        Enemy2 enemy2 = GetComponent<Enemy2>();
        Item_Heal item_Heal = GetComponent<Item_Heal>();

        if(enemy is not null)
        {
            enemy.ResetEnemy();
        }    
        if(enemy2 is not null)
        {
            enemy2.ResetEnemy();
        }        
        if(item_Heal is not null)
        {
            item_Heal.Reset_Item_Heal();
        }
    }
}
