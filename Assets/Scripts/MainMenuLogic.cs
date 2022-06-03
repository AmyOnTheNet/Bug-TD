using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
	public GameObject loadingScreenUI;
	public GameObject mainCanvasUI;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string sceneName){
		//Disable all other UI elements and stop gameplay stuff
		mainCanvasUI.SetActive(false);

		//Display the loading screen and load the next scene
		Instantiate(loadingScreenUI);
		StartCoroutine(LoadScene(sceneName));
	}

	IEnumerator LoadScene(string sceneName){
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(sceneName);
	}
}
