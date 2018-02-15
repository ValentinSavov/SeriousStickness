using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleEffector : MonoBehaviour
{
    GameObject gpParent;
    void Start()
    {
        gpParent = GameObject.Find("GeneralPurposeParent");
    }
    
    private void OnDestroy()
    {
        if (gpParent != null)
        {
            GameObject spawned = Instantiate(Resources.Load("BurnedHole", typeof(GameObject)), gpParent.transform) as GameObject;
            spawned.transform.position = this.transform.position;
            spawned.transform.rotation = this.transform.rotation;
        }
    }
}
