using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PortalLaser : MonoBehaviour
{
    public static bool isBluePortalHit = false;
    public static bool isOrangePortalHit = false;
    public LineRenderer lineRenderer;
    private Vector3 outputPortalPosition;
    private float outputPortalRotation;
    private Vector3 laserFarEnd;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        // Ustaw kolor na czerwony
        lineRenderer.material.color = Color.red;
        // Ustaw szerokoœæ na 0.1 jednostki
        lineRenderer.startWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBluePortalHit)
        {
            Debug.Log("Jestem tu!");
            lineRenderer.enabled = true;
            outputPortalPosition = GameObject.FindGameObjectsWithTag("Orange Portal")[0].transform.position;
            outputPortalRotation = GameObject.FindGameObjectsWithTag("Orange Portal")[0].transform.rotation.eulerAngles.z;
            lineRenderer.SetPosition(0, outputPortalPosition);
            laserFarEnd = new Vector3(outputPortalPosition.x + 100, outputPortalPosition.y, outputPortalPosition.z);
            lineRenderer.SetPosition(1, laserFarEnd);
        }
        else if (isOrangePortalHit)
        {
            lineRenderer.enabled = true;
            outputPortalPosition = GameObject.FindGameObjectsWithTag("Blue Portal")[0].transform.position;
            outputPortalRotation = GameObject.FindGameObjectsWithTag("Blue Portal")[0].transform.rotation.eulerAngles.z;
            lineRenderer.SetPosition(0, outputPortalPosition);
            if (Math.Round(outputPortalRotation, 1) == 0.0)
                laserFarEnd = new Vector3(outputPortalPosition.x - 100, outputPortalPosition.y, outputPortalPosition.z);
            else if (Math.Round(outputPortalRotation, 1) == 0.7)
                laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y - 100, outputPortalPosition.z);
            else if (Math.Round(outputPortalRotation, 1) == -0.7)
                laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y + 100, outputPortalPosition.z);
            else laserFarEnd = new Vector3(outputPortalPosition.x + 100, outputPortalPosition.y, outputPortalPosition.z);
            lineRenderer.SetPosition(1, laserFarEnd);
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }
}
