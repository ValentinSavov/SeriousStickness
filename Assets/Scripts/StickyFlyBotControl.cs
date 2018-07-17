using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickyFlyBotControl : AIControl
{
    public float range = 14f;

    MovementController movement;
    Animator anim;
    
    float direction = 1;
    float changeDirectionCooldown = 5f;
    float previousEngageTime = 0f;
    float idleTime = 0f;
    int autoShotsCounter = 0;
    int nextAutoShotsCount = 10;
    float animSpeedCommand = 0f;
    bool chasing = false;

    new void Start ()
	{
        base.Start();
        movement = GetComponent<MovementController>();
        anim = GetComponent<Animator>();
        movement.moveSpeed += Random.Range(-(movement.moveSpeed *0.2f), movement.moveSpeed *0.2f);
        chasing = false;

        if (Random.Range(0,10) > 5)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        changeDirectionCooldown = Random.Range(2, 10);
    }

    #region AI
    void FixedUpdate()
    {
        animSpeedCommand = 0f;
        anim.SetBool("Jump", false);
        anim.SetBool("Fall", false);
        if ( (chasing == false) || (target == null))
        {
            processIdleState();
        }
        else
        {
            processChaseInRangeState();
        }
        if (!movement.grounded)
        {
            animSpeedCommand = 0f;
            anim.SetBool("Fall", true);
        }
        anim.SetFloat("Speed", animSpeedCommand);
    }
    void processIdleState()
    {
        if(target!=null)
        if ((target.transform.position - this.transform.position).magnitude < range)
        {
            chasing = true;
            return;
        }

        changeDirectionCooldown -= Time.deltaTime;
        if (changeDirectionCooldown <= 0)
        {
            changeDirectionCooldown = Random.Range(4, 10);
            direction *= -1;
        }
        
        if (false == MoveSomehowTowards(direction))
        {
            if(movement.grounded)
            {
                direction *= -1;
            }
        }
        this.transform.localScale = new Vector3(direction, 1, 1);

        idleTime += Time.deltaTime;
        if (idleTime >= 2f)
        {
            if (IsWhereToJumpUp())
            {
                movement.JumpUp();
                anim.SetBool("Jump", true);
                idleTime = 0f;
            }
            else if (IsWhereToJumpDown())
            {
                movement.JumpDown();
                anim.SetBool("Jump", true);
                idleTime = 0f;
            }
        }
    }
    void processChaseInRangeState()
    {
        this.transform.localScale = new Vector3(Mathf.Sign(target.transform.position.x - this.transform.position.x), 1, 1);
        Weapon weap = GetComponentInChildren<Weapon>();
        if (weap != null)
        {
            if ((target.transform.position - this.transform.position).magnitude < weap.range)
            {
                float degreesToRotate = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), (target.transform.position + Vector3.up) - weap.transform.position).eulerAngles.z;
                weap.transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);

                float lookAngleForAnimator = weap.transform.rotation.eulerAngles.z;
                if (Mathf.Abs(lookAngleForAnimator) > 90)
                {
                    lookAngleForAnimator -= 360;
                }
                lookAngleForAnimator *= Mathf.Sign(transform.localScale.x);
                anim.SetFloat("LookAngle", lookAngleForAnimator);

                //move X stuff
                float deltaX = target.transform.position.x - this.transform.position.x;
                if (Mathf.Abs(deltaX) > 0.5f)
                {
                    if (Mathf.Abs(deltaX) > weap.range / 2)
                    {
                        direction = Mathf.Sign(deltaX);
                        MoveSomehowTowards(direction);
                    }
                    else if (Mathf.Abs(deltaX) < weap.range / 3)
                    {
                        direction = Mathf.Sign(-deltaX);
                        MoveSomehowTowards(direction);
                    }
                }
                Attack();
            }
            else
            {
                // out of weapon range but still in bot range

                weap.transform.rotation = Quaternion.identity;
                anim.SetFloat("LookAngle", 0);
                //weap.transform.rotation = Quaternion.AngleAxis(315, Vector3.forward);
                //anim.SetFloat("LookAngle", -45);

                float deltaX = target.transform.position.x - this.transform.position.x;
                direction = Mathf.Sign(deltaX);
                MoveSomehowTowards(direction);
            }

            //move Y stuff
            float deltaY = target.transform.position.y - this.transform.position.y;
            if (Mathf.Abs(deltaY) > weap.range / 1.5f)
            {
                if (deltaY > 2f)
                {
                    if (IsWhereToJumpUp())
                    {
                        movement.JumpUp();
                        anim.SetBool("Jump", true);
                    }
                }
                else if (deltaY < -2f)
                {
                    if (IsWhereToJumpDown())
                    {
                        movement.JumpDown();
                        anim.SetBool("Jump", true);
                    }
                }
            }
        }

        if ((target.transform.position - this.transform.position).magnitude > range)
        {
            if (weap != null)
            {
                weap.transform.rotation = Quaternion.identity;
                anim.SetFloat("LookAngle", 0f);

                //weap.transform.rotation = Quaternion.AngleAxis(315, Vector3.forward);
                //anim.SetFloat("LookAngle", -45);
            }
            chasing = false;
        }
    }
    bool MoveSomehowTowards(float direction)
    {
        bool result = true;
        if ((CanMoveTo(direction)))
        {
            //Debug.Log("Can move to");
            movement.MoveX(direction);
            animSpeedCommand = direction * Mathf.Sign(transform.localScale.x);
        }
        else if (CanJumpForward(direction))
        {
            //Debug.Log("Can JUMP F");
            movement.JumpUp();
            anim.SetBool("Jump", true);
            movement.MoveX(direction);
            animSpeedCommand = direction * Mathf.Sign(transform.localScale.x);
        }
        else if (movement.pushableSideTouch)
        {
            //Debug.Log("Can PUSH");
            movement.MoveX(direction);
            animSpeedCommand = direction * Mathf.Sign(transform.localScale.x);
        }
        else
        {
            result = false;
        }
        return result;
    }
    bool CanMoveTo(float direction)
    {
        bool check = false;
        //if forward is free
        if (direction > 0)
        {
            check = movement.sideTouchR;
        }
        else if (direction < 0)
        {
            check = movement.sideTouchL;
        }
        if (!check)
        {
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, 0.1f, 0), new Vector3(0, -1, 0), 3f, movement.layersToSense))
            {
                return true;
            }
        }
        return false;
    }
    bool IsWhereToJumpUp()
    {
        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + Vector3.up, Vector3.up, 4f);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.GetComponent<FloorTag>() != null)
            {
                found = true;
                break;
            }
        }
        return found;
    }
    bool IsWhereToJumpDown()
    {
        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + (Vector3.down), Vector3.down, 4f, movement.layersToSense);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<FloorTag>() != null)
            {
                found = true;
                break;
            }
        }
        return found;
    }
    bool CanJumpForward(float direction)
    {
        //if 3m forward is free
        RaycastHit2D hitF = Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 3f, movement.layersToSense);
        if (hitF.collider == null)
        {
            //if somewhere far forward - far down is a floor
            for (int i = 2; i < 5; i++)
            {
                RaycastHit2D hitDown = Physics2D.Raycast(this.transform.position + (i * direction * Vector3.right), Vector3.down, 3f, movement.layersToSense);
                if (hitDown.collider != null)
                {
                    //Debug.Log(hitDown.collider.name);
                    return true;
                }
            }
        }
        else
        {
            //if up-forward is free
            RaycastHit2D hitUpF = Physics2D.Raycast(transform.position + new Vector3(0, 3, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 1f, movement.layersToSense);
            if (hitUpF.collider == null)
            {
                //Debug.Log("up forward is free");
                return true;
            }
        }
        return false;
    }

    void Attack()
    {
        Weapon weap = GetComponentInChildren<Weapon>();
        if (weap != null)
        {
            //vsa check if the bot sees the target
            //RaycastHit2D[] hits = Physics2D.RaycastAll(weap.transform.position, target.transform.position - weap.transform.position);

            if ((Time.time - previousEngageTime) >= (1f / (GetComponent<StickStats>().attackSpeed / 100)))
            {
                previousEngageTime = Time.time;
                weap.Engage(target.transform.position + Vector3.up);
                autoShotsCounter = 0;
                nextAutoShotsCount = Random.Range(5, 20);
            }
            else
            {
                if ((weap.isAutomatic) && (autoShotsCounter <= nextAutoShotsCount))
                {
                    if (weap.Engage(target.transform.position + Vector3.up))
                    {
                        autoShotsCounter++;
                    }
                }
            }
        }
    }
    #endregion
    #region  DamageAcceptor
    protected override void Knockback(Vector2 knockback)
    {
        Vector2 knb = (knockback.normalized + Vector2.up) * knockback.magnitude;
        movement.KnockBack(knb);
    }
    protected override void Die(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        Weapon weap = GetComponentInChildren<Weapon>();
        if (weap != null)
        {
            weap.transform.parent = gpParent.transform;
            Destroy(weap.gameObject, 4f);
        }
        Vector3 stickBodyPosition = transform.Find("StickBody").position;
        GameObject ragdoll = Instantiate(Resources.Load("Ragdoll", typeof(GameObject)),
        stickBodyPosition, Quaternion.identity, gpParent.transform) as GameObject;

        ragdoll.GetComponent<Ragdoll>().Push(argInArgs.knockback);
        base.Die(argInArgs);
    }
    #endregion
}