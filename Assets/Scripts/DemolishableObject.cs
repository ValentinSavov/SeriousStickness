using System.Collections.Generic;
using UnityEngine;

public class DemolishableObject : MonoBehaviour, DamageAcceptor
{
    Registry registry;
    public List<string> groups { get; set; }

    void Start ()
    {
        registry = GameObject.FindObjectOfType<Registry>().GetComponent<Registry>();
        registry.damageAcceptors.AddDamageAcceptor(this);
        groups = new List<string>();
        groups.Add("level");
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if (argInArgs.type == "demolition")
        {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        registry.damageAcceptors.RemoveDamageAcceptor(this);
    }
}