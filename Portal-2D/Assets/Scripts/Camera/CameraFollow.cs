using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    Vector3 offset = new Vector3(0, 8, -20f);
    float   smoothTime = 0.25f;
    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
