using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierDamagePerSecond : MonoBehaviour
{
	public float damage = 1f;
	public float duration = 1f;
	public float tickrate = 1f;

	private float timer;
	private float damageTimer;

	private GameObject turretOwner;
	private ParticleSystem p_sys;

	// Start is called before the first frame update
	void Start()
	{
		timer = duration;
		damageTimer = tickrate;

		p_sys = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{

		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()) 
		{
			p_sys.Pause();
			return;
		}else {
			if (p_sys.isPaused) 
			{
				p_sys.Play();
			}
		}

		transform.localScale = transform.parent.GetComponent<EnemyScript>().unitModel.transform.localScale;

		if (!transform.parent.GetComponent<EnemyScript>().IsAlive()) 
			Destroy(gameObject);

		timer -= Time.deltaTime;
		damageTimer -= Time.deltaTime;

		if(timer <= 0f){
			Debug.Log("stopped particle system");
			p_sys.Stop();
		}

		if(!p_sys.IsAlive()){
			Debug.Log("Destroyed dead particle system");
			Destroy(gameObject);
		}

		if(damageTimer <= 0f){
			damageTimer = tickrate;

			transform.parent.SendMessage("TakeDamage", damage);
		}

		foreach (Transform _gt in transform.parent) 
		{
			if (_gt.name == gameObject.name && _gt != transform) 
			{
				Destroy(_gt.gameObject);
				timer = duration;
			}
		}
	}

	public void SetOwner(GameObject owner){
		turretOwner = owner;

		damage = turretOwner.GetComponent<TowerScript>().GetTurretDamage();
	}
}
