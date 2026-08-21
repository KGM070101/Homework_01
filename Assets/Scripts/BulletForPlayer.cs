using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletForPlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 12.0f;

    [SerializeField]
    public float durationTime = 3.0f;

    private new Rigidbody2D rigidbody2D;

    private float angle;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;

        rigidbody2D.linearVelocity = normalizedDirection * speed;

        angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle+90);

        Destroy(gameObject, durationTime);
    }
}
