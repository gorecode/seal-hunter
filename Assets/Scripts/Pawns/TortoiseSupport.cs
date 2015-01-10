using UnityEngine;
using System.Collections;

public class TortoiseSupport : MonoBehaviour {
    [System.Serializable]
    public class Item
    {
        public GameObject prefab;
        public float minTime;
        public float randomTime;
    }

    public Item[] items;

    private float[] nextSpawnTime;

    private SinglePlayerGameController gameController;

    void Awake()
    {
        gameController = GetComponent<SinglePlayerGameController>();
    }

    void OnEnable()
    {
        float ft = Time.fixedTime;
        nextSpawnTime = new float[items.Length];
        for (int i = 0; i < nextSpawnTime.Length; i++)
        {
            nextSpawnTime[i] = ft + Random.Range(items[i].minTime, items[i].minTime + items[i].randomTime);
        }
    }

    void FixedUpdate()
    {
        float ft = Time.fixedTime;

        for (int i = 0; i < items.Length; i++)
        {
            if (ft >= nextSpawnTime[i])
            {
                Creature2 newMob = gameController.Spawn(items[i].prefab).GetComponentInChildren(typeof(Creature2)) as Creature2;
                newMob.tag = Tags.BOSS_SUPPORT;
                nextSpawnTime[i] = ft + Random.Range(items[i].minTime, items[i].minTime + items[i].randomTime);
            }
        }
    }
}
