using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BoxColliderAutoUpdate : MonoBehaviour {
    private SpriteRenderer mySpriteRenderer;
    private BoxCollider2D myBoxCollider2D;

    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Sprite sprite = mySpriteRenderer.sprite;

        if (sprite == null) return;
        if (!myBoxCollider2D.enabled) return;

        Bounds spriteBounds = sprite.bounds;

        myBoxCollider2D.center = spriteBounds.center;
        myBoxCollider2D.size = spriteBounds.size;
    }
}
