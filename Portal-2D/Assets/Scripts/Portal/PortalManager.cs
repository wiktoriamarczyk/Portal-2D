using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [SerializeField] public GameObject objectToClone;
    [SerializeField] GameObject bluePortal;
    [SerializeField] GameObject orangePortal;
    GameObject clone;
    int colliderEnterCount = 0;
    Transform spawnPosition;

    string cloneName = "PortalClone";

    public Vector3 playerPosBeforeTeleport;

    public string cloneNameProperty
    {
        get => cloneName;
        set => cloneName = value;
    }

    void Start()
    {
        // singleton instantiation
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void CreateClone(GameObject portal, GameObject source)
    {
        colliderEnterCount++;
        if (clone)
        {
            return;
        }

        // if the signal to instatiate clone comes from blue portal, then instantiate clone at orange portal spawn point
        if (portal == bluePortal)
        {
            spawnPosition = orangePortal.transform;
        }
        else
        {
            spawnPosition = bluePortal.transform;
        }

        Vector3 yOffset = new Vector3(0, portal.transform.position.y - objectToClone.transform.position.y, 0);
        clone = Instantiate(objectToClone, spawnPosition.position - yOffset, Quaternion.identity);
       // objectToClone.GetComponent<PlayerMovement>().Flip();
        clone.gameObject.name = cloneName;
        clone.AddComponent<PlayerCloneMove>();
        clone.GetComponent<Rigidbody2D>().simulated = false;
        clone.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void Teleport()
    {
        var clonePos = clone.transform.position;
        clone.transform.position = objectToClone.transform.position;
        objectToClone.transform.position = clonePos;

        clone.GetComponent<PlayerCloneMove>().Reset();
    }

    public void DestroyClone(GameObject source)
    {
        colliderEnterCount--;
        if (colliderEnterCount == 0)
            Destroy(clone);
    }
}