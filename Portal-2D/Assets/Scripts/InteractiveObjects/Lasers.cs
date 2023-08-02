using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Laser class - controls laser behaviour
/// </summary>
public class Lasers : MonoBehaviour
{
    /// <summary>
    /// Laser sound
    /// </summary>
    [SerializeField] AudioSource laserSound;
    /// <summary>
    /// Laser on sound
    /// </summary>
    [SerializeField] AudioSource laserOn;
    /// <summary>
    /// Laser off sound
    /// </summary>
    [SerializeField] AudioSource laserOff;
    /// <summary>
    /// Event called when laser hits receiver
    /// </summary>
    [SerializeField] UnityEvent onReceiverHit;
    /// <summary>
    /// Event called when laser is released from receiver
    /// </summary>
    [SerializeField] UnityEvent onReceiverReleased;
    /// <summary>
    /// Layer mask for objects that can stop the laser
    /// </summary>
    [SerializeField] LayerMask layerMask;
    /// <summary>
    /// Default sprite
    /// </summary>
    public Sprite defaultSprite;
    /// <summary>
    /// Activated sprite (after laser is turned on or the receiver is hit)
    /// </summary>
    public Sprite activatedSprite;
    /// <summary>
    /// Start point of the laser
    /// </summary>
    public Vector3 start;
    /// <summary>
    /// End point of the laser (far away)
    /// </summary>
    public Vector3 maxEnd;
    /// <summary>
    /// End point of the laser (real - determined by raycast)
    /// </summary>
    public Vector3 realEnd;
    /// <summary>
    /// Line renderer component - for drawing the laser
    /// </summary>
    private LineRenderer lineRenderer;
    /// <summary>
    /// Sprite renderer component - for changing the sprites
    /// </summary>
    private SpriteRenderer spriteRenderer;
    /// <summary>
    /// True if the laser is on
    /// </summary>
    public static bool isActive = false;


    /// <summary>
    /// Method called before the first frame
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();        // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;                     // Ustawienie liczby punkt�w na 2
        start = transform.position;                         // Pierwszy punkt to pozycja transmitera
        stopLaser();                                        // Wy��czenie lasera na pocz�tku gry
        // Ustawienie w�a�ciwego kierunku i zwrotu lasera w zale�no�ci od rotacji transmitera
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

    /// <summary>
    /// Method to start the laser
    /// </summary>
    public void startLaser()
    {
        isActive = true;
        spriteRenderer.sprite = activatedSprite;
        lineRenderer.enabled = true;    // W��czenie linii
        laserOn.Play();
        laserSound.mute = false;
        laserSound.PlayDelayed(laserOn.clip.length);
        laserSound.loop = true;
    }

    /// <summary>
    /// Method to stop the laser
    /// </summary>
    public void stopLaser()
    {
        // Zmiana sprite'a na domy�lny
        isActive = false;
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;   // Wy��czenie linii
        laserSound.mute = true;
        laserSound.loop = false;
        laserOff.Play();
    }

    /// <summary>
    /// Method called every frame
    /// </summary>
    void Update()
    {
        if (isActive)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start);     // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
            {
                hit.collider.gameObject.GetComponent<MirrorCube>().isHitByTransmitter = true;
            }
            else
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().isHitByTransmitter = false;
                if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
                {
                    hit.collider.gameObject.GetComponent<Receiver>().isHitByTransmitter = true;
                }
                else
                {
                    GameObject.Find("LaserReceiver").GetComponent<Receiver>().isHitByTransmitter = false;
                    if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                    {
                        PortalLaser.isBlueHitByTransmitter = true;
                    }
                    else
                    {
                        PortalLaser.isBlueHitByTransmitter = false;
                        if (hit.collider != null && hit.collider.gameObject.tag == "Orange Portal")
                            PortalLaser.isOrangeHitByTransmitter = true;
                        else
                            PortalLaser.isOrangeHitByTransmitter = false;
                    }
                }
            }
        }
        else 
        {
            GameObject.Find("Receiver").GetComponent<Receiver>().isHitByTransmitter = false;
            GameObject.Find("Mirror").GetComponent<MirrorCube>().isHitByTransmitter = false;
            PortalLaser.isBlueHitByTransmitter = false;
            PortalLaser.isOrangeHitByTransmitter = false;
        }
    }
}
