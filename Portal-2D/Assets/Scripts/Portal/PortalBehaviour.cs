using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PortalBehaviour : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap impostorTilemap;
    const int portalGridHeight = 6;
    const int portalGridWidtht = 2;

    void Start()
    {
        // ----to mo¿na zrobiæ eleganCIEJ----
        tilemap = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Tilemap>();
        impostorTilemap = GameObject.FindGameObjectWithTag("ImpostorTerrain").GetComponent<Tilemap>();

        MakeTilesBehindPortalNonCollidable();
    }

    void MakeTilesBehindPortalNonCollidable()
    {
        Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(transform.position);

        Debug.Log("grid pos: " + cellPosition);

        // przesuñ punkt startowy o 3 w dó³ w przestrzeni lokalnej
        cellPosition += Vector3Int.RoundToInt(gameObject.transform.up * -(portalGridHeight / 2));

        Vector3Int tilePos = Vector3Int.zero;

        // przesuñ punkt startowy ³¹cznie o 2 jednostki w bok
        for (int x = 0; x < portalGridWidtht; ++x)
        {
            Vector3Int shiftX = Vector3Int.RoundToInt(gameObject.transform.right * x);
            // przesuñ punkt startowy ³¹cznie o 6 jednostek w górê
            for (int y = 0; y < portalGridHeight; ++y)
            {
                Vector3Int shiftY = Vector3Int.RoundToInt(gameObject.transform.up * y);

                tilePos = cellPosition + shiftX + shiftY;// + new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(tilePos);
                impostorTilemap.SetTile(tilePos, tile);
                tilemap.SetTile(tilePos, null);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PortalManager.Instance.Teleport();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PortalManager.Instance.DestroyClone(collision.gameObject);
    }
}