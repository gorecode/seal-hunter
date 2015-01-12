using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool : MonoBehaviour {
    public Transform instantiationRoot;

    private Dictionary<GameObject, LinkedList<GameObject>> map = new Dictionary<GameObject, LinkedList<GameObject>>();

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        LinkedList<GameObject> recycled = GetRecycledObjects(prefab);

        if (recycled.First == null)
        {
            GameObject newObject = (GameObject)GameObject.Instantiate(prefab, position, rotation);
            newObject.AddComponent<GameObjectPoolItem>();
            newObject.transform.parent = instantiationRoot;

            GameObjectPoolItem marker = newObject.GetComponent<GameObjectPoolItem>();
            marker.prefab = prefab;
            marker.pool = this;
            marker.referenceCount = 0;

            return newObject;
        } else
        {
            GameObject oldObject = recycled.First.Value;

            oldObject.transform.position = position;
            oldObject.transform.rotation = rotation;
            oldObject.transform.parent = instantiationRoot;
            oldObject.SetActive(true);

            recycled.RemoveFirst();

            GameObjectPoolItem marker = oldObject.GetComponent<GameObjectPoolItem>();
            marker.referenceCount = 0;

            return oldObject;
        }
    }

    public bool Recycle(GameObject gameObject)
    {
        GameObjectPoolItem marker = gameObject.GetComponent<GameObjectPoolItem>();

        if (marker == null)
        {
            throw new System.Exception("Cannot recycle element that was not instantiated with GameObjectPool");
        }

        LinkedList<GameObject> recycled = GetRecycledObjects(marker.prefab);

        gameObject.SetActive(false);

        recycled.AddFirst(gameObject);

        return true;
    }

    private LinkedList<GameObject> GetRecycledObjects(GameObject prefab)
    {
        if (map.ContainsKey(prefab))
        {
            return map[prefab];
        } else
        {
            LinkedList<GameObject> pool = new LinkedList<GameObject>();

            map.Add(prefab, pool);

            return pool;
        }
    }
}
