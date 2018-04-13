using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Spider : MonoBehaviour, DamageAcceptor, DamageProvider
{
    Spider()
    {
        groups = new List<string>();
    }
    public List<string> groups { get; set; }

    
    StickStats stats;
    Registry registry;
    GameObject target;
    Animator anim;
    GameObject gpParent;
    float attackCooldown = 0f;

    public float speed = 4f;
    //public float Yamplitude = 1f;
    //public float Yperiod = 1f;

    void Start ()
	{
        stats = GetComponent<StickStats>();
        anim = GetComponent<Animator>();
        gpParent = GameObject.Find("GeneralPurposeParent");
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("flies");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
    }
    
    #region AI
    void Update()
    {
        if(((target.transform.position - transform.position).magnitude) <= 20f)
        {
            if (Mathf.Abs(target.transform.position.x - transform.position.x) <= 3f)
            {
                transform.position = new Vector3(transform.position.x,
                    Mathf.Lerp(transform.position.y, target.transform.position.y + 8f, Time.deltaTime * speed),
                    transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x,
                    Mathf.Lerp(transform.position.y, target.transform.position.y + 3f, Time.deltaTime * speed),
                    transform.position.z);
            }
        }

        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
        }

        if ((target.transform.position - transform.position).magnitude < 20f)
        {
            if ((attackCooldown <= 0f))
            {
                GameObject proj = Instantiate(Resources.Load("SpiderProjectile", typeof(GameObject)),
                transform.position + (target.transform.position - transform.position).normalized, Quaternion.FromToRotation(Vector3.right,
                target.transform.position - transform.position))
                as GameObject;
                proj.transform.parent = gpParent.transform;

                attackCooldown = 1f;
            }
        }
    }

    #endregion
    #region  DamageAcceptor

    public void ReportKill(DamageAcceptor killed)
    {

    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if(this.gameObject == null)
        {
            return;
        }
        if (stats.isDead)
        {
            return;
        }
        float locDamage = argInArgs.dmg;
        if(argInArgs.type == "melee")
        {
            locDamage *= 3;
        }
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

                this.gameObject.SetActive(false);
                Destroy(this.gameObject, 0.1f);
            }
        }
        GameObject healthbar = transform.Find("HealthBar").gameObject;
        if (healthbar != null)
        {
            healthbar.transform.Find("Level").GetComponent<Image>().fillAmount = stats.currentHitPoints / stats.totalHitPoints;
        }
    }
    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
    #endregion
}