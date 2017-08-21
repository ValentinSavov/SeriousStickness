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

    void LateUpdate()
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
        if (sideTouch != 0)
        {
            if (rbd.velocity.y < 0.2f)
            {
                // rbd.velocity = Vector2.zero;
                //rbd.AddForce(new Vector2(sideTouch, 1f) * jumpSpeed);
                JumpToSpecificPoint(45, transform.position + Vector3.right * 10);
                //vsa do something for side jump
                Debug.Log("Sidejump");
            }
        }
        else if (grounded)
        {
            if (rbd.velocity.y < 0.2f)
            {
                rbd.velocity = Vector2.zero;
                rbd.AddForce(Vector2.up * jumpSpeed);
                Debug.Log("jump");
            }
        }
    }

    public void JumpDown()
    {
        wantToJumpDown = true;
        transform.Translate(0, 0.001f, 0); // this is for trigger detection - if it does not move no trigger event is generated
    }

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

    void UpdateSenses()
    {
        grounded = false;
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
