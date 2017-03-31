﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BotControl : MonoBehaviour, IGoap, DamageAcceptor
{
    #region standart stuff and monobehavior
    public List<string> groups { get; set; }
    GameObject gpParent;
    StickStats stats;
    Registry registry;
    RectTransform healthBar;
    MovementController movement;
    Node[] graph;

    void Start ()
	{
        //graph = GameObject.Find("Graph").GetComponentsInChildren<Node>();
        movement = GetComponent<MovementController>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        stats = GetComponent<StickStats>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        stats.moveSpeed += Random.Range(-(stats.moveSpeed *0.2f), stats.moveSpeed *0.2f);
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("bots");

        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
    }
    #endregion
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
                GetComponent<GoapAgent>().enabled = false;
                Destroy(this.gameObject/*, 10f*/);
                
                //anim.enabled = false;
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
    #region  IGoap
    public HashSet<KeyValuePair<string,object>> getWorldState ()
    {
		HashSet<KeyValuePair<string,object>> worldState = new HashSet<KeyValuePair<string,object>> ();

        worldState.Add(new KeyValuePair<string, object>("playerIsDead", false ) );
        worldState.Add(new KeyValuePair<string, object>("hasWeapon", (GetComponentInChildren<Weapon>() != null)) );
        worldState.Add(new KeyValuePair<string, object>("weaponExists", registry.weapons.IsThereAWeapon()) );

        return worldState;
	}
	public HashSet<KeyValuePair<string,object>> createGoalState ()
    {
		HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();

		goal.Add(new KeyValuePair<string, object>("attackPlayer", true ));
        
        return goal;
	}
    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal) { }
    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions) { }
    public void actionsFinished() { }
    public void planAborted(GoapAction aborter){}
    #endregion


    class TargetReachParams
    {
        public Vector3 targetPos = new Vector3(0, 0, 0);
        public float actionRange = 0.5f;
        public Vector3 nextNodePos = new Vector3(0, 0, 0);
    }
    TargetReachParams targetParams = new TargetReachParams();
    public bool GoToAction(GoapAction nextAction)
    {
        Debug.Log("Go to action");
        //if (targetParams.targetPos == new Vector3(0, 0, 0))
        {
            //Debug.Log("first time");
            targetParams.targetPos = nextAction.target.transform.position;

            //GetComponent<Bot>().MoveTo((Vector2)targetParams.targetPos);
        }
        if ((gameObject.transform.position - nextAction.target.transform.position).magnitude <= 20)
        {
            nextAction.setInRange(true);
            targetParams.nextNodePos = new Vector3(0, 0, 0);
            return true;
        }
        /*if ((gameObject.transform.position - nextAction.target.transform.position).magnitude <= nextAction.range)
        {
            nextAction.setInRange(true);
            targetParams.nextNodePos = new Vector3(0, 0, 0);
            return true;
        }*/

        return false;
    }



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

                GetComponent<GoapAgent>().ResetAgent();
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
            rb.transform.SetParent(this.transform);
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