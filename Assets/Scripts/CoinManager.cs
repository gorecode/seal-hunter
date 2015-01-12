using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;
using System.Text;

public class CoinManager : MonoBehaviour {
    public GameObject coinPrefab;

    private int coins;

    void Start()
    {
    }

    void OnEnable()
    {
        EventBus.OnBecomeDying += OnBecomeDying;
    }

    void OnDisable()
    {
        EventBus.OnBecomeDying -= OnBecomeDying;
    }

    private void OnBecomeDying(GameObject enemy)
    {
        if (coinPrefab == null) return;
        GameObject newCoin = ServiceLocator.current.pool.Instantiate(coinPrefab, enemy.transform.position, Quaternion.identity);
        
        Transform sprite = enemy.transform.GetChild(0);
        BoxCollider2D bb = sprite.collider2D as BoxCollider2D;
        
        newCoin.transform.position += new Vector3(bb.center[0], bb.center[1], 0);

        coins++;
    }
}
