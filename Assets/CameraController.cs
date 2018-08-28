using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float T = 0.05f;
    public AnimationCurve shakeCurve;
    public float cursorFollowStrength = 0.25f;
    Wiggle myWiggle;
    GameObject player;
    GameObject cursorObj;
    Vector3 centralPosition;

    void Start ()
    {
        cursorObj = GameObject.FindObjectOfType<CursorTag>().gameObject;
        player = GameObject.FindObjectOfType<PlayerTag>().gameObject;
        myWiggle = new Wiggle(0.2f, 50f, 0.2f, Vector2.zero);
        PlayerEventsControl.instance.OnExplosion += ShakeOnce;
        centralPosition = player.transform.position;
    }
    
    void FixedUpdate ()
    {
        float lerpAmount = Time.fixedDeltaTime / T;
        Vector3 fromPos = transform.position;
        Vector3 toPos = player.transform.position;
        float percentage = 95;
        if (ScreenCalculations.IsNearScreenBoundry(cursorObj.transform.position, percentage))
        {
            float amount = 1 - (
                ScreenCalculations.DistanceToNearestScreenBoundry(cursorObj.transform.position) /
                ((ScreenCalculations.GetScreenSizeFromNearestToTheOtherSide(cursorObj.transform.position)) * (1 - (percentage/100))) 
                );
            amount = Mathf.Clamp(amount, 0, 1);
            toPos = Vector3.Lerp(player.transform.position, cursorObj.transform.position, amount * cursorFollowStrength);
        }
        centralPosition = Vector3.Lerp(fromPos, toPos, lerpAmount);
        
        Vector3 cameraOffset = myWiggle.GetPosition(shakeCurve);

        transform.position = new Vector3(centralPosition.x + cameraOffset.x, centralPosition.y + cameraOffset.y, transform.position.z);
        //Debug.Log(ScreenCalculations.GetScreenSizeFromNearestToTheOtherSide(cursorObj.transform.position));
    }
    

    void ShakeOnce()
    {
        myWiggle.SetDecay(0.2f);
    }
}

public static class ScreenCalculations
{
    public static bool IsNearScreenBoundry(Vector3 pos, float screenPercentage)
    {
        float distance = DistanceToNearestScreenBoundry(pos);
        return (distance < (GetScreenSizeFromNearestToTheOtherSide(pos)) * (1 - (screenPercentage / 100)));
    }

    public static float GetSmallestScreenSide()
    {
        return Mathf.Min(Camera.main.pixelWidth, Camera.main.pixelHeight);
    }

    public static float GetLargestScreenSide()
    {
        return Mathf.Max(Camera.main.pixelWidth, Camera.main.pixelHeight);
    }

    public static float GetScreenSizeFromNearestToTheOtherSide(Vector3 pos)
    {
        Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(pos);
        if (Mathf.Min(positionOnScreen.x, Camera.main.pixelWidth - positionOnScreen.x) < Mathf.Min(positionOnScreen.y, Camera.main.pixelHeight - positionOnScreen.y))
        {
            return Camera.main.pixelWidth;
        }
        return Camera.main.pixelHeight;
    }

    public static float DistanceToScreenCenter(Vector3 pos)
    {
        return Vector3.Distance(Camera.main.WorldToScreenPoint(pos), new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
    }

    public static float DistanceToNearestScreenBoundry(Vector3 pos)
    {
        Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(pos);

        return Mathf.Min(Mathf.Min(positionOnScreen.x, Camera.main.pixelWidth - positionOnScreen.x),
                         Mathf.Min(positionOnScreen.y, Camera.main.pixelHeight - positionOnScreen.y));
    }
}