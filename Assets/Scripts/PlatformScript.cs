using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
	public Transform attachPoint;
	public GameObject currentTower;
	private bool built;

	private int platformValue;

	public float valueRefundFactor;

	// Start is called before the first frame update
	void Start()
	{
		platformValue = 0;
	}

	// Update is called once per frame
	void Update()
	{
		
	}
	
	public void PlaceTower(GameObject tower){
		currentTower = Instantiate(tower, attachPoint.position, attachPoint.rotation);
		TowerScript _towerComp = currentTower.GetComponent<TowerScript>();

		_towerComp.SetTurretPlatform(gameObject);

		AddPlatformValue(_towerComp.towerPrice);

		GameObject.Find("GameLogic").SendMessage("PlayerTakeMoney", _towerComp.towerPrice);
	}

	public void UpgradeTower(GameObject tower){
		int ct_lvl = currentTower.GetComponent<TowerScript>().GetTowerLevel();
		Debug.Log(ct_lvl);
		Destroy(currentTower);

		currentTower = Instantiate(tower, attachPoint.position, attachPoint.rotation);
		TowerScript _towerComp = currentTower.GetComponent<TowerScript>();

		_towerComp.SetTurretPlatform(gameObject);
		_towerComp.SetTowerLevel(ct_lvl);

		AddPlatformValue(_towerComp.towerPrice);
		GameObject.Find("GameLogic").SendMessage("PlayerTakeMoney", _towerComp.towerPrice);
		GameObject.Find("GameLogic").GetComponent<GameLogic>().SelectDirectly(gameObject);
	}

	public bool GetPlatformBuilt(){
		if (currentTower)
		{
			return true;
		}else {
			return false;
		}
	}

	public GameObject GetPlatformTower(){
		if (currentTower) 
		{
			return currentTower;
		}else {
			return null;
		}
	}

	public int GetPlatformValue(){
		return platformValue;
	}

	public void AddPlatformValue(int value){
		platformValue += value;
	}

	public void SellPlatformTower(){
		GameObject.Find("GameLogic").SendMessage("PlayerGiveMoney", platformValue * valueRefundFactor);

		Destroy(currentTower);
		currentTower = null;
		platformValue = 0;
		GameObject.Find("GameLogic").GetComponent<GameLogic>().SelectDirectly(gameObject);
	}

	public float GetSellValue(){
		return Mathf.Round(platformValue * valueRefundFactor);
	}
}