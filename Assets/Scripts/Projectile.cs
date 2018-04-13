using UnityEngine;
using System.Collections.Generic;

public abstract class Projectile : MonoBehaviour 
{
	public float damage = 100;
    public float explosionArea = 5f;
	public float speed = 5f;
	public float distance = 100f;
    public GameObject damageSource;
    protected Registry registry;
    protected Vector3 startPosition;

    protected void Explode()
    {
        registry.damageAcceptors.doAreaDamage(damageSource, (Vector2)transform.position, explosionArea, damage, "normal", 5000f);

        GameObject explosion = Instantiate(Resources.Load("Explosion", typeof(GameObject)), this.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.parent = this.transform.parent;
        explosion.transform.localScale *= explosionArea;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        Destroy(explosion, 2f);
    }
}