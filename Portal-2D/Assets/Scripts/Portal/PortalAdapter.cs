using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for all objects that can interact with portals
/// </summary>
public class PortalAdapter : MonoBehaviour, IPortalEventsListener
{
    /// <summary>
    /// max velocity of 40 is selected to avoid bugs when moving very fast through portals
    /// </summary>
    static float MAX_VELOCITY = 40;
    /// <summary>
    /// Prefab to clone when object is touching portal. if no object is provided, the object itself will be cloned
    /// </summary>
    [SerializeField] GameObject clonePrefab;
    /// <summary>
    /// Controller for the clone - clones movement and rotation of the original object to the clone
    /// </summary>
    PortalCloneController cloneController;
    /// <summary>
    /// Is the object currently in the portal area?
    /// </summary>
    bool isInPortalArea = false;

    /// <summary>
    /// Is the object currently in the portal area?
    /// </summary>
    /// <returns>true if object is in portal</returns>
    public bool IsInPortalArea()
    {
        return isInPortalArea;
    }
    /// <summary>
    ///  Set if the object is currently in the portal area
    /// </summary>
    /// <param name="value">true if object is in the area</param>
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
    /// <summary>
    /// Create a clone of the object
    /// </summary>
    /// <param name="worldPos">position of clone in world space</param>
    /// <param name="localRotation">local rotaion of clone object</param>
    /// <param name="srcPortal">portal to which object is entering</param>
    /// <param name="dstPortal">portal in which clone should be placed</param>
    /// <returns></returns>
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
    /// <summary>
    /// Get the center of the object
    /// </summary>
    /// <returns>center of the object</returns>
    public Vector3 GetObjectCenter()
    {
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            return transform.position;

        return collider.bounds.center;
    }
    /// <summary>
    /// Clamp velocity of the object to MAX_VELOCITY
    /// </summary>
    void Update()
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
    /// <summary>
    /// Set the position of the object by its center
    /// </summary>
    /// <param name="pos">position that the center of the object should be after this call</param>
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
    /// <summary>
    /// Called when object leaves portal area
    /// </summary>
    /// <param name="portal">source portal</param>
    void IPortalEventsListener.OnExitedPortalArea( PortalLogic portal )
    {
        SetIsInPortalArea(false);
    }
}
