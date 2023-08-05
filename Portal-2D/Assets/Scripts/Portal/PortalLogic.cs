using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for the logic of the portal.
/// </summary>
public class PortalLogic : MonoBehaviour
{
    /// <summary>
    /// Portal object that we reside in
    /// </summary>
    [SerializeField] GameObject ownPortal;
    /// <summary>
    /// Object that contains graphics and inside of the portal
    /// </summary>
    [SerializeField] GameObject ownInterior;
    /// <summary>
    /// Object that clones are placed in
    /// </summary>
    [SerializeField] GameObject ownOutput;
    /// <summary>
    /// Animator component
    /// </summary>
    [SerializeField] Animator animator;
    /// <summary>
    /// Destination portal
    /// </summary>
    [SerializeField] PortalLogic destination;
    /// <summary>
    /// Portal behaviour component
    /// </summary>
    [SerializeField] PortalBehaviour portalBehaviour;
    /// <summary>
    /// List of objects that are currently in the portal
    /// </summary>
    List<GameObject> objectsInPortal = new List<GameObject>();
    /// <summary>
    /// Is Portal being destroyed?
    /// </summary>
    bool isDying = false;
    /// <summary>
    /// Cells that are made non collidable by placing portal
    /// </summary>
    List<Vector3Int> portalModifiedCells = new List<Vector3Int>();

    /// <summary>
    /// Is Portal being destroyed?
    /// </summary>
    /// <returns>true if portal is being destroyed</returns>
    public bool IsDying()
    {
        return isDying;
    }
    /// <summary>
    /// Sets destination portal
    /// </summary>
    /// <param name="dest">destination portal</param>
    public void SetDestination(PortalLogic dest)
    {
        destination = dest;
        MakeTilesBehindPortalNonCollidable();
    }
    /// <summary>
    /// Get PortalBehaviour component
    /// </summary>
    /// <returns>portal behaviour</returns>
    public PortalBehaviour GetPortalBehaviour()
    {
        return portalBehaviour;
    }
    /// <summary>
    /// Get our portal object
    /// </summary>
    /// <returns>portal object</returns>
    public GameObject GetOwnPortal()
    {
        return ownPortal;
    }
    /// <summary>
    /// Get our output object
    /// </summary>
    /// <returns>portal output object</returns>
    public Transform GetOwnOutput()
    {
        return ownOutput.transform;
    }
    /// <summary>
    /// Get our interior object
    /// </summary>
    /// <returns>interior object</returns>
    public GameObject GetOwnInterior()
    {
        return ownInterior;
    }
    /// <summary>
    /// Get vector that points to portal in a world space
    /// </summary>
    /// <returns>vector that points to portal in a world space</returns>
    public Vector3 GetWorldVectorToPortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.right);
    }
    /// <summary>
    /// Get vector that points to an outside of the portal in a world space
    /// </summary>
    /// <returns>outside of the portal</returns>
    public Vector3 GetWorldVectorOutsidePortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.left);
    }
    /// <summary>
    /// Get destination portal object
    /// </summary>
    /// <returns>destination portal</returns>
    public GameObject GetDestinationPortal()
    {
        return destination?.GetOwnPortal();
    }
    /// <summary>
    /// Get destination output object
    /// </summary>
    /// <returns> destination output</returns>
    public Transform GetDestinationOutput()
    {
        return destination?.GetOwnOutput();
    }
    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public Vector3 GetObjectXFlipFactor()
    {
        if (ownInterior == null || ownOutput == null)
            return Vector3.one;

        return new Vector3(ownInterior.transform.localScale.x * ownOutput.transform.localScale.x * -1, 1, 1);
    }
    /// <summary>
    /// This method performs 3 tasks:
    /// - stores objects that touched portal
    /// - informs objects that they touched portals
    /// - creates clone of object that entered portal
    /// </summary>
    /// <param name="collision">object that entered portal area</param>
    void OnTriggerEnter2D( Collider2D collision )
    {
        var portalAdapter = collision.gameObject.GetComponent<PortalAdapter>();
        if (portalAdapter == null || destination==null)
            return;

        objectsInPortal.Add(collision.gameObject);

        portalAdapter.SetIsInPortalArea(true);

        Vector3 worldClonePos = CommonFunctions.TransformPosBetweenPortals(collision.gameObject.transform.position, gameObject, GetDestinationPortal());

        portalAdapter.CreateClone(worldClonePos, collision.gameObject.transform.localRotation, this, destination);
    }
    /// <summary>
    /// Checks if object passed point that indicates it should be teleported and perform the teleportation
    /// </summary>
    /// <param name="collision">object that is in portal area</param>
    void OnTriggerStay2D(Collider2D collision)
    {
        var clone = PortalCloneController.GetCloneFromObject(collision.gameObject);
        if (clone == null || destination==null)
            return;

        var portalAdapter = collision.gameObject.GetComponent<PortalAdapter>();
        if (portalAdapter == null)
            return;

        var ownWorldvecToPortal = GetWorldVectorToPortal();
        var dstWorldvecToPortal = destination.GetWorldVectorToPortal();

        var testpoint = transform.position + ownWorldvecToPortal*10;
        var objpos = portalAdapter.GetObjectCenter();
        float dist = Vector3.Distance(testpoint, objpos);
        if (dist < 9.8)
        {
            var newpos = clone.transform.position + destination.GetWorldVectorOutsidePortal() * 0.25f;

            portalAdapter.SetPositionByCenter(newpos);

            if (Mathf.Abs(Vector3.Angle(ownWorldvecToPortal, dstWorldvecToPortal)) < 100)
            {
                var physics2D = collision.gameObject.GetComponent<Rigidbody2D>();
                var LocalVelocityVector = CommonFunctions.VectorWorldToLocal( transform , physics2D.velocity );
                physics2D.velocity = CommonFunctions.VectorLocalToWorld( destination.GetOwnOutput().transform , LocalVelocityVector );
            }

            var Listerners = collision.GetComponents<IPortalEventsListener>();
            foreach (var listener in Listerners)
            {
                listener.OnTeleported(this, destination);
            }
        }
    }
    /// <summary>
    /// Removes object that leaves portal area from list, informs all components of this object about this fact
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerExit2D( Collider2D collision )
    {
        objectsInPortal.Remove(collision.gameObject);

        var Listerners = collision.GetComponents<IPortalEventsListener>();
        foreach (var listener in Listerners)
        {
            listener.OnExitedPortalArea(this);
        }
    }
    /// <summary>
    /// Informs all objects that are inside portal area that they are leaving portal because portal is being destroyed
    /// </summary>
    void OnDestroy()
    {
        foreach (var obj in objectsInPortal)
        {
            var Listerners = obj.GetComponents<IPortalEventsListener>();
            foreach (var listener in Listerners)
            {
                listener.OnExitedPortalArea(this);
            }
        }
    }
    /// <summary>
    /// Makes cells under this portal non collidable and start animation
    /// </summary>
    void Start()
    {
        MakeTilesBehindPortalNonCollidable();

        if (animator!=null)
            animator.SetTrigger("OpenPortal");
    }
    /// <summary>
    /// Destroy portal
    /// </summary>
    void Destroy()
    {
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Method called on start of the destroyment process
    /// </summary>
    public void OnDestroyBegin()
    {
        MakeTilesBehindPortalCollidable();
        isDying = true;
    }
    /// <summary>
    /// Make cells under this portal non collidable
    /// </summary>
    public void MakeTilesBehindPortalNonCollidable()
    {
        var tilemap = PortalManager.Instance.TilemapProperty;
        var impostorTilemap = PortalManager.Instance.ImpostorTilemapProperty;

        MakeTilesBehindPortalCollidable();
        if (!destination)
            return;

        int portalGridWidth = 3;
        int portalGridHalfHeight = 3;

        var StartPos = Vector3.zero;


        for (int x = 0; x < portalGridWidth; ++x)
        {
            for (int y = -portalGridHalfHeight; y <= portalGridHalfHeight; ++y)
            {
                var localGridPos = StartPos + new Vector3(x*tilemap.cellSize.x, y*tilemap.cellSize.y, 0);
                var worldGridPos = CommonFunctions.PointLocalToWorld(ownInterior.transform, localGridPos);
                var tilemapPos   = tilemap.WorldToCell(worldGridPos);

                var tile = tilemap.GetTile(tilemapPos);
                if (tile!=null)
                {
                    impostorTilemap.SetTile(tilemapPos, tile);
                    tilemap.SetTile(tilemapPos, null);
                    portalModifiedCells.Add(tilemapPos);
                }
            }
        }
    }
    /// <summary>
    /// Make cells under this portal collidable again
    /// </summary>
    public void MakeTilesBehindPortalCollidable()
    {
        var tilemap = PortalManager.Instance.TilemapProperty;
        var impostorTilemap = PortalManager.Instance.ImpostorTilemapProperty;

        foreach (var cell in portalModifiedCells)
        {
            var tile = impostorTilemap.GetTile(cell);
            if (tile!=null)
            {
                tilemap.SetTile(cell, tile);
                impostorTilemap.SetTile(cell, null);
            }
        }
        portalModifiedCells.Clear();
    }

}
