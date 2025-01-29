using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // referinte la alte obiecte / celelalte scripturi
    private Collision coll;
    public Rigidbody2D rb;
    private AnimationScript anim;

    // variabile cu scop parametric pentru jucator
    public float speed = 10;                    // viteza
    public float jumpForce = 80;                // inaltimea sariturii
    public float slideSpeed = .1f;              // viteza de alunecare pe un perete
    public float wallJumpLerp = 60;             // inaltimea sariturii de pe un perete
    public float dashSpeed = 100;               // distanta de dash
    public float maxVerticalSpeed = 20f;        // viteza verticala maxima pentru dash
    private float lastWallJumpY;                // variabila contor pentru saritura pe perete


    // variabile ce tin cont de anumite conditii -- referinte in AnimationScript
    public bool canMove;                        // posibilitatea de a se deplasa
    public bool wallGrab;                       // apuca sau nu peretele
    public bool wallJumped;                     // a sarit sau nu de pe un perete
    public bool wallSlide;                      // aluneca pe un perete
    public bool isDashing;                      // este sau nu in dash
    private bool groundTouch;                   // atinge sau nu pamantul
    private bool hasDashed;                     // a fost sau nu intr-un dash (recent)

    public int side = 1;                        // directia in care playerul priveste


    // Start este apelata inainte de primul frame
    void Start()
    {
        // stabilirea referintelor fata de cele 3 scripturi
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        canMove = true;
    }

    // Update este apelata o data in fiecare frame
    void Update()
    {
        // preluarea directiilor x si y din taste
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);                             // vector de stochare a directiilor x si y

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.linearVelocity.y);       // functie care preia date pentru scriptul animatiilor

        if (coll.onWall && Input.GetButton("Fire3") && canMove)      // verificarea posibilitatii de a se tine de perete
        {
            if(side != coll.wallSide)
                anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)  // lasare de perete
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)                             // activarea sariturii imbunatatie din script
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        if (wallGrab && !isDashing)                                 // modificarea parametriilor in cazul in care se tine de perete
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround)                            // apelarea functiei de alunecare pe perete
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)                           // oprirea alunecarii pe perete
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))                             // apelarea functii de saritura / saritura de pe perete dupa caz
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

        if (Input.GetButtonDown("Fire1") && !hasDashed)              // apelarea funtii de dash dupa conditii
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)                           // apelarea functii de atingere a pamantului
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)                            // modificarea variabilei de pamant
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)                       // oprirea unor miscari in functie de parametrii
            return;

        // orientarea playerului spre directia de deplasare
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

    void GroundTouch()                                      // functie de calibrare a valoriilor booleane respectiv a directiei
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;
    }

    private void Dash(float x, float y)                     // functie dash
    {
        hasDashed = true;
        anim.SetTrigger("dash");                            // pornirea animatiei de dash din celalalt script

        Vector2 dashDir = new Vector2(x, y).normalized;     // .normalized -> o versiune normalizata a vectorului = aceeasi directie, lungime de 1

        rb.linearVelocity += dashDir * dashSpeed;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Clamp(rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed)       // limitarea vitezei verticale (evitarea dashului in sus)
        );

        StartCoroutine(DashWait());                         // apelarea unei functii de pauza
        // coroutine este o functie utilizata in unity pentru a incepe o corutina -- metoda speciala care permite executarea secventiala a codului
        // pe parcursul mai multor cadre sau perioade de timp fara a bloca firul principal de executie
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());                       // tratarea cazului in care este pe pamant (dash pentru pamant)
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;      // oprirea abilitatii de a sarii
        wallJumped = true;
        isDashing = true;

       // yield indica momentul in care metoda se suspenda temporar si cand va relua executia (aici .5 sec)
        yield return new WaitForSeconds(.5f);

        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;       // reactivarea abilitatii de a sarii
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()                                                 // functie pentru saritura pe/de pe pereti
    {
        // prevenirea sariturilor repetate pe acelasi perete
        if (Mathf.Abs(transform.position.y - lastWallJumpY) < 0.1f)
            return;
        
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        lastWallJumpY = transform.position.y;

        // dezactivarea miscarii pentru a preveni intoarcerea pe acelasi perete, intr-un punct mai sus
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.18f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        // cresterea vitezei verticale respectiv orizontale pentru miscare fluida
        Jump((Vector2.up * .9f + wallDir * 1.2f), true);

        wallJumped = true;
    }


    private void WallSlide()
    {
        if(coll.wallSide != side)
            anim.Flip(side * -1);

        if (!canMove) // prevenirea alunecarii pe perete daca playerul nu se poate misca
            return;

        // verificare daca playerul apasa pertele
        bool pushingWall = (rb.linearVelocity.x > 0 && coll.onRightWall) || (rb.linearVelocity.x < 0 && coll.onLeftWall);

        // limitarea velocitatii orizontale in functie de apasarea jucatorului
        float push = pushingWall ? 0 : rb.linearVelocity.x;

        // aplicarea vitezei verticale de alunecare diar daca este pe un perete
        if (coll.onWall)
        {
            rb.linearVelocity = new Vector2(push, Mathf.Max(-slideSpeed, rb.linearVelocity.y));
        }
    }

    private void Walk(Vector2 dir)
    {
        // prevenirea cazurilor in care playerul nu se poate misca sau se tine de perete
        if (!canMove)
            return;

        if (wallGrab)
            return;

        // aplicarea miscarii 
        if (!wallJumped)        // daca jucatorul nu a sarit de pe un perete, miscarea este simpla si instantanee
        {
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
        // dupa ce jucatorul sare de pe un perete, este posibil sa existe o schimbare brusca a directiei. in loc sa
        // actualizeze viteza instantaneu, interpolarea creeaza o tranzitie mai fluida intre viteza curenta si cea dorita
        // .Lerp = Linerar Interpolation -- metoda utilizata pt a calcula o valoare intermediara dintre 2 puncte, pe baza unui factor de progresie
        // Tim.deltaTime -- timpul scurs intre doua cadre consecutive in Unity
    }

    private void Jump(Vector2 dir, bool wall)                       // functia sariturii in functie de directia curenta de deplasare
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)                         // functia de dezactivare a miscarii pentru time secunde
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}