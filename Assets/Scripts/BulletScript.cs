using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public GameObject destroyFX;
	//public GameObject trailFX;

	private bool guided;
	private GameObject target;
	private GameObject ownerTurret;
	private float aimingSpeed;
	private float velocity;

	private int damage;

	private bool explosive;
	private float explosiveRadius;

	private bool persistent;
	private float projectileLifeTime = 10f;

	private GameObject[] modifiers;

	void Start(){
		/*
			float gravity = Physics.gravity.magnitude;
			float distance = Vector3.Distance(target.transform.position, transform.position);
			float angle = 45f * Mathf.Deg2Rad;
			float yOffset = transform.position.y - target.transform.position.y;

			float v0 = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

			GetComponent<Rigidbody>().AddForce(transform.forward * v0,ForceMode.Impulse);
		*/

	}
	
	private bool paused;
	// Update is called once per frame
	void Update()
	{

		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()) 
		{
			if (!paused) 
				SetPaused(true);

			return;
		}else {
			if (paused) 
				SetPaused(false);
		}


		projectileLifeTime -= Time.deltaTime;

		if (guided) 
		{
			if (target != null) 
			{
				//Turn Towards Target
				Vector3 targetDirection = target.transform.position - transform.position;

				float singleStep = aimingSpeed * Time.deltaTime;
				Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
				transform.rotation = Quaternion.LookRotation(newDirection);
			}else{
				//request new target
				target = ownerTurret.GetComponent<TowerScript>().GetCurrentTarget();
			}
		}

		if (projectileLifeTime <= 0f) 
		{
			DestroyProjectile();			
		}
	}

	void FixedUpdate(){

		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()) 
			return;

		//accelerate
		Rigidbody _rb = GetComponent<Rigidbody>();
		
		if (guided) 
		{
			_rb.velocity = transform.forward * velocity;
		}	
	}

	private Vector3 saved_velocity;

	void SetPaused(bool b_paused){
		paused = b_paused;
		Rigidbody _rb = GetComponent<Rigidbody>();

		if(b_paused) 
		{
			saved_velocity = _rb.velocity;
			_rb.Sleep();
			foreach (Transform _child in transform) 
			{
				if (_child.GetComponent<TrailRenderer>()) 
					_child.GetComponent<TrailRenderer>().emitting = b_paused;

				if (_child.GetComponent<ParticleSystem>()) 
					_child.GetComponent<ParticleSystem>().Pause();
			}

		}else{
			_rb.velocity = saved_velocity;
			_rb.WakeUp();

			foreach (Transform _child in transform) 
			{
				if (_child.GetComponent<TrailRenderer>()) 
					_child.GetComponent<TrailRenderer>().emitting = !b_paused;

				if (_child.GetComponent<ParticleSystem>()) 
					_child.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy" || other.tag == "Floor") 
		{
			if (explosive)
			{
				GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

				foreach (GameObject en in allEnemies)
				{
					if (Vector3.Distance(en.transform.position, transform.position) <= explosiveRadius) 
					{
						en.GetComponent<EnemyScript>().TakeDamage(damage);

						foreach (GameObject mod in modifiers) 
						{
							GameObject _m = Instantiate(mod, en.transform.position, en.transform.rotation);
							_m.transform.parent = en.transform;
						}
					}
				}
			}else{
				if(other.tag == "Enemy"){
					other.gameObject.GetComponent<EnemyScript>().TakeDamage(damage);

					foreach (GameObject mod in modifiers) 
					{
						GameObject _m = Instantiate(mod, other.transform.position, other.transform.rotation);
						_m.transform.parent = other.transform;
						_m.SendMessage("SetOwner", ownerTurret);
					}
				}
			}

			if(!persistent)
				DestroyProjectile();
		}
	}

	public void DestroyProjectile(){
		if (destroyFX != null){
			GameObject _obj = Instantiate(destroyFX, transform.position, Quaternion.identity);

			if(explosive)
				_obj.transform.localScale = new Vector3(explosiveRadius, explosiveRadius, explosiveRadius);
		}

		foreach (Transform _child in transform) 
		{
			if (_child.GetComponent<TrailRenderer>()) 
			{
				_child.transform.parent = null;
				_child.GetComponent<TrailRenderer>().autodestruct = true;
			}
		}

		Destroy(gameObject);
	}

	public void SetDamage(int param){
		damage = param;
	}

	public void SetExplosive(bool isExplosive, float radius){
		explosive = isExplosive;
		explosiveRadius = radius;
	}

	public void SetGuided(bool isGuided,float _aimingSpeed, float _velocity, GameObject _target){
		guided = isGuided;
		target = _target;
		aimingSpeed = _aimingSpeed;
		velocity = _velocity;
	}

	public void SetPersistent(bool isPersistent, float _newLifetime){
		persistent = isPersistent;
		projectileLifeTime = _newLifetime;
	}

	public void SetModifiers(GameObject[] param){
		modifiers = param;
	}

	public void SetOwnerTurret(GameObject param){
		ownerTurret = param;
	}
}