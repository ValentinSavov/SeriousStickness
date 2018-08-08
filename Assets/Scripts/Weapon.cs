using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    protected Registry registry;
    protected GameObject gpParent;
    public bool isAutomatic = false;
    public int bullets = 20;
    public float range = 5f;
    public float damage = 100f;
    public Weapon(){}
    public abstract bool Engage(Vector3 newTarget);
    public void Drop()
    {
        Rigidbody2D rbd = this.gameObject.AddComponent<Rigidbody2D>();
        rbd.gravityScale *= 2;
        GetComponent<Collider2D>().isTrigger = false;
    }
}