﻿using UnityEngine;
using System.Collections;

public class ServiceLocator : MonoBehaviour {
    public static ServiceLocator current;

    public GameObjectPool pool;
    public GameObject bloodSparksPrefab;
    public GameObject bulletPrefab;
    public GameObject shellHitPrefab;
    public MoneyController moneyController;
    public SlaughterBackground slaughterBackgroundController;
    public SinglePlayerGameController singlePlayerGame;

    public void Awake()
    {
        current = this;
    }
}
