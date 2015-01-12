using UnityEngine;
using UnityEngineExt;
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

    private ArrayList releaseList;
    private bool releaseOnNextFrame;
    
    void onDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
    }

    void Start()
    {
        releaseList = new ArrayList();
    }

    void OnEnable()
    {
        EventBus.OnBecomeDead += RecycleLater;

        SpawnWithPooling();
        
        SetUpNextSpawnTime();
    }
    
    void OnDisable()
    {
        EventBus.OnBecomeDead -= RecycleLater;
    }
    
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnWithPooling();

            SetUpNextSpawnTime();
        }
    }

    void LateUpdate()
    {
        int count = releaseList.Count;
        
        if (count > 0)
        {
            if (!releaseOnNextFrame)
            {
                releaseOnNextFrame = true;
                return;
            }
        } else
        {
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = releaseList[i] as GameObject;
            
            go.Release();
        }
        
        releaseList.Clear();
        releaseOnNextFrame = false;
    }
    
    void SpawnWithPooling()
    {
        if (enemyPrefabs.Length > 0)
        {
            GameObject prefab = enemyPrefabs[Random.Range((int)0, (int)enemyPrefabs.Length)];
            GameObject go = ServiceLocator.current.pool.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
            go.transform.parent = dynamicObjects.transform;
            go.transform.position += Vector3.up * ((Random.value * 2.0f) - 1.0f) * spawnZoneY;
            (go.GetComponentInChildren(typeof(Creature2)) as Creature2).ForceEnterState(Creature2.State.Alive);
        }
    }

    void FixedUpdate()
    {
        enemiesPerSecond += Time.fixedDeltaTime * enemiesPerSecondSpeed;
    }

    private void RecycleLater(GameObject enemy)
    {
        releaseList.Add(enemy);
    }

    private void SetUpNextSpawnTime()
    {
        nextSpawnTime = Time.time + 1.0f / enemiesPerSecond;
    }
}
