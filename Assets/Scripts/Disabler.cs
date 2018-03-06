using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler : MonoBehaviour
{
    public bool disableAfterTime = false;
    public float lifetime = 1f;

    float lifetimeLeft;
    void Start()
    {
        lifetimeLeft = lifetime;
    }

    void Update()
    {
        if(disableAfterTime)
        {
            lifetimeLeft -= Time.deltaTime;
            if(lifetimeLeft <= 0f)
            {
                lifetimeLeft = lifetime;
                this.gameObject.SetActive(false);
                return;
            }
        }
    }
}
