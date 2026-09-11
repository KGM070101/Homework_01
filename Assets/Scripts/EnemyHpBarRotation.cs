using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBarRotation : MonoBehaviour
{
    [SerializeField] 
    private Transform enemy;

    [SerializeField] 
    private Vector3 offset = new Vector3(0f, 1.2f, 0f);

    [SerializeField]
    private Image hpBar;

    private void LateUpdate()
    {
        transform.position = enemy.position + offset;
        transform.rotation = Quaternion.identity;
    }

    public void SetCurrentHp(float currentHp)
    {
        hpBar.fillAmount = Mathf.Clamp01(currentHp);
    }
}
