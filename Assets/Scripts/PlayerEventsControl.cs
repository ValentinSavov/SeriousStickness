using UnityEngine;
using System.Collections;

public class PlayerEventsControl : MonoBehaviour
{
    public static PlayerEventsControl instance;

    public delegate void GPEvent();

    public event GPEvent OnExplosion;
    public event GPEvent OnGetHit;
    public event GPEvent OnHeal;

    void Awake()
    {
        instance = this;
    }

    public void TriggerEvent_OnExplosion()
    {
        if (OnExplosion != null)
        {
            OnExplosion();
        }
    }
}
