using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoBehaviour
{
    public float lifetimeLeft = 0.5f;
    float speed = 1.1f;
    void Update ()
    {
        if(speed > 0.03f)
        {
            speed -= 0.03f;
        }
        
        this.transform.Translate(0, speed * 7 * Time.deltaTime, 0);
        this.transform.localScale *= 1 - Time.deltaTime/3;
        lifetimeLeft -= Time.deltaTime;
        if( (lifetimeLeft <= 0) /*|| (speed <= 0)*/ )
        {
            Destroy(this.gameObject);
        } 
	}

    public string text
    {
        get
        {
            return GetComponent<TextMesh>().text;
        }
        set
        {
            GetComponent<TextMesh>().text = value;
        }
    }
}
