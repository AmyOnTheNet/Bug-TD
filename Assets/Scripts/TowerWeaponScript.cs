using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class TowerWeaponScript : MonoBehaviour
{
	public GameObject projectilePrefab;

	public bool projectileOblique;

	public bool projectilePersist;
	public float projectileLifeTime;

	public Transform[] muzzle;
	public bool syncBarrels;

	public AudioClip[] shootSFX;

	//These are set by the tower targetting system
	private float distanceToTarget;
	private GameObject currentTarget;

	private float attackDelay;

	private Animator c_Animator;
	private TowerScript p_Tower;
	private AudioSource c_AudioSource;
	// Start is called before the first frame update
	void Start()
	{
		c_Animator = GetComponent<Animator>();
		c_AudioSource = GetComponent<AudioSource>();
		p_Tower = transform.parent.GetComponent<TowerScript>();
	}

	void Update()
	{
		//Check if game is paused before doing anything
		GameObject go_gameLogic = GameObject.Find("GameLogic");
		GameLogic s_gameLogic = go_gameLogic.GetComponent<GameLogic>();

		if (s_gameLogic.GetGamePaused()) 
			return;

		attackDelay -= Time.deltaTime;

		currentTarget = p_Tower.GetCurrentTarget();

		if (currentTarget != null) 
		{
			//Turn Towards Target
			Vector3 targetDirection = currentTarget.transform.position - transform.position;

			float singleStep = p_Tower.GetTurretRotationSpeed() * Time.deltaTime;
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDirection);
			//transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);

			distanceToTarget = Vector3.Distance(currentTarget.transform.position, transform.position);

			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distanceToTarget, Color.red);

			RaycastHit hit;

			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distanceToTarget)){

				if (hit.collider.tag == "Enemy") 
				{
					//Start Attacking
					if (attackDelay <= 0f) 
					{
						StartCoroutine(Attack());
						c_Animator.Play("Fire",0 ,0f);
						attackDelay = 60/p_Tower.GetTurretRateOfFire();
						Debug.Log("ATTACK");
					}
				}
			}
		}else{
			//Do a looking for target sequence
		}
	}

	private IEnumerator Attack(){

		float msd = 0f;

		if(syncBarrels){
			msd = 60/ (p_Tower.GetTurretRateOfFire()*muzzle.Length);
		}else{
			msd = p_Tower.multiShotDelay;
		}

		foreach (Transform _m in muzzle) 
		{
			for (int i = 0; i < p_Tower.numShot; i++) 
			{
				Shoot(_m);
			}
			yield return new WaitForSeconds(msd);
		}
	}

	void Shoot(Transform muzzlePos){
		c_AudioSource.clip = shootSFX[Random.Range(0,shootSFX.Length)];
		c_AudioSource.pitch = Random.Range(0.8f, 1.2f);
		c_AudioSource.Play();

		GameObject bullet = (GameObject)Instantiate(projectilePrefab, muzzlePos.position, muzzlePos.rotation);

		BulletScript bscript = bullet.GetComponent<BulletScript>();

		bullet.transform.Rotate(Random.Range(-p_Tower.attackSpread,p_Tower.attackSpread),Random.Range(-p_Tower.attackSpread,p_Tower.attackSpread),0f, Space.Self);

		bscript.SetOwnerTurret(transform.parent.gameObject);

		bscript.SetDamage(p_Tower.GetTurretDamage());

		if(p_Tower.projectileExplosive)
			bscript.SetExplosive(true, p_Tower.GetTurretExplosiveRadius());

		if(p_Tower.projectileGuided)
			bscript.SetGuided(true, p_Tower.projectileAimingSpeed, p_Tower.GetTurretProjectileVelocity(), currentTarget);

		if(projectilePersist)
			bscript.SetPersistent(true, projectileLifeTime);

		bscript.SetModifiers(p_Tower.GetAttackModifiers());

		if (projectileOblique) 
		{
			float gravity = Physics.gravity.magnitude;
			float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
			float angle = 45f * Mathf.Deg2Rad;
			float yOffset = muzzlePos.position.y - currentTarget.transform.position.y;

			float v0 = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

			bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * v0,ForceMode.Impulse);
		}else{
			bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * p_Tower.GetTurretProjectileVelocity(),ForceMode.Impulse);
		}
	}
}
