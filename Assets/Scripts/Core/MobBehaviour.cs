using UnityEngine;
using System.Collections;

public class MobBehaviour : MonoBehaviour, ITouchable {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RemovePhysics()
    {
        collider2D.enabled = false;

        Destroy(rigidbody2D);
    }

    public virtual void OnTouch() 
    {
    }
}
