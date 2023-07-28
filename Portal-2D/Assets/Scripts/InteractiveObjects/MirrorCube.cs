using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MirrorCube : PickableObject
{
    [SerializeField] private LayerMask layerMask;  // Warstwa obiekt�w, kt�re mog� zablokowa� laser
    [SerializeField] UnityEvent onReceiverHit;
    [SerializeField] UnityEvent onReceiverReleased;
    private LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public Sprite mirrorOnSprite;
    public Sprite mirrorOffSprite;
    Vector3 start;          // Punkt pocz�tkowy lasera
    Vector3 maxEnd;         // Punkt ko�cowy lasera (maksymalny zasi�g)
    Vector3 realEnd;        // Punkt ko�cowy lasera (aktualny zasi�g)

    override protected void Start()
    {
        base.Start();

        //rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();     // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;
        stopLaser();    // Wy��czenie lasera
    }

    override protected void Update()
    {
        base.Update();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        start = transform.position;  // Pierwszy punkt to pozycja transmitera
        maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z); // Odleg�y punkt na lewo od kostki
        realEnd = maxEnd;   // Pocz�tkowo ustawiamy punkt ko�cowy na maksymalny zasi�g
        if (lineRenderer.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start);     // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
            {
                Debug.Log("Hit receiver!");
                if (!DoorOut.isActive) onReceiverHit.Invoke();
            }
            else
            {
                Debug.Log("Released receiver!");
                if (DoorOut.isActive) onReceiverReleased.Invoke();
                if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                {
                    Debug.Log("The blue portal was hit by laser");
                    PortalLaser.isBluePortalHit = true;
                }
                else if (hit.collider != null && hit.collider.gameObject.tag == "Orange Portal")
                {
                    Debug.Log("The orange portal was hit by laser");
                    PortalLaser.isOrangePortalHit = true;
                }
                else
                {
                    PortalLaser.isBluePortalHit = false;
                    PortalLaser.isOrangePortalHit = false;
                }
            }
        }
    }

    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = mirrorOnSprite;
        lineRenderer.enabled = true;    // W��czenie linii
    }

    public void stopLaser()
    {
        // Zmiana sprite'a na nieaktywowany
        spriteRenderer.sprite = mirrorOffSprite;
        lineRenderer.enabled = false;   // Wy��czenie linii
    }
}
