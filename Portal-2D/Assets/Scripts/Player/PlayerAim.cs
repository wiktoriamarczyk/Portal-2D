using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class responsible for the behaviour of the player's aim
/// </summary>
public class PlayerAim : MonoBehaviour
{
    /// <summary>
    /// Array of cursor textures
    /// </summary>
    [SerializeField] Texture2D[] cursorTextures;
    /// <summary>
    /// Arm of the player
    /// </summary>
    [SerializeField] Transform   arm;
    /// <summary>
    /// Spawner of the projectiles
    /// </summary>
    [SerializeField] Transform   projectileSpawner;
    /// <summary>
    /// Prefab of the blue projectile spawned after left mouse button click
    /// </summary>
    [SerializeField] GameObject  blueProjectile;
    /// <summary>
    /// Prefab of the orange projectile spawned after right mouse button click
    /// </summary>
    [SerializeField] GameObject  orangeProjectile;
    /// <summary>
    /// Sound of firing left mouse button
    /// </summary>
    [SerializeField] AudioSource lpmFireSound;
    /// <summary>
    /// Sound of firing right mouse button
    /// </summary>
    [SerializeField] AudioSource ppmFireSound;
    /// <summary>
    /// Debug angle of the arm
    /// </summary>
    [SerializeField] float debugangle;
    /// <summary>
    /// PlayerMovement component
    /// </summary>
    PlayerMovement player;
    PortalAdapter portalAdapter;
    /// <summary>
    /// CursorMode of the cursor
    /// </summary>
    CursorMode cursorMode = CursorMode.ForceSoftware;
    /// <summary>
    /// Hot spot of the cursor
    /// </summary>
    Vector2 hotSpot = Vector2.zero;
    /// <summary>
    /// Tilemap of the level
    /// </summary>
    Tilemap tilemap;
    /// <summary>
    /// Type of the cursor
    /// </summary>
    eCursorType cursor = eCursorType.ORANGE;
    /// <summary>
    /// Minimum angle of the arm
    /// </summary>
    float minAngleRange = -60f;
    /// <summary>
    /// Maximum angle of the arm
    /// </summary>
    float maxAngleRange = 70f;
    /// <summary>
    /// Types of cursors
    /// </summary>
    enum eCursorType
    {
        BLUE,
        ORANGE
    }
    /// <summary>
    /// Layers that are not taken into account by raycast
    /// </summary>
    const int NON_RAYCAST_LAYERS = (int)Common.eLayerType.PLAYER |  (int)Common.eLayerType.NON_COLLIDABLE_UNITS;

    /// <summary>
    /// Start is called when the script instance is being loaded - responsible for initializing the variables
    /// </summary>
    void Start()
    {
        tilemap = PortalManager.Instance.TilemapProperty;
        ChangeCursor(eCursorType.BLUE);
        player = GetComponent<PlayerMovement>();
        portalAdapter = GetComponent<PortalAdapter>();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled. Responsible for managing the behaviour of the player input -
    /// rotates the arm according to the mouse position and spawn projectiles on mouse button click
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ChangeCursor(eCursorType.BLUE);
            Fire();
            lpmFireSound.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ChangeCursor(eCursorType.ORANGE);
            Fire();
            ppmFireSound.Play();
        }

        RotateArm();
    }
    public PickableObject FindPickCandidate()
    {
        var layerMask1 = LayerMask.GetMask("Units");
        var layerMask2 = LayerMask.GetMask("Terrain");
        var layerMask3 = LayerMask.GetMask("MirrorCube");
        var layerMask4 = LayerMask.GetMask("Non-portal");

        // ostatnim parametrem są warstwy brane pod uwagę przez raycast za wyjątkiem warstwy, na której jest gracz
        RaycastHit2D hit = Physics2D.Raycast(arm.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position, 4f, layerMask1 | layerMask2 | layerMask3 | layerMask4 );
        if (hit != true || hit.collider == null)
            return null;

        return hit.collider.GetComponent<PickableObject>();
    }
    /// <summary>
    /// Method responsible for firing the projectile after mouse button click
    /// </summary>
    void Fire()
    {
        if (portalAdapter!=null && portalAdapter.IsInPortalArea())
            return;
        // ostatnim parametrem są warstwy brane pod uwagę przez raycast za wyjątkiem warstwy, na której jest gracz
        RaycastHit2D hit = Physics2D.Raycast(arm.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position, 100f, ~((int)(NON_RAYCAST_LAYERS)));
        int hitLayer = -1;
        if (hit == true)
            hitLayer = hit.transform.gameObject.layer;

        if (hit.collider != null)
        {
            // ----DEBUG----
            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(arm.position, hit.point, UnityEngine.Color.red, 5f);
            Debug.DrawLine(hit.point, hit.point + hit.normal*5, UnityEngine.Color.green , 5f);

            Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(hit.point + hit.normal * -0.1f);
            Debug.Log("hit on grid pos: " + cellPosition);

            GameObject projectile;
            if (cursor == eCursorType.BLUE)
            {
                projectile = Instantiate(blueProjectile, projectileSpawner.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.BLUE);
                //projectile.GetComponent<Projectile>(). = 14;
            }
            else
            {
                projectile = Instantiate(orangeProjectile, projectileSpawner.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().InitializeProjectile(hit.point, Projectile.eProjectileType.ORANGE);

            }

            if (hit == true && hitLayer != (int)Common.eLayerType.NON_PORTAL && hitLayer != (int)Common.eLayerType.UNITS)
                projectile.GetComponent<Projectile>().InitializePortalProperties(hit.normal, cellPosition);
        }
    }
    /// <summary>
    /// Changes cursor to the one specified in the parameter
    /// </summary>
    /// <param name="cursorType">cursor type</param>
    void ChangeCursor(eCursorType cursorType)
    {
        if (cursor == cursorType)
            return;
        cursor = cursorType;
        Cursor.SetCursor(cursorTextures[(int)cursorType], hotSpot, cursorMode);
    }
    /// <summary>
    ///  Method responsible for rotating the arm according to the position of the cursor
    /// </summary>
    void RotateArm()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 ArmToMouse = mousePos - arm.position;

        //Vector3 perpendicular = arm.position + mousePos;
        //Quaternion value = Quaternion.LookRotation(Vector3.forward, perpendicular);
        //value *= Quaternion.Euler(0, 0, 90);

        float angle = GetAngleFromVec( new Vector3(Mathf.Abs(ArmToMouse.x), ArmToMouse.y , ArmToMouse.z) );
        Quaternion value = Quaternion.Euler(0f, 0f, angle );
        arm.rotation = value;

        debugangle = angle;
        // ogranicz zakres obrotu ramienia
        angle = ModularClamp( debugangle , minAngleRange , maxAngleRange );

        var inverse = 1f;
        if( !player.IsFacingRight )
            inverse = -1f;

        arm.rotation = Quaternion.Euler( 0 , 0 , inverse * angle );

        if( !player.IsFacingRight && mousePos.x > player.transform.position.x )
            player.Flip();
        else if( player.IsFacingRight && mousePos.x < player.transform.position.x )
            player.Flip();
    }
    /// <summary>
    /// Returns the modular clamp of the value between min and max
    /// </summary>
    /// <param name="value">value to be clamped</param>
    /// <param name="min">minimum value</param>
    /// <param name="max">maximum value</param>
    /// <returns></returns>
    float ModularClamp(float value, float min, float max)
    {
        var modulus = 360f;
        value %= modulus;
        return Mathf.Clamp(value, min, max);
    }
    /// <summary>
    /// Returns angle of the vector
    /// </summary>
    /// <param name="dir">direction vector</param>
    /// <returns></returns>
    static float GetAngleFromVec(Vector3 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}