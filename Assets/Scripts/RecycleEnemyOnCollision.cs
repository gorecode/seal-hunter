using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class RecycleEnemyOnCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        (collider.transform.parent.GetComponentInChildren(typeof(Creature2)) as Creature2).ForceEnterState(Creature2.State.Dead);

        collider.transform.parent.gameObject.Release();
    }
}
