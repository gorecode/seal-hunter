using UnityEngine;
using System.Collections;
using System;

namespace UnityEngineExt {
    public static class CSharpRuntimeExt {
        public static string GetMethodName(this Action method)
        {
            return method.Method.Name;
        }
    }
}
