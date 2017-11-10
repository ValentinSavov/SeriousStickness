using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStuffInChildren : MonoBehaviour
{
    public string sortingLayerName;
    public bool setsortinglayer = false;
    public int orderInLayer = 0;
    public bool setorderinlayer = false;


    void OnDrawGizmos()
    {
        if (setsortinglayer)
        {
            setsortinglayer = false;
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                rend.sortingLayerName = sortingLayerName;
            }
        }

        if (setorderinlayer)
        {
            setorderinlayer = false;
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                rend.sortingOrder = orderInLayer;
            }
        }
    }
}
