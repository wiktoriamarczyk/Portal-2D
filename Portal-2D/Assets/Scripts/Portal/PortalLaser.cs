using System;
using UnityEngine;

/// <summary>
/// Class to controll the lasers emitted by the portals
/// </summary>
public class PortalLaser : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the blue portal was hit by a laser or not.
    /// </summary>
    public static bool isBluePortalHit = false;
    /// <summary>
    /// Indicates whether the orange portal was hit by a laser or not.
    /// </summary>
    public static bool isOrangePortalHit = false;
    /// <summary>
    /// Indicates whether the blue portal was hit by a laser from transmitter or not.
    /// </summary>
    public static bool isBlueHitByTransmitter = false;
    /// <summary>
    /// Indicates whether the orange portal was hit by a laser from transmitter or not.
    /// </summary>
    public static bool isOrangeHitByTransmitter = false;
    /// <summary>
    /// Indicates whether the blue portal was hit by a laser from mirror or not.
    /// </summary>
    public static bool isBlueHitByMirror = false;
    /// <summary>
    /// Indicates whether the orange portal was hit by a laser from mirror or not.
    /// </summary>
    public static bool isOrangeHitByMirror = false;
    /// <summary>
    /// LineRenderer component - to draw the laser
    /// </summary>
    public LineRenderer lineRenderer;
    /// <summary>
    /// LayerMask component - to detect laser collisions 
    /// </summary>
    public LayerMask layerMask;
    /// <summary>
    /// Position of the 2nd portal (which emits the laser)
    /// </summary>
    private Vector3 outputPortalPosition;
    /// <summary>
    /// Rotation of the 2nd portal (which emits the laser)
    /// </summary>
    private float outputPortalRotation;
    /// <summary>
    /// A far away placed point on the line of the laser
    /// </summary>
    private Vector3 laserFarEnd;
    /// <summary>
    /// Real end of the laser indicated by the collision
    /// </summary>
    private Vector3 laserEnd;
    /// <summary>
    /// Timer to count time from the last hit
    /// </summary>
    private int timeSinceLastHit = 0;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Check if blue portal is hit
        if (isBlueHitByMirror || isBlueHitByTransmitter)
            isBluePortalHit = true;
        else
            isBluePortalHit = false;

        // Check if orange portal is hit
        if (isOrangeHitByMirror || isOrangeHitByTransmitter)
            isOrangePortalHit = true;
        else
            isOrangePortalHit = false;

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
        else {
            lineRenderer.enabled = false;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(outputPortalPosition, (laserFarEnd - outputPortalPosition).normalized, Vector3.Distance(outputPortalPosition, laserFarEnd), layerMask);
        if (hit.collider != null)
        {
            laserEnd = hit.point;
        }
        lineRenderer.SetPosition(0, outputPortalPosition);
        lineRenderer.SetPosition(1, laserEnd);
        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            GameObject.Find("LaserReceiver").GetComponent<Receiver>().isHitByMirror = false;
            if (timeSinceLastHit > 500)
            {
                timeSinceLastHit = 0;
                PlayerHurt.isHurt = true;
            }
            else timeSinceLastHit++;
        }
        else if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
            hit.collider.gameObject.GetComponent<Receiver>().isHitByMirror = true;
        else
        {
            GameObject.Find("LaserReceiver").GetComponent<Receiver>().isHitByMirror = false;
            if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
                hit.collider.gameObject.GetComponent<MirrorCube>().isHitByPortal = true;
            else
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().isHitByPortal = false;
            }
        }

    }
}
