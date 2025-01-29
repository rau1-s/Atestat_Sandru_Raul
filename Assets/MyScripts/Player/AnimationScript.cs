using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator anim;              // referinta la componenta animator a jucatorului care controleaza animatiile in functie de parametrii
    private PlayerMovement move;        // referinta la scriptul PlayerMovement care tine de miscarea jucatorului
    private Collision coll;             // referinta la scriptul Collision care tine cont de coliziuni
    public SpriteRenderer sr;           // referinta la componenta SpriteRenderer pentru a manipula orientarea sprite-ului

    void Start()
    {
        // obtinerea referintelor de mai sus
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        move = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // setarea parametrilor din animator la valorile obtinute in collision
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onRightWall", coll.onRightWall);
        anim.SetBool("wallGrab", move.wallGrab);
        anim.SetBool("wallSlide", move.wallSlide);
        anim.SetBool("canMove", move.canMove);
        anim.SetBool("isDashing", move.isDashing);
        anim.SetBool("walljumping", move.wallJumped);
    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", Mathf.Abs(x));      // miscarea pe axa X (abs pentru sens)
        anim.SetFloat("VerticalAxis", y);                   // miscarea pe axa Y
        anim.SetFloat("VerticalVelocity", yVel);            // viteza verticala a personajului folosita pentru animatii de cadere, saritura, dash
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);                           // declansator de triggeruri specifice: jump walljump etc
    }

    public void Flip(int side)
    {
        // side = directia jucatorului -- 1 dreapta si -1 stanga
        if (move.wallGrab || move.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }
        // sprite-ul se intoarce automat in orice caz in afara de cazurile in care se afla pe perete
        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}