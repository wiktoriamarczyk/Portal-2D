using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    Animator animator;
    [SerializeField] UnityEvent onButtonPressed;
    [SerializeField] UnityEvent onButtonReleased;
    [SerializeField] AudioSource buttonPressed;
    [SerializeField] AudioSource buttonReleased;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        animator.SetTrigger("ButtonPressed");
        onButtonPressed.Invoke();
        buttonPressed.Play();
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetTrigger("ButtonReleased");
        onButtonReleased.Invoke();
        buttonReleased.Play();
    }
}
