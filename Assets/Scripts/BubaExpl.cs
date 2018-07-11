using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BubaExpl : AIControl
{
    public float moveSpeed = 5f;
    public LayerMask layersToSense;
    public bool sideTouch = false;
    public float damage = 5f;
    float direction = 1;
    float prevAttackTime = 0f;
    Rigidbody2D rbd;
    new void Start ()
	{
        base.Start();
        rbd = GetComponent<Rigidbody2D>();
        moveSpeed += Random.Range(-(moveSpeed * 0.2f), moveSpeed * 0.2f);
        this.transform.localScale = new Vector3(direction, 1, 1);
    }
    
    #region AI
    void FixedUpdate()
    {
        if (false == MoveSomehowTowards(direction))
        {
            direction *= -1;
            this.transform.localScale = new Vector3(direction, 1, 1);
        }
    }
    bool MoveSomehowTowards(float direction)
    {
        bool result = true;
        if ((CanMoveTo(direction)))
        {
            Vector2 locvelocity = rbd.velocity;
            locvelocity.x = direction * moveSpeed;
            rbd.velocity = new Vector2(locvelocity.x, locvelocity.y);
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, 0.3f, 0f), new Vector3(Mathf.Sign(direction), 0f, 0f), 2f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.GetComponent<PlayerTag>())
            {
                if ((hit.distance < 0.6f))
                {
                    DamageAcceptor da = hit.collider.GetComponent<DamageAcceptor>();
                    if ((da != null))
                    {
                        if (Time.time - prevAttackTime > 2f)
                        {
                            prevAttackTime = Time.time;
                            DoDamage(da, damage, direction * 3000 * Vector2.right);
                        }
                    }
                }
            }
            if(hit.collider.gameObject.GetComponent<BorderTag>() != null)
            {
                sideTouch = true;
            }
        }
        if(!sideTouch)
        {
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, 0.2f, 0), new Vector3(0, -1, 0), 0.5f, layersToSense))
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
        GameObject proj = Instantiate(Resources.Load("Grenade", typeof(GameObject)),
            this.transform.position + Vector3.up, transform.rotation)
            as GameObject;
        proj.GetComponent<Grenade>().speed = moveSpeed;
        proj.GetComponent<Grenade>().damage = 30f;
        Die(argInArgs);
    }

    #endregion

}