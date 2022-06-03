using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	private Animator c_anim;

	void Awake(){
		GameObject[] objs = GameObject.FindGameObjectsWithTag("SettingsManager");

		if (objs.Length > 1)
		{
			Destroy(this.gameObject);
		}
		
		DontDestroyOnLoad(this.gameObject);

		c_anim = GetComponent<Animator>();

		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualitySetting", 0), true);
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetMusicEnabled(int _musicEnabled){
		PlayerPrefs.SetInt("MusicEnabled", _musicEnabled);

		PlayerPrefs.Save();
	}

	public void SetGraphicsQuality(int _qIndex){
		QualitySettings.SetQualityLevel(_qIndex, true);

		PlayerPrefs.SetInt("QualitySetting", _qIndex);

		PlayerPrefs.Save();
	}

	public void Show(){
		c_anim.SetTrigger("Show");
	}
}
