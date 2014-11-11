using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    public class GameObjects
    {
        public static void DestroyRoot(GameObject go)
        {
            Transform t = go.transform;
            while (t.parent != null)
            {
                t = t.parent;
            }
            GameObject.Destroy(t.gameObject);
        }
    }
