using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MirrorCube : PickableObject
{
    [SerializeField] private LayerMask layerMask;  // Warstwa obiektów, które mog¹ zablokowaæ laser
    [SerializeField] UnityEvent onReceiverHit;
    [SerializeField] UnityEvent onReceiverReleased;
    private LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    public Sprite mirrorOnSprite;
    public Sprite mirrorOffSprite;
    Vector3 start;          // Punkt pocz¹tkowy lasera
    Vector3 maxEnd;         // Punkt koñcowy lasera (maksymalny zasiêg)
    Vector3 realEnd;        // Punkt koñcowy lasera (aktualny zasiêg)
    public bool wasReceiverHit = false;

    override protected void Start()
    {
        base.Start();

        //rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();     // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;
        stopLaser();    // Wy³¹czenie lasera
    }

    override protected void Update()
    {
        base.Update();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        start = transform.position;  // Pierwszy punkt to pozycja transmitera
        maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z); // Odleg³y punkt na lewo od kostki
        realEnd = maxEnd;   // Pocz¹tkowo ustawiamy punkt koñcowy na maksymalny zasiêg
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
                if (!wasReceiverHit)
                {
                    Debug.Log("Hit receiver!");
                    onReceiverHit.Invoke();
                }
                wasReceiverHit = true;
            }
            else
            {
                if (wasReceiverHit)
                {
                    Debug.Log("Released receiver!");
                    onReceiverReleased.Invoke();
                }
                wasReceiverHit = false;
                if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                {
                    // TODO: Implement!
                }
            }
        }
    }

    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = mirrorOnSprite;
        lineRenderer.enabled = true;    // W³¹czenie linii
    }

    public void stopLaser()
    {
        // Zmiana sprite'a na nieaktywowany
        spriteRenderer.sprite = mirrorOffSprite;
        lineRenderer.enabled = false;   // Wy³¹czenie linii
    }
}
