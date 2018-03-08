using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckpoint : Trigger
{
    Checkpoint cp;
    void Update()
    {
        cp = GetComponent<Checkpoint>();
        cp.OnCheck += Set;
        this.enabled = false;
    }
}

