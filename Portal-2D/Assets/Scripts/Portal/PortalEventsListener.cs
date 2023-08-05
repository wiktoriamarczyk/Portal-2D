/// <summary>
/// Interface for objects which can interact with portals
/// </summary>
public interface IPortalEventsListener
{
    public void OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal);
    public void OnExitedPortalArea(PortalLogic portal);
}