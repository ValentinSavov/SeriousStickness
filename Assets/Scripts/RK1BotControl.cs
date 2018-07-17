using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RK1BotControl : AIControl
{
    public float range = 14f;
    MovementController movement;
    Vector2 startPosition;
    float direction = 1;
    float stuckTime = 0f;
    float prevAttackTime = 0f;
    bool chasing = false;

    new void Start ()
	{
        //Debug.Log("Melee Start");
        base.Start();
        //Debug.Log("Melee Continue Start");
        movement = GetComponent<MovementController>();
        movement.moveSpeed += Random.Range(-(movement.moveSpeed *0.2f), movement.moveSpeed *0.2f);
        startPosition = transform.position;

        chasing = false;
        if (Random.Range(0,10) > 5)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        changeDirCooldown = 1f;
    }


    #region AI
    void Update()
    {
        changeDirCooldown -= Time.deltaTime;
        if ( (chasing == false) || (target == null))
        {
            processIdleState();
        }
        else
        {
            processChaseInRangeState();
        }
        //hit boxes if any
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + new Vector3(0, 1, 0), new Vector3(direction, 0, 0), 1f, movement.layersToSense) ;
        foreach (RaycastHit2D hit in hits)
        {
            DamageAcceptor da = hit.collider.gameObject.GetComponent<DamageAcceptor>();
            if (da != null)
            {
                if (Time.time - prevAttackTime > 0.5f)
                {
                    prevAttackTime = Time.time;
                    DoDamage(da, 5, Vector2.up * 20000);
                }
            }
        }
    }
    void processIdleState()
    {
        if (target != null)
        {
            if ((target.transform.position - this.transform.position).magnitude < range)
            {
                chasing = true;
                return;
            }
            float deltaX = this.transform.position.x - startPosition.x;

            if(Mathf.Abs(deltaX) > range)
            {
                direction = -Mathf.Sign(deltaX);
            }

            if ((CanMoveTo(direction)))
            {
                movement.MoveX(direction);
            }
            else
            {
                //if (movement.grounded)
                {
                    ChangeDirection();//direction *= -1;
                }
            }
            this.transform.localScale = new Vector3(direction, 1, 1);
        }
    }
    void processChaseInRangeState()
    {
        this.transform.localScale = new Vector3(direction, 1, 1);
        
        if ((target.transform.position - this.transform.position).magnitude < range)
        {
            //move X stuff
            float deltaX = target.transform.position.x - this.transform.position.x;

            //if it walks towards the target
            if(Mathf.Sign(direction) == Mathf.Sign(deltaX))
            {
                //if (Mathf.Abs(transform.position.x - startPosition.x) > range)
                {
                    //went too far, go home
                    //direction = -Mathf.Sign(transform.position.x - startPosition.x);
                    //movement.MoveX(direction);
                }
                //else
                //if (Mathf.Abs(deltaX) < range / 2)
                {
                    if ((CanMoveTo(direction)))
                    {
                        movement.MoveX(direction);
                    }
                    else
                    {
                        ChangeDirection();//direction *= -1;
                    }
                }
            }
            else
            {
                //if(Mathf.Abs(transform.position.x - startPosition.x) > range)
                {
                    //went too far, go home
                    //direction = -Mathf.Sign(transform.position.x - startPosition.x);
                    //movement.MoveX(direction);
                }
                //else 
                if (Mathf.Abs(deltaX) < range/2)
                {
                    if ((CanMoveTo(direction)))
                    {
                        movement.MoveX(direction);
                    }
                    else
                    {
                        ChangeDirection();//direction *= -1;
                    }
                }
                else
                {
                    ChangeDirection();//direction *= -1;
                }
            }
        }
        else
        {
            chasing = false;
        }

        //stuck check
        if (Mathf.Abs(movement.velocity.x) < movement.moveSpeed / 4)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > 0.3f)
            {
                //Debug.Log("Stucked");
                stuckTime = 0f;
                direction *= -1;
            }
        }

        if ((target.transform.position - this.transform.position).magnitude < 1f)
        {
            if (Time.time - prevAttackTime > 0.5f)
            {
                prevAttackTime = Time.time;
                DoDamage(target.GetComponent<DamageAcceptor>(), Random.Range(5, 20), ((((Component)target.GetComponent<DamageAcceptor>()).transform.position - this.transform.position).normalized) * 5000);
            }
        }
    }

    float changeDirCooldown = 0f;
    void ChangeDirection()
    {
        if(changeDirCooldown <= 0)
        {
            direction *= -1;
            changeDirCooldown = 1f;
        }
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
            CapsuleCollider2D caps = GetComponent<CapsuleCollider2D>();

            Debug.DrawRay(transform.position + new Vector3(direction * caps.size.x * 0.6f, 0.5f, 0), new Vector3(0, -1, 0), Color.red);
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(direction * caps.size.x*0.6f, 0.5f, 0), new Vector3(0, -1, 0), 1f, movement.layersToSense))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region DamageAcceptor
    public override void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        //Debug.Log("MeleeAcceptDmg" + argInArgs.dmg);
        if (argInArgs.type == "melee")
        {
            argInArgs.dmg *= 3;
        }
        base.acceptDamage(argInArgs);
    }
    #endregion
}