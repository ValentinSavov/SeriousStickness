using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]

public class Enabler : MonoBehaviour
{
    public MonoBehaviour whatToEnableOnTrigger;
    [Tooltip("enable all diabled components on this gameobject")]
    public bool findDisbled = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponentInParent<PlayerTag>())
        {
            if (findDisbled)
            {
                MonoBehaviour[] components;
                components = this.gameObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour comp in components)
                {
                    comp.enabled = true;
                }
            }
            else
            {
                whatToEnableOnTrigger.enabled = true;
            }
            this.GetComponent<Collider2D>().enabled = false;
        }
    }
}
