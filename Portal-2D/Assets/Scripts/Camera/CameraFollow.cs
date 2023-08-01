using UnityEngine;

/// <summary>
/// Class responsible for camera following the player
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// Target to follow
    /// </summary>
    [SerializeField] Transform target;
    /// <summary>
    /// Offset from target
    /// </summary>
    Vector3 offset = new Vector3(0, 8, -20f);
    /// <summary>
    /// Time to smooth camera movement
    /// </summary>
    float smoothTime = 0.25f;
    /// <summary>
    /// Velocity of camera
    /// </summary>
    Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Method called after all Update methods, used to follow the player
    /// </summary>
    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
