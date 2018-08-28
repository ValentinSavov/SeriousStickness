using UnityEngine;
using System.Collections;

public class CursorAdjust : MonoBehaviour 
{
    //public Transform objectiveTarget;
	GameObject player;
	GameObject cursorObj;
    //GameObject objectivePointer;

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
        cursorObj = GameObject.FindObjectOfType<CursorTag>().gameObject;
        //objectivePointer = transform.Find("ObjectivePointer").gameObject;
        UnityEngine.Cursor.visible = false;
		UnityEngine.Cursor.lockState = CursorLockMode.Confined;
	}

	Vector3 prevMousePosition;
	Vector3 virtualPointerPosition;
	void Update () 
	{
        switch (cursorType)
		{
		case CursorType.CircleClamp:
			{
				Vector3 deltaMousePosition = (Input.mousePosition - prevMousePosition) / 20;
				virtualPointerPosition += deltaMousePosition;
				virtualPointerPosition = Vector3.ClampMagnitude(virtualPointerPosition, 5f);
                cursorObj.transform.localPosition = virtualPointerPosition;
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
                    cursorObj.transform.position = targetPoint;
				}
			}
			break;
		}
        
        /*if (objectiveTarget != null)
        {
            float degreesToRotateWeapon = Quaternion.FromToRotation(Vector3.right * Mathf.Sign(transform.localScale.x), objectiveTarget.transform.position - this.transform.position).eulerAngles.z;
            objectivePointer.transform.rotation = Quaternion.AngleAxis(degreesToRotateWeapon, Vector3.forward);
        }*/
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