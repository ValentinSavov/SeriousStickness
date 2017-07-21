using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]


public class MovementController : MonoBehaviour
{
    [Tooltip("m/s")]
    public float moveSpeed = 15f; // m/s
    public float jumpSpeed = 80000f; // m/s
    
    public bool grounded = false;
    [Tooltip("-1 = left, 1 = right, 0 = no touch")]
    public float sideTouch = 0;
    bool wantToJumpDown = false;

    public LayerMask layersToSense;

    Rigidbody2D rbd;
    CapsuleCollider2D mainCollider;
    
    void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        UpdateSenses();
    }

    public void MoveX(float horisontalSpeed)
    {
        if (horisontalSpeed != 0)
        {
            this.transform.Translate(new Vector3(horisontalSpeed * moveSpeed * Time.deltaTime, 0,0));
        }
    }
    
    public void JumpUp()
    {
        if (grounded)
        {
            if (rbd.velocity.y < 0.2f)
            {
                rbd.velocity = Vector2.zero;
                rbd.AddForce(Vector2.up * jumpSpeed);
            }
        }
        //else if (sideTouch != 0)
        {
            //if (rbd.velocity.y < 0.2f)
            {
                //rbd.velocity = Vector2.zero;
                //rbd.AddForce(new Vector2(sideTouch, 1f) * jumpSpeed);
                //vsa do something for side jump
            }
        }
    }

    public void JumpDown()
    {
        wantToJumpDown = true;
        transform.Translate(0, 0.001f, 0); // this is for trigger detection - if it does not move no trigger event is generated
    }

    void UpdateSenses()
    {
        grounded = false;
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 0.2f, layersToSense);
        //foreach (Collider2D col in colls)

        if (Physics2D.OverlapCircle(transform.position, GetComponent<CapsuleCollider2D>().size.x/4, layersToSense) != null)
        {
            grounded = true;
        }

        sideTouch = 0f;
        //check right touch
        if (Physics2D.OverlapCircle(transform.position + new Vector3(mainCollider.size.x/2, mainCollider.size.y/2, 0), mainCollider.size.x/4, layersToSense) != null)
        {
            sideTouch += 1;
        }
        //check left touch
        if (Physics2D.OverlapCircle(transform.position + new Vector3(-(mainCollider.size.x/2), mainCollider.size.y/2, 0), mainCollider.size.x/4, layersToSense) != null)
        {
            sideTouch -= 1;
        }
    }

    // stuff for jump down
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<FloorTag>() != null)
        {
            if (Physics2D.GetIgnoreCollision(other, GetCollider()))
            {
                Physics2D.IgnoreCollision(other, GetCollider(), false);
                wantToJumpDown = false;
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (wantToJumpDown)
            if ((other.gameObject.GetComponent<FloorTag>() != null) && (other.gameObject.GetComponent<PlatformEffector2D>()))
            {
                if (other.transform.position.y < this.transform.position.y)
                {
                    if (wantToJumpDown)
                    {
                        Physics2D.IgnoreCollision(other, GetCollider(), true);
                    }
                }
            }
    }

    Collider2D GetCollider()
    {
        Collider2D found = null;
        Collider2D[] colls = GetComponents<Collider2D>();
        foreach (Collider2D col in colls)
        {
            if (!col.isTrigger)
            {
                found = col;
            }
        }
        return found;
    }
}
