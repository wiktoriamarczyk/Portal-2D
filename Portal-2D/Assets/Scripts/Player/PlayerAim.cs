using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] Texture2D[] cursorTextures;
    [SerializeField] Transform   arm;
    [SerializeField] Transform   projectileSpawner;
    [SerializeField] GameObject  blueProjectile;
    [SerializeField] GameObject orangeProjectile;

    PlayerMovement  player;
    CursorMode      cursorMode = CursorMode.ForceSoftware;
    Vector2         hotSpot = Vector2.zero;
    Tilemap         tilemap;
    eCursorType     cursor = eCursorType.ORANGE;
    float           minAngleRange = -60f;
    float           maxAngleRange = 60f;

    enum eCursorType
    {
        BLUE,
        ORANGE
    }

    void Awake()
    {
        ChangeCursor(eCursorType.BLUE);
        player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        tilemap = PortalManager.Instance.TilemapProperty;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ChangeCursor(eCursorType.BLUE);
            Fire();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeCursor(eCursorType.ORANGE);
            Fire();
        }

        RotateArm();
    }

    void Fire()
    {
        // ostatnim parametrem są warstwy brane pod uwagę przez raycast za wyjątkiem warstwy, na której jest gracz
        RaycastHit2D hit = Physics2D.Raycast(arm.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position, 100f, ~(1 << 9));
        if (hit.collider != null)
        {
            // ----DEBUG----
            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(arm.position, hit.point, Color.red, 5f);
            Debug.DrawLine(hit.point, hit.point + hit.normal*5, Color.green , 5f);

            Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(hit.point + hit.normal * -0.1f);
            Debug.Log("hit on grid pos: " + cellPosition);

            Quaternion projectileRot;
            if (GetComponent<PlayerMovement>().IsFacingRight)
                projectileRot = arm.rotation;
            else
                projectileRot = new Quaternion(-arm.rotation.x, -arm.rotation.y, -arm.rotation.z, 0);

            if (cursor == eCursorType.BLUE)
            {
                var projectile = Instantiate(blueProjectile, projectileSpawner.position, projectileRot);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.BLUE);
                projectile.GetComponent<Projectile>().InitializePortalProperties(hit.normal, cellPosition);
            }
            else if (cursor == eCursorType.ORANGE)
            {
                var projectile = Instantiate(orangeProjectile, projectileSpawner.position, projectileRot);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.ORANGE);
                projectile.GetComponent<Projectile>().InitializePortalProperties(hit.normal, cellPosition);
            }
        }
    }

    void ChangeCursor(eCursorType cursorType)
    {
        if (cursor == cursorType)
            return;
        cursor = cursorType;
        Cursor.SetCursor(cursorTextures[(int)cursorType], hotSpot, cursorMode);
    }

    void RotateArm()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 perpendicular = arm.position + mousePos;
        Quaternion value = Quaternion.LookRotation(Vector3.forward, perpendicular);
        value *= Quaternion.Euler(0, 0, 90);
        arm.rotation = value;

        // ogranicz zakres obrotu ramienia
        var angle = ModularClamp(arm.rotation.eulerAngles.z, minAngleRange, maxAngleRange);

        var inverse = 1f;
        if (player.IsFacingRight)
            inverse = -1f;

        arm.rotation = Quaternion.Euler(0, 0, inverse * angle);

        if (!player.IsFacingRight && mousePos.x > player.transform.position.x)
            player.Flip();
        else if (player.IsFacingRight && mousePos.x < player.transform.position.x)
            player.Flip();

    }

    float ModularClamp(float value, float min, float max, float rangemin = -180f, float rangemax = 180f)
    {
        var modulus = Mathf.Abs(rangemax - rangemin);
        if ((value %= modulus) < 0f) value += modulus;
        return Mathf.Clamp(value + Mathf.Min(rangemin, rangemax), min, max);
    }
}