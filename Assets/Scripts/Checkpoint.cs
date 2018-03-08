using UnityEngine;
using System.Collections.Generic;
public class Checkpoint : MonoBehaviour
{
    public delegate void DestructEvent();
    public event DestructEvent OnCheck;


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            transform.Find("ToEnable").gameObject.SetActive(true);
            transform.Find("ToDisable").gameObject.SetActive(false);
            if (OnCheck != null)
            {
                OnCheck();
            }
            Destroy(this);
        }
    }
}
