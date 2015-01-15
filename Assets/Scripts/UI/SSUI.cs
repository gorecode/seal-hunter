using UnityEngine;
using System.Collections;

public class SSUI {
    public static Vector2 WorldToMainWindowPoint(Vector3 point)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(point);
        Vector2 point2d;
        point2d.x = Mathf.Lerp(-320, 320, viewportPoint.x);
        point2d.y = Mathf.Lerp(-240, 240, viewportPoint.y);
        return point2d;
    }
}
