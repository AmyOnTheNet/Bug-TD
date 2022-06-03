using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class TowerScript : MonoBehaviour
{
	public bool targetGround;
	public bool targetFlying;

	public List<GameObject> targetsInRange;
	public int targettingMode;

	public int towerPrice;

	public GameObject towerWeapon;
	private GameObject currentTarget;

	public GameObject[] UIElements;
	public GameObject TurretCanvas;
	public GameObject LevelUpButton;
	public GameObject basePlatform;

	private int towerLevel;

	//Turret stats
	public float targettingRange;
	public float rangePerLevel;

	public int attackDamage;
	public float damagePerLevel;

	public float projectileVelocity;
	public float projectileVelocityPerLevel;

	public bool projectileExplosive;
	public float projectileExplosiveRadius;
	public float projectileExplosiveRadiusPerLevel;

	public float rateOfFire;
	public float rateOfFirePerLevel;

	public float aimingSpeed;
	public float aimingSpeedPerLevel;

	public bool projectileGuided;
	public float projectileAimingSpeed;

	public float attackSpread;

	public float multiShotDelay;

	public int numShot;

	public GameObject[] attackModifiers;

	// Start is called before the first frame update
	void Start()
	{
		targettingMode = 0; //0=First,1=Last,2=Closest,3=Farthest,4=Strongest,5=Weakest,6=Random
		UpdateUI();
	}

	// Update is called once per frame
	void Update()
	{
		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()) 
			return;

		if (GameObject.Find("GameLogic").GetComponent<GameLogic>().GetPlayerMoney() >= GetLevelUpPrice()) 
		{
			LevelUpButton.GetComponent<Button>().interactable = true;
		}else{
			LevelUpButton.GetComponent<Button>().interactable = false;
		}


		if (currentTarget != null) 
		{
			if (!currentTarget.GetComponent<EnemyScript>().IsAlive() || Vector3.Distance(currentTarget.transform.position, transform.position) > GetTurretRange()) 
			{
				currentTarget = null;
			}
			
		}else{
			targetsInRange.Clear();

			GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

			foreach (GameObject en in allEnemies) 
			{
				if (en.GetComponent<EnemyScript>().IsAlive() && Vector3.Distance(en.transform.position, transform.position) <= GetTurretRange() && !targetsInRange.Contains(en)) 
				{
					if (en.GetComponent<EnemyScript>().IsFlying() && targetFlying || !en.GetComponent<EnemyScript>().IsFlying() && targetGround) 
					{
						targetsInRange.Add(en);
					}
				}
			}

			//Targetting
			if (targetsInRange.Count > 0) 
			{
				if (targettingMode==0) 
				{
					currentTarget = targetsInRange[0];
				}
			}
		}
	}

	private bool b_isSelected;
	public void SetSelected(bool b_selected){
		if(b_isSelected != b_selected){
			b_isSelected = b_selected;

			if(b_isSelected){
				TurretCanvas.GetComponent<Animator>().SetTrigger("Show");
			}else{
				TurretCanvas.GetComponent<Animator>().SetTrigger("Hide");
			}
		}
	}

	void UpdateUI(){
		foreach (GameObject _go in UIElements) 
		{
			TMP_Text _txt = _go.GetComponent<TMP_Text>();

			if (_go.name.Contains("DAMAGE")){
				_txt.text = GetTurretDamage().ToString();

			}else if (_go.name.Contains("ROF")) {
				_txt.text = GetTurretRateOfFire().ToString("0.0");

			}else if (_go.name.Contains("ATTACK_CAP")) {
				_txt.text = GetTurretAttackCapabilties().ToString();

			}else if (_go.name.Contains("RANGE")) {
				_txt.text = GetTurretRange().ToString("0.0");

			}else if (_go.name.Contains("EXPLOSIVE_RADIUS")){
				_txt.text = GetTurretExplosiveRadius().ToString("0.0");

			}else if (_go.name.Contains("P_VELOCITY")){
				_txt.text = GetTurretProjectileVelocity().ToString("0.0");
				
			}else if (_go.name.Contains("R_SPEED")){
				_txt.text = GetTurretRotationSpeed().ToString("0.0");
				
			}else if (_go.name.Contains("SELL_VALUE")){
				_go.GetComponent<ButtonManagerBasicWithIcon>().buttonText = basePlatform.GetComponent<PlatformScript>().GetSellValue().ToString();
				_go.GetComponent<ButtonManagerBasicWithIcon>().UpdateUI();
				
			}else if (_go.name.Contains("INCR_COST")){
				_txt.text = GetLevelUpPrice().ToString();

			}else if (_go.name.Contains("T_LVL")){
				_txt.text = towerLevel.ToString();
			}
		}
	}

	public void SetTurretPlatform(GameObject platform){
		basePlatform = platform;
	}

	public void TowerLevelUp(){
		GameObject.Find("GameLogic").SendMessage("PlayerTakeMoney", GetLevelUpPrice());
		basePlatform.GetComponent<PlatformScript>().AddPlatformValue(GetLevelUpPrice());
		towerLevel += 1;
		UpdateUI();
		GameObject.Find("GameLogic").SendMessage("SetRangeFinderSize", GetTurretRange());
	}

	public void SetTowerLevel(int lvl){
		towerLevel = lvl;
		UpdateUI();
	}

	public void TowerUpgrade(GameObject upgradeGO){
		basePlatform.GetComponent<PlatformScript>().UpgradeTower(upgradeGO);
	}

	public int GetLevelUpPrice(){
		return (int)Mathf.Ceil(towerPrice * (Mathf.Pow(1.25f, towerLevel)));
	}

	public int GetTowerLevel(){
		return towerLevel;
	}

	public void SellTower(){
		basePlatform.GetComponent<PlatformScript>().SellPlatformTower();
	}

	public string GetName(){
		//localize in the future		
		return gameObject.name;
	}

	public int GetAttackCapabilities(){
		int ATTACK_CAP = 0;

		if (targetGround) 
		{
			ATTACK_CAP += 1;
		}
		if (targetFlying) 
		{
			ATTACK_CAP += 2;
		}

		return ATTACK_CAP;
	}

	public GameObject GetCurrentTarget(){
		if (currentTarget != null) 
		{
			return currentTarget;
		}else {
			return null;
		}
	}

	public string GetTurretAttackCapabilties(){
		switch (GetAttackCapabilities()) 
		{
			case 1:
				return "GROUND";

			case 2:
				return "FLYING";

			case 3:
				return "GROUND & FLY";

			default:
				return "NONE";
		}
	}

	public int GetTurretDamage(){
		return (int)Mathf.Round(attackDamage * (1 + damagePerLevel * GetTowerLevel()));
	}

	public float GetTurretRateOfFire(){
		return rateOfFire * (1 + rateOfFirePerLevel * GetTowerLevel());
	}

	public float GetTurretRange(){
		return targettingRange * (1 + rangePerLevel * GetTowerLevel());
	}

	public float GetTurretExplosiveRadius(){
		return projectileExplosiveRadius * (1 + projectileExplosiveRadiusPerLevel * GetTowerLevel());
	}

	public float GetTurretProjectileVelocity(){
		return projectileVelocity * (1 + projectileVelocityPerLevel * GetTowerLevel());
	}

	public float GetTurretRotationSpeed(){
		return aimingSpeed * (1 + aimingSpeedPerLevel * GetTowerLevel());
	}

	public GameObject[] GetAttackModifiers(){
		return attackModifiers;
	}
}
