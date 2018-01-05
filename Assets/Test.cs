using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, 1f);
        if (hit.collider != null)
        {
            Debug.Log("in the if");
        }
	}
}
