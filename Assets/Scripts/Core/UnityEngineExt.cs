using UnityEngine;
using System.Collections;

namespace UnityEngineExt {
    public static class UnityEngineExt {
        public static Vector2 ToVector2(this Vector3 v)
        {
            Vector2 r;
            r.x = v.x;
            r.y = v.y;
            return r;
        }
    }
}
