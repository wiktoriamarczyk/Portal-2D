using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortalEventsListener
{
    public void OnTeleported(PortalCloner srcPortal, PortalCloner dstPortal);
    public void OnExitedPortalArea(PortalCloner portal);
}