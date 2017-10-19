using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroyObject : Trigger
{
    void OnDestroy()
    {
        Set();
    }
}
