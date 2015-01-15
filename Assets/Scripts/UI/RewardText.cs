using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class RewardText : MonoBehaviour {
    public float tweenDuration = 0.25f;
    public float distance;

    private UILabel label;
    private float startTime;
    private float speed;

    void Awake()
    {
        label = GetComponent<UILabel>();
    }

    void OnEnable()
    {
        startTime = Time.time;
        speed = distance / tweenDuration;
    }

    void Update()
    {
        float t = Mathf.Min(1.0f, (Time.time - startTime) / tweenDuration);
        Color color = label.color;
        color.a = Mathf.Lerp(1.0f, 0.0f, t);
        label.color = color;

        Vector3 pos = transform.localPosition;
        pos.y += Time.deltaTime * speed;
        transform.localPosition = pos;

        if (t >= 1.0f) gameObject.Release();
    }
}
