using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Layers
    {
        public static readonly int ENEMY_CORPSES = LayerMask.NameToLayer("Enemy Corpses");
        public static readonly int BACKGROUND = LayerMask.NameToLayer("Background");

        private Layers()
        {
        }
    }
}
