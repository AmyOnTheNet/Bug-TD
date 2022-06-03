using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    public void ChangeLevel(string levelname){
    	GameObject _sceneManager = GameObject.FindWithTag("SceneController");

    	_sceneManager.GetComponent<SceneController>().ChangeLevel(levelname);
    }
}
