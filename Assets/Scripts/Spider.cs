using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Spider : AIControl
{    
    float attackCooldown = 0f;
    public float speed = 4f;

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

    #region DamageAcceptor
    public override void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        //Debug.Log("MeleeAcceptDmg" + argInArgs.dmg);
        if (argInArgs.type == "melee")
        {
            argInArgs.dmg *= 3;
        }
        base.acceptDamage(argInArgs);
    }
    #endregion
}