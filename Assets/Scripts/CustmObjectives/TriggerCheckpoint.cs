using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckpoint : Trigger
{
    void Update()
    {
        if(GetComponent<Checkpoint>() == null)
        Set();
    }
}

