using UnityEngine;

/// <summary>
/// Class responsible for the behavior of excursion funnels
/// </summary>
public class ExcursionFunnel : MonoBehaviour
{
    /// <summary>
    /// Force of attraction
    /// </summary>
    [SerializeField] float force = 20f;
    /// <summary>
    /// End of the funnel
    /// </summary>
    [SerializeField] GameObject end;

    /// <summary>
    /// Finds the nearest point on the line
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        Vector3 lhs = point - origin;

        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
    }
    /// <summary>
    /// Attracts objects to the center of the funnel
    /// </summary>
    /// <param name="collider">the object with which the collision occurred</param>
    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject objectInside = collider.gameObject;
        if (collider.gameObject.GetComponent<Rigidbody2D>() != null && collider == objectInside.GetComponent<BoxCollider2D>())
        {
            PortalAdapter adapter = objectInside.GetComponent<PortalAdapter>();
            Rigidbody2D objectInsideRb = objectInside.GetComponent<Rigidbody2D>();
            Vector3 objectInsidePos = objectInside.transform.position;

            var player = objectInside.GetComponent<PlayerMovement>();;

            var right = CommonFunctions.VectorLocalToWorld(gameObject.transform, Vector3.right).normalized;

            if (player==null || !player.IsMoving())
            {
                // calculate object position as center of its collider
                var ObjectPos = objectInside.GetComponent<BoxCollider2D>().bounds.center;
                // calculate closest point from object to line
                var ClosestPoint = FindNearestPointOnLine( transform.position , right , ObjectPos );
                // calculate vector from object to closest point
                var VectorToCenter = ClosestPoint - ObjectPos;

                // attract object to center of line
                objectInside.transform.position += (VectorToCenter*3 + right)*force*Time.deltaTime;
                objectInside.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else
            {
                // object moved - stop the attraction force
                objectInside.GetComponent<Rigidbody2D>().velocity = new Vector2(objectInsideRb.velocity.x, objectInsideRb.velocity.y);
            }
        }
    }

}
