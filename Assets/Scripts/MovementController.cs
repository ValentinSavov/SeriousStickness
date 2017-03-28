using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public bool doubleJumpAllowed = false;
    public bool grounded = false;
    public bool jumping = false;
    public Vector2 velocity;

    //Collider2D groundedTo;
    //Animator anim;
    StickStats stats;
    Vector3 turntarget = new Vector3(0, 0, 0);
    int jumpCounter = 0;
    Rigidbody2D rbd;

    void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        stats = GetComponent<StickStats>();
    }

    //remove this after movement ready
    void Update()
    {
        CheckGround();
    }

    public void MoveX(float horisontalDirection)
    {
        //velocity.x = Mathf.Sign(horisontalDirection) * stats.moveSpeed;
        this.transform.Translate(new Vector3(Mathf.Sign(horisontalDirection) * stats.moveSpeed * Time.deltaTime, 0,0));

        //turn to needed target position
        //if ((turntarget.x - this.transform.position.x) > 0.05f)
        if (velocity.x > 0.05f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        //if ((turntarget.x - this.transform.position.x) < -0.05f)
        if (velocity.x < -0.05f)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    public void JumpUp()
    {
        if (IsStable())
        {
            jumpCounter = 0;
        }
        if ( (IsStable()) || ((doubleJumpAllowed) && (jumpCounter < 2)))
        {
            jumpCounter++;
            jumping = true;
            rbd.velocity.Set(0, 0);
            jumpTrigger = true;
        }
    }
    bool jumpTrigger = false;
    void FixedUpdate()
    {
        if (jumpTrigger)
        {
            rbd.AddForce((stats.jumpStartSpeed / Time.deltaTime) * Vector2.up, ForceMode2D.Force);
            jumpTrigger = false;
        }
    }
    
    public bool IsStable()
    {
        CheckGround();
        if (rbd.velocity.y == 0)
        {
            jumping = false;
        }

        return ((grounded) && (!jumping));
    }

    void CheckGround()
    {
        grounded = false;
        CapsuleCollider2D capcol = GetComponent<CapsuleCollider2D>();
        Vector3 groundCheckPos = transform.position + (Vector3.down * (capcol.size.y / 2));
        float groundCheckRadius = capcol.size.y / 4;
        Collider2D[] colls = Physics2D.OverlapCircleAll(groundCheckPos, groundCheckRadius);
        foreach (Collider2D col in colls)
        {
            if (col.GetComponent<FloorTag>() != null)
            {
                //groundedTo = col;
                grounded = true;
                break;
            }
        }
    }

    public bool CanJumpTo(Vector3 target)
    {
        if (IsStable())
        {
            GameObject bot = this.gameObject;
            StickStats stats = bot.GetComponent<StickStats>();

            // so far
            float vertVelocity = bot.GetComponent<Rigidbody2D>().velocity.y + stats.jumpStartSpeed;
            float timeStep = 0.3f;
            Vector2 startPosition = bot.transform.position;
            int i = 0;
            Vector2 nextPosition = new Vector2(bot.transform.position.x, bot.transform.position.y);

            while ((nextPosition.y >= startPosition.y) && ((++i < 20)))
            {
                vertVelocity += Physics2D.gravity.y * timeStep;
                //verticalOffset += vertVelocity * timeStep;
                Vector3 prevPosition = nextPosition;
                nextPosition.x += Mathf.Sign(target.x - bot.transform.position.x) * (stats.moveSpeed * timeStep);
                nextPosition.y += vertVelocity * timeStep;

                Debug.DrawLine(prevPosition, nextPosition);

                if ((nextPosition.y >= target.y) && Mathf.Abs(startPosition.x - nextPosition.x) >= Mathf.Abs(startPosition.x - target.x))
                {
                    nextPosition = (Vector2)target;
                    return true;
                }
            }
        }
        return false;
    }

    public void JumpDown()
    {
        wantToJumpDown = true;
    }
    /* stuff for jump down */
    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("ExitTrigger");
        if (other.gameObject.GetComponent<FloorTag>() != null)
        {
            if (Physics2D.GetIgnoreCollision(other, GetCollider()))
            {
                Physics2D.IgnoreCollision(other, GetCollider(), false);
                wantToJumpDown = false;
            }
        }
    }
    bool wantToJumpDown = false;
    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("StayTrigger");
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
