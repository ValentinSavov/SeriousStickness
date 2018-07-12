using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour 
{
    public string startScene = "MainMenu";
    public string deadScene = "DeadMenu";
    public string endScene = "EndMenu";
    void Start () 
	{
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
        //Screen.SetResolution(630, 500, false);
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
    }*/

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Unloaded scene: " + scene.name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name + ". Mode: " + mode.ToString());
        SceneManager.SetActiveScene(scene);
    }

    public void Play()
    {
        RemoveLoadedScenes();
        SceneManager.LoadSceneAsync("main", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Lvl0", LoadSceneMode.Additive);
    }

    public void Play(string sceneName)
    {
        RemoveLoadedScenes();
        SceneManager.LoadSceneAsync("main", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void Die()
    {
        RemoveLoadedScenes();
        SceneManager.LoadSceneAsync(deadScene, LoadSceneMode.Additive);
    }

    public void EndLevel()
    {
        RemoveLoadedScenes();
        SceneManager.LoadSceneAsync(endScene, LoadSceneMode.Additive);
    }

    void RemoveLoadedScenes()
    {
        List<string> scenesToUnload = new List<string>();
        if (SceneManager.sceneCount > 0)
        {
            for (int n = 0; n < SceneManager.sceneCount; ++n)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (scene.name != "SceneControl")
                {
                    scenesToUnload.Add(scene.name);
                }
            }
        }
        foreach (string scene in scenesToUnload)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }
}