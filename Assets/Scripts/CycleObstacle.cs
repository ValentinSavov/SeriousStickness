using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleObstacle : MonoBehaviour
{
    public float period = 1f;
    public float amplitude = 5f;
    //public enum MovementType
    //{
        //sin,
        //lin,
        //jump
    //};
    public MovementType movementType = MovementType.sin;
    public float damage = 10f;
    Vector3 startPos;
    float lerpTime = 0f;
    float lerpDir = 1f;

    void Start ()
    {
        startPos = transform.position;
    }

    float cooldown = 0f;
	void Update ()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0) cooldown = 0f;

        //if (movementType == MovementType.sin)
        {
            float theta = Time.timeSinceLevelLoad / period;
            float distance = amplitude * Mathf.Sin(theta);
            transform.position = startPos + transform.up * distance;
        }
        //if (movementType == MovementType.lin)
        {
            if (lerpDir == 1)
            {
                lerpTime += Time.deltaTime;
                if (lerpTime >= 1)
                {
                    lerpDir = -1f;
                }
            }
            else if (lerpDir == -1)
            {
                lerpTime -= Time.deltaTime;
                if (lerpTime <= 0)
                {
                    lerpDir = 1f;
                }
            }
            transform.position = Vector3.Lerp(startPos - transform.up * amplitude/2, startPos + transform.up * amplitude/2, lerpTime);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            if (cooldown == 0)
            {
                DoDamage(other.GetComponent<DamageAcceptor>(), damage, Vector2.zero);
                cooldown = 1f;
            }
        }
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
}
