using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Events;
using System;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [SerializeField] GameObject chellPrefab;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject bluePortalPrefab;
    [SerializeField] GameObject orangePortalPrefab;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap impostorTilemap;

    PortalBehaviour bluePortal;
    PortalBehaviour orangePortal;

    const int portalGridHalfHeight = 3;

    public static event Action OnPortalChange;

    public Tilemap TilemapProperty
    {
        get => tilemap;
    }
    public Tilemap ImpostorTilemapProperty
    {
        get => impostorTilemap;
    }

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;

        tilemap = PortalManager.Instance.TilemapProperty;
    }

    bool TrySpawnPortalCommon(ref PortalBehaviour portal, GameObject portalPrefab, Vector2 normal, Vector3Int gridPosition)
    {
        if (portal != null)
            portal.InitDestroyment();

        Vector3Int right = new Vector3Int((int)normal.x, (int)normal.y, 0);
        var up = Vector3Int.RoundToInt(Quaternion.Euler(0, 0, 90) * right);

        Vector3Int[] cells = { gridPosition
                             , gridPosition + up    , gridPosition - up
                             , gridPosition + up * 2, gridPosition - up * 2
                             , gridPosition + up * 3, gridPosition - up * 3 };

        foreach (var cell in cells)
        {
            if (IsValidPortalPosition(tilemap, normal, cell))
            {
                portal = SpawnPortal(portalPrefab, normal, cell);
                PortalBehaviour.Link(bluePortal, orangePortal);
                OnPortalChange?.Invoke();
                return true;
            }
        }

        return false;
    }

    public bool TrySpawnBluePortal(Vector2 normal, Vector3Int gridPosition)
    {
        return TrySpawnPortalCommon(ref bluePortal, bluePortalPrefab, normal, gridPosition);
    }

    public bool TrySpawnOrangePortal(Vector2 normal, Vector3Int gridPosition)
    {
        return TrySpawnPortalCommon(ref orangePortal, orangePortalPrefab, normal, gridPosition);
    }

    PortalBehaviour SpawnPortal(GameObject portalPrefab, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3( normal.x , normal.y , 0.0f );

        Debug.Log( "Valid placement for blue portal" );
        float angle = Vector3.SignedAngle( normal , Vector3.right , Vector3.back ) + 180;
        Debug.Log( "Angle: " + angle);

        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angle);

        GameObject portalObject = Instantiate(portalPrefab, tilemap.layoutGrid.CellToWorld(gridPosition)+tilemap.cellSize*0.5f, rotationQuaternion);

        return portalObject.GetComponent<PortalBehaviour>();
    }

    bool IsValidPortalPosition(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);
        var up = Quaternion.Euler(0, 0, -90) * right;

        for (int i = -portalGridHalfHeight; i <= portalGridHalfHeight; ++i)
        {
            Vector3Int shiftY = Vector3Int.RoundToInt(up * i);
            DrawRectangle(tilemap, gridPosition + shiftY);
            var tile = tilemap.GetTile(gridPosition + shiftY);
            if (tile == null)
                return false;
        }

        gridPosition += Vector3Int.RoundToInt(right * 1);

        for (int i = -portalGridHalfHeight; i <= portalGridHalfHeight; ++i)
        {
            Vector3Int shiftY = Vector3Int.RoundToInt(up * i);
            var tile = tilemap.GetTile(gridPosition + shiftY);
            DrawRectangle(tilemap, gridPosition + shiftY);
            if (tile != null)
                return false;
        }

        return true;
    }

    // ----DEBUG----
    void DrawRectangle(Tilemap tilemap, Vector3Int a)
    {
        var tl = tilemap.layoutGrid.CellToWorld(a);
        var br = tl + tilemap.cellSize;
        var tr = new Vector3(br.x, tl.y, 0);
        var bl = new Vector3(tl.x, br.y, 0);


        Debug.DrawLine(tl, tr, Color.yellow, 2);
        Debug.DrawLine(tr, br, Color.yellow, 2);
        Debug.DrawLine(br, bl, Color.yellow, 2);
        Debug.DrawLine(bl, tl, Color.yellow, 2);
    }
}