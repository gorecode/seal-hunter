using UnityEngine;
using System.Collections;

public class BaseBehaviour : MonoBehaviour, ITouchable {
    public void RemovePhysics()
    {
        collider2D.enabled = false;

        Destroy(rigidbody2D);
    }

    public virtual void OnTouch() 
    {
    }
}
