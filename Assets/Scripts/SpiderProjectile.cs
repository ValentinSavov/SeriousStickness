using UnityEngine;
using System.Collections.Generic;

public class SpiderProjectile : Projectile 
{
	void Start () 
	{
        registry = GameObject.FindObjectOfType<Registry>();
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
        if ((registry != null))
        {
            if (/*(other.gameObject.GetComponent<BorderTag>() != null) ||*/ (other.gameObject.GetComponent<DamageAcceptor>() != null))
            {
                registry.damageAcceptors.doTargetDamage(other.gameObject.GetComponentInParent<DamageAcceptor>(), this.gameObject, damage, "normal",
                    (transform.right + (other.transform.position - transform.position).normalized) * 3000f);

                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }
}