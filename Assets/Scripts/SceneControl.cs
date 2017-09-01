using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour 
{
    public string startScene = "MainMenu";
    public string deadScene = "Dead";

	void Start () 
	{
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
    }

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
    }

    public void Die()
    {
        RemoveLoadedScenes();
        SceneManager.LoadSceneAsync("Dead", LoadSceneMode.Additive);
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