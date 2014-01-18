using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public float spawnZoneY = 5;

	public float enemiesPerSecond = 0.1f;
	public float enemiesPerSecondSpeed = 0.1f;

	public GameObject enemy;

	private float nextSpawnTime;

	void onDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawLine (transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
	}

	// Use this for initialization
	void Start () {
		setUpNextSpawnTime();
	}
	
	void Update() {
		if (Time.time >= nextSpawnTime) {
			GameObject newEnemy = Instantiate(enemy, transform.position, Quaternion.identity) as GameObject;

			newEnemy.transform.position += Vector3.up * ((Random.value * 2.0f) - 1.0f) * spawnZoneY;

			setUpNextSpawnTime();
		}
	}

	void FixedUpdate () {
		enemiesPerSecond += Time.fixedDeltaTime * enemiesPerSecondSpeed;
	}

	private void setUpNextSpawnTime() {
		nextSpawnTime = Time.time + 1.0f / enemiesPerSecond;
	}
}
