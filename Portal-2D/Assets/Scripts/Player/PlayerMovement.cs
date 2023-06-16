using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 14f;

    Rigidbody2D    rigidbody;
    eMovementState movementState;
    Animator       animator;
    float          dirX = 0f;
    bool           isJumping = false;
    bool           isFacingRight = true;

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
            // flip sprite
            //if (!isFacingRight)
            //{
            //    Flip();
            //}
            movementState = eMovementState.WALK;
        }
        //else if (dirX < 0)
        //{
        //    // flip sprite
        //    if (isFacingRight)
        //    {
        //        Flip();
        //    }
        //    movementState = eMovementState.WALK;
        //}
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