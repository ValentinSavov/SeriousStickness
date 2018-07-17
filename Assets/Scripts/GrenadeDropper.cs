using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GrenadeDropper : AIControl
{
    public float moveSpeed = 5f;
    public float attackSpeed = 1f;
    public LayerMask layersToSense;
    public bool sideTouch = false;
    public float damage = 5f;
    float direction = 1;
    float prevAttackTime = 0f;
    public Animator clawsAnim;
    Transform spawnPoint;
    new void Start ()
	{
        base.Start();
        moveSpeed += Random.Range(-(moveSpeed * 0.2f), moveSpeed * 0.2f);
        spawnPoint = transform.Find("SpawnPoint");
        //this.transform.localScale = new Vector3(direction, 1, 1);
    }

    #region AI
    float cooldown = 0f;
    void FixedUpdate()
    {
        cooldown -= Time.fixedDeltaTime;
        if (cooldown <= 0f) cooldown = 0f;

        if (false == MoveSomehowTowards(direction))
        {
            direction *= -1;
            //this.transform.localScale = new Vector3(direction, 1, 1);
        }
        if (Mathf.Abs(target.transform.position.x - transform.position.x) < 1f)
        {
            if(cooldown == 0)
            {
                if(clawsAnim != null) clawsAnim.SetTrigger("Open");
                cooldown = 1/attackSpeed;
                DropGrenade();
            }
        }
    }
    bool MoveSomehowTowards(float direction)
    {
        bool result = true;
        if ((CanMoveTo(direction)))
        {
            transform.Translate(Vector3.right * direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            result = false;
        }
        return result;
    }

    bool CanMoveTo(float direction)
    {
        sideTouch = false;
        //if forward is free
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, -0.3f, 0f), new Vector3(Mathf.Sign(direction), 0f, 0f), 2f);
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.GetComponent<BorderTag>() != null)
            {
                sideTouch = true;
            }
        }
        if(!sideTouch)
        {
            //if forward-up is a ceiling
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, -0.2f, 0), new Vector3(0, 1, 0), 0.5f, layersToSense))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region  DamageAcceptor

    public override void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        //DropGrenade();
        //Die(argInArgs);
    }

    #endregion


    void DropGrenade()
    {
        GameObject proj = Instantiate(Resources.Load("Grenade", typeof(GameObject)),
                    spawnPoint.position, transform.rotation)
                    as GameObject;
        proj.GetComponent<Grenade>().speed = 0f;
        proj.GetComponent<Grenade>().damage = damage;
    }
}