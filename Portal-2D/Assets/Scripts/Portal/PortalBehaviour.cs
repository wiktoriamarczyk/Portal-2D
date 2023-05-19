using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PortalManager.Instance.Teleport();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PortalManager.Instance.DestroyClone(collision.gameObject);
    }
}
