using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleObstacle : AIControl
{
    public float period = 1f;
    public float amplitude = 5f;
    public float rotationSpeed = 1f;
    public enum MovementType
    {
        sin,
        lin,
        //jump
    };
    public MovementType movementType = MovementType.sin;
    public float damage = 10f;
    Vector3 startPos;
    float lerpTime = 0f;
    float lerpDir = 1f;
    float cooldown = 0f;

    Vector3 up;
    new void Start ()
    {
        //base.Start();
        startPos = transform.position;
        up = transform.up;
    }
    
	void Update ()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0) cooldown = 0f;

        if (movementType == MovementType.sin)
        {
            float theta = Time.timeSinceLevelLoad / period;
            float distance = amplitude * Mathf.Sin(theta);
            transform.position = startPos + up * distance;
        }
        if (movementType == MovementType.lin)
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
            transform.position = Vector3.Lerp(startPos - up * amplitude/2, startPos + up * amplitude/2, lerpTime);
        }

        transform.RotateAround(transform.position, Vector3.forward, 360 * rotationSpeed * Time.deltaTime);
    }

    public override void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        return;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<DamageAcceptor>() != null)
        {
            if (cooldown == 0)
            {
                //Debug.Log("trigger " + other.gameObject.name);
                DoDamage(other.gameObject.GetComponent<DamageAcceptor>(), damage, Vector2.zero);
                cooldown = 1f;
            }
        }
    }
}
