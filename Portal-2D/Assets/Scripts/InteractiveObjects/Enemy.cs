using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : Cube
{
    public Sprite activeSprite;          // alive
    public Sprite inactiveSprite;        // dead
    public Sprite attackSprite;
    public float userMass = 80f;
    public float agonyTime = 1.5f;
    //public Transform GameObject;

    private SpriteRenderer spriteRenderer;
    //Rigidbody2D rigidbody2D;

    //bool taken = false;
    bool alive = true;
    float maxTiltAngle = 45f;
    //float backupMass = 0;

    //public void Take()
    //{
    //    if (taken)      return;

    //    backupMass = rigidbody2D.mass;
    //    rigidbody2D.mass = 0.08f;
    //    rigidbody2D.gravityScale = 0;
    //    taken = true;
    //}

    //public void Drop()
    //{
    //    if (!taken)     return;
    //    rigidbody2D.mass = backupMass;
    //    rigidbody2D.gravityScale = 1;
    //    taken = false;
    //}

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


