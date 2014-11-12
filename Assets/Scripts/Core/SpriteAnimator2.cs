using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteList
{
    public Sprite[] sprites;
}

public class SpriteAnimator2 : MonoBehaviour {
    public int index;
    public int sheet;

    public SpriteList[] sheets;

    private SpriteRenderer mySpriteRenderer;

    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        mySpriteRenderer.sprite = sheets[sheet].sprites[index];
    }
}