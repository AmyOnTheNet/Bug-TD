using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;
using TMPro;

public class SceneController : MonoBehaviour
{
	private Animator c_anim;

	void Awake(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneController");

		if (objs.Length > 1)
		{
			Destroy(this.gameObject);
		}
		
		DontDestroyOnLoad(this.gameObject);

		c_anim = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void ChangeLevel(string sceneName){
		//Display the loading screen and load the next scene
		StartCoroutine(LoadScene(sceneName));
	}

	IEnumerator LoadScene(string sceneName){
		c_anim.SetTrigger("Show");
		yield return new WaitForSeconds(2f);

		SceneManager.LoadScene(sceneName);

		yield return new WaitForSeconds(1f);
		c_anim.SetTrigger("Hide");
		GameObject.FindWithTag("Jukebox").SendMessage("PlayRandom");
	}
}
