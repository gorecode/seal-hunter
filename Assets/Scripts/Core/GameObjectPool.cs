using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool {
    public static readonly GameObjectPool Instance = new GameObjectPool();

    private Dictionary<GameObject, LinkedList<GameObject>> map = new Dictionary<GameObject, LinkedList<GameObject>>();

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        LinkedList<GameObject> recycled = GetRecycledObjects(prefab);

        if (recycled.First == null)
        {
            GameObject newObject = (GameObject)GameObject.Instantiate(prefab, position, rotation);
            newObject.AddComponent<PrefabField>();

            PrefabField marker = newObject.GetComponent<PrefabField>();
            marker.prefab = prefab;

            return newObject;
        } else
        {
            Debug.Log("Prefab reused.");

            GameObject oldObject = recycled.First.Value;

            oldObject.transform.position = position;
            oldObject.transform.rotation = rotation;
            oldObject.SetActive(true);

            recycled.RemoveFirst();

            return oldObject;
        }
    }

    public void Recycle(GameObject gameObject)
    {
        PrefabField marker = gameObject.GetComponent<PrefabField>();

        if (marker == null) Debug.LogError("PrefabField component is required to use GameObjectPool.");

        LinkedList<GameObject> recycled = GetRecycledObjects(marker.prefab);

        gameObject.SetActive(false);

        recycled.AddFirst(gameObject);
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
