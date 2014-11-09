using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animation))]
public class SpriteAnimator : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    private Animation myAnimation;
    
    void Awake()
    {
        mySpriteRenderer = renderer as SpriteRenderer;
        myAnimation = animation as Animation; 
    }

    public void SetSprite(Sprite sprite)
    {
       mySpriteRenderer.sprite = sprite;
    }
}