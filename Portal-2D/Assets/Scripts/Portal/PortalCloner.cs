using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PortalCloner : MonoBehaviour
{
    [SerializeField] GameObject clonePrefab;
    [SerializeField] GameObject ownPortal;
    [SerializeField] GameObject ownOutput;
    [SerializeField] PortalCloner destination;

    public GameObject GetOwnPortal()
    {
        return ownPortal;
    }
    public Transform GetOwnOutput()
    {
        return ownOutput.transform;
    }
    public Vector3 GetWorldVectorToPortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.right);
    }
    public Vector3 GetWorldVectorOutsidePortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.left);
    }

    public GameObject GetDestinationPortal()
    {
        return destination.GetOwnPortal();
    }
    public Transform GetDestinationOutput()
    {
        return destination.GetOwnOutput();
    }

    void OnTriggerEnter2D( Collider2D collision )
    {
        var portalAdapter = collision.gameObject.GetComponent<PortalAdapter>();
        if (portalAdapter == null)
            return;

        Vector3 worldClonePos = CommonFunctions.TransformPosBetweenPortals(collision.gameObject.transform.position, gameObject, GetDestinationPortal());

        GameObject clone = portalAdapter.CreateClone(worldClonePos, Quaternion.identity, GetDestinationOutput());
        clone.transform.localRotation = collision.gameObject.transform.localRotation;

        var clonerController = collision.gameObject.GetComponent<PortalCloneController>();
        if (clonerController == null)
            clonerController = collision.gameObject.AddComponent<PortalCloneController>();

        clonerController.ResetClone(clone, this, destination);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        var clone = PortalCloneController.GetCloneFromObject(collision.gameObject);
        if (clone == null)
            return;

        var portalAdapter = collision.gameObject.GetComponent<PortalAdapter>();
        if (portalAdapter == null)
            return;

        var ownWorldvecToPortal = GetWorldVectorToPortal();
        var dstWorldvecToPortal = destination.GetWorldVectorToPortal();

        var testpoint = transform.position + ownWorldvecToPortal*10;
        var objpos = portalAdapter.GetObjectCenter();
        float dist = Vector3.Distance(testpoint, objpos);
        if( dist < 9.8 )
        {
            var newpos = clone.transform.position + destination.GetWorldVectorOutsidePortal() * 0.2f;

            portalAdapter.SetPositionByCenter( newpos );

            //collision.gameObject.transform.position = ;

            if (Mathf.Abs(Vector3.Angle(ownWorldvecToPortal, dstWorldvecToPortal)) < 90)
            {
                var physics2D = collision.gameObject.GetComponent<Rigidbody2D>();
                var LocalVelocityVector = CommonFunctions.VectorWorldToLocal( transform , physics2D.velocity );
                var old = physics2D.velocity;
                physics2D.velocity = CommonFunctions.VectorLocalToWorld( destination.GetOwnOutput().transform , LocalVelocityVector );
            }

            var Listerners = collision.GetComponents<IPortalEventsListener>();
            foreach (var listener in Listerners)
            {
                listener.OnTeleported(this, destination);
            }
        }
    }

    void OnTriggerExit2D( Collider2D collision )
    {
        var Listerners = collision.GetComponents<IPortalEventsListener>();
        foreach (var listener in Listerners)
        {
            listener.OnExitedPortalArea(this);
        }
    }

}
