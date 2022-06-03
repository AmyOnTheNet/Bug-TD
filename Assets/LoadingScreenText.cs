using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadingScreenText : MonoBehaviour
{
	public TextAsset loadingPhrases;
	public float cycleDelay;

	private string[] linesFromfile;
	private TMP_Text textComponent;
	private float timer;
	// Start is called before the first frame update
	void Start()
	{
		linesFromfile = loadingPhrases.text.Split("\n"[0]);

		textComponent = GetComponent<TMP_Text>();

		textComponent.text = linesFromfile[Random.Range(0,linesFromfile.Length)];

		timer = cycleDelay;
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0f) 
		{
			timer = cycleDelay;

			textComponent.text = linesFromfile[Random.Range(0,linesFromfile.Length)];
		}
	}
}
