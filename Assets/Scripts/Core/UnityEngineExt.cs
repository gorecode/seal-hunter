using UnityEngine;
using System.Collections;

namespace UnityEngineExt {
    public static class UnityEngineExt {
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
