using UnityEngine;
using System.Collections;

public class ServiceLocator : MonoBehaviour {
    public static ServiceLocator INSTANCE;

    public GameObject bloodSparksPrefab;
    public GameObject bulletPrefab;
    public SlaughterBackground slaughterBackgroundController;

    public void Awake()
    {
        INSTANCE = this;
    }
}
