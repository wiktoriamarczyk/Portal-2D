using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIn : MonoBehaviour
{
    [SerializeField] AudioSource doorClose;
    [SerializeField] AudioSource doorLock;

    void Awake()
    {
        doorClose.Play();
        doorLock.PlayDelayed(doorClose.clip.length);
    }
}
