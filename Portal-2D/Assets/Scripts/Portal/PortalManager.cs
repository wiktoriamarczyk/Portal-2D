using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using System.Collections.Generic;
using System.Threading;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [SerializeField] GameObject chellPrefab;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject bluePortalPrefab;
    [SerializeField] GameObject orangePortalPrefab;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap impostorTilemap;

    GameObject objectToClone;
    GameObject clone;

    PortalBehaviour bluePortal;
    PortalBehaviour orangePortal;
    Transform spawnPosition;

    int colliderEnterCount = 0;
    string cloneName = "PortalClone";
    const int portalGridHeight = 6;


    /* public properties */
    public string CloneNameProperty
    {
        get => cloneName;
        set => cloneName = value;
    }
    public GameObject ObjectToCloneProperty
    {
        get => objectToClone;
        set => objectToClone = value;
    }

    public Tilemap TilemapProperty
    {
        get => tilemap;
    }
    public Tilemap ImpostorTilemapProperty
    {
        get => impostorTilemap;
    }

    void Start()
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

    public void CreateClone(GameObject portal, GameObject source)
    {
        colliderEnterCount++;
        if (clone)
            return;

        // stwórz instancjê klona w miejscu portalu, przez który przeszliœmy
        if (portal.CompareTag("Orange Portal"))
            spawnPosition = bluePortal.transform;
        else
            spawnPosition = orangePortal.transform;

        // stwórz klona w miejscu portala, przez który przechodzimy i wy³¹cz mu fizykê
        Vector3 yOffset = new Vector3(0, portal.transform.position.y - objectToClone.transform.position.y, 0);
        clone = Instantiate(objectToClone, spawnPosition.position - yOffset, Quaternion.identity);
        // objectToClone.GetComponent<PlayerMovement>().Flip();
        if (clone.CompareTag("Player"))
            clone.GetComponent<PlayerMovement>().IsFacingRight = objectToClone.GetComponent<PlayerMovement>().IsFacingRight;
        clone.gameObject.name = cloneName;
        clone.AddComponent<PlayerCloneMove>();
        clone.GetComponent<Rigidbody2D>().simulated = false;
        clone.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void Teleport()
    {
        var clonePos = clone.transform.position;
        clone.transform.position = objectToClone.transform.position;
        objectToClone.transform.position = clonePos;
        clone.GetComponent<PlayerCloneMove>().Reset();
    }

    public void DestroyClone(GameObject source)
    {
        colliderEnterCount--;
        if (colliderEnterCount == 0)
            Destroy(clone);
    }

    public bool TrySpawnBluePortal(Vector2 normal, Vector3Int gridPosition)
    {
        if (bluePortal != null)
            bluePortal.InitDestroyment();

        Vector3Int right = new Vector3Int((int)normal.x, (int)normal.y, 0);
        var up = Vector3Int.RoundToInt(Quaternion.Euler(0, 0, 90) * right);

        List<Vector3Int> cells = new List<Vector3Int> { gridPosition };//, gridPosition + up, gridPosition - up, gridPosition + up * 2, gridPosition - up * 2 };

        foreach (var cell in cells)
        {
            if (IsValidPortalPosition(tilemap, normal, cell))
            {
                bluePortal = SpawnPortal(bluePortalPrefab, normal, cell);
                bluePortal.otherEnd = orangePortal;
                orangePortal.otherEnd = bluePortal;
                return true;
            }
        }

        return false;
    }

    public bool TrySpawnOrangePortal(Vector2 normal, Vector3Int gridPosition)
    {
        if (orangePortal != null)
            orangePortal.InitDestroyment();


        Vector3Int right = new Vector3Int((int)normal.x, (int)normal.y, 0);
        var up = Vector3Int.RoundToInt(Quaternion.Euler(0, 0, 90) * right);

        List<Vector3Int> cells = new List<Vector3Int> { gridPosition };//, gridPosition + up, gridPosition - up, gridPosition + up * 2, gridPosition - up * 2 };

        foreach (var cell in cells)
        {
            if (IsValidPortalPosition(tilemap, normal, cell))
            {
                orangePortal = SpawnPortal(orangePortalPrefab, normal, cell);
                bluePortal.otherEnd = orangePortal;
                orangePortal.otherEnd = bluePortal;
                return true;
            }
        }

        return false;
    }

    PortalBehaviour SpawnPortal(GameObject portalPrefab, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);

        Debug.Log("Valid placement for blue portal");
        float angle = Vector3.SignedAngle(normal, Vector3.right, Vector3.forward);

        Debug.Log("angle " + angle);

        // ------- TO FIX ------------
        if (Mathf.Abs(angle) - 90 < 0.01f)
            angle = -angle;

        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angle);

        GameObject portalObject = Instantiate(portalPrefab, tilemap.layoutGrid.CellToWorld(gridPosition), rotationQuaternion);

        //if (portalPrefab == orangePortalPrefab)
        //    orangePortal = portalObject.GetComponent<PortalBehaviour>();
        //else if (portalPrefab == bluePortalPrefab)
        //    bluePortal = portalObject.GetComponent<PortalBehaviour>();
        return portalObject.GetComponent<PortalBehaviour>();
    }

    bool IsValidPortalPosition(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);
        var up = Quaternion.Euler(0, 0, -90) * right;

        gridPosition += Vector3Int.RoundToInt(up * -(portalGridHeight / 2));

        for (int i = 0; i < portalGridHeight; ++i)
        {
            Vector3Int shiftY = Vector3Int.RoundToInt(up * i);
            DrawRectangle(tilemap, gridPosition + shiftY);
            var tile = tilemap.GetTile(gridPosition + shiftY);
            if (tile == null)
                return false;
        }

        gridPosition += Vector3Int.RoundToInt(right * 1);

        for (int i = 0; i < portalGridHeight; ++i)
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