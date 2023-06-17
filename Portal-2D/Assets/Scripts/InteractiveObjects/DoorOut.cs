using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOut : MonoBehaviour
{
    bool isActive = false;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        isActive = true;
        animator.SetTrigger("OpenDoor");
    }

    public void CloseDoor()
    {
        isActive = false;
        animator.SetTrigger("CloseDoor");
    }
}
