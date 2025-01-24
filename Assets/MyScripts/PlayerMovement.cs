using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    public Rigidbody2D rb;
    private AnimationScript anim;

    public float speed = 10;
    public float jumpForce = 80;  // was 50
    public float slideSpeed = .1f;
    public float wallJumpLerp = 60;
    public float dashSpeed = 100;
    public float maxVerticalSpeed = 20f;
    private float lastWallJumpY;



    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;


    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.linearVelocity.y);

        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            // if(side != coll.wallSide)
            //     anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {

            if (coll.onGround) {
                anim.SetTrigger("jump");
                Jump(Vector2.up, false);
            }
            if (coll.onWall && !coll.onGround) {
                anim.SetTrigger("walljump");
                WallJump();
            }
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }

    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;
    }

    private void Dash(float x, float y)
    {
        hasDashed = true;
        anim.SetTrigger("dash");

        Vector2 dashDir = new Vector2(x, y).normalized;

        rb.linearVelocity += dashDir * dashSpeed;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed)
        );

        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.5f);

        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            // Optional: Flip player sprite
            // side *= -1;
            // anim.Flip(side);
        }

        // Prevent repeated jumps on the same wall
        if (Mathf.Abs(transform.position.y - lastWallJumpY) < 0.1f)
            return;

        lastWallJumpY = transform.position.y;

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.18f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        // Increase jump height for better upward motion
        Jump((Vector2.up * .9f + wallDir * 1.2f), true);

        wallJumped = true;
    }


    private void WallSlide()
    {
        if (!canMove) // Prevent wall sliding if the player can't move
            return;

        // Check if the player is pushing against the wall
        bool pushingWall = (rb.linearVelocity.x > 0 && coll.onRightWall) || (rb.linearVelocity.x < 0 && coll.onLeftWall);

        // Limit horizontal velocity based on whether the player is pushing the wall
        float push = pushingWall ? 0 : rb.linearVelocity.x;

        // Apply vertical slide speed only if on a wall
        if (coll.onWall)
        {
            rb.linearVelocity = new Vector2(push, Mathf.Max(-slideSpeed, rb.linearVelocity.y));
        }
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.linearDamping = x;
    }
}