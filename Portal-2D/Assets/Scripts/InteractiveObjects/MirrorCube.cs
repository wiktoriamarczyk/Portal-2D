using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class representing a mirror cube
/// </summary>
public class MirrorCube : PickableObject
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] LayerMask layerMask;  // Warstwa obiektów, które mog¹ zablokowaæ laser
    [SerializeField] UnityEvent onReceiverHit;
    [SerializeField] UnityEvent onReceiverReleased;
    private LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public Sprite mirrorOnSprite;
    public Sprite mirrorOffSprite;
    public bool isHitByLaser = false;
    public bool isHitByTransmitter = false;
    public bool isHitByPortal = false;
    Vector3 start;          // Punkt pocz¹tkowy lasera
    Vector3 maxEnd;         // Punkt koñcowy lasera (maksymalny zasiêg)
    Vector3 realEnd;        // Punkt koñcowy lasera (aktualny zasiêg)

    override protected void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = mirrorOffSprite;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    override protected void Update()
    {
        // Calling the base class method
        base.Update();
        // Check if the cube is hit by the laser
        if (Lasers.isActive && (isHitByPortal || isHitByTransmitter))
        {
            spriteRenderer.sprite = mirrorOnSprite;
            start = transform.position;  // Set the first point as the cube's position
            maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z); // Second point - far away
            realEnd = maxEnd;   // Set the real end as the far away point (in case the laser doesn't hit anything)
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
                realEnd = hit.point;                // If the laser hits something, set the real end as the hit point
            lineRenderer.SetPosition(0, start);     // Setting the first point of the line
            lineRenderer.SetPosition(1, realEnd);   // Setting the second point of the line
            lineRenderer.enabled = true;            // Displaying the line
            if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
                hit.collider.gameObject.GetComponent<Receiver>().isHitByMirror = true;
            else
            {
                GameObject.Find("LaserReceiver").GetComponent<Receiver>().isHitByMirror = false;
                if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                    PortalLaser.isBlueHitByMirror = true;
                else
                {
                    PortalLaser.isBlueHitByMirror = false;
                    if (hit.collider != null && hit.collider.gameObject.tag == "Orange Portal")
                        PortalLaser.isOrangeHitByMirror = true;
                    else
                        PortalLaser.isOrangeHitByMirror = false;
                }
            }
        }
        else
        {
            spriteRenderer.sprite = mirrorOffSprite;
            lineRenderer.enabled = false;  // Hiding the line
            // Turning off potentially turned on objects
            GameObject.Find("Receiver").GetComponent<Receiver>().isHitByMirror = false;
            PortalLaser.isBlueHitByMirror = false;
            PortalLaser.isOrangeHitByMirror = false;
        }
    }
}
