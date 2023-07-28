using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalCloneController : MonoBehaviour , IPortalEventsListener
{
    [SerializeField] GameObject clone;
    PortalLogic ourPortal;
    PortalLogic dstPortal;

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

        clone.transform.localPosition   = CommonFunctions.PointWorldToLocal(ourPortal.transform, portalAdapter.GetObjectCenter());
        clone.transform.localScale      = Vector3.Scale(transform.localScale, ourPortal.GetObjectXFlipFactor());
        clone.transform.rotation        = Quaternion.Euler( 0 , 0 , transform.rotation.eulerAngles.z );
    }

    public void ResetClone(GameObject newClone, PortalLogic srcInPortal, PortalLogic dstInPortal)
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

    void IPortalEventsListener.OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal)
    {
        //if (portal==ourPortal)
        //    DestroyClone();
    }

    void IPortalEventsListener.OnExitedPortalArea(PortalLogic portal)
    {
        if (portal==ourPortal)
            DestroyClone();
    }
}
