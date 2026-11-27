using UnityEngine;

public class SummonerOrb : MonoBehaviour
{
    private Summoner summonmer;

    private void OnEnable()
    {
        summonmer = FindFirstObjectByType<Summoner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().TakeDamage(summonmer.Power);
        }

        IDamageable enemies =
            collision.GetComponentInParent<IDamageable>();

        if (enemies is null)
            return;

        enemies.Heal(5);
    }
}
