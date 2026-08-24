using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] 
    private Transform target;

    [SerializeField]
    private float followDelay = 0.5f;

    private Vector3 followVelocity;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            followDelay
            );
    }
}
