using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class EventBus
    {
        public static readonly EnemyDiedEvent EnemyDied = new EnemyDiedEvent();
    }
}
