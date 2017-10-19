using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveOpenDoor : Objective
{
    public bool completed = false;
    public override void ProcessCompletionAction()
    {
        completed = true;
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<PlayerTag>())
        {
            if(completed)
            {

            }
            else
            {

            }
        }
    }
}
