using EntityFramework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanGame.Game.Core.Events
{
    public class DetonateEvent : Event
    {
        public Vector2 center { get; set; }

        public Vector2 size { get; set; }

        public EntityRef Weapon { get; set; }

        public EntityRef Owner { get; set; }
    }
}
