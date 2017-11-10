using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]

public class Enabler : MonoBehaviour
{
    public GameObject whatToEnableOnTrigger;
    public bool disableColliderAftertrigger = true;
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponentInParent<PlayerTag>())
        {
            if(disableColliderAftertrigger)
            {
                this.GetComponent<Collider2D>().enabled = false;
            }
            whatToEnableOnTrigger.SetActive(true);
        }
    }
}
