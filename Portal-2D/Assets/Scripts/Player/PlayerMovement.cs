using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour , IPortalEventsListener
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] Transform holdPoint;

    Rigidbody2D    rigidbody;
    eMovementState movementState;
    Animator       animator;
    float          dirX = 0f;
    float          dirXMultiplier = 1.0f;
    float          cubeSavedMass = 0;
    bool           isJumping = false;
    bool           isFacingRight = true;
    BoxCollider2D  boxCollider2D;

    GameObject holdingGameObj = null;


    public bool IsFacingRight
    {
        get => isFacingRight;
        set => isFacingRight = value;
    }

    enum eMovementState
    {
        IDLE,
        WALK,
        JUMP,
        FALL
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(dirX) >= 0.1f;
    }
    public void InverseXMovementAxis()
    {
        dirXMultiplier = -1f;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

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

        if (IsHoldingItem())
        {
            UpdateItemsPosition();
        }

    }

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
            if (collider.GetComponent<Cube>()!=null && !IsHoldingItem())
            {
                holdingGameObj = collider.gameObject;
                holdingGameObj.GetComponent<Cube>().Take(holdPoint);
                break;
            }
        }
    }

    bool IsHoldingItem()
    {
        return holdingGameObj != null;
    }

    public void OnTeleported(PortalCloner srcPortal, PortalCloner dstPortal)
    {
        if (Mathf.Abs(Vector3.Angle(srcPortal.GetWorldVectorToPortal(), dstPortal.GetWorldVectorToPortal())) < 90)
            InverseXMovementAxis();

        DropItem();
    }

    public void OnExitedPortalArea(PortalCloner portal)
    {
    }

    void DropItem()
    {
        if (!holdingGameObj) return;

        if (holdingGameObj.GetComponent<Cube>() != null)
        {
            holdingGameObj.GetComponent<Cube>().Drop();
            holdingGameObj = null;
        }
    }

    void UpdateItemsPosition()
    {
        if (holdingGameObj.GetComponent<Cube>() != null)
            return;

        //if (holdingGameObj != null)
        //{
        //    var enemyPosition = holdingGameObj.transform.position;
        //    var targetPos = holdPoint.transform.position;
        //    var diff = (targetPos - enemyPosition) * 10;
        //    holdingGameObj.GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        //}
    }
    #endregion ITEM
}