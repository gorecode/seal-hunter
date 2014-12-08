using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteAnimator))]
public class Muzzle : SSBehaviour {
    public float flashDuration = 0.1f;

    private float onEnableTime;

    void OnEnable()
    {
        onEnableTime = Time.time;

        Update();

        mySpriteAnimator.Update();
    }

    void Update()
    {
        float t = Mathf.Min(1.0f, (Time.time - onEnableTime) / flashDuration);

        mySpriteAnimator.index = Mathf.Lerp(0, mySpriteAnimator.sheets[0].sprites.Length, t);

        if (t >= 1.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
