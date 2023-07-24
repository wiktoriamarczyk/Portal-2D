using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] Transform holdPoint;

    Rigidbody2D rigidbody;
    BoxCollider2D boxCollider2D;
    eMovementState movementState;
    Animator animator;

    GameObject currentCube = null;
    GameObject currentEnemy = null;
    GameObject holdingGameObj = null;

    float dirX = 0f;
    bool isJumping = false;
    bool isFacingRight = true;

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

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position - Vector3.down * 0.1f, Vector2.down, 0.3f, layerMask1 | layerMask2);
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
            if (collider.CompareTag("Cube") && !IsHoldingItem())
            {
                currentCube = collider.gameObject;
                currentCube.GetComponent<Cube>().Take();
                holdingGameObj = currentCube;
                break;
            }
            else if (collider.CompareTag("Enemy") && !IsHoldingItem())
            {
                currentEnemy = collider.gameObject;
                currentEnemy.GetComponent<Enemy>().Take();
                holdingGameObj = currentEnemy;
                break;
            }
        }
    }

    bool IsHoldingItem()
    {
        return holdingGameObj || currentCube != null || currentEnemy != null;
    }

    void DropItem()
    {
        if (!holdingGameObj) return;

        if (holdingGameObj.GetComponent<Cube>() != null)
        {
            currentCube.GetComponent<Cube>().Drop();
            currentCube = null;
            holdingGameObj = null;
        }
        else if (holdingGameObj.GetComponent<Enemy>() != null)
        {
            currentEnemy.GetComponent<Enemy>().Drop();
            currentEnemy = null;
            holdingGameObj = null;
        }

    }

    void UpdateItemsPosition()
    {
        if (currentCube != null)
        {
            var cubePosition = currentCube.transform.position;
            var targetPos = holdPoint.transform.position;
            var diff = (targetPos - cubePosition) * 10;
            currentCube.GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        }

        else if (currentEnemy != null)
        {
            var enemyPosition = currentEnemy.transform.position;
            var targetPos = holdPoint.transform.position;
            var diff = (targetPos - enemyPosition) * 10;
            currentEnemy.GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        }
    }
    #endregion ITEM
}