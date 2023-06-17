using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    float offset = 5f;
    GameObject spawnedCube;

    public void SpawnCube()
    {
        if (spawnedCube != null)
        {
            Destroy(spawnedCube);
        }

        Vector3 dispenserPosition = transform.position;
        Vector3 cubePosition = new Vector3(dispenserPosition.x, dispenserPosition.y + offset, dispenserPosition.z);

        spawnedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
    }
}
