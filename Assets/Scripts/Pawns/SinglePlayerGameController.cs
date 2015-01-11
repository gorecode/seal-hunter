using UnityEngine;
using UnityEngineExt;
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
    public GameObject bossPrefab;
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

    public UISprite levelProgressBar;

    public int initialLevel;

    public UILabel levelLabel;

    private LevelDescriptor currentLevel;
    private float levelStartTime;

    private ArrayList releaseList;
    private bool releaseOnNextFrame;
    private int levelIndex;
    private LevelDescriptor level;

    private float[][] mobSpawnTimeline;
    private int[] mobSpawnTimelineOffsets;

    private List<Creature2> currentBosses = new List<Creature2>();
    private int numAliveEnemies;
    private GameObjectPool bossPool = new GameObjectPool();

    private TortoiseSupport tortoiseSupport;

    void Awake()
    {
        tortoiseSupport = GetComponent<TortoiseSupport>();
    }

    public void SetLevelIndex(int current)
    {
        Debug.Log("Set Level to " + current);

        levelIndex = current;
        level = levels[current];
        levelStartTime = Time.fixedTime;

        mobSpawnTimeline = new float[level.mobs.Length][];
        for (int i = 0; i < mobSpawnTimeline.Length; i++)
        {
            mobSpawnTimeline[i] = CreateSpawnTimeArray(level.duration - 5.0f, level.mobs[i].count);
        }

        mobSpawnTimelineOffsets = new int[mobSpawnTimeline.Length];

        currentBosses.Clear();

        levelProgressBar.spriteName = "Timebar";

        ShowAndHideLevelLabel("Level " + (levelIndex + 1));
    }

    void ShowAndHideLevelLabel(string text)
    {
        levelLabel.text = text;
        levelLabel.color = Color.clear;
        TweenColor.Begin(levelLabel.gameObject, 2.0f, Color.black).AddOnFinished(delegate {
            TweenColor.Begin(levelLabel.gameObject, 2.0f, Color.clear);
        });
    }

    void onDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + transform.up * spawnZoneY, transform.position - transform.up * spawnZoneY);
    }

    void Start()
    {
        releaseList = new ArrayList();

        SetLevelIndex(initialLevel);
    }

    void OnEnable()
    {
        EventBus.OnBecomeDead += OnEnemyDeath;
        EventBus.OnBecomeAlive += OnEnemyAlive;
    }
    
    void OnDisable()
    {
        EventBus.OnBecomeDead -= OnEnemyDeath;
        EventBus.OnBecomeDead -= OnEnemyAlive;
    }

    void Update()
    {
        if (currentBosses.Count > 0) 
        {
            float maxHealth = 0;
            float health = 0;

            for (int i = 0; i < currentBosses.Count; i++)
            {
                maxHealth += currentBosses[i].GetInitialHealth();
                health += Mathf.Max(0f, currentBosses[i].GetHealth());
            }

            levelProgressBar.fillAmount = health / maxHealth;
        } else
        {
            levelProgressBar.fillAmount = Mathf.Min(1.0f, (Time.fixedTime - levelStartTime) / level.duration);
        }
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

        if (currentBosses.Count > 0)
        {
            NextLevelIfBossComplete();
        } else
        {
            bool levelTimeout = ft - levelStartTime >= level.duration;

            if (levelTimeout)
            {
                bool nextStage = false;

                if (level.bossSpawnCondition == BossSpawnCondition.TIME_IS_OUT) nextStage = true;
                if (level.bossSpawnCondition == BossSpawnCondition.TIME_IS_OUT_AND_MOBS_ARE_DEAD && AreAllEnemiesAreDead()) nextStage = true;

                if (nextStage)
                {
                    if (level.bossPrefab != null)
                    {
                        SpawnBossForCurrentLevel();

                        if (levelIndex == 3) tortoiseSupport.enabled = true;
                    }
                    else NextLevel();
                }
            }
        }
    }

    bool AreAllEnemiesAreDead()
    {
        return numAliveEnemies == 0;
    }

    void SpawnBossForCurrentLevel()
    {
        currentBosses.Clear();

        Debug.Log("Spawn boss for level " + levelIndex);

        levelProgressBar.spriteName = "Lifebar";

        switch (levelIndex)
        {
            case 0:
                GameObject bearObj = SpawnInactiveBoss(level.bossPrefab);
                BearController bear = bearObj.GetComponentInChildren<BearController>();
                bear.tag = Tags.BOSS;
                bear.maxHealth = 1250;
                bear.maxHealthIncrementByLevel = 0;
                bear.ForceEnterState(Creature2.State.Alive);
                break;
            case 1:
                int count = 7;

                for (int i = 0; i < count; i++)
                {
                    GameObject pinguinObj = SpawnInactiveBoss(level.bossPrefab);

                    Vector3 pinguinPosition = transform.position;
                    pinguinPosition.y += Mathf.Lerp(-spawnZoneY, spawnZoneY, (float)i / count);
                    pinguinObj.transform.position = pinguinPosition;

                    Pinguin pinguinScript = pinguinObj.GetComponentInChildren<Pinguin>();
                    pinguinScript.delayBeforeSliding = Random.Range(3.5f, 4.0f);
                    pinguinScript.ForceEnterState(Pinguin.State.Alive);
                }
                break;
            case 2:
                GameObject bigBearObj = SpawnInactiveBoss(level.bossPrefab);
                BigBear bigBearScript = bigBearObj.GetComponentInChildren<BigBear>();
                bigBearScript.maxHealth = 2000;
                bigBearScript.delayBeforeRun = 4.0f;
                bigBearScript.ForceEnterState(BigBear.State.Alive);
                break;
            case 3:
            case 4:
                GameObject boss = SpawnInactiveBoss(level.bossPrefab);
                (boss.GetComponentInChildren(typeof(Creature2)) as Creature2).ForceEnterState(Creature2.State.Alive);
                break;
        }

        ShowAndHideLevelLabel("Boss");
    }

    void NextLevelIfBossComplete()
    {
        int count = currentBosses.Count;
        
        for (int i = 0; i < count; i++)
        {
            if (currentBosses[i].GetCurrentState() != Creature2.State.Dead) return;
        }

        if (levelIndex == 3) tortoiseSupport.enabled = false;

        if (AreAllEnemiesAreDead())
        {
            NextLevel();
        }
    }
    
    void NextLevel()
    {
        if (++levelIndex == levels.Length) levelIndex = 0;
        
        SetLevelIndex(levelIndex);
    }

    GameObject SpawnInactiveBoss(GameObject prefab)
    {
        GameObject boss = bossPool.Instantiate(prefab, transform.position, Quaternion.identity);
        currentBosses.Add(boss.GetComponentInChildren(typeof(Creature2)) as Creature2);
        return boss;
    }

    public GameObject Spawn(GameObject prefab, bool makeAlive = true)
    {
        GameObject go = GameObjectPool.Instance.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = dynamicObjects.transform;
        go.transform.position += Vector3.up * ((Random.value * 2.0f) - 1.0f) * spawnZoneY;

        Creature2 mob = (go.GetComponentInChildren(typeof(Creature2)) as Creature2);
        mob.tag = "";
        mob.ForceEnterState(Creature2.State.Alive);

        return go;
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

    private void OnEnemyAlive(GameObject enemy)
    {
        numAliveEnemies++;
    }

    private void OnEnemyDeath(GameObject enemy)
    {
        releaseList.Add(enemy);

        numAliveEnemies--;
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
        }

        return t;
    }
}
