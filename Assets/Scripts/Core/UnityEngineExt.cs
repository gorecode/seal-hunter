using UnityEngine;
using System.Collections;

namespace UnityEngineExt {
    public static class UnityEngineExt {
        public static Vector2 Rotate(this Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            Vector3 r = Vector3.zero;
            r.x = v.x;
            r.y = v.y;
            return r;
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            Vector2 r;
            r.x = v.x;
            r.y = v.y;
            return r;
        }
    }
}
