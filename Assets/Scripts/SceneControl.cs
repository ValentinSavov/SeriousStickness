using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour 
{
    public string sceneToReload = "main";

	void Start () 
	{
        //sceneController = this.gameObject as Object;
        //Object.DontDestroyOnLoad(sceneController);
        //InvokeRepeating("ReloadScene", 5f, 5f);
        //SceneManager.LoadScene(sceneToReload, LoadSceneMode.Additive);
        if (!Debug.isDebugBuild)
            ReloadScene();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Unloaded scene: " + scene.name);
        if (scene.name == sceneToReload)
        {
            SceneManager.LoadScene(sceneToReload, LoadSceneMode.Additive);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name + ". Mode: " + mode.ToString());
    }

    public void ReloadScene()
	{
		SceneManager.UnloadSceneAsync (sceneToReload);
    }
}
