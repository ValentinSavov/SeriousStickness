using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class DestroyableObject : MonoBehaviour , DamageAcceptor
{
    Registry registry;

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if(argInArgs.dmg > 0)
        {
            Destroy(this.gameObject);
        }
    }
    public List<string> groups { get; set; }

    void Start ()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
    }
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
}