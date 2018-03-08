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

    //GameObject gpParent;
    StickStats stats;
    MovementController movement;
    Registry registry;
    GameObject target;
    Animator anim;

    Vector2 startPosition;
    float direction = 1;
    float stuckTime = 0f;
    float prevAttackTime = 0f;
    bool chasing = false;

    void Start ()
	{
        //gpParent = GameObject.Find("GeneralPurposeParent");
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
                Attack(da, 5, Vector2.up * 20000);
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
            Attack(target.GetComponent<DamageAcceptor>(), Random.Range(5,20), (((Component)target.GetComponent<DamageAcceptor>()).transform.position - this.transform.position).normalized * 5000);
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
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 2, 0.5f, 0), new Vector3(0, -1, 0), 1f, movement.layersToSense))
            {
                return true;
            }
        }
        return false;
    }
    void Attack(DamageAcceptor acceptor, float damage, Vector2 knockback)
    {
        if(Time.time - prevAttackTime > 0.5f)
        {
            prevAttackTime = Time.time;
            registry.damageAcceptors.doTargetDamage(
                        acceptor,
                        GetComponentInParent<Tag>().gameObject,
                        damage,
                        "normal",
                        knockback);
        }
    }
    #endregion
    #region  DamageAcceptor

    public void ReportKill(DamageAcceptor killed)
    {

    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if(this.gameObject == null)
        {
            return;
        }
        if (stats.isDead)
        {
            return;
        }
        float locDamage = argInArgs.dmg;
        if(argInArgs.type == "melee")
        {
            locDamage *= 3;
        }
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