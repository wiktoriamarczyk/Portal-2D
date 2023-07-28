using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : PickableObject
{
    public Sprite activeSprite;          // alive
    public Sprite inactiveSprite;        // dead
    public Sprite attackSprite;
    public LayerMask layerMask;
    public float userMass = 80f;
    public float agonyTime = 1.5f;
    public AudioSource audioSource;
    public AudioClip detected;
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


