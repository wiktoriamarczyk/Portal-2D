using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [SerializeField] GameObject objectToClone;
    [SerializeField] GameObject bluePortalPrefab;
    [SerializeField] GameObject orangePortalPrefab;

    GameObject chellClone;
    GameObject instantiatedObjectsParent;
    GameObject bluePortal;
    GameObject orangePortal;
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

        // ----to mo¿na zrobiæ eleganCIEJ----
        instantiatedObjectsParent = GameObject.Find("---Environment---/Units");
    }

    public void CreateClone(GameObject portal, GameObject source)
    {
        colliderEnterCount++;
        if (chellClone)
            return;

        // jeœli sygna³ przyszed³ z niebieskiego portalu, to stwórz instancjê klona na pozycji spawnu pomarañczowego portalu
        if (portal == bluePortalPrefab)
            spawnPosition = orangePortalPrefab.transform;
        else
            spawnPosition = bluePortalPrefab.transform;

        // stwórz klona w miejscu portala, przez który przechodzimy i wy³¹cz mu fizykê
        Vector3 yOffset = new Vector3(0, portal.transform.position.y - objectToClone.transform.position.y, 0);
        chellClone = Instantiate(objectToClone, spawnPosition.position - yOffset, Quaternion.identity);
        chellClone.transform.SetParent(instantiatedObjectsParent.transform);
        // objectToClone.GetComponent<PlayerMovement>().Flip();
        chellClone.gameObject.name = cloneName;
        chellClone.AddComponent<PlayerCloneMove>();
        chellClone.GetComponent<Rigidbody2D>().simulated = false;
        chellClone.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void Teleport()
    {
        var clonePos = chellClone.transform.position;
        chellClone.transform.position = objectToClone.transform.position;
        objectToClone.transform.position = clonePos;

        chellClone.GetComponent<PlayerCloneMove>().Reset();
    }

    public void DestroyClone(GameObject source)
    {
        colliderEnterCount--;
        if (colliderEnterCount == 0)
            Destroy(chellClone);
    }

    public void TrySpawnBluePortal(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        if (!IsValidPortalPosition(tilemap, normal, gridPosition))
            return;

        SpawnPortal(bluePortalPrefab, tilemap, normal, gridPosition);
    }

    public void TrySpawnOrangePortal(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        if (!IsValidPortalPosition(tilemap, normal, gridPosition))
            return;

        SpawnPortal(orangePortalPrefab, tilemap, normal, gridPosition);
    }

    void SpawnPortal(GameObject portalPrefab, Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);

        Debug.Log("Valid placement for blue portal");
        float angle = Vector3.Angle(normal, Vector3.left);
        Debug.Log("angle " + angle);
        Quaternion rotationQuaternion = Quaternion.Euler(0, 0, angle);

        GameObject portalObject;

        if (portalPrefab == orangePortalPrefab)
            portalObject = orangePortal;
        else if (portalPrefab == bluePortal)
            portalObject = bluePortal;

        portalObject = Instantiate(portalPrefab, tilemap.layoutGrid.CellToWorld(gridPosition), rotationQuaternion);
        portalObject.transform.SetParent(instantiatedObjectsParent.transform);
    }

    bool IsValidPortalPosition(Tilemap tilemap, Vector2 normal, Vector3Int gridPosition)
    {
        var right = new Vector3(normal.x, normal.y, 0.0f);
        var up = Quaternion.Euler(0, 0, 90) * right;

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