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
    Vector3 endPosition;
    eProjectileType projectileType;

    Vector2 wallNormal;
    Vector3Int positionOnGrid;

    public enum eProjectileType
    {
        BLUE,
        ORANGE
    }

    void Update()
    {
        bool portalSpawned = false;

        // jeœli minê³o odpowiednio du¿o czasu, wy³¹cz pocisk
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);

        // jeœli pocisk dotar³ do celu, zespawnuj tam portal i wy³¹cz pocisk
        if (transform.position == endPosition)
        {
            if (projectileType == eProjectileType.BLUE)
                portalSpawned = PortalManager.Instance.TrySpawnBluePortal(wallNormal, positionOnGrid);
            else if (projectileType == eProjectileType.ORANGE)
                portalSpawned = PortalManager.Instance.TrySpawnOrangePortal(wallNormal, positionOnGrid);

            //if (portalSpawned)
            //    TODO FANCY PARTICLES
            //else
            //    Destroy();

            Destroy(gameObject);
        }

        // niech pocisk przemieszcza siê w kierunku celu
        MoveTowards();
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public void InitializeProjectile(Vector3 endPos, eProjectileType type)
    {
        endPosition = endPos;
        projectileType = type;
    }

    public void InitializePortalProperties(Vector2 normal, Vector3Int gridPosition)
    {
        wallNormal = normal;
        positionOnGrid = gridPosition;
    }

    void MoveTowards()
    {
        var step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}
