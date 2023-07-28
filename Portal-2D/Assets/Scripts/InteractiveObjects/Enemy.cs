using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : PickableObject
{
    public Sprite activeSprite;          // alive
    public Sprite inactiveSprite;        // dead
    public Sprite attackSprite;
    public float userMass = 80f;
    public float agonyTime = 1.5f;

    private SpriteRenderer spriteRenderer;

    bool alive = true;
    float maxTiltAngle = 45f;

    void Die()
    {
        alive = false;
        spriteRenderer.sprite = inactiveSprite;
    }

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.mass = userMass;
    }

    protected override  void Update()
    {
        base.Update();

        if (!alive) return;

        if (Mathf.Abs(transform.localRotation.eulerAngles.z) > maxTiltAngle && alive)
        {
            StartCoroutine(DieWithDelay());
        }
    }

    private IEnumerator DieWithDelay()
    {
        // wait x sec before callin
        yield return new WaitForSeconds(agonyTime);

        Die(); // no hurt feelings
    }


}


