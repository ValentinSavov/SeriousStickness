using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour, DamageAcceptor, DamageProvider
{
    public List<string> groups { get; set; }
    public bool findStartPoint = false;
    PlayerGear gear;
    Animator anim;
    StickStats stats;
    MovementController movement;
    GameObject cursor;
    Registry registry;
    SticknessLevel slevel;
    GameObject gpParent;
    Text health;
    GameObject deadStickyNote;
    GameObject ui;
    List<SourcesAndCooldowns> damageSourcesInCooldown = new List<SourcesAndCooldowns>();

    float originalMoveSpeed;
    float originalJumpSpeed;

    void Start()
    {
        gear = GetComponent<PlayerGear>();
        anim = GetComponent<Animator>();
        stats = GetComponent<StickStats>();
        movement = GetComponent<MovementController>();
        cursor = GameObject.FindObjectOfType<CursorTag>().gameObject;
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        slevel = GetComponent<SticknessLevel>();
        ui = GameObject.Find("UI");
        deadStickyNote = ui.transform.Find("DeadStickyNote").gameObject;

        gpParent = GameObject.Find("GeneralPurposeParent");
        health = ui.transform.Find("Health").GetComponent<Text>();
        health.text = ((int)(stats.currentHitPoints)).ToString();

        originalMoveSpeed = movement.moveSpeed;
        originalJumpSpeed = movement.jumpSpeed;

        groups = new List<string>();
        groups.Add("players");
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;

        Time.timeScale = 1f;

        if (findStartPoint)
        {
            this.enabled = false;
            InvokeRepeating("WaitForStartPoint", 0.1f, 0.1f);
        }
    }
    void WaitForStartPoint()
    {
        GameObject startPoint = GameObject.Find("StartPoint");
        if(startPoint != null)
        {
            this.transform.position = startPoint.transform.position;
            CancelInvoke("WaitForStartPoint");
            this.enabled = true;
        }
    }

    void FixedUpdate()
    {
        anim.SetBool("Jump", false);
        anim.SetBool("Fall", false);
        anim.SetFloat("Slide", 0);

        //UpdateDamageCooldowns();
        HealthUpdate();

        if (Input.GetButton("Fire2") && (slevel.level > 0))
        {
            slevel.DecreaseLevel(Time.fixedDeltaTime * 10f);
            Time.timeScale = 0.4f;
            bonusHealAmount = 10;
        }
        else
        {
            bonusHealAmount = 0;
            Time.timeScale = 1f;
        }

        //speed boost
        if ( (Input.GetKey(KeyCode.LeftShift)) && (slevel.level > 0))
        {
            movement.moveSpeed = originalMoveSpeed * 1.5f;
            movement.jumpSpeed = originalJumpSpeed * 1.2f;
            slevel.DecreaseLevel(Time.fixedDeltaTime * 10f);
        }
        else
        {
            movement.moveSpeed = originalMoveSpeed;
            movement.jumpSpeed = originalJumpSpeed;
        }


        //vsa suicide
        if (Input.GetKeyDown("k"))
        {
            stats.currentHitPoints = 0;
            stats.isDead = true;
            this.enabled = false;
            gear.enabled = false;
            anim.enabled = false;
            GameObject.FindObjectOfType<SceneControl>().Invoke("Die", 1f);
            return;
        }

        //if (Input.GetKeyDown("n"))
        {
            //movement.KnockBack(new Vector2 (5000f, 5000f) );
        }


        if (Input.GetButton("Cancel"))
        {
            if (ui.transform.Find("Tip"))
            {
                ui.transform.Find("Tip").gameObject.SetActive(false);
            }
        }


        //move
        movement.MoveX(Input.GetAxis("Horizontal"));
        
        //turn to needed position
        if ((cursor.transform.position.x - this.transform.position.x) > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        
        if(!movement.grounded)
        {
            if(movement.sideTouchR)
            {
                anim.SetFloat("Slide", 1);
                this.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if(movement.sideTouchL)
            {
                anim.SetFloat("Slide", -1);
                this.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                anim.SetBool("Fall", true);
            }
        }
        
        //look at
        float degreesToRotateWeapon = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), cursor.transform.position - gear.GetSelectedWeapon().transform.position).eulerAngles.z;
        gear.GetSelectedWeapon().transform.rotation = Quaternion.AngleAxis(degreesToRotateWeapon, Vector3.forward);
        float lookAngleForAnimator = gear.GetSelectedWeapon().transform.rotation.eulerAngles.z;
        if (Mathf.Abs(lookAngleForAnimator) > 90)
        {
            lookAngleForAnimator -= 360;
        }
        lookAngleForAnimator *= Mathf.Sign(transform.localScale.x);
        anim.SetFloat("LookAngle", lookAngleForAnimator);
        
        anim.SetFloat("Speed", Input.GetAxis("Horizontal") * Mathf.Sign(transform.localScale.x));
        
        if (Input.GetAxis("Vertical") < -0.5f)
        {
            movement.JumpDown();
        }
        else if ( (Input.GetButton("Jump") == true) /*|| (Input.GetAxis("Vertical") > 0.5f)*/ )
        {
            if (movement.JumpUp())
            {
                anim.SetBool("Jump", true);
            }
        }
    }
    int bonusHealAmount = 0;
    float healTimeCounter = 0;
    void HealthUpdate()
    {
        healTimeCounter += Time.deltaTime;
        if (healTimeCounter >= 1)
        {
            healTimeCounter = 0;
            int healAmount = 2 + bonusHealAmount;
            if (slevel.level > 50)
            {
                healAmount += 8;
            }
            stats.currentHitPoints += healAmount;
            if (stats.currentHitPoints > stats.totalHitPoints * 1.2f)
            {
                stats.currentHitPoints = stats.totalHitPoints * 1.2f;
            }
            else
            {
                //GameObject popup = Instantiate(Resources.Load("HealPopup", typeof(GameObject)),
                //this.transform.position, Quaternion.identity)
                //as GameObject;
                //popup.GetComponent<Popup>().text = "" + healAmount.ToString();
                //popup.transform.parent = gpParent.transform;
            }
            health.text = ((int)(stats.currentHitPoints)).ToString();
        }
        if(stats.currentHitPoints < 50)
        {
            GameObject.Find("UI").transform.Find("Blood").gameObject.SetActive(true);
        }
    }

    void Heal(int healAmount)
    {
        stats.currentHitPoints += healAmount;
        if (stats.currentHitPoints > stats.totalHitPoints * 1.2f)
        {
            stats.currentHitPoints = stats.totalHitPoints * 1.2f;
        }
        health.text = ((int)(stats.currentHitPoints)).ToString();
    }

    void HealTo(int healthLevel)
    {
        if (stats.currentHitPoints < healthLevel)
        {
            stats.currentHitPoints = Mathf.Clamp(healthLevel, 0, stats.totalHitPoints);
            health.text = ((int)(stats.currentHitPoints)).ToString();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Checkpoint cp = other.GetComponent<Checkpoint>();
        if (cp != null)
        {
            slevel.Restart();// IncreaseLevel(40);
            HealTo(100);
        }
    }

    #region damage stuff

    public void ReportKill(DamageAcceptor killed)
    {
        slevel.IncreaseLevel(UnityEngine.Random.Range(1,5));
        GameObject popup = Instantiate(Resources.Load("KillPopup", typeof(GameObject)),
                    ((Component)killed).gameObject.transform.position + Vector3.up*2, Quaternion.identity,
                    gpParent.transform)
                    as GameObject;
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        //if (damageSourcesInCooldown.Find(x => x.source == argInArgs.source) == null)
        {
            if(stats.isDead)
            {
                return;
            }
            float locDamage = argInArgs.dmg;
            //damageSourcesInCooldown.Add(new SourcesAndCooldowns(argInArgs.source));

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
                GameObject popup = Instantiate(Resources.Load("DmgPopup", typeof(GameObject)),
                this.transform.position, Quaternion.identity)
                as GameObject;
                popup.GetComponent<Popup>().text = "-" + locDamage.ToString();
                popup.transform.parent = gpParent.transform;

                Animator healthBgAnim = GameObject.Find("UI").transform.Find("HealthBackground").GetComponent<Animator>();
                if (healthBgAnim) healthBgAnim.SetTrigger("decrease");
                
                if (stats.currentHitPoints > locDamage)
                {
                    stats.currentHitPoints -= locDamage;
                    locDamage = 0;
                    if (argInArgs.knockback != new Vector2(0, 0))
                    {
                        //add vertical knockback effect
                        Vector2 knb = (argInArgs.knockback.normalized + Vector2.up) * argInArgs.knockback.magnitude;
                        movement.KnockBack(knb);
                    }
                }
                else
                {
                    stats.currentHitPoints = 0;
                    stats.isDead = true;
                    this.enabled = false;
                    gear.enabled = false;
                    if (anim) { anim.enabled = false; }

                    //vsa ragdoll stuff here...
                    GameObject stickBody = transform.Find("StickBody").gameObject;
                    stickBody.SetActive(false);
                    Vector3 stickBodyPosition = stickBody.transform.position;
                    GameObject ragdoll = Instantiate(Resources.Load("Ragdoll", typeof(GameObject)),
                    stickBodyPosition, Quaternion.identity, gpParent.transform) as GameObject;
                    ragdoll.GetComponent<Ragdoll>().Push(argInArgs.knockback);
                    GetComponent<CapsuleCollider2D>().enabled = false;
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    registry.damageAcceptors.RemoveDamageAcceptor(this);

                    deadStickyNote.SetActive(true);

                    Time.timeScale = 0.5f;

                    SceneControl sceneControl = GameObject.FindObjectOfType<SceneControl>();
                    if (sceneControl != null)
                    {
                        sceneControl.Invoke("Die", 2f);
                    }
                }
            }
        }
        health.text = ((int)(stats.currentHitPoints)).ToString();
    }
    
    private class SourcesAndCooldowns
    {
        public GameObject source;
        public float remainingCooldown = 1f;

        public SourcesAndCooldowns(GameObject argInGameObject)
        {
            source = argInGameObject;
            remainingCooldown = 1f;
        }
    }

    private void UpdateDamageCooldowns()
    {
        foreach (SourcesAndCooldowns dmgSrc in damageSourcesInCooldown)
        {
            dmgSrc.remainingCooldown -= Time.deltaTime;
        }

        List<SourcesAndCooldowns> newList = new List<SourcesAndCooldowns>();

        foreach (SourcesAndCooldowns dmgSrc in damageSourcesInCooldown)
        {
            if (dmgSrc.remainingCooldown > 0)
            {
                newList.Add(dmgSrc);
            }
        }
        damageSourcesInCooldown.Clear();
        damageSourcesInCooldown = newList;
    }
    #endregion
}
