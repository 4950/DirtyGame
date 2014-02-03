using EntityFramework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGame.game.Core.Events
{
    public class DetonateEvent : Event
    {
        public Vector2 center { get; set; }

        public EntityRef Weapon { get; set; }

        public EntityRef Owner { get; set; }
    }
}
