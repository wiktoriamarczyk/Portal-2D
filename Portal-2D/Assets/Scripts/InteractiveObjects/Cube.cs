using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cube : MonoBehaviour
{
    [SerializeField] AudioSource cubeSound;
    [SerializeField] float detectionRadius = 4f;
    TMP_Text promptText;
    bool taken = false;
    Rigidbody2D rigidbody2D;
    static bool promptWasDisplayed = false;

    float backupMass = 0;

    public void Take()
    {
        if (taken)
            return;

        backupMass = rigidbody2D.mass;
        rigidbody2D.mass = 0.008f;
        rigidbody2D.gravityScale = 0;
        taken = true;
        UnityEngine.Debug.Log("Podnosze kostke (kostka)");

    }

    public void Drop()
    {
        if (!taken)
            return;
        rigidbody2D.mass = backupMass;
        rigidbody2D.gravityScale = 1;
        taken = false;
    }

    void Start()
    {
        GameObject textObject = GameObject.FindGameObjectWithTag("PromptText");
        if (textObject != null)
        {
            promptText = textObject.GetComponent<TMP_Text>();
        }
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (taken)
        {
            promptWasDisplayed= true;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") && !taken && !promptWasDisplayed)
            {
                promptText.enabled = true;
                promptText.text = "Press   E   to  pickup";
                break;
            }
            else if (promptText.enabled)
            {
                promptText.enabled = false;
                promptText.text = "";
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        cubeSound.Play();
    }


}
