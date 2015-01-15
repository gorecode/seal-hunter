using UnityEngine;
using UnityEngineExt;
using UnityEditor;
using System.Collections;
using System.Linq;

public class ShellHit : SSBehaviour {
    public Texture2D spriteTexture;
    public float duration;

    private float startTime;
    private Sprite[] sprites;

    new void Awake()
    {
        base.Awake();

        if (spriteTexture != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(spriteTexture);
        
            sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
        }
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (sprites == null) return;

        float t = Mathf.Min(1f, (Time.time - startTime) / duration);

        int index = (int)Mathf.Lerp(0f, sprites.Length - 1, t);

        mySpriteRenderer.sprite = sprites[index];

        if (t >= 1.0f) gameObject.Release();
    }
}
