using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour 
{
	//Object sceneController;
	void Start () 
	{
		//sceneController = this.gameObject as Object;
		//Object.DontDestroyOnLoad(sceneController);
		//InvokeRepeating("ReloadScene", 5f, 5f);

		//SceneManager.UnloadScene ("Round");
		SceneManager.UnloadSceneAsync ("Round");
		SceneManager.LoadScene ("Round", LoadSceneMode.Additive);
	}
	

	public void ReloadScene()
	{
		//SceneManager.UnloadScene ("Round");
		SceneManager.UnloadSceneAsync ("Round");
		SceneManager.LoadScene ("Round", LoadSceneMode.Additive);
	}
}
