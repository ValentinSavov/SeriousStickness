using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickyFlyBotControl : AIControl
{
    Vector3 flyAroundPos;
    public float amplitude = 5f;
    public float period = 0.4f;

    public float damage = 5f;
    public float attackSpeed = 0.4f;

    float attackCooldown = 0f;
    float lookAtDirection = 1;
    bool isAttacking = false;
    Animator anim;

    new void Start()
    {
        base.Start();
        flyAroundPos = transform.position;
        period = Random.Range(period * 0.9f, period * 1.1f);
        anim = GetComponent<Animator>();
    }
    
    #region AI
    void Update()
    {
        /*attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
            isAttacking = true;
        }

        if (isAttacking)
        {
            ProcessAttackState();
            flyAroundPos = transform.position;
        }
        else
        {
            ProcessIdleState();
            lastTargetPosition = target.transform.position;
        }
        */
        //transform.RotateAround(this.transform.position, Vector3.forward, 20 * Time.deltaTime);
        RotateAroundPivotExtensions.RotateAroundPivot(transform, flyAroundPos, new Vector3(0, 0, Time.deltaTime * 360));
    }

    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        Vector3 finalPos = point - pivot;
        //Center the point around the origin
        finalPos = angle* finalPos;
        //Rotate the point.
        finalPos += pivot;
        //Move the point back to its original offset. 
        return finalPos;

    }

    void ProcessIdleState()
    {
        float theta = Time.timeSinceLevelLoad / period;
        float Xoffset = amplitude * Mathf.Sin(theta);
        theta = Time.timeSinceLevelLoad / period;
        float Yoffset = amplitude * Mathf.Cos(theta);

        if ((target.transform.position - transform.position).magnitude < 20f)
        {
            flyAroundPos = Vector3.Lerp(flyAroundPos, target.transform.position, Time.deltaTime);
        }

        transform.position = flyAroundPos + new Vector3(Xoffset, Yoffset, 0f);

        lookAtDirection = Mathf.Sign(target.transform.position.x - transform.position.x);
        this.transform.localScale = new Vector3(lookAtDirection, 1, 1);
    }


    Vector3 lastTargetPosition;
    Vector3 startAttackPosition;
    public float preAttackTime = 0.5f;
    float attackTime = 0f;
    void ProcessAttackState()
    {
        anim.SetBool("PreAttack", true);
        attackTime += Time.deltaTime;
        if(attackTime >= preAttackTime)
        {
            transform.position = Vector3.Lerp(startAttackPosition, lastTargetPosition, attackTime - preAttackTime);
            if((attackTime - preAttackTime) >= 1f)
            {
                attackTime = 0f;
                isAttacking = false;
                attackCooldown = 1 / attackSpeed;
            }
        }
        else
        {
            startAttackPosition = transform.position;
        }
    }
    #endregion
}