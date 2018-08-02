using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class AIControl : MonoBehaviour, DamageAcceptor
{
    protected StickStats stats;
    public List<string> groups { get; set; }
    protected GameObject target;
    protected GameObject gpParent;

    protected AIControl()
    {
        groups = new List<string>();
    }

    protected void Start()
    {
        //Debug.Log("AIControl Start");
        Registry.instance.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("AI");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats = GetComponent<StickStats>();
        if (stats != null)
        {
            stats.currentHitPoints = stats.totalHitPoints;
            stats.currentArmorPoints = stats.totalArmorPoints;
        }
        gpParent = GameObject.Find("GeneralPurposeParent");
    }
    
    public virtual void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        //Debug.Log("AIAcceptDmg" + argInArgs.dmg);
        if (this.gameObject == null)
        {
            return;
        }
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
                Knockback(argInArgs.knockback);
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
                Die(argInArgs);
            }
        }
        GameObject healthbar = transform.Find("HealthBar").gameObject;
        if (healthbar != null)
        {
            healthbar.transform.Find("Level").GetComponent<Image>().fillAmount = stats.currentHitPoints / stats.totalHitPoints;
        }
    }

    protected virtual void Knockback(Vector2 knockback)
    {
        
    }
    protected virtual void Die(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject, 0.1f);
    }

    protected void OnDestroy()
    {
        //Debug.Log("AIDestroy");
        Registry.instance.damageAcceptors.RemoveDamageAcceptor(this);
    }

    protected virtual void DoDamage(DamageAcceptor acceptor, float damage, Vector2 knockback)
    {
        Registry.instance.damageAcceptors.doTargetDamage(
                    acceptor,
                    GetComponentInParent<Tag>().gameObject,
                    damage,
                    "normal",
                    knockback);
    }

    protected virtual void Bloodify(Vector3 point)
    {
        GameObject effect = Instantiate(Resources.Load("RandomBloodEffect", typeof(GameObject)), point, Quaternion.identity) as GameObject;
    }

    protected virtual void SpitBloodParticle(Vector3 direction, Vector3 point)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
        GameObject effect = Instantiate(Resources.Load("BloodParticle", typeof(GameObject)), point, rotation) as GameObject;
    }
}