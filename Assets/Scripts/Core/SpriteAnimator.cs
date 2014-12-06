using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpriteList
{
    /// <summary>
    /// The sprites.
    /// </summary>
    public Sprite[] sprites;
    /// <summary>
    /// The pivots. XY = position of sprite, Z = index of sprite.
    /// </summary>
    public Vector3[] pivots;
}

[ExecuteInEditMode]
public class SpriteAnimator : MonoBehaviour {
    public float index;
    public float sheet;

    public SpriteList[] sheets;

    private SpriteRenderer mySpriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        mySpriteRenderer.sprite = sprite;
    }

    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (mySpriteRenderer == null || sheets == null) return;

        int sheetIndex = (int)sheet;
        int spriteIndex = (int)index;

        SpriteList sheetObj = sheets[sheetIndex];

        mySpriteRenderer.sprite = sheetObj.sprites[Mathf.Clamp(spriteIndex, 0, sheetObj.sprites.Length - 1)];

        Vector3[] pivots = sheets[sheetIndex].pivots;

        if (pivots != null && pivots.Length > 0)
        {
            Vector3 pivot = Vector3.zero;

            for (int i = pivots.Length - 1; i >= 0; i--)
            {
                if ((int)pivots[i].z <= spriteIndex)
                {
                    pivot = pivots[i];
                    break;
                }
            }

            pivot.z = 0.0f;

            transform.localPosition = pivot;
        }
    }
}