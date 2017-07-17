using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour, DamageAcceptor, DamageProvider
{
    public bool grounded = false;
    public bool wantToJumpDown = false;
    
    public List<string> groups { get; set; }

    private StickStats stats;
    private GameObject cursor;
    private Registry registry;

    private List<SourcesAndCooldowns> damageSourcesInCooldown = new List<SourcesAndCooldowns>();
    Animator anim;
    PlayerGear gear;
    GameObject gpParent;
    GameObject ui;
    Text health;
    SticknessLevel slevel;

    void Start()
    {
        gear = GetComponent<PlayerGear>();
        anim = GetComponent<Animator>();
        stats = GetComponent<StickStats>();
        cursor = GameObject.FindObjectOfType<Cursor>().gameObject;
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);

        slevel = GameObject.FindObjectOfType<SticknessLevel>() as SticknessLevel;
        groups = new List<string>();
        groups.Add("players");

        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
        gpParent = GameObject.Find("GeneralPurposeParent");
        ui = GameObject.Find("UI");
        //if (ui != null)
        {
            health = ui.transform.Find("Health").GetComponent<Text>();
            health.text = "+ " + ((int)(stats.currentHitPoints)).ToString();
        }
    }

    
    void FixedUpdate()
    {
        UpdateDamageCooldowns();
        SticknessLevelResponse();
        
        CheckGround();
        
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * stats.moveSpeed;
        if (anim != null) anim.SetFloat("Speed", Input.GetAxis("Horizontal"));

        float degreesToRotate = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), cursor.transform.position - gear.GetSelectedWeapon().transform.position).eulerAngles.z;
        gear.GetSelectedWeapon().transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);
        
        //turn to needed position
        if((cursor.transform.position.x - this.transform.position.x) > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        transform.Translate(x, 0, 0);
        
        if (Input.GetAxis("Vertical") < -0.5f)
        {
            wantToJumpDown = true;
            transform.Translate(0, 0.001f, 0); // this is for trigger detection - if it does not move no trigger event is generated
        }
        else if (Input.GetButton("Jump") == true)
        {
            if (grounded)
            {
                //Debug.Log("Jump");
                if(GetComponent<Rigidbody2D>().velocity.y < 0.2f)
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * stats.jumpSpeed);
                }
            }
        }
    }

    float healTimeCounter = 0;
    void SticknessLevelResponse()
    {
        if (slevel.level < 10)
        {
            DamageAcceptorRegistry.DamageArgs args = new DamageAcceptorRegistry.DamageArgs();
            args.dmg = Random.Range(2, 7);
            args.source = slevel.gameObject;
            acceptDamage(args);
        }
        else if (slevel.level > 65)
        {
            healTimeCounter += Time.deltaTime;
            if (healTimeCounter >= 0.2)
            {
                healTimeCounter = 0;
                stats.currentHitPoints += 1;
                if(slevel.level > 80)
                {
                    stats.currentHitPoints += 1;
                }
                if (stats.currentHitPoints > stats.totalHitPoints * 1.2f)
                {
                    stats.currentHitPoints = stats.totalHitPoints * 1.2f;
                }
                else
                {
                    GameObject popup = Instantiate(Resources.Load("HealPopup", typeof(GameObject)),
                    this.transform.position, Quaternion.identity)
                    as GameObject;
                    popup.GetComponent<Popup>().text = "+1";
                    popup.transform.parent = gpParent.transform;
                }
                health.text = "+ " + ((int)(stats.currentHitPoints)).ToString();
            }
        }
    }

    #region damage stuff

    public void ReportKill(DamageAcceptor killed)
    {
        slevel.IncreaseLevel(Random.Range(10,15));

    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (damageSourcesInCooldown.Find(x => x.source == argInArgs.source) == null)
        {
            float locDamage = argInArgs.dmg;
            //argInArgs.cbIsAccepted = true;

            damageSourcesInCooldown.Add(new SourcesAndCooldowns(argInArgs.source));

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


                if (stats.currentHitPoints > locDamage)
                {
                    stats.currentHitPoints -= locDamage;
                    locDamage = 0;
                }
                else
                {
                    stats.currentHitPoints = 0;
                    stats.isDead = true;
                    this.enabled = false;
                    if (anim) { anim.enabled = false; }
                    GameObject.FindObjectOfType<SceneControl>().Invoke("ReloadScene", 1f); ;
                }
            }

            if (argInArgs.knockback != new Vector2(0, 0))
            {
                // vsa do something for knockback
                //firstPersonControllerRef.inertia = argInArgs.knockback;
            }
        }

        health.text = "+ " + ((int)(stats.currentHitPoints)).ToString();

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

    #region jump and coliders stuff

    void CheckGround()
    {
        grounded = false;
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 0.2f );
        foreach (Collider2D col in colls)
        {
            if (col.GetComponent<FloorTag>() != null)
            {
                grounded = true;
                break;
            }
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
    #endregion
    
}
