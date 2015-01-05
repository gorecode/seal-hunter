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

    private ArrayList recycleList;
    private bool recycleOnNextFrame;
    
    void onDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
    }

    void Start()
    {
        recycleList = new ArrayList();
        
        SpawnWithPooling();

        SetUpNextSpawnTime();
    }

    void OnEnable()
    {
        EventBus.OnBecomeDead += RecycleLater;
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
        int count = recycleList.Count;
        
        if (count > 0)
        {
            if (!recycleOnNextFrame)
            {
                recycleOnNextFrame = true;
                return;
            }
        } else
        {
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            GameObject go = recycleList[i] as GameObject;
            
            if (!GameObjectPool.Instance.Recycle(go))
            {
                GameObject.Destroy(go);
            }
        }
        
        recycleList.Clear();
        
        recycleOnNextFrame = false;
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

    private void RecycleLater(GameObject enemy)
    {
        recycleList.Add(enemy);
    }

    private void SetUpNextSpawnTime()
    {
        nextSpawnTime = Time.time + 1.0f / enemiesPerSecond;
    }
}
