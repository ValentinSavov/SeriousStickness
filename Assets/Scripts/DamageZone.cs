using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class DamageZone : MonoBehaviour, DamageAcceptor, DamageProvider
{
    public float damage = 30f;
    //public float knockback = 2000;
    GameObject gpParent;
    public List<string> groups { get; set; }

    void Start ()
    {
        Registry.instance.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs) { }

    void OnDestroy()
    {
        Registry.instance.damageAcceptors.RemoveDamageAcceptor(this);
    }
    void OnDisable()
    {
        acceptorsInZone.Clear();
    }
    class AcceptorAndCooldown
    {
        public DamageAcceptor da;
        public float cooldown;
        public AcceptorAndCooldown(DamageAcceptor argInAcceptor)
        {
            da = argInAcceptor;
            cooldown = 1f;
        }
    }
    List<AcceptorAndCooldown> acceptorsInZone = new List<AcceptorAndCooldown>();

    void Update()
    {
        UpdateCooldowns();
    }
    void UpdateCooldowns()
    {
        foreach(AcceptorAndCooldown anc in acceptorsInZone)
        {
            if(anc.da == null)
            {
                acceptorsInZone.Remove(anc);
                break;
            }
            anc.cooldown -= Time.deltaTime;
            if(anc.cooldown <= 0)
            {
                anc.cooldown = 1f;
                DoDamage(anc.da, damage, new Vector2(0,0));
                break;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        DamageAcceptor otherDA = other.gameObject.GetComponent<DamageAcceptor>();
        if (otherDA != null)
        {
            acceptorsInZone.Add(new AcceptorAndCooldown(otherDA));
            DoDamage(otherDA, damage, new Vector2(0, 0));
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        DamageAcceptor otherDA = other.gameObject.GetComponent<DamageAcceptor>();
        if (otherDA != null)
        {
            AcceptorAndCooldown anc = acceptorsInZone.Find(x => x.da == otherDA);
            if (anc != null)
            {
                acceptorsInZone.Remove(anc);
            }
        }
    }
    void DoDamage(DamageAcceptor acceptor, float damage, Vector2 knockback)
    {
        Registry.instance.damageAcceptors.doTargetDamage(
                    acceptor,
                    gameObject,
                    damage,
                    "normal",
                    knockback);
    }

    public void ReportKill(DamageAcceptor killed)
    {
        AcceptorAndCooldown anc = acceptorsInZone.Find(x => x.da == killed);
        if (anc != null)
        {
            acceptorsInZone.Remove(anc);
        }
    }
}