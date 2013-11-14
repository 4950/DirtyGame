using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Events
{
    class CollisionEvent : Event
    {
        public Entity entityA { get; set; }
        public Entity entityB { get; set; }
    }
}
