using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    protected Registry registry;
    protected GameObject gpParent;
    public bool isAutomatic = false;
    public float range = 5f;
    public List<string> groups = new List<string>();
    public float damage = 100f;

    public Weapon(){}

    public abstract void Arm();
    public abstract void Disarm();

    public abstract bool Engage(Vector3 newTarget);
}