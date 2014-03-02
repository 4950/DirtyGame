using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.SGraphics;
using System.Xml.Serialization;

namespace CleanGame.Game.Core.Components
{
    public class MeleeComponent : Component
    {
        [XmlIgnoreAttribute]
        private Entity meleeEntity;
        public float TimePresent;

        public MeleeComponent()
        {
            TimePresent = .3f;
        }

        [XmlIgnoreAttribute]
        public Entity Owner
        {
            get
            {
                return meleeEntity;
            }
            set
            {
                meleeEntity = value;
            }
        }
        public Entity Weapon { get; set; }
        [XmlIgnoreAttribute]
        public List<Entity> targetsHit = new List<Entity>();
    }
}
