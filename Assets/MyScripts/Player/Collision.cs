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

    public float collisionRadius = 0.25f;                   // dimensiunea razei de detectare a coliziunilor
    public Vector2 bottomOffset1, bottomOffset2,
                    rightOffset1, rightOffset2, rightOffset3,
                    leftOffset1, leftOffset2, leftOffset3;   // offseturi care specifica pozitiile pentru verificarile de coliziune
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
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset1, collisionRadius, groundLayer)   // bottomOffset pt pamant
            || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset2, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset1, collisionRadius, groundLayer)       // right si left pt pereti
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset2, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset3, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset1, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset2, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset3, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset1, collisionRadius, groundLayer) // right pt perete drept
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset2, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset1, collisionRadius, groundLayer)   // left pt perete stang
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset2, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset3, collisionRadius, groundLayer);   

        wallSide = onRightWall ? -1 : 1;              //    calculeaza pe ce parte a unui perete se afla jucatorul
    }

    void OnDrawGizmos()             // doar scop in debugging pentru a arata cele 3 zone de coliziune in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset1, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset2, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset1, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset2, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset3, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset1, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset2, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset3, collisionRadius);
    }
}