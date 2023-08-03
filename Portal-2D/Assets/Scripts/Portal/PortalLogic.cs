using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class PortalLogic : MonoBehaviour
{
    [SerializeField] GameObject ownPortal;
    [SerializeField] GameObject ownInterior;
    [SerializeField] GameObject ownOutput;
    [SerializeField] Animator animator;
    [SerializeField] PortalLogic destination;
    [SerializeField] PortalBehaviour portalBehaviour;
    bool isDying = false;
    public bool IsDying()
    {
        return isDying;
    }

    public void SetDestination(PortalLogic dest)
    {
        destination = dest;
        MakeTilesBehindPortalNonCollidable();
    }
    public PortalBehaviour GetPortalBehaviour()
    {
        return portalBehaviour;
    }

    public GameObject GetOwnPortal()
    {
        return ownPortal;
    }
    public Transform GetOwnOutput()
    {
        return ownOutput.transform;
    }
    public GameObject GetOwnInterior()
    {
        return ownInterior;
    }
    public Vector3 GetWorldVectorToPortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.right);
    }
    public Vector3 GetWorldVectorOutsidePortal()
    {
        return CommonFunctions.VectorLocalToWorld(transform, Vector3.left);
    }

    public GameObject GetDestinationPortal()
    {
        return destination?.GetOwnPortal();
    }
    public Transform GetDestinationOutput()
    {
        return destination?.GetOwnOutput();
    }

    public Vector3 GetObjectXFlipFactor()
    {
        if (ownInterior == null || ownOutput == null)
            return Vector3.one;

        return new Vector3(ownInterior.transform.localScale.x * ownOutput.transform.localScale.x * -1, 1, 1);
    }

    void OnTriggerEnter2D( Collider2D collision )
    {
        var portalAdapter = collision.gameObject.GetComponent<PortalAdapter>();
        if (portalAdapter == null || destination==null)
            return;

        Vector3 worldClonePos = CommonFunctions.TransformPosBetweenPortals(collision.gameObject.transform.position, gameObject, GetDestinationPortal());

        portalAdapter.CreateClone(worldClonePos, collision.gameObject.transform.localRotation, this, destination);
    }
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
        if (dist < 9.9)
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

    void OnTriggerExit2D( Collider2D collision )
    {
        var Listerners = collision.GetComponents<IPortalEventsListener>();
        foreach (var listener in Listerners)
        {
            listener.OnExitedPortalArea(this);
        }
    }

    List<Vector3Int> cells = new List<Vector3Int>();

    private void Start()
    {
        MakeTilesBehindPortalNonCollidable();

        if (animator!=null)
            animator.SetTrigger("OpenPortal");
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void OnDestroyBegin()
    {
        MakeTilesBehindPortalCollidable();
        isDying = true;
    }

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
                    cells.Add(tilemapPos);
                }
            }
        }
    }

    public void MakeTilesBehindPortalCollidable()
    {
        var tilemap = PortalManager.Instance.TilemapProperty;
        var impostorTilemap = PortalManager.Instance.ImpostorTilemapProperty;

        foreach (var cell in cells)
        {
            var tile = impostorTilemap.GetTile(cell);
            if (tile!=null)
            {
                tilemap.SetTile(cell, tile);
                impostorTilemap.SetTile(cell, null);
            }
        }
        cells.Clear();
    }

}
