using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    [SerializeField] AudioSource cakeSound;
    Animator animator;
    bool activated = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void UpdatePhysics()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !activated)
        {
            activated = true;
            cakeSound.Play();
            animator.SetTrigger("CakeEating");
            Invoke("Destroy", 1.75f);
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
       // UpdatePhysics();
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
