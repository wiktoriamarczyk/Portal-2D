using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Lasers : MonoBehaviour
{
    [SerializeField] AudioSource laserSound;
    [SerializeField] AudioSource laserOn;
    [SerializeField] AudioSource laserOff;
    [SerializeField] private LayerMask layerMask;  // Warstwa obiektów, które mog¹ zablokowaæ laser
    [SerializeField] UnityEvent onReceiverHit;
    [SerializeField] UnityEvent onReceiverReleased;

    public Sprite defaultSprite;            // Domyœlny sprite
    public Sprite activatedSprite;          // Aktywowany sprite
    public Vector3 start;                   // Pocz¹tek linii lasera (pozycja emitera)
    public Vector3 maxEnd;                  // Odleg³y punkt na prostej lasera
    public Vector3 realEnd;                 // Faktyczny koniec lasera (po kolizji)
    private LineRenderer lineRenderer;      // Komponent LineRenderer
    private SpriteRenderer spriteRenderer;  // Komponent SpriteRenderer


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();     // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;
        start = transform.position;  // Pierwszy punkt to pozycja transmitera
        stopLaser();    // Wy³¹czenie lasera
        if (Math.Round(transform.rotation.z, 1) == -0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y - 100, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        }
        else if (transform.rotation.z == 0.0)
        {
            maxEnd = new Vector3(transform.position.x + 100, transform.position.y, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 3.1)
        {
            maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z);
        }
    }

    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = activatedSprite;
        lineRenderer.enabled = true;    // W³¹czenie linii
        laserOn.Play();
        laserSound.mute = false;
        laserSound.PlayDelayed(laserOn.clip.length);
        laserSound.loop = true;
    }

    public void stopLaser()
    {
        // Zmiana sprite'a na domyœlny
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;   // Wy³¹czenie linii
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
        GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
        laserSound.mute = true;
        laserSound.loop = false;
        if (DoorOut.isActive) GameObject.Find("DoorOut").GetComponent<DoorOut>().CloseDoor();
        laserOff.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start); // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
            {
                hit.collider.gameObject.GetComponent<MirrorCube>().startLaser();
            }
            else
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
                if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = activatedSprite;
                    if (!DoorOut.isActive) onReceiverHit.Invoke();
                }
                else
                {
                    if (DoorOut.isActive) onReceiverReleased.Invoke();
                    GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
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
        else
        {
            PortalLaser.isBluePortalHit = false;
            PortalLaser.isOrangePortalHit = false;
        }
    }
    public void ReceiverHit()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = activatedSprite;
    }

    public void ReceiverReleased()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }
}
