using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalCloneController : MonoBehaviour , IPortalEventsListener
{
    [SerializeField] GameObject clone;
    GameObject ourPortal;
    GameObject clonePortalOutput;

    public GameObject GetClone()
    {
        return clone;
    }

    public static GameObject GetCloneFromObject(GameObject obj)
    {
        var cloneController = obj.GetComponent<PortalCloneController>();
        if (cloneController == null)
            return null;
        return cloneController.GetClone();
    }

    void Update()
    {
        if (clone == null || ourPortal == null || clonePortalOutput == null)
            return;

        clone.transform.localPosition = CommonFunctions.PointWorldToLocal(ourPortal.transform, transform.position);


        var localScaleA = CommonFunctions.VectorWorldToLocal(ourPortal.transform, transform.localScale);

        clone.transform.localScale = CommonFunctions.VectorLocalToWorld(clonePortalOutput.transform.parent, localScaleA);

        clone.transform.rotation = Quaternion.Euler( 0 , 0 , transform.rotation.eulerAngles.z );
    }

    public void ResetClone(GameObject newClone, GameObject srcPortal, GameObject dstPortalOutput)
    {
        if (clone!=null && clone!=newClone)
            DestroyClone();

        clone = newClone;
        ourPortal = srcPortal;
        clonePortalOutput = dstPortalOutput;

        if (clone==null)
            return;

        var animSync = clone.GetComponent<CloneAnimSync>();
        if (animSync)
            animSync.StartAnim(gameObject);
    }

    void DestroyClone()
    {
        if (clone!=null)
        {
            Destroy(clone);
            clone = null;
        }
    }

    void IPortalEventsListener.OnTeleported( GameObject srcPortal , GameObject dstPortal , Vector3 srcPortalRight , Vector3 dstPortalRight )
    {
        //if (portal==ourPortal)
        //    DestroyClone();
    }

    void IPortalEventsListener.OnExitedPortalArea( GameObject portal )
    {
        if (portal==ourPortal)
            DestroyClone();
    }
}
