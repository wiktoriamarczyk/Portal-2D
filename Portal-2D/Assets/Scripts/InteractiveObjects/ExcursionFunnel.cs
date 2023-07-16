using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ExcursionFunnel : MonoBehaviour
{
    [SerializeField] float force = 20f;
    [SerializeField] GameObject end;


    public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        Vector3 lhs = point - origin;

        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject objectInside = collider.gameObject;
        if (collider.gameObject.GetComponent<Rigidbody2D>() != null && collider == objectInside.GetComponent<BoxCollider2D>())
        {
            if (objectInside.transform.position.x >= end.transform.position.x)
                return;

            Rigidbody2D objectInsideRb = objectInside.GetComponent<Rigidbody2D>();
            Vector3 objectInsidePos = objectInside.transform.position;

            var player = objectInside.GetComponent<PlayerMovement>();;

            if (player==null || !player.IsMoving())
            {
                // calculate object position as center of its collider
                var ObjectPos = objectInside.GetComponent<BoxCollider2D>().bounds.center;
                // calculate closest point from object to line
                var ClosestPoint = FindNearestPointOnLine( transform.position , transform.right , ObjectPos );
                // calculate vector from object to closest point
                var VectorToCenter = ClosestPoint - ObjectPos;

                // attract object to center of line
                objectInside.transform.position += (VectorToCenter*3 + transform.right)*force*Time.deltaTime;
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
