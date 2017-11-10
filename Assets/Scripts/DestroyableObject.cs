using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class DestroyableObject : MonoBehaviour , DamageAcceptor
{
    public GameObject effectOnDestroy;
    GameObject gpParent;
    Registry registry;
    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if(argInArgs.dmg > 0)
        {
            {
                Destroy(this.gameObject);
            }
        }
    }
    public List<string> groups { get; set; }

    void Start ()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
        gpParent = GameObject.Find("GeneralPurposeParent");
    }
	
	void Update ()
    {
		
	}

    void OnDestroy()
    {
        if ( (effectOnDestroy != null) && (gpParent != null) )
        {
            GameObject effect = Instantiate(effectOnDestroy, gpParent.transform);
            effect.transform.position = this.transform.position;
            Destroy(effect, 2f);
        }
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
}