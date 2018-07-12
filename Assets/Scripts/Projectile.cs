using UnityEngine;
using System.Collections.Generic;

public abstract class Projectile : MonoBehaviour 
{
	public float damage = 100;
    public float knockback = 5000f;
    public float explosionArea = 5f;
	public float speed = 5f;
	public float distance = 100f;
    public GameObject damageSource;
    protected Vector3 startPosition;

    protected void Start()
    {
        startPosition = transform.position;
    }

    protected void Explode()
    {
        Registry.instance.damageAcceptors.doAreaDamage(damageSource, (Vector2)transform.position, explosionArea, damage, "normal", knockback);

        GameObject explosion = Instantiate(Resources.Load("Explosion", typeof(GameObject)), this.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.parent = this.transform.parent;
        explosion.transform.localScale *= explosionArea;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        Destroy(explosion, 2f);
    }
}