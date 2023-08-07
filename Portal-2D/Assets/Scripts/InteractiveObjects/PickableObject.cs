using UnityEngine;
using TMPro;

/// <summary>
/// Class responsible for behaviour of pickable objects. It implements the IPortalEventsListener interface.
/// </summary>
public class PickableObject : MonoBehaviour , IPortalEventsListener
{
    /// <summary>
    /// Sound of object hitting the ground
    /// </summary>
    [SerializeField] protected AudioSource hittingTheGroundSound;

    /// <summary>
    /// Detection radius for interaction with the player.
    /// </summary>
    [SerializeField] protected float detectionRadius = 4f;

    TMP_Text promptText;
    /// <summary>
    /// Indicates whether the object is currently taken by the player
    /// </summary>
    protected bool taken = false;
    /// <summary>
    /// Rigidbody2D component of the object
    /// </summary>
    protected Rigidbody2D rigidbody2D;

    /// <summary>
    /// Indicates whether the prompt was displayed
    /// </summary>
    static bool promptWasDisplayed = false;

    float backupMass = 0;
    Transform attachpoint;

    /// <summary>
    /// Allows the object to be picked up and attached to a specific point.
    /// </summary>
    /// <param name="attach">The attachment 'point' (coordinates) to which the object will be sticked.</param>
    public void Take(Transform attach)
    {
        if (taken || !attach)
            return;

        backupMass = rigidbody2D.mass;
        rigidbody2D.mass = 0.008f;
        rigidbody2D.gravityScale = 0;
        taken = true;
        attachpoint = attach;
        UnityEngine.Debug.Log("Podnosze kostke (kostka)");

    }

    /// <summary>
    /// Drops the object, restoring its original properties, and detaches it from the attachment point.
    /// </summary>
    public void Drop()
    {
        if (!taken)
            return;
        rigidbody2D.mass = backupMass;
        rigidbody2D.gravityScale = 1;
        taken = false;
        attachpoint = null;
    }

    protected virtual void Start()
    {
        GameObject textObject = GameObject.FindGameObjectWithTag("PromptText");
        if (textObject != null)
        {
            promptText = textObject.GetComponent<TMP_Text>();
        }
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (attachpoint != null)
        {
            var cubePosition = transform.position;
            var targetPos    = attachpoint.transform.position;
            var diff         = (targetPos - cubePosition)*10;
            GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (taken)
        {
            promptWasDisplayed= true;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") && !taken && !promptWasDisplayed)
            {
                promptText.enabled = true;
                promptText.text = "Press   E   to  pickup";
                break;
            }
            else if (promptText.enabled)
            {
                promptText.enabled = false;
                promptText.text = "";
            }
        }
    }
    /// <summary>
    /// Method called when the collision occured
    /// </summary>
    /// <param name="collision">the object with which the collision occurred</param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (hittingTheGroundSound != null)
            hittingTheGroundSound.Play();
    }
    /// <summary>
    /// Method called when the object was teleported by portal
    /// </summary>
    /// <param name="srcPortal">source portal</param>
    /// <param name="dstPortal">destination portal</param>
    public virtual void OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal)
    {
        Drop();
    }
    public virtual void OnExitedPortalArea(PortalLogic portal)
    {
    }
}
