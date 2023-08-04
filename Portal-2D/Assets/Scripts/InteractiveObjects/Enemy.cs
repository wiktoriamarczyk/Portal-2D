using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class representing an enemy - turret. It handles updating its sprites based on its state (alive/dead),
/// tracks the player to kill, and allows the user to eliminate it from the scene.
/// </summary>
public class Enemy : PickableObject
{
    /// <summary>
    /// Sprite to display when the turret is alive.
    /// </summary>
    public Sprite activeSprite;

    /// <summary>
    /// Sprite to display when the turret is dead.
    /// </summary>
    public Sprite inactiveSprite;

    /// <summary>
    /// Sprite to display when the turret is attacking.
    /// </summary>
    public Sprite attackSprite;

    /// <summary>
    /// Layer mask for collision detection.
    /// </summary>
    public LayerMask layerMask;

    /// <summary>
    /// Mass of the turret for physics calculations.
    /// </summary>
    public float userMass = 80f;

    /// <summary>
    /// Time in seconds before the turret is considered dead after being disabled.
    /// </summary>
    public float agonyTime = 1.5f;

    /// <summary>
    /// AudioSource component for playing sounds.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Sound clip to play when the turret is detected.
    /// </summary>
    public AudioClip detected;

    /// <summary>
    /// Sound clip to play when the turret is disabled.
    /// </summary>
    public AudioClip disabled;

    /// <summary>
    /// Sound clip to play when the turret is shooting;
    /// </summary>
    public AudioClip shoot;

    /// <summary>
    /// True if the player was previously detected by the turret.
    /// </summary>
    public bool wasPlayerDetected = false;

    /// <summary>
    /// LineRenderer component - for drawing the laser
    /// </summary>
    private LineRenderer lineRenderer;

    /// <summary>
    /// SpriteRenderer component - for changing sprites
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Final point of the laser
    /// </summary>
    Vector3 laserend;

    /// <summary>
    /// True if the turret wasn't killed
    /// </summary>
    bool alive = true;

    /// <summary>
    /// Maximum angle untill tampering is detected
    /// </summary>
    float maxTiltAngle = 45f;

    /// <summary>
    /// Auxiliary variable to count time
    /// </summary>
    float timeSine = 0f;
    
    /// <summary>
    /// Changes the status and sprite of the turret
    /// </summary>
    private void Die()
    {
        alive = false;
        spriteRenderer.sprite = inactiveSprite;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.mass = userMass;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    protected override  void Update()
    {
        base.Update();

        if (!alive)
        {
            lineRenderer.enabled = false;
            return;
        }
        if (Mathf.Abs(transform.localRotation.eulerAngles.z) > maxTiltAngle && alive)
        {
            StartCoroutine(DieWithDelay());
        }

        if (wasPlayerDetected)
        {
            // draw a line from the turret to the player
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            playerPosition.y += 2f;
            lineRenderer.SetPosition(1, playerPosition);
            lineRenderer.enabled = true;
            // check if the player is in the line of sight
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, (playerPosition - transform.position).normalized, 10f, layerMask);
            if (hit2.collider != null && hit2.collider.gameObject.tag == "Player")
            {
                if (!audioSource.isPlaying) audioSource.PlayOneShot(shoot);
            }
            else wasPlayerDetected = false;
        }
        if (!wasPlayerDetected)
        {
            laserend = transform.position;
            laserend.x += 10f;
            laserend.y += 2 * Mathf.Sin(timeSine);
            timeSine += 0.02f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, laserend);
            lineRenderer.enabled = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (laserend - transform.position).normalized, Vector3.Distance(transform.position, laserend), layerMask);
            if (hit.collider != null && hit.collider.gameObject.tag == "Player")
            {
                // odtwórz dŸwiêk
                if (!audioSource.isPlaying) audioSource.PlayOneShot(detected);
                wasPlayerDetected = true;
            }
        }
    }

    /// <summary>
    /// Begins the procedure of dying
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator DieWithDelay()
    {
        if (!audioSource.isPlaying) audioSource.PlayOneShot(disabled);
        // wait x sec before callin
        yield return new WaitForSeconds(agonyTime);
        Die(); // no hurt feelings
    }


}


