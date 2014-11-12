using UnityEngine;
using System;

public class EventBus
{
    public delegate void OnDeathMethod(GameObject go);

    public static OnDeathMethod OnDeath;
}
