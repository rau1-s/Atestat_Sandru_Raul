using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public LayerMask groundLayer;
    // groundLayer specifica stratul obiectelor considerate ground. este folosit pentru a detecta daca jucatorul atinge podeaua sau peretii
    public bool onGround;               // indica daca jucatorul atinge pamantul
    public bool onWall;                 // indica daca jucatorul atinge peretele
    public bool onRightWall;            // indica daca jucatorul atinge peretele din dreapta
    public bool onLeftWall;             // indica daca jucatorul atinge peretele din stange
    public int wallSide;                // retine directia peretelui

    public Vector2 bottomSize = new Vector2(0.5f, 0.2f);
    public Vector2 sideSize = new Vector2(0.2f, 1f);
    public Vector2 bottomOffset, rightOffset, leftOffset;   // offseturi care specifica pozitiile pentru verificarile de coliziune
    private Color debugCollisionColor = Color.red;          // culoare folosita pentru a desena sferele de coliziune in editor (debugging)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Physics2D.OverlapCircle -- creeaza un cerc la pozitia specificata si verifica daca se intersecteaza cu obiecte din stratul groundLayer
        // transform.position + bottomOffset -- pozitia jucatorului este dedusa din pozitia jucatorului si offsetul specificat
        // Check for ground using a box positioned at the bottom
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0f, groundLayer);

        // Check for walls using tall, narrow boxes on the left and right sides
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, sideSize, 0f, groundLayer);
        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, sideSize, 0f, groundLayer);


        // onWall is true if either left or right detection is active
        onWall = onLeftWall || onRightWall;

        wallSide = onRightWall ? -1 : 1;              //    calculeaza pe ce parte a unui perete se afla jucatorul
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset, bottomSize);
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, sideSize);
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, sideSize);
    }
}