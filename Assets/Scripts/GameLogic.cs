using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Michsky.UI.ModernUIPack;
using TMPro;

public class GameLogic : MonoBehaviour
{
	public int playerStartingHealth;
	public int playerStartingMoney;

	public float waveDuration;
	public float waveCallInterval;
	public GameObject[] enemyTypes;

	private int playerHealth;
	private int playerMoney;
	private int currentWave;

	private bool spawning;
	private bool firstWaveCalled;
	private bool waveCallAuto;
	private float waveCallTimer;
	private float waveDurationTimer;
	private float waveSpawnTimer;
	private float waveSpawnDelay;
	private float waveEnemyCount;

	private GameObject enemyPortal;

	private GameObject platformSelector;
	private GameObject rangeFinder;
	private GameObject CameraDolly;

	private GameObject selectedPlatform;

	//UI Elements
	private GameObject playerHealthUI;
	private GameObject playerMoneyUI;
	private GameObject levelWaveUI;
	private GameObject buttonCallWave;
	private GameObject waveCallTimerUI;
	private GameObject waveCallAutoUI;

	public GameObject mainCanvasUI;

	public GameObject gameOverUI;

	private bool gameEnded;
	private bool gamePaused;

	private List<GameObject> BuyButtons = new List<GameObject>();

	private int enemyTypeIndex;

	// Start is called before the first frame update
	void Start()
	{
		playerHealth = playerStartingHealth;
		playerMoney = playerStartingMoney;
		currentWave = 0;
		waveEnemyCount = 10 + currentWave;
		firstWaveCalled = false;

		gameEnded = false;

		enemyPortal = GameObject.Find("EnemyPortal");
		platformSelector = GameObject.Find("PlatformSelector");
		rangeFinder = GameObject.Find("RangeFinder");
		CameraDolly = GameObject.Find("CameraDolly");

		//Setup UI Elements
		playerHealthUI = GameObject.Find("UIElement_PlayerHealth");
		playerMoneyUI = GameObject.Find("UIElement_PlayerMoney");
		levelWaveUI = GameObject.Find("UIElement_LevelWave");
		buttonCallWave = GameObject.Find("UIElement_ButtonCallWave");
		waveCallTimerUI = GameObject.Find("UIElement_WaveCallTimer");
		
		waveCallAutoUI = GameObject.Find("UIElement_AutoToggle");

		playerHealthUI.GetComponent<TMP_Text>().text = playerHealth.ToString();
		playerMoneyUI.GetComponent<TMP_Text>().text = playerMoney.ToString();
		levelWaveUI.GetComponent<TMP_Text>().text = currentWave.ToString();
		buttonCallWave.GetComponent<Button>().interactable = true;
		platformSelector.SetActive(false);
		rangeFinder.SetActive(false);

		int _i = 0;
		foreach (GameObject _item in GetChildren(GetChildren(GameObject.Find("UIElement_BuyPanel"))[0])) 
		{
			BuyButtons.Add(_item);
			_i++;
		}

		BuyPanelManager(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (gameEnded || gamePaused){
			return ;
		}

		if (spawning) 
		{
			waveSpawnTimer -= Time.deltaTime;
			waveDurationTimer -= Time.deltaTime;

			if (waveSpawnTimer <= 0f) 
			{
				Debug.Log("SPAWN");
				enemyPortal.GetComponent<EnemyPortal>().SpawnEnemy(enemyTypes[enemyTypeIndex], currentWave);
				waveSpawnTimer = waveSpawnDelay;
			}

			waveCallTimerUI.GetComponent<ProgressBar>().currentPercent = Mathf.Lerp(100f, 0f, waveDurationTimer/waveDuration);

			if (waveDurationTimer <= 0f) 
			{
				spawning = false;
				buttonCallWave.GetComponent<Button>().interactable = true;
				
				if (waveCallAuto) 
				{
					waveCallTimer = 0f;
				}else {
					waveCallTimer = waveCallInterval;
				}

				Debug.Log("WAVE ENDED");
			}
		}else if (firstWaveCalled) {
			waveCallTimer -= Time.deltaTime;

			waveCallTimerUI.GetComponent<ProgressBar>().currentPercent = (waveCallTimer/waveCallInterval)*100;

			if (waveCallTimer <= 0f) 
			{
				CallNextWave();
			}
		}

		if (GameObject.Find("UIElement_BuyPanel").activeSelf) 
		{
			foreach (GameObject btn in BuyButtons) 
			{
				int cost = int.Parse(GetChildren(btn)[0].GetComponent<TMP_Text>().text);

				if (cost > playerMoney) 
				{
					btn.GetComponent<Button>().interactable = false;
				}
				else {
					btn.GetComponent<Button>().interactable = true;
				}
			}
		}
	}

	public void HandleTouch(int i_tid){

		Touch touch = Input.GetTouch(i_tid);
		
		if (!EventSystem.current.IsPointerOverGameObject(i_tid)) 
		{
			Ray ray;
			RaycastHit hit;

			ray = Camera.main.ScreenPointToRay(touch.position);

			if(Physics.Raycast(ray, out hit))
			{
				InGameClick(hit);
			}
		}
	}

	public void HandleMouse(){
		if (!EventSystem.current.IsPointerOverGameObject()) 
		{
			Ray ray;
			RaycastHit hit;

			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(ray, out hit))
			{
				InGameClick(hit);
			}
		}
	}
	public void SelectDirectly(GameObject platform){
		if(platform.tag == "Platform"){
			if(selectedPlatform){
				if (selectedPlatform.GetComponent<PlatformScript>().GetPlatformBuilt()) {
					selectedPlatform.GetComponent<PlatformScript>().GetPlatformTower().GetComponent<TowerScript>().SetSelected(false);
				}
			}

			selectedPlatform = platform;
			PlatformScript pscript = selectedPlatform.GetComponent<PlatformScript>();

			platformSelector.SetActive(true);
			platformSelector.transform.position = selectedPlatform.transform.position;
			//wp.GetComponent<TowerScript>().SetSelected(false);

			if (pscript.GetPlatformBuilt()) 
			{
				BuyPanelManager(false);
				rangeFinder.SetActive(true);
				GameObject t_tower = pscript.GetPlatformTower();
				TowerScript _tower = t_tower.GetComponent<TowerScript>();
				rangeFinder.transform.position = selectedPlatform.transform.position;
				_tower.SetSelected(true);

				SetRangeFinderSize(_tower.GetTurretRange());
			}
			else {
	 			BuyPanelManager(true);
	 			rangeFinder.SetActive(false);
			}
		}
	}

	void InGameClick(RaycastHit hit){
		if (hit.collider.gameObject == selectedPlatform) 
			return;

		if (hit.collider.gameObject.tag == "Platform") 
		{
			if(selectedPlatform){
				if (selectedPlatform.GetComponent<PlatformScript>().GetPlatformBuilt()) {
					selectedPlatform.GetComponent<PlatformScript>().GetPlatformTower().GetComponent<TowerScript>().SetSelected(false);
				}
			}

			selectedPlatform = hit.collider.gameObject;
			PlatformScript pscript = selectedPlatform.GetComponent<PlatformScript>();

			platformSelector.SetActive(true);
			platformSelector.transform.position = selectedPlatform.transform.position;
			//wp.GetComponent<TowerScript>().SetSelected(false);

			if (pscript.GetPlatformBuilt()) 
			{
				BuyPanelManager(false);
				rangeFinder.SetActive(true);
				GameObject t_tower = pscript.GetPlatformTower();
				TowerScript _tower = t_tower.GetComponent<TowerScript>();
				rangeFinder.transform.position = selectedPlatform.transform.position;
				_tower.SetSelected(true);

				SetRangeFinderSize(_tower.GetTurretRange());
			}
			else {
	 			BuyPanelManager(true);
	 			rangeFinder.SetActive(false);
			}

		}else{
			BuyPanelManager(false);
			platformSelector.SetActive(false);
			rangeFinder.SetActive(false);

			if(selectedPlatform){
				if (selectedPlatform.GetComponent<PlatformScript>().GetPlatformBuilt()) {
					selectedPlatform.GetComponent<PlatformScript>().GetPlatformTower().GetComponent<TowerScript>().SetSelected(false);
				}
			}

			selectedPlatform = null;
		}
	}

	public void SetRangeFinderSize(float size){
		rangeFinder.GetComponent<RectTransform>().sizeDelta = new Vector2(size*2,size*2);
	}

	public void BuildTower(GameObject tower){
		PlatformScript pscript = selectedPlatform.GetComponent<PlatformScript>();
		pscript.PlaceTower(tower);

		SelectDirectly(selectedPlatform);
	}

	public void PlayerTakeDamage(int ammount){
		playerHealth -= ammount;
		playerHealthUI.GetComponent<TMP_Text>().text = playerHealth.ToString();

		if (playerHealth <= 0) 
		{
			GameOver();
		}
	}

	public void PlayerTakeMoney(int ammount){
		playerMoney -= ammount;
		playerMoneyUI.GetComponent<TMP_Text>().text = playerMoney.ToString();
	}

	public void PlayerGiveMoney(int ammount){
		playerMoney += ammount;
		playerMoneyUI.GetComponent<TMP_Text>().text = playerMoney.ToString();
	}

	public int GetPlayerMoney(){
		return playerMoney;
	}

	public void SetAutoWaveCall(){
		waveCallAuto = waveCallAutoUI.GetComponent<Toggle>().isOn;
	}

	public void CallNextWave(){
		buttonCallWave.GetComponent<Button>().interactable = false;

		Debug.Log("NEXT WAVE!");
		
		if (!firstWaveCalled) 
		{
			firstWaveCalled = true;
		}

		currentWave += 1;

		waveEnemyCount = 15 + Random.Range(5,15);

		waveSpawnDelay = waveDuration/waveEnemyCount;
		waveDurationTimer = waveDuration;

		enemyTypeIndex = Random.Range(0,enemyTypes.Length);

		levelWaveUI.GetComponent<TMP_Text>().text = currentWave.ToString();

		spawning = true;

	}

	public int GetCurrentWave(){
		return currentWave;
	}

	List<GameObject> GetChildren(GameObject param){

		List<GameObject> list = new List<GameObject>();

		foreach (Transform child in param.transform)
		{
			list.Add(child.gameObject);
		}

		return list;
	}

	public void GameOver(){

		GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject en in allEnemies) 
		{
			en.GetComponent<EnemyScript>().UIElement_HealthBar.SetActive(false);
		}

		if (PlayerPrefs.GetInt("HiScore") < currentWave) 
		{
			PlayerPrefs.SetInt("HiScore", currentWave);
		}

		gameEnded = true;
		gamePaused = true;

		mainCanvasUI.GetComponent<Animator>().SetTrigger("Hide");
		gameOverUI.GetComponent<Animator>().SetTrigger("Show");

		CameraDolly.GetComponent<ClickDragCamera>().SetGameOverCamera();
	}

	public void TogglePaused(){
		SetPaused(!gamePaused);
	}

	public bool GetGamePaused(){
		return gamePaused;
	}

	public void SetPaused(bool _pausedVal){
		gamePaused = _pausedVal;

		BuyPanelManager(false);
	}

	public GameObject GetSelectedTower(){
		PlatformScript pscript = selectedPlatform.GetComponent<PlatformScript>();

		return pscript.GetPlatformTower();
	}

	void BuyPanelManager(bool buyPanelState){

		GameObject buyPanelUI = GameObject.Find("UIElement_BuyPanel");
		Animator _anim = buyPanelUI.GetComponent<Animator>();

		if (buyPanelState != _anim.GetBool("Show")) 
		{
			_anim.SetBool("Show", buyPanelState);
		}
	}
}