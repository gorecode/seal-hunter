using UnityEngine;
using System.Collections;

public class MoneyController : MonoBehaviour {
    public int amount;
    public Transform hudRoot;
    public GameObject hudTextPrefab;

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

        if (reward == 0) return;

        amount += reward;

        BoxCollider2D bb = creature.gameObject.collider2D as BoxCollider2D;

        Vector3 spawnPoint = creature.transform.position;

        spawnPoint.x += bb.center.x;
        spawnPoint.y += bb.center.y;

        //NGUITools.AddChild(null, null);
        GameObject hudItem = ServiceLocator.current.pool.Instantiate(hudTextPrefab, Vector3.zero, Quaternion.identity, false);

        hudItem.transform.parent = hudRoot;
        hudItem.transform.localPosition = SSUI.WorldToMainWindowPoint(spawnPoint);
        hudItem.transform.localScale = Vector3.one;
        hudItem.gameObject.layer = hudRoot.gameObject.layer;

        hudItem.GetComponent<UILabel>().text = "+" + reward + "$";

        UpdateUi();
    }

    public void UpdateUi()
    {
        label.text = "$ = " + amount;
    }
}
