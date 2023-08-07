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
    /// Counts time since the last time the player was hit
    /// </summary>
    private int timeSinceLastHit = 0;


    /// <summary>
    /// Method called before the first frame
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        lineRenderer = GetComponent<LineRenderer>();     
        lineRenderer.positionCount = 2;                  
        start = transform.position;                      
        stopLaser();                                     
        // Setting the correct end point of the laser (depending on the rotation of the transmitter)
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
        lineRenderer.enabled = true;
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
        isActive = false;
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;
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
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, realEnd);   
            if (hit.collider != null && hit.collider.gameObject.tag == "Player")
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().isHitByTransmitter = false;
                if (timeSinceLastHit > 500)
                {
                    timeSinceLastHit = 0;
                    PlayerHurt.isHurt = true;
                }
                else timeSinceLastHit++;
            }
            else if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
            {
                hit.collider.gameObject.GetComponent<MirrorCube>().isHitByTransmitter = true;
                timeSinceLastHit = 0;
            }
            else
            {
                timeSinceLastHit = 0;
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
            GameObject.Find("LaserReceiver").GetComponent<Receiver>().isHitByTransmitter = false;
            GameObject.Find("Mirror").GetComponent<MirrorCube>().isHitByTransmitter = false;
            PortalLaser.isBlueHitByTransmitter = false;
            PortalLaser.isOrangeHitByTransmitter = false;
        }
    }
}
