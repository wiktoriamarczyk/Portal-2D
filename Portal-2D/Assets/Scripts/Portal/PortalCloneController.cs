using UnityEngine;

/// <summary>
/// Helper class for managing position and rotation of cloned object
/// </summary>
public class PortalCloneController : MonoBehaviour , IPortalEventsListener
{
    /// <summary>
    /// Cloned object
    /// </summary>
    [SerializeField] GameObject clone;
    /// <summary>
    /// Source portal
    /// </summary>
    PortalLogic ourPortal;
    /// <summary>
    /// Destination portal
    /// </summary>
    PortalLogic dstPortal;
    /// <summary>
    /// Returns cloned object
    /// </summary>
    /// <returns>cloned object</returns>
    public GameObject GetClone()
    {
        return clone;
    }
    /// <summary>
    /// Returns cloned object from given object
    /// </summary>
    /// <param name="obj">object from which we try to obtain clone</param>
    /// <returns>clone from the given object</returns>
    public static GameObject GetCloneFromObject(GameObject obj)
    {
        var cloneController = obj.GetComponent<PortalCloneController>();
        if (cloneController == null)
            return null;
        return cloneController.GetClone();
    }
    /// <summary>
    /// Update is called once per frame - updates position and rotation of cloned object
    /// </summary>
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
    /// <summary>
    /// Resets clone object to which we copy our position, rotation and scale
    /// </summary>
    /// <param name="newClone">new target clone</param>
    /// <param name="srcInPortal">source portal logic</param>
    /// <param name="dstInPortal">destination portal logic</param>
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
    /// <summary>
    /// Destroys clone object
    /// </summary>
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
    /// <summary>
    /// Called when we leave portal area
    /// </summary>
    /// <param name="portal">source portal</param>
    void IPortalEventsListener.OnExitedPortalArea(PortalLogic portal)
    {
        if (portal==ourPortal)
            DestroyClone();
    }
}
