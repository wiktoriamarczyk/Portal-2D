using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalCloneController : MonoBehaviour , IPortalEventsListener
{
    [SerializeField] GameObject clone;
    PortalCloner ourPortal;
    PortalCloner dstPortal;

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
        if (clone == null || ourPortal == null || this.dstPortal == null)
            return;

        var portalAdapter = GetComponent<PortalAdapter>();
        if (portalAdapter == null)
            return;

        var srcPortal = ourPortal.GetOwnPortal();
        var dstPortal = this.dstPortal.GetOwnPortal();

        clone.transform.localPosition = CommonFunctions.PointWorldToLocal(ourPortal.transform, portalAdapter.GetObjectCenter());


        //var localScaleA = CommonFunctions.VectorWorldToLocal(srcPortal.transform, transform.localScale);

        //clone.transform.localScale = CommonFunctions.VectorLocalToWorld(dstPortal.transform, localScaleA);

        clone.transform.localScale = transform.localScale;

        clone.transform.rotation = Quaternion.Euler( 0 , 0 , transform.rotation.eulerAngles.z );
    }

    public void ResetClone(GameObject newClone, PortalCloner srcInPortal, PortalCloner dstInPortal)
    {
        if (clone!=null && clone!=newClone)
            DestroyClone();

        clone = newClone;
        ourPortal = srcInPortal;
        dstPortal = dstInPortal;

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

    void IPortalEventsListener.OnTeleported(PortalCloner srcPortal, PortalCloner dstPortal)
    {
        //if (portal==ourPortal)
        //    DestroyClone();
    }

    void IPortalEventsListener.OnExitedPortalArea(PortalCloner portal)
    {
        if (portal==ourPortal)
            DestroyClone();
    }
}
