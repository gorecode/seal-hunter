using UnityEngine;
using System.Collections;

public class SSBehaviour : MonoBehaviour, ITouchable {
    protected Transform myParent;
    protected SpriteRenderer mySpriteRenderer;
    protected SpriteAnimator mySpriteAnimator;
    protected Animation myAnimation;
    protected Rigidbody2D myRigidbody2D;
    protected Animator myAnimator;

    protected void Awake()
    {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySpriteAnimator = GetComponent<SpriteAnimator>();

        myParent = transform.parent;
        myAnimation = animation;
        myRigidbody2D = rigidbody2D;
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
