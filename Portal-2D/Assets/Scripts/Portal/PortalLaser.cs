using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class PortalLaser : MonoBehaviour
{
    public static bool isBluePortalHit = false;
    public static bool isOrangePortalHit = false;
    public LineRenderer lineRenderer;
    public LayerMask layerMask;
    private Vector3 outputPortalPosition;
    private float outputPortalRotation;
    private Vector3 laserFarEnd;
    private Vector3 laserEnd;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBluePortalHit)
        {
            if (GameObject.FindGameObjectsWithTag("Orange Portal").Length > 0)
            {
                lineRenderer.enabled = true;
                outputPortalPosition = GameObject.FindGameObjectsWithTag("Orange Portal")[0].transform.position;
                outputPortalRotation = GameObject.FindGameObjectsWithTag("Orange Portal")[0].transform.rotation.eulerAngles.z;
                lineRenderer.SetPosition(0, outputPortalPosition);
                if (Math.Round(outputPortalRotation, 1) == 0.0)
                    laserFarEnd = new Vector3(outputPortalPosition.x - 100, outputPortalPosition.y, outputPortalPosition.z);
                else if (Math.Round(outputPortalRotation, 1) == 90.0)
                    laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y - 100, outputPortalPosition.z);
                else if (Mathf.Round(outputPortalRotation) == 270)
                    laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y + 100, outputPortalPosition.z);
                else laserFarEnd = new Vector3(outputPortalPosition.x + 100, outputPortalPosition.y, outputPortalPosition.z);
                lineRenderer.SetPosition(1, laserFarEnd);
            }
        }
        else if (isOrangePortalHit)
        {
            if (GameObject.FindGameObjectsWithTag("Blue Portal").Length > 0)
            {
                lineRenderer.enabled = true;
                outputPortalPosition = GameObject.FindGameObjectsWithTag("Blue Portal")[0].transform.position;
                outputPortalRotation = GameObject.FindGameObjectsWithTag("Blue Portal")[0].transform.rotation.eulerAngles.z;
                lineRenderer.SetPosition(0, outputPortalPosition);
                if (Math.Round(outputPortalRotation, 1) == 0.0)
                    laserFarEnd = new Vector3(outputPortalPosition.x - 100, outputPortalPosition.y, outputPortalPosition.z);
                else if (Math.Round(outputPortalRotation, 1) == 90.0)
                    laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y - 100, outputPortalPosition.z);
                else if (Mathf.Round(outputPortalRotation) == 270)
                    laserFarEnd = new Vector3(outputPortalPosition.x, outputPortalPosition.y + 100, outputPortalPosition.z);
                else laserFarEnd = new Vector3(outputPortalPosition.x + 100, outputPortalPosition.y, outputPortalPosition.z);
                lineRenderer.SetPosition(1, laserFarEnd);
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }

        RaycastHit2D hit = Physics2D.Raycast(outputPortalPosition, (laserFarEnd - outputPortalPosition).normalized, Vector3.Distance(outputPortalPosition, laserFarEnd), layerMask);
        if (hit.collider != null)
        {
            laserEnd = hit.point;
        }
        lineRenderer.SetPosition(0, outputPortalPosition); // Ustawienie pierwszego punktu linii
        lineRenderer.SetPosition(1, laserEnd);   // Ustawienie drugiego punktu linii
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Mirror")
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().startLaser();
            }
        }

    }
}