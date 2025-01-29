using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumping : MonoBehaviour
{
    private Rigidbody2D rb;                         // referinta la componenta rigid body pentru a manipula gravitatia si altele
    public float fallMultiplier = 2.5f;             // controleaza cat de rapid cade jucatorul in timpul unei sarituri (amplificarea gravitatiei)
    public float lowJumpMultiplier = 2f;            // ajusteaza viteza sariturilor joase in cazul in care butonul de saritura este eliberat prematur

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();           // preluarea componenentei rigidbody atasata obiectului
    }

    void Update()
    {
        if(rb.linearVelocity.y < 0)     // in cazul in care jucatorul cade
        {
            // gravitatia este amplificata de fallMultiplier, accelerand caderea
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else 
            if(rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))     //tratarea cazului unei sarituri joase (velocitate pozitiva dar buton lasat)
            {
                // gravitatia este amplificata de lowJumpMultiplier, reducand inaltimea sariturii
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
    }
}