using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDisableEnable : Objective
{
    public bool completed = false;
    public GameObject toDisable;
    public GameObject toEnable;
    public override void ProcessCompletionAction()
    {
        completed = true;
        toDisable.SetActive(false);
        toEnable.SetActive(true);
    }

    /*void OnTriggerEnter2D(Collider2D col)
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
    }*/
}
