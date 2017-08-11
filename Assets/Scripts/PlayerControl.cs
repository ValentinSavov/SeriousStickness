using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour, DamageAcceptor, DamageProvider
{
    public List<string> groups { get; set; }

    PlayerGear gear;
    Animator anim;
    StickStats stats;
    MovementController movement;
    GameObject cursor;
    Registry registry;
    SticknessLevel slevel;
    GameObject gpParent;
    Text health;
    List<SourcesAndCooldowns> damageSourcesInCooldown = new List<SourcesAndCooldowns>();
    
    void Start()
    {
        gear = GetComponent<PlayerGear>();
        anim = GetComponent<Animator>();
        stats = GetComponent<StickStats>();
        movement = GetComponent<MovementController>();
        cursor = GameObject.FindObjectOfType<Cursor>().gameObject;
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        slevel = GameObject.FindObjectOfType<SticknessLevel>() as SticknessLevel;

        gpParent = GameObject.Find("GeneralPurposeParent");
        health = GameObject.Find("UI").transform.Find("Health").GetComponent<Text>();
        health.text = "+ " + ((int)(stats.currentHitPoints)).ToString();

        groups = new List<string>();
        groups.Add("players");
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
    }

    
    void FixedUpdate()
    {
        UpdateDamageCooldowns();
        SticknessLevelResponse();
        
        if(Input.GetKeyDown("k"))
        {
            stats.currentHitPoints = 0;
            stats.isDead = true;
            this.enabled = false;
            gear.enabled = false;
            if (anim) { anim.enabled = false; }
            GameObject.FindObjectOfType<SceneControl>().Invoke("Die", 1f);
            return;
        }

        if (anim != null) anim.SetFloat("Speed", Input.GetAxis("Horizontal"));

        movement.MoveX(Input.GetAxis("Horizontal"));

        float degreesToRotateWeapon = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), cursor.transform.position - gear.GetSelectedWeapon().transform.position).eulerAngles.z;
        gear.GetSelectedWeapon().transform.rotation = Quaternion.AngleAxis(degreesToRotateWeapon, Vector3.forward);
        
        //turn to needed position
        if((cursor.transform.position.x - this.transform.position.x) > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        
        if (Input.GetAxis("Vertical") < -0.5f)
        {
            movement.JumpDown();
        }
        else if ( (Input.GetButton("Jump") == true) || (Input.GetAxis("Vertical") > 0.5f) )
        {
            movement.JumpUp();
            //movement.JumpToSpecificPoint(100, cursor.transform.position);
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
        else if (slevel.level > 50)
        {
            healTimeCounter += Time.deltaTime;
            if (healTimeCounter >= 0.2)
            {
                healTimeCounter = 0;
                int healAmount = 0;
                healAmount += 2;
                if(slevel.level > 80)
                {
                    healAmount += 1;
                }
                stats.currentHitPoints += healAmount;
                if (stats.currentHitPoints > stats.totalHitPoints * 1.2f)
                {
                    stats.currentHitPoints = stats.totalHitPoints * 1.2f;
                }
                else
                {
                    GameObject popup = Instantiate(Resources.Load("HealPopup", typeof(GameObject)),
                    this.transform.position, Quaternion.identity)
                    as GameObject;
                    popup.GetComponent<Popup>().text = "+" + healAmount.ToString();
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
                    gear.enabled = false;
                    if (anim) { anim.enabled = false; }
                    GameObject.FindObjectOfType<SceneControl>().Invoke("Die", 1f);
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
    
}
