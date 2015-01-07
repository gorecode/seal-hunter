using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public const int DEFAULT_FRAMES_TO_LIVE = 2;

    public int framesToLive = DEFAULT_FRAMES_TO_LIVE;

    void OnEnable()
    {
        framesToLive = DEFAULT_FRAMES_TO_LIVE;
    }

    void Update()
    {
        if (framesToLive-- == 0) GameObjectPool.Instance.Release(gameObject);
    }
}
