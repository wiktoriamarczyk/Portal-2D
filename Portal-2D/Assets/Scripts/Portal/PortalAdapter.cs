using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAdapter : MonoBehaviour, IPortalEventsListener
{
    // max velocity of 50 is selected to avoid bugs when moving very fast through portals
    static float MAX_VELOCITY = 50;

    [SerializeField] GameObject clonePrefab;
    PortalCloneController cloneController;
    bool isInPortalArea = false;

    public bool IsInPortalArea()
    {
        return isInPortalArea;
    }
    public void SetIsInPortalArea(bool value)
    {
        isInPortalArea = value;
    }

    void Awake()
    {
        cloneController = gameObject.GetComponent<PortalCloneController>();
        if (cloneController == null)
            cloneController = gameObject.AddComponent<PortalCloneController>();
    }

    public GameObject CreateClone(Vector3 worldPos, Quaternion localRotation, PortalLogic srcPortal, PortalLogic dstPortal)
    {
        GameObject prefabToClone = clonePrefab;
        if (prefabToClone == null)
            prefabToClone = gameObject;

        GameObject clone = Instantiate(prefabToClone, worldPos, Quaternion.identity, dstPortal.GetOwnOutput());
        clone.transform.localRotation = localRotation;

        if (clonePrefab==null)
        {
            Object.Destroy( clone.GetComponent<Rigidbody2D>() );
            Object.Destroy( clone.GetComponent<BoxCollider2D>() );
            Object.Destroy( clone.GetComponent<PortalAdapter>() );
            Object.Destroy( clone.GetComponent<PortalCloneController>() );

            var images = clone.GetComponents<SpriteRenderer>();
            foreach (var image in images)
            {
                image.sortingLayerID = SortingLayer.NameToID("Clones");
                image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        cloneController.ResetClone(clone, srcPortal, dstPortal);

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
            // clamp velocity to MAX_VELOCITY
            if (mag > MAX_VELOCITY)
            {
                float ratio = MAX_VELOCITY / mag;
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

    void IPortalEventsListener.OnTeleported( PortalLogic srcPortal , PortalLogic dstPortal )
    {
    }

    void IPortalEventsListener.OnExitedPortalArea( PortalLogic portal )
    {
        SetIsInPortalArea(false);
    }
}
