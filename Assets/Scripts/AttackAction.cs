using System;
using UnityEngine;

public class AttackAction : GoapAction
{
	private bool done = false;

	public AttackAction() 
	{
		addPrecondition ("hasWeapon", true);
		addPrecondition ("playerIsDead", false);

		addEffect ("attackPlayer", true);
	}


	public override void reset ()
	{
        //vsa take it somehow from the registry instead:
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;

        //vsa think about the range to depends on the weapon
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            
            range = weapon.range;
        }
        //cost = 2;
        done = false;
	}

	public override bool isDone ()
	{
		return done;
	}

	public override bool requiresInRange ()
	{
		return true;
	}

	public override bool checkProceduralPrecondition (GameObject agent)
	{
        //vsa check if has ammo or weapon is not broken
        //vsa take it somehow from the registry instead:
        target = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        return target.gameObject != null;
	}


    float previousEngageTime = 0f;
    public override bool perform (GameObject agent)
	{
        // shoot stuff
        // done - this will be in the weapon in perform procedure - start animation of effect for shoot
        // done - activate shooting procedure for the  gun object
        // done - this will be checked by callng isDone- how to say that it is done ?
        
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            //if (weapon.isDone())
            {
                done = true;
            }
            //else 
            if ((Time.time - previousEngageTime) >= (1f / (GetComponent<StickStats>().attackSpeed/100) ))
            {
                previousEngageTime = Time.time;
                weapon.Engage(target);
            }  
            return true;
        }

        return false;
	}
}


