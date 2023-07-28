using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortalEventsListener
{
    public void OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal);
    public void OnExitedPortalArea(PortalLogic portal);
}