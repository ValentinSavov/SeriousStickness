﻿using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour 
{
	public float damage = 100;
	public float speed = 5f;
	public float distance = 100f;
	Vector3 startPosition;
    public GameObject parent;
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
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if( (other.gameObject == parent.gameObject) || (other.transform.IsChildOf(parent.transform)) )
        {
            return;
        }
        else if ( (other.gameObject.GetComponent<BorderTag>() != null) || (other.gameObject.GetComponent<DamageAcceptor>() != null))
        {
            registry.damageAcceptors.doAreaDamage(parent, (Vector2)transform.position, 5f, damage, "normal", 5000f);
            Explode();
        }
    }

    void Explode()
    {
        GameObject explosion = Instantiate(Resources.Load("Explosion", typeof(GameObject)), this.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.parent = this.transform.parent;
        Destroy(this.gameObject);
        Destroy(explosion, 2f);
    }
}