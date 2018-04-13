using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FlyingAI : MonoBehaviour, DamageAcceptor, DamageProvider
{
    FlyingAI()
    {
        groups = new List<string>();
    }
    public List<string> groups { get; set; }

    StickStats stats;
    Registry registry;
    GameObject target;
    Animator anim;

    Vector3 flyAroundPos;
    public bool randomizePeriods = true;
    public float Xamplitude = 5f;
    public float Xperiod = 0.4f;
    public float Yamplitude = 5f;
    public float Yperiod = 0.4f;

    public float damage = 5f;
    float attackCooldown = 0f;

    void Start ()
	{
        stats = GetComponent<StickStats>();
        anim = GetComponent<Animator>();
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups.Add("flies");
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        stats.currentHitPoints = stats.totalHitPoints;
        stats.currentArmorPoints = stats.totalArmorPoints;
        flyAroundPos = transform.position;

        if (randomizePeriods)
        {
            Xperiod = Random.Range(Xperiod * 0.9f, Xperiod * 1.1f);
            Yperiod = Xperiod / 2;
        }
    }
    
    #region AI
    void Update()
    {
        float theta = Time.timeSinceLevelLoad / Xperiod;
        float Xoffset = Xamplitude * Mathf.Sin(theta);
        theta = Time.timeSinceLevelLoad / Yperiod;
        float Yoffset = Yamplitude * Mathf.Sin(theta);

        if((target.transform.position - transform.position).magnitude < 20f)
        {
            flyAroundPos = Vector3.Lerp(flyAroundPos, target.transform.position, Time.deltaTime);
        }

        transform.position = flyAroundPos + new Vector3(Xoffset, Yoffset, 0f);


        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
        }

        if ((target.transform.position - transform.position).magnitude < 1f)
        {
            if ((attackCooldown <= 0f))
            {
                Attack(target.GetComponent<DamageAcceptor>(), damage, Vector2.zero);
                attackCooldown = 1f;
            }
        }
    }

    void Attack(DamageAcceptor acceptor, float argInDamage, Vector2 knockback)
    {
        registry.damageAcceptors.doTargetDamage(
                    acceptor,
                    GetComponentInParent<Tag>().gameObject,
                    argInDamage,
                    "normal",
                    knockback);
    }
    #endregion
    #region  DamageAcceptor

    public void ReportKill(DamageAcceptor killed) { }

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