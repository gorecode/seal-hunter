using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemyDiedEvent
{
    private readonly List<Action<GameObject>> _callbacks = new List<Action<GameObject>>();

    public void Subscribe(Action<GameObject> callback)
    {
        _callbacks.Add(callback);
    }

    public void Publish(GameObject unit)
    {
        Debug.Log("Enemy died");

        foreach (Action<GameObject> callback in _callbacks)
        {
            callback(unit);
        }
    }
}
