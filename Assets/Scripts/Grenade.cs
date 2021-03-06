﻿using UnityEngine;
using System.Collections.Generic;

public class Grenade : Projectile, DamageAcceptor
{
    public float timeToExplode = 2f;
    public List<string> groups { get; set; }
    public float pulseAnimSpeed = 2f;
    Animator anim;
    new void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().AddForce(transform.right * speed, ForceMode2D.Impulse);
        groups = new List<string>();
        groups.Add("level");
        anim = GetComponentInChildren<Animator>();
        anim.speed = pulseAnimSpeed;
    }

    void OnDestroy()
    {
    }

    void Update()
    {
        timeToExplode -= Time.deltaTime;
        if (timeToExplode <= 0)
        {
            Explode();
        }
        if (damageSource == null)
        {
            damageSource = this.gameObject;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        ProcessCollision(coll.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other.gameObject);
    }

    void ProcessCollision(GameObject other)
    {
        if ((damageSource != null) && (Registry.instance != null))
        {
            if ((other.gameObject == damageSource.gameObject) || (other.gameObject.transform.IsChildOf(damageSource.transform)))
            {
                return;
            }
            else if ((other.gameObject.GetComponentInParent<DamageAcceptor>() != null))
            {
                Explode();
            }
        }
    }

    public void acceptDamage(DamageAcceptorRegistry.DamageArgs argInArgs)
    {
        if(argInArgs.dmg > 0)
        {
            Explode();
        }
    }
}