using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MeleeBotControl : MonoBehaviour, DamageAcceptor, DamageProvider
{
    MeleeBotControl()
    {
        groups = new List<string>();
    }
    public List<string> groups { get; set; }
    public float range = 14f;

    GameObject gpParent;
    StickStats stats;
    MovementController movement;
    Registry registry;
    GameObject target;
    Animator anim;
    
    float direction = 1;
    float changeDirectionCooldown = 5f;
    float idleTime = 0f;
    Vector3 prevPosition;
    float stuckTime = 0f;
    float prevAttackTime = 0f;
    bool chasing = false;

    void Start ()
	{
        gpParent = GameObject.Find("GeneralPurposeParent");
        stats = GetComponent<StickStats>();
        movement = GetComponent<MovementController>();
        anim = GetComponent<Animator>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        movement.moveSpeed += Random.Range(-(movement.moveSpeed *0.2f), movement.moveSpeed *0.2f);
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("bots");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;

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
        prevPosition = this.transform.position;
    }

    #region AI
    void Update()
    {
        if ( (chasing == false) || (target == null))
        {
            processIdleState();
        }
        else
        {
            processChaseInRangeState();
        }
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
                idleTime = 0f;
            }
            else if (IsWhereToJumpDown())
            {
                movement.JumpDown();
                idleTime = 0f;
            }
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
                MoveSomehowTowards(direction);
            }
            else
            {
                //if (Mathf.Abs(deltaY) > range / 1.5f)
                if (Mathf.Abs(deltaX) < range/2)
                {
                    if(!MoveSomehowTowards(direction))
                    {
                        direction *= -1;
                    }
                }
                else
                {
                    direction *= -1;
                }
            }
        }
        else
        {
            chasing = false;
        }

        if( (this.transform.position - prevPosition).magnitude < (stats.attackSpeed * Time.deltaTime)/4 )
        {
            stuckTime += Time.deltaTime;
            if(stuckTime > 1f)
            {
                direction *= -1;
            }
        }
        else
        {
            if((target.transform.position - this.transform.position).magnitude < 1f)
            {
                Attack();
            }
            stuckTime = 0f;
        }

        prevPosition = this.transform.position;

        //move Y stuff
        float deltaY = target.transform.position.y - this.transform.position.y;
        //if (Mathf.Abs(deltaY) > range / 1.5f)
        {
            if (deltaY > 3f)
            {
                if (IsWhereToJumpUp())
                {
                    movement.JumpUp();
                }
            }
            else if (deltaY < -3f)
            {
                if (IsWhereToJumpDown())
                {
                    movement.JumpDown();
                }
            }
        }

    }
    bool MoveSomehowTowards(float direction)
    {
        bool result = true;
        if ((CanMoveTo(direction)))
        {
            //Debug.Log("Can move to");
            movement.MoveX(direction);
        }
        else if (CanJumpForward(direction))
        {
            //Debug.Log("Can JUMP F");
            movement.JumpUp();
            movement.MoveX(direction);
        }
        else if (movement.canPushSideTouch)
        {
            //Debug.Log("Can PUSH");
            movement.MoveX(direction);
        }
        else
        {
            result = false;
        }
        return result;
    }
    bool CanMoveTo(float direction)
    {
        //if forward is free
        if(false == Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 1f, movement.layersToSense) )
        {
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, 0.1f, 0), new Vector3(0, -1, 0), 2f, movement.layersToSense))
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + (2*Vector3.down), Vector3.up, 10f);
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
        if(Time.time - prevAttackTime > 0.3f)
        {
            prevAttackTime = Time.time;
            registry.damageAcceptors.doTargetDamage(
                        target.GetComponent<DamageAcceptor>(),
                        GetComponentInParent<Tag>().gameObject,
                        Random.Range(3,10),
                        "normal",
                        (target.transform.position - this.transform.position).normalized * 2000f,
                        groups);
        }
    }
    #endregion
    #region  DamageAcceptor

    public void ReportKill(DamageAcceptor killed)
    {

    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (stats.isDead)
        {
            return;
        }
        float locDamage = argInArgs.dmg;
        if (stats.currentArmorPoints > 0)
        {
            if (stats.currentArmorPoints > locDamage)
            {
                stats.currentArmorPoints -= locDamage;
                locDamage = 0;
            }
            else
            {
                stats.currentArmorPoints = 0;
                locDamage -= stats.currentArmorPoints;
            }
        }
        if (locDamage > 0)
        {
            if (stats.currentHitPoints > locDamage)
            {
                stats.currentHitPoints -= locDamage;
                locDamage = 0;
            }
            else
            {
                stats.currentHitPoints = 0;
                stats.isDead = true;
                DamageProvider dp = argInArgs.source.GetComponent<DamageProvider>();
                if (dp != null)
                {
                    dp.ReportKill(this);
                }

                this.gameObject.SetActive(false);
                Destroy(this.gameObject, 0.1f);
            }
        }
        GameObject healthbar = transform.Find("HealthBar").gameObject;
        if (healthbar != null)
        {
            healthbar.transform.Find("Level").GetComponent<Image>().fillAmount = stats.currentHitPoints / stats.totalHitPoints;
        }
    }
    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
    #endregion
}