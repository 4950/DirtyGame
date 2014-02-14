using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class GrenadeComponent : Component
    {
        public Vector2 origin;
        public Vector2 direction;
        [XmlIgnoreAttribute]
        public Entity owner;
        public Entity weapon;
        public float range;
        public float fuseTime;
        public Vector2 explosionSize;
    }
}
