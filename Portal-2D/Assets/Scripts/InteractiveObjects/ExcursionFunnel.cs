using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ExcursionFunnel : MonoBehaviour
{
    [SerializeField] float force = 20f;
    [SerializeField] GameObject end;

    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject objectInside = collider.gameObject;
        if (collider.gameObject.GetComponent<Rigidbody2D>() != null && collider == objectInside.GetComponent<BoxCollider2D>())
        {
            if (objectInside.transform.position.x >= end.transform.position.x)
                return;

            Rigidbody2D objectInsideRb = objectInside.GetComponent<Rigidbody2D>();
            Vector3 objectInsidePos = objectInside.transform.position;

            if (Mathf.Abs( objectInsideRb.velocity.x ) < 0.01f)
            {
               // // Obiekt jest nieruchomy - przyci¹gaj do end.transform.position
               objectInside.transform.position = Vector3.Lerp(objectInside.transform.position, end.transform.position, force * Time.deltaTime * GetComponent<BoxCollider2D>().bounds.size.x);

               // Vector3 right = transform.right;
               // Vector3 up    = transform.up;

               // Vector3 direction = right;
               // Vector3 startingPoint = transform.position;

               // Ray ray = new Ray(startingPoint, direction);
               // var ToCenter = Vector3.Cross(ray.direction, objectInside.transform.position - ray.origin);

               //   objectInside.transform.position += (right + ToCenter)*Time.deltaTime;

               // //var direction = (end.transform.position - objectInside.transform.position).normalized;
               // //objectInsideRb.AddForce(direction * force);

               // var Direction = (end.transform.position - objectInside.transform.position).normalized;
               // objectInside.transform.position += Direction*Time.deltaTime*force;

                objectInside.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                // Obiekt jest w ruchu - zatrzymaj przyci¹ganie
                objectInside.GetComponent<Rigidbody2D>().velocity = new Vector2(objectInsideRb.velocity.x, objectInsideRb.velocity.y);
            }
        }
    }
}
