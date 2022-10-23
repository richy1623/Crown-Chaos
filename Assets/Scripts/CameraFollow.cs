
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 enhancedSightOffset;
    public static bool enhancedSight = false;

    private void FixedUpdate()
    {
        Vector3 desiredPosition;
        if (!enhancedSight)
        {
            desiredPosition = player.position + offset;
        }
        else
        {
            desiredPosition = player.position + enhancedSightOffset;
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(75f, player.eulerAngles.y, 0f);
    }
}
