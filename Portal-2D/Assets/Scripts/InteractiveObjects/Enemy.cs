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

    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;
    Vector3 laserend;
    bool alive = true;
    float maxTiltAngle = 45f;
    float timeSine = 0f;
    
    void Die()
    {
        alive = false;
        spriteRenderer.sprite = inactiveSprite;
    }

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.mass = userMass;
        audioSource = GetComponent<AudioSource>();
    }

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
        // Narysuj laser (celownik)
        laserend = transform.position;
        laserend.x += 10f;
        laserend.y += 2*Mathf.Sin(timeSine);
        timeSine += 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, laserend);
        lineRenderer.enabled = true;
        // SprawdŸ, czy gracz jest w zasiêgu lasera
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (laserend - transform.position).normalized, Vector3.Distance(transform.position, laserend), layerMask);
        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            // odtwórz dŸwiêk
            if (!audioSource.isPlaying) audioSource.PlayOneShot(detected);
        }


    }

    private IEnumerator DieWithDelay()
    {
        if (!audioSource.isPlaying) audioSource.PlayOneShot(disabled);
        // wait x sec before callin
        yield return new WaitForSeconds(agonyTime);
        Die(); // no hurt feelings
    }


}


