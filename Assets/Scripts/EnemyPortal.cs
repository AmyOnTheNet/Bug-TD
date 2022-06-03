using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
	public Transform[] path;

	private float radius;

	// Start is called before the first frame update
	void Start()
	{
		//SpawnEnemy(enemyTypes[0]);
		//InvokeRepeating("SpawnEnemy", 1f, 1f);
		radius = 0.3f;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void SpawnEnemy(GameObject enemyPrefab,int currentWave){
		Vector3 offset = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

		GameObject spawn = (GameObject)Instantiate(enemyPrefab, path[0].position + offset, Quaternion.identity);
		EnemyScript script = spawn.GetComponent<EnemyScript>();
		script.currentPath = path;
		script.pathOffset = offset;
		script.bounty = 1 + (currentWave / 5);

		script.SetLevel(currentWave);
		script.RefreshHealth();
	}
}
