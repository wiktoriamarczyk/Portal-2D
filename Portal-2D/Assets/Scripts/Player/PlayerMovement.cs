using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D            rigidbody;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 14f;
    private eMovementState         movementState;
    private Animator               animator;

    private enum eMovementState
    {
        IDLE,
        WALK,
        JUMP,
        FALL
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");
        rigidbody.velocity = new Vector2(dirX * speed, rigidbody.velocity.y);

        if (Input.GetButtonDown("Jump") && Math.Abs(rigidbody.velocity.y) < 0.01)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
        }

        AnimationUpdate(dirX);
    }

    void AnimationUpdate(float dirX)
    {
        if (dirX > 0)
        {
            // flip sprite
            transform.localScale = new Vector3(-1, 1, 1);
            movementState = eMovementState.WALK;
        }
        else if (dirX < 0)
        {
            // flip sprite
            transform.localScale = new Vector3(1, 1, 1);
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
