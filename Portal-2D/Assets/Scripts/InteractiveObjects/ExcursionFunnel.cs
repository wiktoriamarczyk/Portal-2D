using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcursionFunnel : MonoBehaviour
{
    [SerializeField] float forceX = 0.02f;
    [SerializeField] float forceY = 0.2f;
    [SerializeField] GameObject end;
    GameObject objectInside;


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            objectInside = collider.gameObject;
        }
    }

     void OnTriggerStay2D(Collider2D collider)
     {
        if (objectInside != null && collider == objectInside.GetComponent<BoxCollider2D>())
        {
            if (objectInside.transform.position.x >= end.transform.position.x)
                return;

            Rigidbody2D objectInsideRb = objectInside.GetComponent<Rigidbody2D>();
            Vector3 objectInsidePos = objectInside.transform.position;

            if (objectInsideRb.velocity.x < 0.01f)
            {
                // Obiekt jest nieruchomy - przyci¹gaj do end.transform.position
                objectInside.transform.position = Vector3.Lerp(objectInside.transform.position, end.transform.position, forceX * Time.deltaTime);
                objectInside.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                // Obiekt jest w ruchu - zatrzymaj przyci¹ganie
                objectInside.GetComponent<Rigidbody2D>().velocity = new Vector2(objectInsideRb.velocity.x, objectInsideRb.velocity.y);
            }

            if (objectInsidePos.y > end.transform.position.y)
            {
                objectInside.transform.position = new Vector3(objectInsidePos.x, end.transform.position.y, objectInsidePos.z);
            }


            // objectInside.transform.position = Vector3.Lerp(objectInside.transform.position, end.transform.position, forceX * Time.deltaTime);
            // objectInside.GetComponent<Rigidbody2D>().AddForce(Vector3.up * Physics.gravity.y * -1 * objectInside.GetComponent<Rigidbody2D>().mass);
            // objectInside.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
     }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != objectInside)
        {
            return;
        }
        objectInside = null;
    }
}
