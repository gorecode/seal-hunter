using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public float spawnZoneY = 5;

    public float enemiesPerSecond = 0.1f;
    public float enemiesPerSecondSpeed = 0.1f;

    public GameObject dynamicObjects;

    public GameObject[] enemyPrefabs;

    private float nextSpawnTime;

    void onDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
    }

    // Use this for initialization
    void Start()
    {
        EventBus.OnBecomeDead += OnEnemyBecomeDead;

        SpawnWithPooling();

        SetUpNextSpawnTime();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnWithPooling();

            SetUpNextSpawnTime();
        }
    }

    void SpawnWithPooling()
    {
        if (enemyPrefabs.Length > 0)
        {
            GameObject prefab = enemyPrefabs[Random.Range((int)0, (int)enemyPrefabs.Length)];
            GameObject go = GameObjectPool.Instance.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
            go.transform.parent = dynamicObjects.transform;
            go.transform.position += Vector3.up * ((Random.value * 2.0f) - 1.0f) * spawnZoneY;
        }
    }

    void FixedUpdate()
    {
        enemiesPerSecond += Time.fixedDeltaTime * enemiesPerSecondSpeed;
    }

    private void OnEnemyBecomeDead(GameObject enemy)
    {
        if (!GameObjectPool.Instance.Recycle(enemy))
        {
            GameObject.Destroy(enemy);
        }
    }

    private void SetUpNextSpawnTime()
    {
        nextSpawnTime = Time.time + 1.0f / enemiesPerSecond;
    }
}
