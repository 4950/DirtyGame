using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using System.Xml.Serialization;

namespace DirtyGame.game.Core.Components
{
    public class MeleeComponent : Component
    {
        [XmlIgnoreAttribute]
        private Entity meleeEntity;

        public MeleeComponent()
        {

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
        public float Damage { get; set; }
        [XmlIgnoreAttribute]
        public List<Entity> targetsHit = new List<Entity>();
    }
}
