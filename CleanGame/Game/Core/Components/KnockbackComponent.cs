using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace CleanGame.Game.Core.Components
{
    public class KnockbackComponent : Component
    {
        public Vector2 direction { get; set; } 
        public float distance { get; set; } //distance per tick
        public float ticks { get; set; }

        public KnockbackComponent()
        {
        }

        public KnockbackComponent(Vector2 dir, float dist, float tck)
        {
            direction = dir;
            direction.Normalize();
            distance = dist;
            ticks = tck;
        }
    }
}
