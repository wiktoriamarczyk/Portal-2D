using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PortalCloner : MonoBehaviour
{
    [SerializeField] GameObject clonePrefab;
    [SerializeField] GameObject destinationPortal;

    //GameObject clone;

    void OnTriggerEnter2D( Collider2D collision )
    {
        GameObject clone;

        Vector3 worldClonePos = CommonFunctions.TransformPosBetweenPortals(collision.gameObject.transform.position, gameObject, destinationPortal);

        var prefabToClone = clonePrefab;

        bool NoPrefab = false;

        if (collision.gameObject.GetComponent<PlayerMovement>() == null)
        {
            prefabToClone = collision.gameObject;
            NoPrefab = true;
        }

        clone = Instantiate(prefabToClone, worldClonePos, Quaternion.identity, destinationPortal.transform);
        clone.transform.localRotation = collision.gameObject.transform.localRotation;

        if (NoPrefab)
        {
            Object.Destroy( clone.GetComponent<Rigidbody2D>() );
            Object.Destroy( clone.GetComponent<BoxCollider2D>() );
            var images = clone.GetComponents<SpriteRenderer>();
            foreach ( var image in images )
            {
                image.sortingLayerID = SortingLayer.NameToID("Clones");
                image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        var clonerController = collision.gameObject.GetComponent<PortalCloneController>();
        if (clonerController == null)
            clonerController = collision.gameObject.AddComponent<PortalCloneController>();

        clonerController.ResetClone(clone, gameObject, destinationPortal);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        var clone = PortalCloneController.GetCloneFromObject(collision.gameObject);
        if (clone == null)
            return;

        var srcTrueRight = CommonFunctions.VectorLocalToWorld(transform, Vector3.right);
        var dstTrueRight = CommonFunctions.VectorLocalToWorld(destinationPortal.transform, Vector3.right);
        var testpoint = transform.position + srcTrueRight*10;
        if( Vector3.Distance( testpoint , collision.gameObject.transform.position ) < 10 )
        {
            collision.gameObject.transform.position = clone.transform.position + dstTrueRight*0.2f;

            if (Mathf.Abs(Vector3.Angle(srcTrueRight, dstTrueRight)) > 90)
            {
                var physics2D = collision.gameObject.GetComponent<Rigidbody2D>();
                var A = CommonFunctions.VectorWorldToLocal( transform , physics2D.velocity );
                physics2D.velocity = CommonFunctions.VectorLocalToWorld( destinationPortal.transform , A );
            }

            var Listerners = collision.GetComponents<IPortalEventsListener>();
            foreach (var listener in Listerners)
            {
                listener.OnTeleported(gameObject, destinationPortal, srcTrueRight, dstTrueRight);
            }
        }
    }

    void OnTriggerExit2D( Collider2D collision )
    {
        var Listerners = collision.GetComponents<IPortalEventsListener>();
        foreach (var listener in Listerners)
        {
            listener.OnExitedPortalArea(gameObject);
        }
    }

}
