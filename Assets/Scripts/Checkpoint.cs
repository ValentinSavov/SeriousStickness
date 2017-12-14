using UnityEngine;
using System.Collections.Generic;
public class Checkpoint : MonoBehaviour
{
    public bool isFirst = false;
    //public bool isLast = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            transform.Find("ToEnable").gameObject.SetActive(true);
            Destroy(this);
        }
    }
}
