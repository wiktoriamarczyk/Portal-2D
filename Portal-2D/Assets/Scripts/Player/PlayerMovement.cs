using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] Transform holdPoint;

    Rigidbody2D rigidbody;
    eMovementState movementState;
    Animator       animator;
    float          dirX = 0f;
    bool           isJumping = false;
    bool           isFacingRight = true;
    GameObject     currentCube;

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

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && Math.Abs(rigidbody.velocity.y) < 0.01)
        {
            isJumping = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentCube == null)
            {
                TryPickUpCube();
            }
            else
            {
                DropCube();
            }
        }
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

    #region CUBE
    void TryPickUpCube()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        if (colliders == null)
        {
            return;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Cube") && currentCube == null)
            {
                currentCube = collider.gameObject;
                currentCube.GetComponent<Rigidbody2D>().isKinematic = true;
                currentCube.transform.position = holdPoint.position;
                currentCube.transform.SetParent(transform);
                Cube.taken = true;
                break;
            }
        }
    }

    void DropCube()
    {
        if (currentCube == null) return;
        currentCube.GetComponent<Rigidbody2D>().isKinematic = false;
        currentCube.transform.SetParent(null);
        currentCube = null;
        Cube.taken = false;
    }
    #endregion CUBE
}