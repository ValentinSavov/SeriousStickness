using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    SceneControl sceneControl;
    public Texture2D cursorTexture;
    //public CursorMode cursorMode = CursorMode.Auto;

    void Start ()
    {
        UnityEngine.Cursor.visible = true;
        sceneControl = GameObject.FindObjectOfType<SceneControl>();
        //Cursor.SetCursor(cursorTexture);
    }
    
    public void OnPlay()
    {
        if (sceneControl != null)
        {
            sceneControl.Play();
        }
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
