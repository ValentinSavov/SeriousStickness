using UnityEngine;
using System.Collections.Generic;

public class SpiderProjectile : Projectile 
{
	new void Start () 
	{
        base.Start();
        startPosition = transform.position;
	}
	void Update()
	{
		transform.position += transform.right * speed * Time.deltaTime;

		if((transform.position - startPosition).sqrMagnitude > Mathf.Pow(distance,2))
		{
			Destroy(this.gameObject);
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{
        if ((Registry.instance != null))
        {
            if (/*(other.gameObject.GetComponent<BorderTag>() != null) ||*/ (other.gameObject.GetComponent<DamageAcceptor>() != null))
            {
                Registry.instance.damageAcceptors.doTargetDamage(other.gameObject.GetComponentInParent<DamageAcceptor>(), this.gameObject, damage, "normal",
                    (transform.right + (other.transform.position - transform.position).normalized) * 3000f);

                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }
}