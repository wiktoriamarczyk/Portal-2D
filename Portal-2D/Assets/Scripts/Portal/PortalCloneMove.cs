using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for moving the portal clone based on source and destination portals.
/// </summary>
public class PortalCloneMove : MonoBehaviour
{
    /// <summary>
    /// The source object to be moved via portal.
    /// </summary>
    [SerializeField] GameObject sourceObject;
    GameObject sourcePortal;
    GameObject destinationPortalOutput;

    void Update()
    {
        if (sourceObject == null || sourcePortal == null || destinationPortalOutput == null)
            return;

        transform.localPosition = CommonFunctions.PointWorldToLocal(sourcePortal.transform, sourceObject.transform.position);

        var loaclScaleA = CommonFunctions.VectorWorldToLocal(sourcePortal.transform, sourceObject.transform.localScale);

        transform.localScale = CommonFunctions.VectorLocalToWorld(destinationPortalOutput.transform, loaclScaleA);
    }

    /// <summary>
    /// Sets the source object (player), source portal, and destination portal output for the clone.
    /// </summary>
    /// <param name="source">The object to be moved via portal.</param>
    /// <param name="srcPortal">The source portal from where the object is being cloned.</param>
    /// <param name="dstPortalOutput">The destination portal where the object will be pased.</param>
    public void ResetSource(GameObject source, GameObject srcPortal, GameObject dstPortalOutput)
    {
        sourceObject = source;
        sourcePortal = srcPortal;
        destinationPortalOutput = dstPortalOutput;
    }
}