using UnityEngine;
using System.Collections;

namespace UnityEngineExt {
    public static class MonoBehaviourExt {
        public static void RemovePhysics(this MonoBehaviour thiz)
        {
            thiz.collider2D.enabled = false;

            GameObject.Destroy(thiz.rigidbody2D);
        }
    }
}