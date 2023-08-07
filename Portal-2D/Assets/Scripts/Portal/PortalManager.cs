using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that manages portals and their spawning.
/// </summary>
public class PortalManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static PortalManager Instance { get; private set; }
    /// <summary>
    /// Chell prefab
    /// </summary>
    [SerializeField] GameObject chellPrefab;
    /// <summary>
    /// Cube prefab
    /// </summary>
    [SerializeField] GameObject cubePrefab;
    /// <summary>
    /// Blue portal prefab
    /// </summary>
    [SerializeField] GameObject bluePortalPrefab;
    /// <summary>
    /// Orange portal prefab
    /// </summary>
    [SerializeField] GameObject orangePortalPrefab;
    /// <summary>
    /// Tilemap object
    /// </summary>
    [SerializeField] Tilemap tilemap;
    /// <summary>
    /// Impostor tilemap object
    /// </summary>
    [SerializeField] Tilemap impostorTilemap;
    /// <summary>
    /// Blue portal behaviour
    /// </summary>
    PortalBehaviour bluePortal;
    /// <summary>
    /// Orange portal behaviour
    /// </summary>
    PortalBehaviour orangePortal;
    /// <summary>
    /// Half of portal grid height
    /// </summary>
    const int portalGridHalfHeight = 3;
    /// <summary>
    /// Event invoked when portal changes
    /// </summary>
    public static event Action OnPortalChange;
    /// <summary>
    /// Tilemap property
    /// </summary>
    public Tilemap TilemapProperty
    {
        get => tilemap;
    }
    /// <summary>
    /// Impostor tilemap property
    /// </summary>
    public Tilemap ImpostorTilemapProperty
    {
        get => impostorTilemap;
    }
    /// <summary>
    /// Awake is called when the script instance is being loaded - singleton initialization
    /// </summary>
    void Awake()
    {
        PortalLaser.ResetState();

        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;
    }
    /// <summary>
    /// Clear singleton instance on destroy
    /// </summary>
    private void OnDestroy()
    {
        // singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
    /// <summary>
    /// Tries to spawn portal
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">portal spawn position</param>
    /// <returns>true if portal was spawned</returns>
    bool TrySpawnPortalCommon(ref PortalBehaviour portal, GameObject portalPrefab, Vector2 normal, Vector3Int gridPosition)
    {
        if (portal != null)
        {
            OnPortalChange?.Invoke();
            portal.InitDestroyment();
        }

        Vector3Int right = Vector3Int.RoundToInt( new Vector3(normal.x,normal.y, 0) );
        var up = Vector3Int.RoundToInt(Quaternion.Euler(0, 0, 90) * right);

        List<Vector3Int> cells = new List<Vector3Int>{ gridPosition };

        for (int i=1; i<=4; ++i)
        {
            cells.Add(gridPosition + up * i);
            cells.Add(gridPosition - up * i);
        }

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
    /// <summary>
    /// Tries to spawn blue portal
    /// </summary>
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">portal spawn position</param>
    /// <returns>true if portal was spawned</returns>
    public bool TrySpawnBluePortal(Vector2 normal, Vector3Int gridPosition)
    {
        return TrySpawnPortalCommon(ref bluePortal, bluePortalPrefab, normal, gridPosition);
    }
    /// <summary>
    /// Tries to spawn orange portal
    /// </summary>
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">portal spawn position</param>
    /// <returns>true if portal was spawned</returns>
    public bool TrySpawnOrangePortal(Vector2 normal, Vector3Int gridPosition)
    {
        return TrySpawnPortalCommon(ref orangePortal, orangePortalPrefab, normal, gridPosition);
    }
    /// <summary>
    /// Spawns portal
    /// </summary>
    /// <param name="portalPrefab">portal prefab</param>
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">portal spawn position</param>
    /// <returns>true if portal was spawned</returns>
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
    /// <summary>
    /// Checks if portal can be spawned at given position
    /// </summary>
    /// <param name="tilemap">tilemap object</param>
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">portal spawn position</param>
    /// <returns>true if position is valid</returns>
    bool IsValidPortalPosition(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);
        var up = Quaternion.Euler(0, 0, -90) * right;

        // how many additional tiles we need to not be empty above and below portal
        const int ADDITIONAL_NON_EMPTY_COUNT_VERTICALY = 0;

        for (int i = -(portalGridHalfHeight+ADDITIONAL_NON_EMPTY_COUNT_VERTICALY); i <= portalGridHalfHeight+ADDITIONAL_NON_EMPTY_COUNT_VERTICALY; ++i)
        {
            Vector3Int shiftY = Vector3Int.RoundToInt(up * i);
            DrawRectangle(tilemap, gridPosition + shiftY);
            var tile = tilemap.GetTile(gridPosition + shiftY);
            if (tile == null)
                return false;
        }

        gridPosition += Vector3Int.RoundToInt(right * 1);

        for (int i = 1-portalGridHalfHeight; i < portalGridHalfHeight; ++i)
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