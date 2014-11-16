using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class Coin : MonoBehaviour {
    public float flyUpSpeed = 1.2f;
    public float rotationSpeed = 1000;
    public float lifeTime = 0.4f;

    public AudioClip audioClip;

    public Action recycleAction;

    void Awake()
    {
        recycleAction = () => Recycle();
    }

    void OnEnable()
    {
        Invoke(recycleAction.GetMethodName(), lifeTime);

        AudioCenter.PlayClipAtMainCamera(audioClip);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        transform.position += flyUpSpeed * Time.deltaTime * Vector3.up;
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }

    void Recycle()
    {
        GameObjectPool.Instance.Recycle(gameObject);
    }
}
