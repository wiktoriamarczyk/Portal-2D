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
    [SerializeField] Transform arm;

    PlayerMovement  player;
    CursorMode      cursorMode = CursorMode.ForceSoftware;
    Vector2         hotSpot = Vector2.zero;
    eCursorType     cursor = eCursorType.ORANGE;
    float           minAngleRange = -60f;
    float           maxAngleRange = 60f;

    [SerializeField] Tilemap tilemap;


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
        // ostatnim parametrem s¹ warstwy brane pod uwagê przez raycast za wyj¹tkiem warstwy, na której jest gracz
        RaycastHit2D hit = Physics2D.Raycast(arm.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position, 100f, ~(1 << 9));
        if (hit.collider != null)
        {
            // ----DEBUG----
            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(arm.position, hit.point, Color.red, 5f);
            Debug.DrawLine(hit.point, hit.point + hit.normal*5, Color.green , 5f);

            Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(hit.point + hit.normal * -0.1f);
            Debug.Log("hit on grid pos: " + cellPosition);

            if (cursor == eCursorType.BLUE)
                PortalManager.Instance.TrySpawnBluePortal(tilemap, hit.normal, cellPosition);
            else if (cursor == eCursorType.ORANGE)
                PortalManager.Instance.TrySpawnOrangePortal(tilemap, hit.normal, cellPosition);
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
        var inverse = 1f;
        if (!player.IsFacingRight)
            inverse = -1f;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 perpendicular = arm.position + mousePos;
        Quaternion value = Quaternion.LookRotation(Vector3.forward, perpendicular);
        value *= Quaternion.Euler(0, 0, inverse * (-90));
        arm.rotation = value;

        // ogranicz zakres obrotu ramienia
        var angle = ModularClamp(arm.rotation.eulerAngles.z, minAngleRange, maxAngleRange);
        arm.rotation = Quaternion.Euler(0, 0, angle);
    }

    float ModularClamp(float value, float min, float max, float rangemin = -180f, float rangemax = 180f)
    {
        var modulus = Mathf.Abs(rangemax - rangemin);
        if ((value %= modulus) < 0f) value += modulus;
        return Mathf.Clamp(value + Mathf.Min(rangemin, rangemax), min, max);
    }
}