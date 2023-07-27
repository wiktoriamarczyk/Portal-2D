using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortalEventsListener
{
    public void OnTeleported(GameObject srcPortal, GameObject dstPortal, Vector3 srcPortalRight, Vector3 dstPortalRight);
    public void OnExitedPortalArea(GameObject portal);
}