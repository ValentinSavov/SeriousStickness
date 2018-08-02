

using System;
using UnityEngine;

public class MeleeAttackAction : GoapAction
{
	//private GameObject player;
    Registry registry;
	private bool done = false;

	public MeleeAttackAction() 
	{
		addPrecondition ("playerIsDead", false);
		addEffect ("attackPlayer", true);
	}


	public override void reset ()
	{
        registry = GameObject.FindObjectOfType<Registry>();

        //vsa take it somehow from the registry instead:
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;

        //startTime = 0;
        range = 0.5f;
		//cost = 8;
	}

	public override bool isDone ()
	{
        bool tdone = done;
        done = false;
		return tdone;
	}

	public override bool requiresInRange ()
	{
		return true;
	}

	public override bool checkProceduralPrecondition (GameObject agent)
	{
        //vsa take it somehow from the registry instead:
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;

        return target.gameObject != null;
	}

    float previousEngageTime = 0f;
    public override bool perform (GameObject agent)
	{
        //vsa melee attack stuff
        // attack animation
        //  change this

        //engage somehow

        if ((Time.time - previousEngageTime) >= (1f / GetComponent<StickStats>().attackSpeed))
        {
            previousEngageTime = Time.time;

            registry.damageAcceptors.doTargetDamage(
                target.GetComponent<DamageAcceptor>(),
                this.gameObject,
                100f, "melee", new Vector2(0, 0));

            done = true;
        }
		return true;
	}

}