using UnityEngine;
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class SpriteList
{
    public Sprite[] sprites;
}

public class SpriteAnimator : MonoBehaviour {
    public float index;
    public float sheet;

    public SpriteList[] sheets;

    private SpriteRenderer mySpriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        mySpriteRenderer.sprite = sprite;
    }

    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (mySpriteRenderer != null && sheets != null)
        {
            mySpriteRenderer.sprite = sheets[(int)sheet].sprites[(int)index];
        }
    }
}