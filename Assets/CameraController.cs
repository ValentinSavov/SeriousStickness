using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Wiggle myWiggle;

    void Start ()
    {
        myWiggle = new Wiggle(0.2f, 50f, 0f, Vector2.zero);
    }
	
	void Update ()
    {
        Vector3 cameraOffset = myWiggle.GetPosition();
        transform.position += cameraOffset;

    }
}
