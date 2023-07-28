using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAdapter : MonoBehaviour
{
    [SerializeField] GameObject clonePrefab;

    public GameObject CreateClone(Vector3 worldPos, Quaternion rotation, Transform parent)
    {
        GameObject prefabToClone = clonePrefab;
        if (prefabToClone == null)
            prefabToClone = gameObject;

        GameObject clone = Instantiate(prefabToClone, worldPos, rotation, parent);

        if (clonePrefab==null)
        {
            Object.Destroy( clone.GetComponent<Rigidbody2D>() );
            Object.Destroy( clone.GetComponent<BoxCollider2D>() );
            Object.Destroy( clone.GetComponent<PortalAdapter>() );

            var images = clone.GetComponents<SpriteRenderer>();
            foreach ( var image in images )
            {
                image.sortingLayerID = SortingLayer.NameToID("Clones");
                image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        return clone;
    }
    public Vector3 GetObjectCenter()
    {
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            return transform.position;

        return collider.bounds.center;
    }

    private void Update()
    {
        var rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D != null)
        {
            var velocity = rigidbody2D.velocity;
            var mag = velocity.magnitude;
            // clamp velocity to 50
            if (mag > 50)
            {
                float ratio = 50 / mag;
                velocity *= ratio;
                rigidbody2D.velocity = velocity;
            }
        }
    }

    public void SetPositionByCenter(Vector3 pos)
    {
        Vector3 Offset = Vector3.zero;
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Offset = collider.bounds.center - transform.position;
        }

        transform.position = pos-Offset;
    }
}
