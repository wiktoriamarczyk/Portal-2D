using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    [SerializeField] GameObject  orangeProjectile;
    [SerializeField] AudioSource lpmFireSound;
    [SerializeField] AudioSource ppmFireSound;

    [SerializeField] float debugangle;

    PlayerMovement  player;
    CursorMode      cursorMode = CursorMode.ForceSoftware;
    Vector2         hotSpot = Vector2.zero;
    Tilemap         tilemap;
    eCursorType     cursor = eCursorType.ORANGE;
    float           minAngleRange = -60f;
    float           maxAngleRange = 70f;

    enum eCursorType
    {
        BLUE,
        ORANGE
    }

    const int NON_RAYCAST_LAYERS = (int)Common.eLayerType.PLAYER |  (int)Common.eLayerType.NON_COLLIDABLE_UNITS;

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
            lpmFireSound.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeCursor(eCursorType.ORANGE);
            Fire();
            ppmFireSound.Play();
        }

        RotateArm();
    }

    void Fire()
    {
        // ostatnim parametrem są warstwy brane pod uwagę przez raycast za wyjątkiem warstwy, na której jest gracz
        RaycastHit2D hit = Physics2D.Raycast(arm.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position, 100f, ~((int)(NON_RAYCAST_LAYERS)));
        int hitLayer = -1;
        if (hit == true)
            hitLayer = hit.transform.gameObject.layer;

        if (hit.collider != null)
        {
            // ----DEBUG----
            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(arm.position, hit.point, UnityEngine.Color.red, 5f);
            Debug.DrawLine(hit.point, hit.point + hit.normal*5, UnityEngine.Color.green , 5f);

            Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(hit.point + hit.normal * -0.1f);
            Debug.Log("hit on grid pos: " + cellPosition);

            GameObject projectile;
            if (cursor == eCursorType.BLUE)
            {
                projectile = Instantiate(blueProjectile, projectileSpawner.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.BLUE);
                //projectile.GetComponent<Projectile>(). = 14;
            }
            else
            {
                projectile = Instantiate(orangeProjectile, projectileSpawner.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.ORANGE);

            }

            if (hit == true && hitLayer != (int)Common.eLayerType.NON_PORTAL && hitLayer != (int)Common.eLayerType.UNITS)
                projectile.GetComponent<Projectile>().InitializePortalProperties(hit.normal, cellPosition);
        }
    }

    void ChangeCursor(eCursorType cursorType)
    {
        if (cursor == cursorType)
            return;
        cursor = cursorType;
        Cursor.SetCursor(cursorTextures[(int)cursorType], hotSpot, cursorMode);
    }

    static float GetAngleFromVec(Vector3 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    /* TO FIX */
    void RotateArm()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 ArmToMouse = mousePos - arm.position;


        //Vector3 perpendicular = arm.position + mousePos;
        //Quaternion value = Quaternion.LookRotation(Vector3.forward, perpendicular);
        //value *= Quaternion.Euler(0, 0, 90);

        float angle = GetAngleFromVec( new Vector3(Mathf.Abs(ArmToMouse.x), ArmToMouse.y , ArmToMouse.z) );
        Quaternion value = Quaternion.Euler(0f, 0f, angle );
        arm.rotation = value;

        debugangle = angle;
        // ogranicz zakres obrotu ramienia
        angle = ModularClamp( debugangle , minAngleRange , maxAngleRange );

        var inverse = 1f;
        if( !player.IsFacingRight )
            inverse = -1f;

        arm.rotation = Quaternion.Euler( 0 , 0 , inverse * angle );

        if( !player.IsFacingRight && mousePos.x > player.transform.position.x )
            player.Flip();
        else if( player.IsFacingRight && mousePos.x < player.transform.position.x )
            player.Flip();
    }

    float ModularClamp(float value, float min, float max)
    {
        var modulus = 360f;
        value %= modulus;
        return Mathf.Clamp(value, min, max);
    }
}