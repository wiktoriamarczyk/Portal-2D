using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lasers : MonoBehaviour
{
    public Sprite defaultSprite;       // Domyœlny sprite
    public Sprite activatedSprite;     // Aktywowany sprite
    public Vector3 start;         // Punkty na linii lasera
    public Vector3 end;           // Punkty na linii lasera
    private LineRenderer lineRenderer; // Komponent LineRenderer
    private SpriteRenderer spriteRenderer;  // Komponent SpriteRenderer


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();     // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;
        start = transform.position;  // Pierwszy punkt to pozycja transmitera
        stopLaser();    // Wy³¹czenie lasera
        if (Math.Round(transform.rotation.z, 1) == -0.7)
        {
            end = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 0.7)
        {
            end = new Vector3(transform.position.x, transform.position.y + 7, transform.position.z);
        }
        else if (transform.rotation.z == 0.0)
        {
            end = new Vector3(transform.position.x + 7, transform.position.y, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 3.1)
        {
            end = new Vector3(transform.position.x - 7, transform.position.y, transform.position.z);
        }
    }

    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = activatedSprite;
        lineRenderer.enabled = true;    // W³¹czenie linii
    }

    public void stopLaser()
    {
        // Zmiana sprite'a na domyœlny
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;   // Wy³¹czenie linii
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            Debug.Log("Rysujê linie");
            lineRenderer.SetPosition(0, start); // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, end);   // Ustawienie drugiego punktu linii
        }
    }
}
