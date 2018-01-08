using UnityEngine;
using System.Collections.Generic;
public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            transform.Find("ToEnable").gameObject.SetActive(true);
            transform.Find("ToDisable").gameObject.SetActive(false);
            Destroy(this);
        }
    }
}
