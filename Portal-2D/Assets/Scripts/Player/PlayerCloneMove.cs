using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCloneMove : MonoBehaviour
{
    Vector3 offsetToPlayer;

    void Start()
    {
        Reset();
    }

    void Update()
    {
        transform.position = PortalManager.Instance.ObjectToCloneProperty.transform.position + offsetToPlayer;
    }

    public void Reset()
    {
        offsetToPlayer = transform.position - PortalManager.Instance.ObjectToCloneProperty.transform.position;
    }
}
