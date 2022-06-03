using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{

	private TMP_Text c_text;

	// Start is called before the first frame update
	void Start()
	{
		c_text = GetComponent<TMP_Text>();

		int survivedWaves = GameObject.Find("GameLogic").GetComponent<GameLogic>().GetCurrentWave();
		int hiScore = PlayerPrefs.GetInt("HiScore");
		c_text.text = "You survived "+survivedWaves.ToString()+" waves! The Highest survived waves is "+hiScore.ToString()+"!";
	}

	// Update is called once per frame
	void Update()
	{
		int survivedWaves = GameObject.Find("GameLogic").GetComponent<GameLogic>().GetCurrentWave();
		int hiScore = PlayerPrefs.GetInt("HiScore");
		c_text.text = "You survived "+survivedWaves.ToString()+" waves!\nThe highest score is "+hiScore.ToString()+"!";
	}
}
