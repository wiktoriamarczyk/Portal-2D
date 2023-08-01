using UnityEngine;

/// <summary>
/// Class representing a projectile
/// </summary>
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Speed of the projectile
    /// </summary>
    [SerializeField] float speed = 10f;
    /// <summary>
    /// Time after which the projectile will be destroyed
    /// </summary>
    [SerializeField] float lifeTime = 5f;
    /// <summary>
    /// Prefab of the hit effect particle system
    /// </summary>
    [SerializeField] GameObject hitEffectPrefab;
    /// <summary>
    /// Prefab of the shot effect particle system
    /// </summary>
    [SerializeField] GameObject shotEffectPrefab;

    Vector3Int positionOnGrid;
    Vector3 endPosition;
    Vector2 wallNormal;
    eProjectileType projectileType;

    GameObject hitEffect;
    GameObject shotEffect;

    bool isAlive = true;
    bool instantiatePortal = false;
    bool spawnHitWallParticles = false;

    Color blueColor;
    Color orangeColor;
    Color currentColor;

    /// <summary>
    /// Method responsible for changing the color of the projectile
    /// </summary>
    /// <param name="redValue">value of the rec color</param>
    /// <param name="greenValue">value of the green color</param>
    /// <param name="blueValue">value of the blue color</param>
    /// <returns>projectile's new color</returns>
    Color ConvertColor(int redValue, int greenValue, int blueValue)
    {
        float red = redValue / 255f;
        float green = greenValue / 255f;
        float blue = blueValue / 255f;

        return new Color(red, green, blue);
    }
    /// <summary>
    /// Projectile types
    /// </summary>
    public enum eProjectileType
    {
        BLUE,
        ORANGE
    }
    /// <summary>
    /// Update is called every frame. Here mainly responsible for moving the projectile towards the target and managing its lifetime
    /// </summary>
    void Update()
    {
        if(!isAlive)
            return;

        // jeœli minê³o odpowiednio du¿o czasu, wy³¹cz pocisk
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            isAlive = false;
            Destroy(gameObject);
        }

        // jeœli pocisk dotar³ do celu, zespawnuj tam portal i wy³¹cz pocisk
        if ( Vector3.Distance(transform.position,endPosition)<0.01f )
        {
            isAlive = false;
            if (instantiatePortal)
            {
                Destroy(gameObject);
                InstantiatePortal();
            }
        }

        MoveTowards();
    }
    /// <summary>
    /// Instantiates portal
    /// </summary>
    void InstantiatePortal()
    {
        bool wasPortalSpawned = false;
        if (projectileType == eProjectileType.BLUE)
            wasPortalSpawned = PortalManager.Instance.TrySpawnBluePortal(wallNormal, positionOnGrid);
        else if (projectileType == eProjectileType.ORANGE)
            wasPortalSpawned = PortalManager.Instance.TrySpawnOrangePortal(wallNormal, positionOnGrid);
        spawnHitWallParticles = !wasPortalSpawned;
    }
    /// <summary>
    /// Initializes projectile
    /// </summary>
    /// <param name="endPos">target position</param>
    /// <param name="type">type of the projectile</param>
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
    /// <summary>
    /// Initializes portal properties
    /// </summary>
    /// <param name="normal">normal vector</param>
    /// <param name="gridPosition">position on grid on which portal will be spawned</param>
    public void InitializePortalProperties(Vector2 normal, Vector3Int gridPosition)
    {
        instantiatePortal = true;
        wallNormal = normal;
        positionOnGrid = gridPosition;
    }
    /// <summary>
    /// Method responsible for moving the projectile towards the target
    /// </summary>
    void MoveTowards()
    {
        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
    }
    /// <summary>
    /// Method called when projectile dies
    /// </summary>
    void OnDestroy()
    {
        StopAllCoroutines();

        if (hitEffect != null)
            Destroy(hitEffect);
        if (shotEffect != null)
            Destroy(shotEffect);
    }
}
