using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOut : MonoBehaviour
{
    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource doorClose;
    [SerializeField] AudioSource levelWinning;

    public static bool isActive = false;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        Debug.Log("Otwieram drzwi");
        isActive = true;
        animator.SetTrigger("OpenDoor");
        doorOpen.Play();
    }

    public void CloseDoor()
    {
        Debug.Log("Zamykam drzwi");
        isActive = false;
        animator.SetTrigger("CloseDoor");
        doorClose.Play();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isActive)
        {
            levelWinning.Play();
            Invoke("LoadNextLevel", 2f);

        }
    }

    void LoadNextLevel()
    {
        PanelManager.Instance.LoadNextLevel();
    }
}
