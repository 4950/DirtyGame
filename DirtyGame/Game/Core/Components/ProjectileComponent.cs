using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components
{
    class ProjectileComponent : Component
    {
        public Vector2 origin;
        public Vector2 direction;
        public Entity owner;
        public float damage;
        public float range;
    }
}
