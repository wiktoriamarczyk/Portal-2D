using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D            rigidbody;
    private SpriteRenderer         sprite;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 14f;
    private eMovementState         movementState;
    private Animator               animator;
    private eMovementState         previousState = 0;

    private enum eMovementState
    {
        IDLE,
        WALK_START,
        WALK,
        WALK_END,
        JUMP,
        FALL
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
            sprite.flipX = false;
           // movementState = eMovementState.WALK_START;

            if (movementState == eMovementState.IDLE)
            {
                movementState = eMovementState.WALK_START;
            }
            else
            {
                movementState = eMovementState.WALK;
            }
        }
        else if (dirX < 0)
        {
            sprite.flipX = true;
            // movementState = eMovementState.WALK;
            if (movementState == eMovementState.IDLE)
            {
                movementState = eMovementState.WALK_START;
            }
            else
            {
                movementState = eMovementState.WALK;
            }
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

        previousState = movementState;

        animator.SetInteger("state", (int)movementState);
    }

}
