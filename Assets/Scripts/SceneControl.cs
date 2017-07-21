using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour 
{
    public string sceneToReload = "main";
	//Object sceneController;
	void Start () 
	{
        //sceneController = this.gameObject as Object;
        //Object.DontDestroyOnLoad(sceneController);
        //InvokeRepeating("ReloadScene", 5f, 5f);

        //ReloadScene();
    }


    public void ReloadScene()
	{
		//SceneManager.UnloadScene ("Round");
		SceneManager.UnloadSceneAsync (sceneToReload);
		SceneManager.LoadScene (sceneToReload, LoadSceneMode.Additive);
    }
}
