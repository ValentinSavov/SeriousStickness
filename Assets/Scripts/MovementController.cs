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
    [HideInInspector]
    public bool canPushSideTouch = false;
    Rigidbody2D rbd;
    CapsuleCollider2D mainCollider;
    
    void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
    }

    void LateUpdate()
    {
        UpdateSenses();
    }


    public void MoveX(float horisontalSpeed)
    {
        if (horisontalSpeed != 0)
        {
            Vector2 velocity = rbd.velocity;

            velocity.x = horisontalSpeed * moveSpeed;

            if(sideTouch == Mathf.Sign(horisontalSpeed))
            {
                velocity.x = 0;
            }
            if(canPushSideTouch)
            {
                velocity.x /= 2;
            }
            //this.transform.Translate(new Vector3(horisontalSpeed * moveSpeed * Time.fixedDeltaTime, 0,0));
            //rbd.MovePosition(rbd.position + new Vector2(horisontalSpeed * moveSpeed * Time.deltaTime, 0));
            rbd.velocity = new Vector2(velocity.x, velocity.y);
            //rbd.AddForce(velocity - rbd.velocity, ForceMode2D.Impulse);
        }
    }

    public bool JumpUp()
    {
        wantToJumpDown = false;

        if ((sideTouch != 0) && (!grounded))
        {
            //side jump
            if (rbd.velocity.y < 0.2f)
            {
                //rbd.velocity = new Vector2((jumpSpeed * -sideTouch)/2, jumpSpeed);
                //Debug.Log("Sidejump");
                //return true;
                return false;
            }
        }
        else if (grounded)
        {
            //up jump
            if (rbd.velocity.y < 1f)
            {
                rbd.velocity = new Vector2(rbd.velocity.x, jumpSpeed);
                //Debug.Log("jump");
                return true;
            }
        }
        return false;
    }

    public void JumpDown()
    {
        wantToJumpDown = true;
        transform.Translate(0, 0.001f, 0); // this is for trigger detection - if it does not move no trigger event is generated
    }

    /*
    public void JumpToSpecificPoint(float initialAngle, Vector3 p)
    {
        float gravity = Physics2D.gravity.magnitude;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        //rbd.velocity = finalVelocity;
        //rbd.velocity.Set(finalVelocity.x, finalVelocity.y);
        Debug.Log(finalVelocity.x);
        Debug.Log(finalVelocity.y);
        // Alternative way:
        rbd.AddForce((Vector2)(finalVelocity * rbd.mass));
    }
    */
    
    void UpdateSenses()
    {
        canPushSideTouch = false;
        grounded = false;
        
        Collider2D[] colsG = Physics2D.OverlapPointAll(transform.position - (Vector3.up * GetComponent<CapsuleCollider2D>().size.x / 10), layersToSense);
        foreach(Collider2D col in colsG)
        {
            if(col.isTrigger == false)
            {
                grounded = true;
            }
        }

        Collider2D[] colsR = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(mainCollider.size.x / 2, mainCollider.size.y / 2, 0), new Vector2(mainCollider.size.x, mainCollider.size.y * 0.8f), CapsuleDirection2D.Vertical, 0f, layersToSense);
        Collider2D[] colsL = Physics2D.OverlapCapsuleAll(transform.position + new Vector3(-(mainCollider.size.x / 2), mainCollider.size.y / 2, 0), new Vector2(mainCollider.size.x, mainCollider.size.y * 0.8f), CapsuleDirection2D.Vertical, 0f, layersToSense);
        sideTouch = 0f;
        foreach (Collider2D col in colsR)
        {
            if(col.isTrigger == false)
            {
                if(col.GetComponent<InteractableObject>() != null)
                {
                    canPushSideTouch = true;
                }
                if ((!col.usedByEffector) && (col.GetComponent<BorderTag>() != null))
                {
                    canPushSideTouch = false;
                    sideTouch += 1;
                    break;
                }
            }
        }
        foreach (Collider2D col in colsL)
        {
            if (col.isTrigger == false)
            {
                if (col.GetComponent<InteractableObject>() != null)
                {
                    canPushSideTouch = true;
                }
                if ((!col.usedByEffector) && (col.GetComponent<BorderTag>() != null))
                {
                    canPushSideTouch = false;
                    sideTouch -= 1;
                    break;
                }
            }
        }
    }

    #region StuffForJumpDown
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
        {
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
    #endregion
}
