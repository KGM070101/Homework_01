using UnityEngine;

public interface IKnockbackable
{
    void Knockback(Vector2 direction,float duration,float force);
}
