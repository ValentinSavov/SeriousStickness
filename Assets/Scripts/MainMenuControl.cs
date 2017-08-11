using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    SceneControl sceneControl;
    //public Texture2D cursorTexture;
    //public CursorMode cursorMode = CursorMode.Auto;

    void Start ()
    {
        UnityEngine.Cursor.visible = true;
        sceneControl = GameObject.FindObjectOfType<SceneControl>();
        //Cursor.SetCursor(cursorTexture, cursorMode);
    }
    
    public void OnPlay()
    {
        Debug.Log("PLAY button click");
        sceneControl.Play();
    }


}
