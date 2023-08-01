using UnityEngine;

/// <summary>
/// Class responsible for portal sprite animation
/// </summary>
public class PortalSpriteController : MonoBehaviour
{
    /// <summary>
    /// Reference to portal
    /// </summary>
    [SerializeField] PortalBehaviour portalBehaviour;
    /// <summary>
    /// Method called on the end of the animation of portal destruction
    /// </summary>
    public void OnDestroyEnd()
    {
        portalBehaviour.Destroy();
    }
}
