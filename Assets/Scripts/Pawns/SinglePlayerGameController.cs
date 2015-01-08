using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BossSpawnCondition {
    TIME_IS_OUT,
    TIME_IS_OUT_AND_MOBS_ARE_DEAD
}

[System.Serializable]
public class LevelDescriptor
{
    public float duration;
    public MobCountDescriptor[] mobs;
    public BossSpawnCondition bossSpawnCondition = BossSpawnCondition.TIME_IS_OUT;
}

[System.Serializable]
public class MobCountDescriptor
{
    public GameObject prefab;
    public int count;
}


public class SinglePlayerGameController : MonoBehaviour
{
    public float spawnZoneY = 5;

    public GameObject dynamicObjects;
    public LevelDescriptor[] levels;
    public AnimationCurve spawnCurve;

    private LevelDescriptor currentLevel;
    private float levelStartTime;

    private ArrayList releaseList;
    private bool releaseOnNextFrame;
    private int levelIndex;
    private LevelDescriptor level;

    private float[][] mobSpawnTimeline;
    private int[] mobSpawnTimelineOffsets;

    public void SetLevelIndex(int current)
    {
        Debug.Log("Set Level to " + current);

        levelIndex = current;
        level = levels[current];
        level.duration = 15;

        levelStartTime = Time.fixedTime;

        mobSpawnTimeline = new float[level.mobs.Length][];
        for (int i = 0; i < mobSpawnTimeline.Length; i++)
        {
            mobSpawnTimeline[i] = CreateSpawnTimeArray(level.duration, level.mobs[i].count);
        }

        mobSpawnTimelineOffsets = new int[mobSpawnTimeline.Length];
    }

    void onDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
    }

    void Start()
    {
        releaseList = new ArrayList();

        SetLevelIndex(0);
    }

    void OnEnable()
    {
        EventBus.OnBecomeDead += RecycleLater;
    }
    
    void OnDisable()
    {
        EventBus.OnBecomeDead -= RecycleLater;
    }

    void FixedUpdate()
    {
        float ft = Time.fixedTime;

        int count = mobSpawnTimeline.Length;

        for (int i = 0; i < count; i++)
        {
            int j = mobSpawnTimelineOffsets[i];

            if (j == mobSpawnTimeline[i].Length) continue;

            float nextSpawnTime = mobSpawnTimeline[i][j];

            if (ft >= nextSpawnTime) {
                Spawn(level.mobs[i].prefab);

                mobSpawnTimelineOffsets[i]++;
            }
        }

        if (ft - levelStartTime >= level.duration)
        {
            levelIndex++;
            if (levelIndex >= levels.Length) levelIndex = 0;
            SetLevelIndex(levelIndex);
        }
    }

    void Spawn(GameObject prefab)
    {
        GameObject go = GameObjectPool.Instance.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = dynamicObjects.transform;
        go.transform.position += Vector3.up * ((Random.value * 2.0f) - 1.0f) * spawnZoneY;
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
            
            GameObjectPool.Instance.Release(go);
        }
        
        releaseList.Clear();
        releaseOnNextFrame = false;
    }

    private void RecycleLater(GameObject enemy)
    {
        releaseList.Add(enemy);
    }

    private float[] CreateSpawnTimeArray(float duration, int count)
    {
        float ct = Time.fixedTime;

        EquationSolver.EquationFunc enemiesCountFromTimeFunc = delegate(float x) {
            return spawnCurve.Evaluate(x / duration) * (float)count;
        };

        float[] t = new float[count];

        for (int i = 0; i < count; i++)
        {
            t[i] = ct + EquationSolver.Dihotomy(enemiesCountFromTimeFunc, i + 1, 0f, duration);
            //Debug.Log("i = " + i + ", t = " + t[i]);
        }

        return t;
    }
}
