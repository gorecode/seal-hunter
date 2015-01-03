using UnityEngine;
using System.Collections;

public class PrefabLocator : MonoBehaviour {
    public static PrefabLocator INSTANCE;

    public GameObject bloodSparksPrefab;

    public void Awake()
    {
        INSTANCE = this;
    }
}
