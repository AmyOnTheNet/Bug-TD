using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasableButton : MonoBehaviour
{
	public GameObject CostComponent;

	private TMP_Text _text;
	private Button _btn;
    // Start is called before the first frame update
    void Start()
    {
        _text = CostComponent.GetComponent<TMP_Text>();
        _btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        int cost = int.Parse(_text.text);

        if(GameObject.Find("GameLogic").GetComponent<GameLogic>().GetPlayerMoney() >= cost){
        	_btn.interactable = true;
        }else{
        	_btn.interactable = false;
        }
    }
}
