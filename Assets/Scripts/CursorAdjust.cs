using UnityEngine;
using System.Collections;

public class CursorAdjust : MonoBehaviour 
{
	GameObject player;
	GameObject cursorr;

	public enum CursorType
	{
		CircleClamp,
		Free,

	}
	public CursorType cursorType = CursorType.Free;

	//float degreesToRotate = 0;

	void Start()
	{
		player = GameObject.FindObjectOfType<PlayerTag>().gameObject;
		cursorr = GameObject.FindObjectOfType<CursorTag>().gameObject;
		UnityEngine.Cursor.visible = false;
		UnityEngine.Cursor.lockState = CursorLockMode.Confined;
	}

	Vector3 prevMousePosition;
	Vector3 virtualPointerPosition;
	void Update () 
	{/*
		if(Input.GetButtonDown("o"))
		{
			if(UnityEngine.Cursor.lockState == CursorLockMode.None)
			{
				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.Confined;
			}
			else if(UnityEngine.Cursor.lockState == CursorLockMode.Confined)
			{
				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.None;
			}
		}*/
		/*
		Plane playerPlane = new Plane(new Vector3(0,0,-1), transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float hitdist = 0.0f;

		if (playerPlane.Raycast (ray, out hitdist)) 
		{
			Vector3 targetPoint = ray.GetPoint(hitdist);

			Quaternion tempRot = Quaternion.FromToRotation(Vector3.right, (targetPoint - transform.position));

			degreesToRotate = tempRot.eulerAngles.z;

			transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);
		}
		*/

		switch(cursorType)
		{
		case CursorType.CircleClamp:
			{
				Vector3 deltaMousePosition = (Input.mousePosition - prevMousePosition) / 20;
				virtualPointerPosition += deltaMousePosition;
				virtualPointerPosition = Vector3.ClampMagnitude(virtualPointerPosition, 5f);

				cursorr.transform.localPosition = virtualPointerPosition;

				//Quaternion tempRot = Quaternion.FromToRotation(Vector3.right, deltaMousePosition);
				//degreesToRotate = tempRot.eulerAngles.z;
				//transform.rotation = Quaternion.AngleAxis(degreesToRotate, Vector3.forward);

				transform.position = player.transform.position;
				prevMousePosition = Input.mousePosition;
			}
			break;
		case CursorType.Free:
			{
				transform.position = player.transform.position;

				Plane playerPlane = new Plane(new Vector3(0,0,-1), transform.position);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				float hitdist = 0.0f;

				if (playerPlane.Raycast (ray, out hitdist))
				{
					Vector3 targetPoint = ray.GetPoint(hitdist);
					cursorr.transform.position = targetPoint;
				}
			}
			break;
		}
	}


    Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }



}