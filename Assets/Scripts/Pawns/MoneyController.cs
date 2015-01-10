﻿using UnityEngine;
using System.Collections;

public class MoneyController : MonoBehaviour {
    public int amount;

    private UILabel label;

    void Awake()
    {
        label = GetComponent<UILabel>();
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
                reward = 200;
                // TODO: 400 for boss.
                break;
            case Creature2.MobType.BigBear:
            case Creature2.MobType.Tortoise:
                reward = 500;
                break;
            case Creature2.MobType.Valrus:
                reward = 1000;
                break;
        }

        amount += reward;

        UpdateUi();
    }

    public void UpdateUi()
    {
        label.text = "$ = " + amount;
    }
}