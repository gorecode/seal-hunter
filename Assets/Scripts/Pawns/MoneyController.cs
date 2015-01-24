using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoneyController : MonoBehaviour {
    public struct RewardEntry {
        public Creature2 target;
        public int reward;
    }

    public int amount;
    public Transform hudRoot;
    public GameObject hudTextPrefab;

    private List<RewardEntry> deferredRewards;
    private UILabel label;

    void Awake()
    {
        label = GetComponent<UILabel>();
        deferredRewards = new List<RewardEntry>();
    }

    void OnEnable()
    {
        EventBus.OnBecomeDying += AddRewardForDeath;
    }

    void OnDisable()
    {
        EventBus.OnBecomeDying -= AddRewardForDeath;
    }

    void Start()
    {
        UpdateUi();
    }

    public void AddRewardForHeadshot(Creature2 creature)
    {
        if (creature.CompareTag(Tags.BOSS_SUPPORT)) return;

        int reward = 0;

        switch (creature.GetMobType())
        {
            case Creature2.MobType.Seal:
            case Creature2.MobType.Pinguin:
                reward = 10;
                break;
            case Creature2.MobType.Activist:
                reward = 15;
                break;
        }

        AddRewardLater(creature, reward);
    }

    public void AddRewardForDeath(GameObject mob)
    {
        Creature2 creature = mob.GetComponentInChildren(typeof(Creature2)) as Creature2;

        int reward = 0;

        switch (creature.GetMobType())
        {
            case Creature2.MobType.Seal:
                reward = 50;
                break;
            case Creature2.MobType.Activist:
                Activist activist = creature as Activist;
                reward = activist.HasDroppedSealChild() ? 45 : 100;
                break;
            case Creature2.MobType.Pinguin:
                reward = 100;
                break;
            case Creature2.MobType.SealChild:
                reward = 25;
                break;
            case Creature2.MobType.Bear:
                BearController bear = creature as BearController;
                reward = bear.CompareTag(Tags.BOSS) ? 400 : 200;
                break;
            case Creature2.MobType.BigBear:
            case Creature2.MobType.Tortoise:
                reward = 500;
                break;
            case Creature2.MobType.Valrus:
                reward = 1000;
                break;
        }

        if (creature.CompareTag(Tags.BOSS_SUPPORT)) reward /= 4;

        AddRewardLater(creature, reward);
    }

    public void AddRewardLater(Creature2 creature, int reward)
    {
        RewardEntry entry;

        entry.target = null;
        entry.reward = 0;

        int count = deferredRewards.Count;
        int indexOfEntryWithTarget = -1;

        for (int i = 0; i < count; i++)
        {
            RewardEntry each = deferredRewards[i];

            if (each.target == creature)
            {
                indexOfEntryWithTarget = i;
                entry = each;
                break;
            }
        }

        entry.target = creature;
        entry.reward += reward;

        if (indexOfEntryWithTarget == -1)
        {
            deferredRewards.Add(entry);
        } else
        {
            deferredRewards[indexOfEntryWithTarget] = entry;
        }
    }

    public void AddReward(Creature2 creature, int reward)
    {
        if (reward == 0) return;
        
        amount += reward;

        BoxCollider2D bb = creature.gameObject.collider2D as BoxCollider2D;
        
        Vector3 spawnPoint = creature.transform.position;

        if (creature.hudRoot != null)
        {
            spawnPoint = creature.hudRoot.position;
        } else if (bb != null)
        {
            spawnPoint.x += bb.center.x;
            spawnPoint.y += bb.center.y;
        }
        
        //NGUITools.AddChild(null, null);
        GameObject hudItem = ServiceLocator.current.pool.Instantiate(hudTextPrefab, Vector3.zero, Quaternion.identity, false);
        
        hudItem.transform.parent = hudRoot;
        hudItem.transform.localPosition = SSUI.WorldToMainWindowPoint(spawnPoint);
        hudItem.transform.localScale = Vector3.one;
        hudItem.gameObject.layer = hudRoot.gameObject.layer;
        
        hudItem.GetComponent<UILabel>().text = "+" + reward + "$";
        
        UpdateUi();
    }

    void LateUpdate()
    {
        int count = deferredRewards.Count;

        for (int i = 0; i < count; i++)
        {
            RewardEntry entry = deferredRewards[i];

            AddReward(entry.target, entry.reward);
        }

        deferredRewards.Clear();
    }

    public void UpdateUi()
    {
        label.text = "$ = " + amount;
    }
}
