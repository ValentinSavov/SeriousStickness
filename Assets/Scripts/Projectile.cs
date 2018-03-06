using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour 
{
	public float damage = 100;
    public float explosionArea = 5f;
	public float speed = 5f;
	public float distance = 100f;
	Vector3 startPosition;
    public GameObject damageSource;
    Registry registry;

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
        if(damageSource == null)
        {
            damageSource = this.gameObject;
        }
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if(damageSource)
        if( (other.gameObject == damageSource.gameObject) || (other.transform.IsChildOf(damageSource.transform)) )
        {
            return;
        }
        else if ( (other.gameObject.GetComponent<BorderTag>() != null) 
            || (other.gameObject.GetComponentInParent<DamageAcceptor>() != null))
        {
            registry.damageAcceptors.doTargetDamage(other.gameObject.GetComponentInParent<DamageAcceptor>(), damageSource, damage, "normal",
                (transform.right + (other.transform.position - transform.position).normalized) * 5000f);
            registry.damageAcceptors.doAreaDamage(damageSource, (Vector2)transform.position, explosionArea, damage, "normal", 5000f);
            Explode();
        }
    }

    void Explode()
    {
        GameObject explosion = Instantiate(Resources.Load("Explosion", typeof(GameObject)), this.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.parent = this.transform.parent;
        explosion.transform.localScale *= explosionArea;
        Destroy(this.gameObject);
        Destroy(explosion, 2f);
    }
}