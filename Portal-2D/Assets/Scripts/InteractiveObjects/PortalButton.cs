using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    Animator animator;
    [SerializeField] UnityEvent onButtonPressed;
    [SerializeField] UnityEvent onButtonReleased;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        animator.SetTrigger("ButtonPressed");
        onButtonPressed.Invoke();
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetTrigger("ButtonReleased");
        onButtonReleased.Invoke();
    }
}
