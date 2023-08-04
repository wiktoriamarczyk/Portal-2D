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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !activated)
        {
            activated = true;
            cakeSound.Play();
            animator.SetTrigger("CakeEating");
            Invoke("Destroy", 1.75f);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
