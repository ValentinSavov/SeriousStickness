using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickyFlyBotControl : AIControl
{
    Vector3 flyAroundPos;
    public float preferedAmplitude = 5f;

    public float damage = 5f;
    public float knockback = 2000f;
    public float preAttackTime = 0.5f;
    public float attackFrequency = 0.4f;
    public float attackMoveSpeed = 5f;
    public float moveSpeed = 2f;

    float attackCooldown = 2f;
    float lookAtDirection = 1;
    public bool isAttacking = false;
    Animator anim;
    bool rotate = false;
    
    new void Start()
    {
        base.Start();
        flyAroundPos = transform.position;
        //period = Random.Range(period * 0.9f, period * 1.1f);
        anim = GetComponent<Animator>();
        attackDistance = preferedAmplitude * 2;
        preAttackCooldown = preAttackTime;
        weap = transform.Find("Weapon");
    }
    
    #region AI
    void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown < 0f)
        {
            attackCooldown = 0f;
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        if (isAttacking)
        {
            ProcessAttackState();
            flyAroundPos = transform.position;
        }
        else
        {
            ProcessChaseState();
            //attackDirection = target.transform.position - transform.position;
            //attackDistance = preferedAmplitude * 2f;
        }
    }
    
    void ProcessChaseState()
    {
        anim.SetBool("PreAttack", false);
        flyAroundPos = Vector3.Lerp(flyAroundPos, target.transform.position, Time.deltaTime);
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        //radius hysteresis
        if (rotate)
        {
            if (distanceToTarget > preferedAmplitude * 1.5f)
            {
                rotate = false;
            }
        }
        else
        {
            if (distanceToTarget < preferedAmplitude)
            {
                rotate = true;
            }
            
        }

        if (distanceToTarget < (preferedAmplitude / 2))
        {
            //Debug.Log("distantiate");
            transform.Translate((transform.position - target.transform.position).normalized * moveSpeed/3 * Time.deltaTime);
        }

        if (rotate)
        {
            //rotate around the target
            float angleSpeed = Mathf.Rad2Deg * (moveSpeed * distanceToTarget) / (Mathf.Pow(distanceToTarget, 2));
            RotateAroundPivotExtensions.RotateAroundPivot(transform, flyAroundPos, new Vector3(0, 0, (Time.deltaTime * angleSpeed)%360));
        }
        else
        {
            // move towards the target
            transform.Translate((flyAroundPos - transform.position).normalized * Time.deltaTime * moveSpeed);
        }
        lookAtDirection = Mathf.Sign(target.transform.position.x - transform.position.x);
        this.transform.localScale = new Vector3(lookAtDirection, 1, 1);
        transform.rotation = Quaternion.identity;

        
        
    }
    Transform weap;
    public GameObject firePrefab;
    //Vector3 attackDirection;
    public float attackDistance;
    //public float passedAttackDistance = 0f;
    float preAttackCooldown;
    float passedFireTime = 0f;
    //bool fired = false;
    void ProcessAttackState()
    {
        preAttackCooldown -= Time.deltaTime;
        if (preAttackCooldown <= 0f)
        {
            preAttackCooldown = 0f;
            //anim.SetTrigger("Attack");
            /*Vector3 translation = (attackDirection).normalized * Time.deltaTime * attackMoveSpeed * Mathf.Sin(90f * (1 - passedAttackDistance/attackDistance) );
            transform.position += translation;
            //transform.Translate(translation);
            passedAttackDistance += translation.magnitude;
            if (passedAttackDistance >= attackDistance)
            {
                passedAttackDistance = 0f;
                attackCooldown = 1 / attackFrequency;
                preAttackCooldown = preAttackTime;
            }*/
            passedFireTime += Time.deltaTime;
            if (passedFireTime <= 1f)
            {
                ParticleSystem fire = weap.GetComponentInChildren<ParticleSystem>();//Instantiate(firePrefab, transform.position, Quaternion.identity, transform);
                fire.Play();
                Debug.Log("FIRE_Flamethrower");

                RaycastHit2D[] hits = Physics2D.RaycastAll(fire.transform.position, fire.transform.forward, passedFireTime * fire.startSpeed);
                //Debug.DrawLine(fire.transform.position, fire.transform.position + (fire.transform.forward * passedFireTime * fire.startSpeed), Color.green);

                foreach(RaycastHit2D hit in hits)
                {
                    DamageAcceptor da = hit.collider.GetComponent<DamageAcceptor>();
                    if (da != null)
                    {
                        DoDamage(da, damage, fire.transform.forward * knockback);
                    }
                }
            }
            else
            {
                passedFireTime = 0f;
                //fired = false;
                ParticleSystem fire = weap.GetComponentInChildren<ParticleSystem>();//Instantiate(firePrefab, transform.position, Quaternion.identity, transform);
                fire.Stop();
                attackCooldown = 1 / attackFrequency;
                preAttackCooldown = preAttackTime;

                weap.transform.rotation = Quaternion.identity;
                anim.SetFloat("LookAngle", 0);
            }
        }
        else
        {
            //anim.SetTrigger("PreAttack");

            // flamethrower rotate
            float degreesToRotate = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), (target.transform.position + Vector3.up) - weap.transform.position).eulerAngles.z;
            weap.transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);

            lookAtDirection = Mathf.Sign(target.transform.position.x - transform.position.x);
            this.transform.localScale = new Vector3(lookAtDirection, 1, 1);
            transform.rotation = Quaternion.identity;

            float lookAngleForAnimator = weap.transform.rotation.eulerAngles.z;
            if (Mathf.Abs(lookAngleForAnimator) > 90)
            {
                lookAngleForAnimator -= 360;
            }
            lookAngleForAnimator *= Mathf.Sign(transform.localScale.x);
            anim.SetFloat("LookAngle", lookAngleForAnimator);
            
        }
    }
    #endregion

    protected override void Die(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        Vector3 stickBodyPosition = transform.Find("StickBody").position;
        GameObject ragdoll = Instantiate(Resources.Load("Ragdoll", typeof(GameObject)),
        stickBodyPosition, Quaternion.identity, gpParent.transform) as GameObject;

        ragdoll.GetComponent<Ragdoll>().Push(argInArgs.knockback);

        Registry.instance.damageAcceptors.doAreaDamage(this.gameObject, (Vector2)transform.position, 5, damage, "normal", knockback);

        GameObject explosion = Instantiate(Resources.Load("Explosion", typeof(GameObject)), this.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.parent = this.transform.parent;
        explosion.transform.localScale *= 5;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        Destroy(explosion, 2f);
    }
}