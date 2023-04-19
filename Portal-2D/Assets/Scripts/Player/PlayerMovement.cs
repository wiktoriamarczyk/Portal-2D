using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D            rigidbody;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 14f;
    private eMovementState         movementState;
    private Animator               animator;
    private float                  dirX = 0f;
    private bool                   isJumping = false;
    private bool                   isFacingRight = true;

    private enum eMovementState
    {
        IDLE,
        WALK,
        JUMP,
        FALL
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && Math.Abs(rigidbody.velocity.y) < 0.01)
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(dirX * speed, rigidbody.velocity.y);
        if (isJumping)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            isJumping = false;
        }
        AnimationUpdate(dirX);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void AnimationUpdate(float dirX)
    {
        if (dirX > 0)
        {
            // flip sprite
            if (isFacingRight)
            {
                Flip();
            }
            movementState = eMovementState.WALK;
        }
        else if (dirX < 0)
        {
            // flip sprite
            if (!isFacingRight)
            {
                Flip();
            }
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

}
