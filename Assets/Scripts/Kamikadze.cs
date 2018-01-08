using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Kamikadze : MonoBehaviour, DamageAcceptor, DamageProvider
{
    Kamikadze()
    {
        groups = new List<string>();
    }
    public List<string> groups { get; set; }
    public float range = 20f;

    public GameObject effectOnExplode;
    public float damage = 50f;
    public float damageRadius = 2f;
    public float knockback = 5000f;
    
    GameObject gpParent;
    MovementController movement;
    Registry registry;
    GameObject target;
    StickStats stats;
    float direction = 1;


    void Start()
    {
        stats = GetComponent<StickStats>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        movement = GetComponent<MovementController>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        movement.moveSpeed += Random.Range(-(movement.moveSpeed * 0.2f), movement.moveSpeed * 0.2f);
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("bots");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;

        if (Random.Range(0, 10) > 5)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
    }

    #region AI

    void Update()
    {
        if ((target.transform.position - this.transform.position).magnitude < range)
        {
            float deltaX = target.transform.position.x - this.transform.position.x;
            if(Mathf.Abs(deltaX) > 0.4f)
            {
                float direction = Mathf.Sign(deltaX);
                this.transform.localScale = new Vector3(direction, 1, 1);
                MoveSomehowTowards(direction);
            }

            //move Y stuff
            float deltaY = target.transform.position.y - this.transform.position.y;
            if (Mathf.Abs(deltaY) > 2f)
            {
                if (deltaY > 2f)
                {
                    if (IsWhereToJumpUp())
                    {
                        movement.JumpUp();
                    }
                }
                else if (deltaY < -2f)
                {
                    if (IsWhereToJumpDown())
                    {
                        movement.JumpDown();
                    }
                }
            }

            if ((target.transform.position - this.transform.position).magnitude < 1.5f)
            {
                Invoke("Explode", 0.1f);
            }
        }
    }
    void Explode()
    {
        registry.damageAcceptors.doAreaDamage(this.gameObject, (Vector2)transform.position, damageRadius, damage, "demolition", knockback);

        if (effectOnExplode != null)
        {
            GameObject effect = Instantiate(effectOnExplode, gpParent.transform);
            effect.transform.position = this.transform.position;
            effect.transform.localScale *= damageRadius;
            Destroy(effect, 2f);
        }
        Destroy(this.gameObject);
    }
    bool MoveSomehowTowards(float direction)
    {
        if ((CanMoveTo(direction)))
        {
            //Debug.Log("Can move to");
            movement.MoveX(direction);
        }
        else if (CanJumpForward(direction))
        {
            //Debug.Log("Can JUMP F");
            movement.JumpUp();
            movement.MoveX(direction);
        }
        else if (movement.canPushSideTouch)
        {
            //Debug.Log("Can PUSH");
            movement.MoveX(direction);
        }
        else
        {
            return false;
        }
        return true;
    }
    bool CanMoveTo(float direction)
    {
        //if forward is free
        if (false == Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 2f, movement.layersToSense))
        {
            //if forward-down is a floor
            if (true == Physics2D.Raycast(transform.position + new Vector3(Mathf.Sign(direction) * 1, 0.1f, 0), new Vector3(0, -1, 0), 2f, movement.layersToSense))
            {
                return true;
            }
        }
        return false;
    }
    bool CanJumpForward(float direction)
    {
        //if 3m forward is free
        RaycastHit2D hitF = Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 3f, movement.layersToSense);
        if (hitF.collider == null)
        {
            //if somewhere far forward - far down is a floor
            for (int i = 2; i < 5; i++)
            {
                RaycastHit2D hitDown = Physics2D.Raycast(this.transform.position + (i * direction * Vector3.right), Vector3.down, 2f, movement.layersToSense);
                if (hitDown.collider != null)
                {
                    //Debug.Log(hitDown.collider.name);
                    return true;
                }
            }
        }
        else
        {
            //if up-forward is free
            RaycastHit2D hitUpF = Physics2D.Raycast(transform.position + new Vector3(0, 3, 0), new Vector3(Mathf.Sign(direction) * 1, 0, 0), 1f, movement.layersToSense);
            if (hitUpF.collider == null)
            {
                //Debug.Log("up forward is free");
                return true;
            }
        }
        return false;
    }
    bool IsWhereToJumpUp()
    {
        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + Vector3.up, Vector3.up, 4f);
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
    bool IsWhereToJumpDown()
    {
        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + (2 * Vector3.down), Vector3.up, 10f);
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
    #endregion
    #region  DamageAcceptor

    public void ReportKill(DamageAcceptor killed)
    {

    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (stats.isDead)
        {
            return;
        }
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
                Invoke("Explode", 0.1f);
                this.gameObject.SetActive(false);
            }
        }
    }
    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
    #endregion
}