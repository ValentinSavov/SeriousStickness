using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Buba : AIControl
{
    public float moveSpeed = 2f;
    public float damage = 5f;
    public LayerMask layersToSense;
    public bool sideTouch = false;
    Rigidbody2D rbd;
    float direction = 1;
    float hitCooldown = 0f;
    float prevAttackTime = 0f;
    GameObject head;

    new void Start ()
	{
        base.Start();
        rbd = GetComponent<Rigidbody2D>();
        head = transform.Find("Head").gameObject;
        moveSpeed += Random.Range(-(moveSpeed * 0.2f), moveSpeed * 0.2f);
        this.transform.localScale = new Vector3(direction, 1, 1);
    }

    #region AI
    void FixedUpdate()
    {
        hitCooldown -= Time.deltaTime;
        if (hitCooldown <= 0)
        {
            hitCooldown = 0f;
            head.SetActive(true);
            if (false == MoveSomehowTowards(direction))
            {
                direction *= -1;
                this.transform.localScale = new Vector3(direction, 1, 1);
            }
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

    float lastBloodifyTime = 0f;
    public override void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        float locDamage = argInArgs.dmg;

        if (locDamage > 0)
        {
            head.SetActive(false);
            hitCooldown = 3f;
            SpitBloodParticle((transform.position - argInArgs.source.transform.position).normalized, transform.position + Vector3.up/2);
            if (Time.time - lastBloodifyTime > 3f)
            {
                lastBloodifyTime = Time.time;
                Bloodify(this.transform.position + Vector3.up);
            }
        }
    }
    #endregion
}