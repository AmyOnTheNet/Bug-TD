using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextShadow : MonoBehaviour
{
	public GameObject textSource;

	void Update()
	{
		GetComponent<Text>().text = textSource.GetComponent<Text>().text;
	}
}
