using System.Collections;
using UnityEngine;

/// <summary>
/// Class responsible for dispensing cubes
/// </summary>
public class Dispenser : MonoBehaviour
{
    /// <summary>
    /// Prefab of the cube
    /// </summary>
    [SerializeField] GameObject cubePrefab;
    /// <summary>
    /// Sound of the dispenser
    /// </summary>
    [SerializeField] AudioSource dispenserSound;
    /// <summary>
    /// Offset of the cube from the dispenser
    /// </summary>
    const float offset = 12f;
    /// <summary>
    /// Spawned cube
    /// </summary>
    GameObject spawnedCube;
    /// <summary>
    /// Fade of the cube
    /// </summary>
    float fade = 1;
    /// <summary>
    /// Is cube dematerializing
    /// </summary>
    bool isCubeDematerializing = false;
    /// <summary>
    /// Method called when the dispenser is activated
    /// </summary>
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
    /// <summary>
    /// Method responsible for creating a cube
    /// </summary>
    void CreateCube()
    {
        Vector3 dispenserPosition = transform.position;
        Vector3 cubePosition = new Vector3(dispenserPosition.x, dispenserPosition.y + offset, dispenserPosition.z);
        spawnedCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
    }
    /// <summary>
    /// Coroutine responsible for dematerializing and destroying a cube
    /// </summary>
    /// <returns></returns>
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
