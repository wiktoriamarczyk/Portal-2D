using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class to control the laser receiver.
/// </summary>
public class Receiver : MonoBehaviour
{
    /// <summary>
    /// Event called when laser hits receiver
    /// </summary>
    [SerializeField] UnityEvent onReceiverHit;
    /// <summary>
    /// Event called when laser is released from receiver
    /// </summary>
    [SerializeField] UnityEvent onReceiverReleased;
    /// <summary>
    /// Sprite renderer component - for changing the sprites
    /// </summary>
    private SpriteRenderer spriteRenderer;
    /// <summary>
    /// Default sprite - for when the receiver is not active
    /// </summary>
    public Sprite defaultSprite;
    /// <summary>
    /// Activated sprite - for when the receiver is hit by the laser
    /// </summary>
    public Sprite activatedSprite;
    /// <summary>
    /// True if the receiver was hit from any source
    /// </summary>
    public bool isHitByLaser = true;
    /// <summary>
    /// True if the receiver was hit directly from the transmitter
    /// </summary>
    public bool isHitByTransmitter = false;
    /// <summary>
    /// True if the receiver was hit from the mirror cube
    /// </summary>
    public bool isHitByMirror = false;
    /// <summary>
    /// True if the receiver was hit from a portal
    /// </summary>
    public bool isHitByPortal = false;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //  If the laser is on and the receiver is hit by a transmitter, mirror, or portal, then the receiver will turn on.
        if (Lasers.isActive && (isHitByTransmitter || isHitByMirror || isHitByPortal))
        {
            if (isHitByLaser)
                return;
            spriteRenderer.sprite = activatedSprite;
            isHitByLaser = true;
            onReceiverHit?.Invoke();
        }
        else
        {
            if (!isHitByLaser)
                return;
            spriteRenderer.sprite = defaultSprite;
            isHitByLaser = false;
            onReceiverReleased?.Invoke();
        }
    }
}
