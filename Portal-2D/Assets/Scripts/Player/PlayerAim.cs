using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] Texture2D[] cursorTextures;
    [SerializeField] Transform arm;

    CursorMode cursorMode = CursorMode.ForceSoftware;
    Vector2 hotSpot = Vector2.zero;
    eCursorType cursor = eCursorType.ORANGE;
    float offset = -90;
    PlayerMovement player;


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
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeCursor(eCursorType.ORANGE);
        }

        RotateArm();
    }

    void ChangeCursor(eCursorType cursorType)
    {
        if (cursor == cursorType)
        {
            return;
        }
        cursor = cursorType;
        Cursor.SetCursor(cursorTextures[(int)cursorType], hotSpot, cursorMode);
    }

    void RotateArm()
    {
        var inverse = 1f;
        if (!player.IsFacingRight)
        {
            inverse = -1f;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 perpendicular = arm.position + mousePos;
        Quaternion val = Quaternion.LookRotation(Vector3.forward, perpendicular);
        val *= Quaternion.Euler(0, 0, inverse * offset);
        arm.rotation = val;


        // Limits the viewing angle
        var angle = ModularClamp(arm.rotation.eulerAngles.z, -60f, 60f);
        arm.rotation = Quaternion.Euler(0, 0, angle);
    }

    float ModularClamp(float val, float min, float max, float rangemin = -180f, float rangemax = 180f)
    {
        var modulus = Mathf.Abs(rangemax - rangemin);
        if ((val %= modulus) < 0f) val += modulus;
        return Mathf.Clamp(val + Mathf.Min(rangemin, rangemax), min, max);
    }
}
