using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
	//variables
	public int baseHealth;
	public int healthPerLevel;

	public float moveSpeed;
	public float turnRate;

	public bool flying;

	public int strength;
	public int bounty;

	public float damageMultiplier;

	public Transform[] currentPath;
	public Vector3 pathOffset;
	public float unitHeight;

	public Transform hitLoc;

	public GameObject UIElement_HealthBar;
	public Slider slider;

	public GameObject deathFX;
	public AudioClip deathSFX;

	public GameObject[] modifiers;

	public GameObject unitModel;

	private int targetWP;

	private int maxHealth;
	private int health;

	private int currentLevel;

	private GameObject gameLogic;
	private AudioSource c_AudioSource;
	private Vector3 nativeScale;
	private Vector3 targetScale;
	private float fraction = 0f;

	// Start is called before the first frame update
	void Start()
	{
		//maxHealth = baseHealth;
		//health = maxHealth;
		targetWP = 1;

		gameLogic = GameObject.Find("GameLogic");

		pathOffset.y = unitHeight;
		transform.Translate(new Vector3(0f,unitHeight,0f));

		c_AudioSource = GetComponent<AudioSource>();

		nativeScale = transform.localScale;
		transform.localScale = new Vector3(0f,0f,0f);
		targetScale = nativeScale;
		fraction = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()){
			unitModel.GetComponent<Animator>().speed = 0f;
			return;
		}else{
			unitModel.GetComponent<Animator>().speed = 1f * moveSpeed;
		}

		if(fraction < 1){
			fraction += Time.deltaTime * 0.5f;
			transform.localScale = Vector3.Lerp(transform.localScale, targetScale, fraction);
		}
		
		UIElement_HealthBar.transform.LookAt(Camera.main.transform);
		UIElement_HealthBar.transform.eulerAngles = new Vector3(-UIElement_HealthBar.transform.eulerAngles.x,0,UIElement_HealthBar.transform.eulerAngles.z);

		if (IsAlive()) 
		{
			//Check if near current waypoint
			if(Vector3.Distance(currentPath[targetWP].position + pathOffset, transform.position) < 0.3){

				//check if last WP
				if (targetWP == currentPath.Length-1) 
				{
					//last WP, remove health from player
					StartCoroutine(DamagePlayer());
				}else{
					//not last WP, change target to next
					targetWP++;
				}
			}

			//Face towards waypoint and move forward
			Vector3 targetDirection = (currentPath[targetWP].position + pathOffset) - transform.position;

			float singleStep = turnRate * Time.deltaTime;
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDirection);

			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);	
		}
	}

	public void TakeDamage(int ammout){
		if (IsAlive()) 
		{
			health -= (int)Mathf.Round(ammout * damageMultiplier);
			slider.value = CalcMaxHP();

			if (health <= 0) 
			{
				StartCoroutine(Die());
			}
		}
	}

	public void SetLevel(int _lvl){
		currentLevel = _lvl;
	}

	public void RefreshHealth(){
		maxHealth = baseHealth + (healthPerLevel * currentLevel);
		health = maxHealth;
	}

	private IEnumerator DamagePlayer(){
		health = 0;
		gameLogic.GetComponent<GameLogic>().PlayerTakeDamage(strength);
		targetScale = new Vector3(0f, 0f, 0f);
		fraction = 0f;

		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}

	float CalcMaxHP(){
		return (float)health / (float)maxHealth;
	}

	public bool IsAlive(){
		if(health > 0){
			return true;
		}else {
			return false;
		}
	}

	public bool IsFlying(){
		return flying;
	}

	private IEnumerator Die(){
		gameLogic.GetComponent<GameLogic>().PlayerGiveMoney(bounty);
		Instantiate(deathFX, transform.position, transform.rotation);

		c_AudioSource.clip = deathSFX;
		c_AudioSource.pitch = Random.Range(0.8f, 1.2f);
		c_AudioSource.Play();

		unitModel.GetComponent<Animator>().Play("Death");

		GetComponent<SphereCollider>().enabled = false;

		targetScale = new Vector3(0f, 0f, 0f);
		fraction = 0f;

		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}
}
