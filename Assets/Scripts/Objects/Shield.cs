using UnityEngine;

public class Shield : MonoBehaviour
{
    private Player player;

    private void OnEnable()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable =
            collision.GetComponentInParent<IDamageable>();

        if (damageable is null)
            return;

        damageable.Damage(player.power);
    }
}
