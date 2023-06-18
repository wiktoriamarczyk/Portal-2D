using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] GameObject shotEffectPrefab;

    Vector3Int positionOnGrid;
    Vector3 endPosition;
    Vector2 wallNormal;
    eProjectileType projectileType;

    GameObject hitEffect;
    GameObject shotEffect;

    bool instantiatePortal = false;
    bool spawnHitWallParticles = false;

    Color blueColor;
    Color orangeColor;
    Color currentColor;

    Color ConvertColor(int redValue, int greenValue, int blueValue)
    {
        float red = redValue / 255f;
        float green = greenValue / 255f;
        float blue = blueValue / 255f;

        return new Color(red, green, blue);
    }


    public enum eProjectileType
    {
        BLUE,
        ORANGE
    }


    void Update()
    {
        // jeœli minê³o odpowiednio du¿o czasu, wy³¹cz pocisk
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);

        // jeœli pocisk dotar³ do celu, zespawnuj tam portal i wy³¹cz pocisk
        if (transform.position == endPosition)
        {
            if (instantiatePortal)
            {
                InstantiatePortal();
            }
            //if (spawnHitWallParticles && hitEffect == null)
            //{
            //    hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            //    ParticleSystem hitParticles = hitEffect.GetComponent<ParticleSystem>();
            //    hitParticles.startColor = projectileType == eProjectileType.BLUE ? blueColor : orangeColor;
            //    var myParticles = GetComponent<ParticleSystem>();
            //    Destroy(myParticles);
            //    StartCoroutine(DyingCoroutine());
            //}
           // else
            //{
                Destroy(gameObject);
            //}
        }

        // niech pocisk przemieszcza siê w kierunku celu
        MoveTowards();
    }

    IEnumerator DyingCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void InstantiatePortal()
    {
        bool wasPortalSpawned = false;
        if (projectileType == eProjectileType.BLUE)
            wasPortalSpawned = PortalManager.Instance.TrySpawnBluePortal(wallNormal, positionOnGrid);
        else if (projectileType == eProjectileType.ORANGE)
            wasPortalSpawned = PortalManager.Instance.TrySpawnOrangePortal(wallNormal, positionOnGrid);
        spawnHitWallParticles = !wasPortalSpawned;
    }

    public void InitializeProjectile(Vector3 endPos, eProjectileType type)
    {
        endPosition = endPos;
        projectileType = type;

        blueColor = ConvertColor(34, 226, 250);
        orangeColor = ConvertColor(255, 100, 0);

        shotEffect = Instantiate(shotEffectPrefab, transform.position, Quaternion.identity);
        ParticleSystem shotParticles = shotEffect.GetComponent<ParticleSystem>();
        shotParticles.startColor = projectileType == eProjectileType.BLUE ? blueColor : orangeColor;
    }

    public void InitializePortalProperties(Vector2 normal, Vector3Int gridPosition)
    {
        instantiatePortal = true;
        wallNormal = normal;
        positionOnGrid = gridPosition;
    }

    void MoveTowards()
    {
        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
    }

    void OnDestroy()
    {
        StopAllCoroutines();

        if (hitEffect != null)
            Destroy(hitEffect);
        if (shotEffect != null)
            Destroy(shotEffect);
    }
}
