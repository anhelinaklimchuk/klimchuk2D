using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float jumpHeight = 5.0f;

    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 26f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;


    private enum MovementState { idle, running, jump, falling }

    Vector2 movementVector;

    Rigidbody2D rbody;
    CircleCollider2D circleCollider;
    Animator animator;
    SpriteRenderer sprite;
    [SerializeField] private TrailRenderer tr;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Vector2 playerVelocity = new Vector2(movementVector.x * movementSpeed, rbody.velocity.y);
        rbody.velocity = playerVelocity;


        UpdateAnimationUpdate();

        Flip();

    }

    //Input System
    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (!circleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                return;
            }
            if (Input.GetButtonDown("Jump"))
            {
                rbody.velocity += new Vector2(0f, jumpHeight);
            }
        }
    }
    private void OnMove(InputValue value)
    {
        if (isDashing)
        {
            return;
        }
        movementVector = value.Get<Vector2>();
        Debug.Log(movementVector);
    }
    private void OnDash(InputValue value)
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rbody.gravityScale;
        rbody.gravityScale = 0f;
        rbody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rbody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    private void Flip()
    {
        sprite.flipX = false;
        if (isFacingRight && movementVector.x < 0f || !isFacingRight && movementVector.x > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
            sprite.flipX = true;
        }
    }

    //Animation
    public void UpdateAnimationUpdate()
    {
        MovementState state;
        if (movementVector.x > 0.0f)
        {
            state = MovementState.running;
        }
        else if (movementVector.x < 0.0f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rbody.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        else if (rbody.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }
}
