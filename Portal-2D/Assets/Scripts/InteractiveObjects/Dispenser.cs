using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] AudioSource dispenserSound;
    const float offset = 12f;
    GameObject spawnedCube;
    float fade = 1;
    bool isCubeDematerializing = false;
    public void SpawnCube()
    {
        dispenserSound.PlayDelayed(1f);
        if (spawnedCube != null && !isCubeDematerializing)
        {
            isCubeDematerializing = true;
            StartCoroutine(DematerializeCube());
        }
        else if (!isCubeDematerializing)
        {
            CreateCube();
        }
    }

    void CreateCube()
    {
        Vector3 dispenserPosition = transform.position;
        Vector3 cubePosition = new Vector3(dispenserPosition.x, dispenserPosition.y + offset, dispenserPosition.z);
        spawnedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
    }

    IEnumerator DematerializeCube()
    {
        Material cubeMaterial = spawnedCube.GetComponent<SpriteRenderer>().material;
        fade -= Time.deltaTime;
        while (fade > 0)
        {
            cubeMaterial.SetFloat("_Fade", fade);
            fade -= Time.deltaTime;
            yield return null;
        }
        Destroy(spawnedCube);
        fade = 1f;
        isCubeDematerializing = false;
        CreateCube();
    }
}
