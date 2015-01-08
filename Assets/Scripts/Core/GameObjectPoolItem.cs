using UnityEngine;
using System.Collections;

public class GameObjectPoolItem : MonoBehaviour {
    public GameObject prefab;
    public GameObjectPool pool;
    public int referenceCount;
}
