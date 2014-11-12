using UnityEngine;
using System.Collections;

public class RecycleEnemyOnCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObjectPool.Instance.Recycle(collider.transform.parent.gameObject);
    }
}
