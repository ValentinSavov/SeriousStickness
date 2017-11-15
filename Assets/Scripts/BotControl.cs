using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BotControl : MonoBehaviour, DamageAcceptor
{
    BotControl()
    {
        groups = new List<string>();
    }
    public List<string> groups { get; set; }
    public float range = 14f;
    public string startWeapon = "RocketLauncher";

    GameObject gpParent;
    StickStats stats;
    MovementController movement;
    Registry registry;
    GameObject target;
    Animator anim;

    FSM stateMachine;
    FSM.FSMState idleState; // finds something to do
    FSM.FSMState chaseInRangeState; // moves to a target
    
    float direction = 1;
    float changeDirectionCooldown = 5f;

    float previousEngageTime = 0f;

    void Start ()
	{
        //Debug.Log("BotStart");
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
        stateMachine = new FSM();

        createIdleState();
        createChaseInRangeState();
        stateMachine.pushState(idleState);

        Transform weaponSpot = GetComponentInChildren<WeaponSpot>().transform;
        GearDatabase gearDatabase = GameObject.FindObjectOfType<GearDatabase>();

        GameObject weap = Instantiate(gearDatabase.weapons.Find(x => x.gamePref.name == startWeapon).gamePref, weaponSpot) as GameObject;
        weap.transform.localPosition = Vector3.zero;
        weap.transform.localRotation = Quaternion.identity;
        weap.transform.localScale = Vector3.one;

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


    float idleTime = 0f;
    void createIdleState()
    {
        idleState = (fsm, gameObj) => 
        {
            changeDirectionCooldown -= Time.deltaTime;
            if (changeDirectionCooldown <= 0)
            {
                changeDirectionCooldown = Random.Range(2, 10);
                direction *= -1;
            }
            if ( (CanMoveTo(direction)) )
            {
                movement.MoveX(direction);
                this.transform.localScale = new Vector3(direction, 1, 1);
            }
            else
            {
                direction *= -1;
            }
            
            if ((target.transform.position - this.transform.position).magnitude < range)
            {
                fsm.pushState(chaseInRangeState);
            }

            idleTime += Time.deltaTime;
            if(idleTime >= 2f)
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
        };
    }

    void createChaseInRangeState()
    {
        chaseInRangeState = (fsm, gameObj) =>
        {
            this.transform.localScale = new Vector3( Mathf.Sign(target.transform.position.x - this.transform.position.x), 1, 1);
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
                    if (Mathf.Abs(deltaX) > weap.range/2)
                    {
                        direction = Mathf.Sign(deltaX);
                        if (CanMoveTo(direction))
                        {
                            movement.MoveX(direction);
                        }
                        else
                        {
                            //vsa CanJumpForward() or something similiar...
                        }
                    }
                    else if (Mathf.Abs(deltaX) < weap.range/3)
                    {
                        direction = Mathf.Sign(-deltaX);
                        if (CanMoveTo(direction))
                        {
                            movement.MoveX(direction);
                        }
                    }

                    //move Y stuff
                    float deltaY = target.transform.position.y - this.transform.position.y;
                    if ( Mathf.Abs(deltaY) > weap.range / 1.5f)
                    {
                        if(deltaY > 2f)
                        {
                            if (IsWhereToJumpUp())
                            {
                                movement.JumpUp();
                            }
                        }
                        else if(deltaY < -2f)
                        {
                            if (IsWhereToJumpDown())
                            {
                                movement.JumpDown();
                            }
                        }
                    }
                    
                    //attacking stuff
                    Attack();
                }
                else
                {
                    // out of weapon range but still in bot range
                    weap.transform.rotation = Quaternion.identity;
                    anim.SetFloat("LookAngle", 0f);
                    float deltaX = target.transform.position.x - this.transform.position.x;
                    direction = Mathf.Sign(deltaX);
                    if (CanMoveTo(direction))
                    {
                        movement.MoveX(direction);
                    }
                    else
                    {
                        //vsa CanJumpForward() or something similiar...
                    }
                }
            }

            if ((target.transform.position - this.transform.position).magnitude > range)
            {
                if (weap != null)
                {
                    weap.transform.rotation = Quaternion.identity;
                    anim.SetFloat("LookAngle", 0f);
                }
                fsm.popState();
            }
        };
    }

    int autoShotsCounter = 0;
    int nextAutoShotsCount = 10;
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
                if ( (weap.isAutomatic) && (autoShotsCounter <= nextAutoShotsCount) )
                {
                    if (weap.Engage(target.transform.position + Vector3.up))
                    {
                        autoShotsCounter++;
                    }
                }
            }
        }
    }

    void Update()
    {
        stateMachine.Update(this.gameObject);
    }
    
    bool CanMoveTo(float direction)
    {
        if(false == Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 0.5f, movement.layersToSense) )
        {
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, 0.1f, 0), new Vector3(0, -1, 0), 0.5f, movement.layersToSense))
            {
                return true;
            }
        }
        return false;
    }

    bool IsWhereToJumpUp()
    {
        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + Vector3.up, Vector3.up, 7f);
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + (2*Vector3.down), Vector3.up, 15f);
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


    #region weapon stuff
    /*void OnTriggerStay2D(Collider2D other)
    {
        if ( (other.gameObject.GetComponent<Weapon>() != null) && (!stats.isDead) && (GetComponentInChildren<Weapon>() == null) )
        {
            if (other.gameObject.GetComponent<Weapon>().GetComponentInParent<WeaponSpot>() == null)
            {
                //Debug.Log("Collide with weapon");
                //take weapon
                other.transform.parent = this.transform;
                other.transform.parent = GetComponentInChildren<WeaponSpot>().transform;
                other.transform.localPosition = Vector3.zero;
                other.transform.localRotation = Quaternion.identity;
                other.transform.localScale = new Vector3(1, 1, 1);
                other.gameObject.GetComponent<Weapon>().groups = groups;

                //GetComponent<GoapAgent>().ResetAgent();
            }
        }
    }*/
    #endregion 

    #region  DamageAcceptor
    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (stats.isDead)
        {
            //AddForceToRandomBones(argInArgs.knockback);
            return;
        }
        //Debug.Log(this.gameObject.name);
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
                Weapon weap = GetComponentInChildren<Weapon>();
                if (weap != null)
                {
                    weap.gameObject.transform.parent = gpParent.transform;
                    weap.transform.parent = gpParent.transform;
                }

                this.enabled = false;
                registry.damageAcceptors.RemoveDamageAcceptor(this);
                SwitchToRagdoll();
                AddForceToRandomBones(argInArgs.knockback * 1000);

                Destroy(this.gameObject);
                //Animator anim = GetComponent<Animator>();
                //if (anim) anim.enabled = false;

                
                
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

    #region ragdoll stuff
    void SwitchToRagdoll()
    {
        this.enabled = false;
        Destroy(movement);
        if (anim != null) anim.enabled = false;
        //remove the main colliders and rbs
        Rigidbody2D rbd = GetComponent<Rigidbody2D>();
        if (rbd)
        {
            rbd.isKinematic = true;
            Destroy(rbd);
        }
        Collider2D[] colls = GetComponents<Collider2D>();
        for (int i = 0; i < colls.Length; i++)
        {
            //if(colls[i].isTrigger == false)
            {
                colls[i].enabled = false;
                Destroy(colls[i]);
                //break;
            }
        }
        //////
        



        //activate child colliders and rbs
        Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rbs)
        {
            //rb.transform.SetParent(this.transform);
            if (rb.isKinematic == true)
            {
                rb.isKinematic = false;
                Collider2D col = rb.GetComponent<Collider2D>();
                if (col != null)
                {
                    if (col.isTrigger == true)
                    {
                        col.isTrigger = false;
                    }
                }
            }
        }
        GameObject stickBody = transform.Find("StickBody").gameObject;
        stickBody.transform.SetParent(gpParent.transform);
        Destroy(stickBody, 10f);
        /////
    }
    void AddForceToRandomBones(Vector2 knockback)
    {
        Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
        if (rbs != null)
        {
            Debug.Log(knockback);
            int rand1 = 0;//Random.Range(0, rbs.Length);
            //rbs[rand1].velocity = new Vector2(0, 0);
            rbs[rand1].AddForce(knockback);
        }
    }
    #endregion
}