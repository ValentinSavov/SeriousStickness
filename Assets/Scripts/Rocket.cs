using UnityEngine;
using System.Collections.Generic;

public class Rocket : Projectile
{
    new void Start()
    {
        base.Start();
    }
    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        if ((transform.position - startPosition).sqrMagnitude > Mathf.Pow(distance, 2))
        {
            Destroy(this.gameObject);
        }
        if (damageSource == null)
        {
            damageSource = this.gameObject;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((damageSource != null) && (Registry.instance != null))
        {
            if ((other.gameObject == damageSource.gameObject) || (other.transform.IsChildOf(damageSource.transform)))
            {
                return;
            }
            else if ((other.gameObject.GetComponent<BorderTag>() != null) || (other.gameObject.GetComponentInParent<DamageAcceptor>() != null))
            {
                Registry.instance.damageAcceptors.doTargetDamage(other.gameObject.GetComponentInParent<DamageAcceptor>(), damageSource, (int)(damage / 3), "normal",
                    (transform.right + (other.transform.position - transform.position).normalized) * 3000f);
                Explode();
                
            }
        }
    }
}