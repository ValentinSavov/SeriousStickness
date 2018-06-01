using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Buba : MonoBehaviour, DamageAcceptor
{
    Buba()
    {
        groups = new List<string>();
    }
    public float moveSpeed = 2f;
    public float damage = 5f;
    public List<string> groups { get; set; }
    public LayerMask layersToSense;
    public bool sideTouch = false;

    GameObject gpParent;
    Registry registry;
    Animator anim;
    Collider2D mainCollider;
    Rigidbody2D rbd;
    //GameObject target;
    float direction = 1;
    float hitCooldown = 0f;
    float previousEngageTime = 0f;
    float idleTime = 0f;
    float animSpeedCommand = 0f;
    GameObject head;
    void Start ()
	{
        gpParent = GameObject.Find("GeneralPurposeParent");
        anim = GetComponent<Animator>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        mainCollider = GetComponent<Collider2D>();
        rbd = GetComponent<Rigidbody2D>();
        //target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        head = transform.Find("Head").gameObject;
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("bots");
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

            animSpeedCommand = direction * Mathf.Sign(transform.localScale.x);
        }
        else
        {
            result = false;
        }
        return result;
    }

    bool canMove = true;
    bool targetIsNear = false;
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
                        Attack(da, damage, direction * 3000 * Vector2.right);
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

    float prevAttackTime = 0f;
    void Attack(DamageAcceptor arginAcceptor, float argInDamage, Vector2 argInKnockback)
    {
        if (Time.time - prevAttackTime > 2f)
        {
            prevAttackTime = Time.time;
            registry.damageAcceptors.doTargetDamage(
                        arginAcceptor,
                        GetComponentInParent<Tag>().gameObject,
                        argInDamage,
                        "normal",
                        argInKnockback);
        }
    }

    #endregion
    #region  DamageAcceptor

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        float locDamage = argInArgs.dmg;

        if (locDamage > 0)
        {
            head.SetActive(false);
            hitCooldown = 3f;
        }
    }
    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
    #endregion
}