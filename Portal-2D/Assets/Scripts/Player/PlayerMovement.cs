using UnityEngine;

/// <summary>
/// Class responsible for managing player movement
/// </summary>
public class PlayerMovement : MonoBehaviour , IPortalEventsListener
{
    /// <summary>
    /// Speed of the player
    /// </summary>
    [SerializeField] float speed = 8f;
    /// <summary>
    /// Jump force of the player
    /// </summary>
    [SerializeField] float jumpForce = 14f;
    /// <summary>
    /// Distance to object which can be interacted with
    /// </summary>
    [SerializeField] float interactDistance = 2f;
    /// <summary>
    /// Hold point of the player
    /// </summary>
    [SerializeField] Transform holdPoint;

    Rigidbody2D    rigidbody;
    eMovementState movementState;
    Animator       animator;
    float          dirX = 0f;
    float          dirXMultiplier = 1.0f;
    bool           isJumping = false;
    bool           isFacingRight = true;
    BoxCollider2D  boxCollider2D;

    GameObject holdingGameObj = null;

    /// <summary>
    /// Property which indicates whether the player is facing right
    /// </summary>
    public bool IsFacingRight
    {
        get => isFacingRight;
        set => isFacingRight = value;
    }
    /// <summary>
    /// Types of movement
    /// </summary>
    enum eMovementState
    {
        IDLE,
        WALK,
        JUMP,
        FALL
    }
    /// <summary>
    /// Method responsible for flipping the direction of the player
    /// </summary>
    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    /// <summary>
    /// Returns the information about player movement state
    /// </summary>
    /// <returns>true if player is moving</returns>
    public bool IsMoving()
    {
        return Mathf.Abs(dirX) >= 0.1f;
    }
    /// <summary>
    /// Method responsible for inversing player movement
    /// </summary>
    public void InverseXMovementAxis()
    {
        dirXMultiplier = -1f;
    }
    /// <summary>
    /// Start is called before the first frame update. Here mainly responsible for getting components
    /// </summary>
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// Update is called once per frame. Here mainly responsible for player input handling
    /// </summary>
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal") * dirXMultiplier;
        if (dirX==0)
            dirXMultiplier = 1.0f;

        if (IsTouchingGround() && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!IsHoldingItem())
            {
                TryPickUpItem();
            }
            else
            {
                DropItem();
            }
        }
    }
    /// <summary>
    /// Return the information about player touching the ground
    /// </summary>
    /// <returns>true if player is touching the ground</returns>
    public bool IsTouchingGround()
    {
        var layerMask1 = LayerMask.GetMask("Units");
        var layerMask2 = LayerMask.GetMask("Terrain");
        RaycastHit2D hit = Physics2D.Raycast( boxCollider2D.bounds.center - new Vector3(0,boxCollider2D.bounds.extents.y) - Vector3.down * 0.1f, Vector2.down, 0.3f , layerMask1 | layerMask2 );
        if (hit.collider != null)
        {
            //Debug.DrawLine(transform.position, hit.point, Color.green);
            return true;
        }
        //Debug.DrawLine(transform.position - Vector3.down * 0.3f, transform.position + Vector3.down );
        return false;
    }
    /// <summary>
    /// Fixed Update is called every fixed framerate frame. Here mainly
    /// responsible for managing player movement according to player input
    /// </summary>
    void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(dirX * speed, rigidbody.velocity.y);
        if (isJumping)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            isJumping = false;
        }
        AnimationUpdate(dirX);
    }
    /// <summary>
    /// Method responsible for managing player animation
    /// </summary>
    /// <param name="dirX">direction of the player</param>
    void AnimationUpdate(float dirX)
    {
        if (dirX > 0 || dirX < 0)
        {
            movementState = eMovementState.WALK;
        }
        else
        {
            movementState = eMovementState.IDLE;
        }

        if (rigidbody.velocity.y > 0.1f)
        {
            movementState = eMovementState.JUMP;
        }
        else if (rigidbody.velocity.y < -0.1f)
        {
            movementState = eMovementState.FALL;
        }

        animator.SetInteger("state", (int)movementState);
    }


    #region ITEM
    void TryPickUpItem()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        if (colliders == null) return;

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PickableObject>()!=null && !IsHoldingItem())
            {
                holdingGameObj = collider.gameObject;
                holdingGameObj.GetComponent<PickableObject>().Take(holdPoint);
                break;
            }
        }
    }

    bool IsHoldingItem()
    {
        return holdingGameObj != null;
    }

    public void OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal)
    {
        if (Mathf.Abs(Vector3.Angle(srcPortal.GetWorldVectorToPortal(), dstPortal.GetWorldVectorToPortal())) < 90)
            InverseXMovementAxis();

        DropItem();
    }

    public void OnExitedPortalArea(PortalLogic portal)
    {
    }

    void DropItem()
    {
        if (!holdingGameObj) return;

        if (holdingGameObj.GetComponent<PickableObject>() != null)
        {
            holdingGameObj.GetComponent<PickableObject>().Drop();
            holdingGameObj = null;
        }
    }
    #endregion ITEM
}