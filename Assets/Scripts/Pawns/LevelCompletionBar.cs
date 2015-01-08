using UnityEngine;
using System.Collections;

public class LevelCompletionBar : MonoBehaviour {
    private LineRenderer lineRenderer;

    public float minX = -3.2f;
    public float maxX = 3.2f;
    public float y = -2.4f;

    public float progress;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 v1 = Vector3.zero;
        Vector3 v2 = Vector3.zero;

        v1.x = minX;
        v2.x = Mathf.Lerp(minX, maxX, progress);
        v1.y = v2.y = y;

        lineRenderer.SetPosition(0, v1);
        lineRenderer.SetPosition(1, v2);
    }
}
