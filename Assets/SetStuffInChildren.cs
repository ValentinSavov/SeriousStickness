using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStuffInChildren : MonoBehaviour
{
    public int orderInLayer = 0;
    public bool setorderinlayer = false;

	void Start ()
    {
        
	}
	

    void OnDrawGizmos()
    {
        if(setorderinlayer)
        {
            setorderinlayer = false;
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in rends)
            {
                rend.sortingOrder = orderInLayer;
            }
        }
    }
}
