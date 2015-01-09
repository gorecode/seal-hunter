using UnityEngine;
using System;

public class EventBus
{
    public delegate void GameObjectCallback(GameObject go);

    public static GameObjectCallback OnBecomeAlive;
    public static GameObjectCallback OnBecomeDead;
    public static GameObjectCallback OnBecomeDying;
}
