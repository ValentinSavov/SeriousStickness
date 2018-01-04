using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            SceneControl sceneControl = GameObject.FindObjectOfType<SceneControl>();
            if (sceneControl != null)
            {
                sceneControl.Invoke("EndLevel", 0.1f);
                Destroy(this.GetComponent<Collider2D>());
            }
        }
    }
}
