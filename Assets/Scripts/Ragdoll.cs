using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour, DamageAcceptor
{
    //GameObject gpParent;
    Registry registry;

    public List<string> groups { get; set; }

    void Start ()
    {
        //gpParent = GameObject.Find("GeneralPurposeParent");
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        groups = new List<string>();
        groups.Add("level");
        registry.damageAcceptors.AddDamageAcceptor(this);
        Destroy(this.gameObject, 10f);
    }

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        Vector3 direction = (transform.position - argInArgs.source.transform.position).normalized;
        GameObject effect = 
            Instantiate(Resources.Load("BloodParticle", 
            typeof(GameObject)),
            transform.position, 
            Quaternion.FromToRotation(Vector3.right, direction), 
            transform.parent) 
            as GameObject;
        Push(argInArgs.knockback);
    }

    public void Push(Vector2 knockback)
    {
        AddForceToRandomBones(knockback);
    }

    void AddForceToRandomBones(Vector2 knockback)
    {
        Vector2 locKnockback = new Vector2(knockback.x / 2, knockback.y * 2);
        Rigidbody2D[] rbs = this.GetComponentsInChildren<Rigidbody2D>();
        if (rbs.Length > 2)
        {
            int rand1 = Random.Range(0, rbs.Length);
            int rand2 = Random.Range(0, rbs.Length);
            int rand3 = Random.Range(0, rbs.Length);
            rbs[rand1].AddForce(locKnockback / 3);
            rbs[rand2].AddForce(locKnockback / 4);
            rbs[rand3].AddForce(locKnockback / 5);
        }
    }
}