using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStuffInChildren : MonoBehaviour
{
    public string sortingLayerName;
    public bool setsortinglayer = false;
    public int orderInLayer = 0;
    public bool setorderinlayer = false;
    public bool removeChild2DPhysicsStuff = false;


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

        if (removeChild2DPhysicsStuff)
        {
            removeChild2DPhysicsStuff = false;
            Collider2D[] cols = GetComponentsInChildren<Collider2D>();
            Joint2D[] joints = GetComponentsInChildren<Joint2D>();
            Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
            foreach (Collider2D col in cols)
            {
                DestroyImmediate(col);
            }
            foreach (Joint2D col in joints)
            {
                DestroyImmediate(col);
            }
            foreach (Rigidbody2D col in rbs)
            {
                DestroyImmediate(col);
            }
        }
    }
}
