using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smasher : MonoBehaviour
{
    Rigidbody2D rbd;
    Registry registry;
	void Start ()
    {
        rbd = GetComponent<Rigidbody2D>();
        registry = GameObject.FindObjectOfType<Registry>();
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        DamageAcceptor acceptor = collision.collider.GetComponent<DamageAcceptor>();
        if (acceptor != null)
        {
            if(collision.relativeVelocity.magnitude > 5f)
            {
                Debug.Log("damage");
                registry.damageAcceptors.doTargetDamage(acceptor, this.gameObject, 1000f, "", Vector2.zero);
            }
        }
    }

}
