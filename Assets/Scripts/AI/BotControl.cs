using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BotControl : MonoBehaviour, DamageAcceptor
{

    public List<string> groups { get; set; }
    GameObject gpParent;
    StickStats stats;
    Registry registry;
    //NavMoveAgent agent;
    RectTransform healthBar;

    GameObject target;

    void Start ()
	{
        gpParent = GameObject.Find("GeneralPurposeParent");
        stats = GetComponent<StickStats>();
        //agent = GetComponent<NavMoveAgent>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        stats.moveSpeed += Random.Range(-(stats.moveSpeed *0.2f), stats.moveSpeed *0.2f);
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("bots");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
        float time = Random.Range(0.8f, 2f);
        //InvokeRepeating("TargetUpdate", 0f, time);
    }

    void TargetUpdate()
    {
        //agent.MoveTo((Vector2)target.transform.position);
    }

    float previousEngageTime = 0f;
    void Update()
    {
        Weapon weap = GetComponentInChildren<Weapon>();
        if (weap != null)
        {
            if ((target.transform.position - this.transform.position).magnitude < weap.range)
            {
                float degreesToRotate = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), target.transform.position - weap.transform.position).eulerAngles.z;
                weap.transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);

                if ((Time.time - previousEngageTime) >= (1f / (GetComponent<StickStats>().attackSpeed / 100)))
                {
                    previousEngageTime = Time.time;
                    weap.Engage(target);
                }
            }
        }
                
    }

    #region  DamageAcceptor
    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
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
                Weapon weap = GetComponentInChildren<Weapon>();
                if (weap != null)
                {
                    weap.gameObject.transform.parent = gpParent.transform;
                    weap.transform.parent = gpParent.transform;
                }
                
                this.enabled = false;
                //GetComponent<GoapAgent>().enabled = false;
                Destroy(this.gameObject, 10f);

                Animator anim = GetComponent<Animator>();
                if(anim)anim.enabled = false;
                
                SwitchToRagdoll();
                AddForceToRandomBones(argInArgs.knockback);
            }
        }


        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name == "Foreground")
            {
                healthBar = child.gameObject.GetComponent<RectTransform>();
            }
        }
        healthBar.sizeDelta = new Vector2(200 * stats.currentHitPoints / stats.totalHitPoints, healthBar.sizeDelta.y);

    }
    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
    #endregion
    
    #region weapon stuff
    void OnTriggerStay2D(Collider2D other)
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
    }
    #endregion 

    #region ragdoll stuff
    void SwitchToRagdoll()
    {
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
        /////
    }
    void AddForceToRandomBones(Vector2 knockback)
    {
        Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
        if (rbs != null)
        {
            int rand1 = 0;//Random.Range(0, rbs.Length);
            rbs[rand1].velocity = new Vector2(0, 0);
            rbs[rand1].AddForce(knockback);
        }
    }
    #endregion

}