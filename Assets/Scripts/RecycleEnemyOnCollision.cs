using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class RecycleEnemyOnCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Creature2 c = (collider.transform.parent.GetComponentInChildren(typeof(Creature2)) as Creature2);
        c.Kill();
    }
}
