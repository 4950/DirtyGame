﻿using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace DirtyGame.game.Core.Components
{
    public class ProjectileComponent : Component
    {
        public Vector2 origin;
        public Vector2 direction;
        [XmlIgnoreAttribute]
        public Entity owner;
        public Entity weapon;
        public float range;
    }
}
