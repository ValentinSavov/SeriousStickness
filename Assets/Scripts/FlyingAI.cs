using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FlyingAI : AIControl
{
    Vector3 flyAroundPos;
    public bool randomizePeriods = true;
    public float Xamplitude = 5f;
    public float Xperiod = 0.4f;
    public float Yamplitude = 5f;
    public float Yperiod = 0.4f;

    public float damage = 5f;
    public bool flyAroundStartPoint = false;

    float attackCooldown = 0f;
    float lookAtDirection = 1;

    new void Start ()
	{
        base.Start();
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

        lookAtDirection = Mathf.Sign(target.transform.position.x - transform.position.x);
        this.transform.localScale = new Vector3(lookAtDirection, 1, 1);

        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
        }

        if ((target.transform.position - transform.position).magnitude < 1f)
        {
            if ((attackCooldown <= 0f))
            {
                DoDamage(target.GetComponent<DamageAcceptor>(), damage, Vector2.zero);
                attackCooldown = 1f;
            }
        }
    }
    #endregion
}