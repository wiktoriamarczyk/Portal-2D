using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpriteController : MonoBehaviour
{
    [SerializeField] PortalBehaviour portalBehaviour;

    public void OnDestroyEnd()
    {
        portalBehaviour.Destroy();
    }
}
