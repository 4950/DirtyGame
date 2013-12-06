using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;

namespace DirtyGame.game.Core.Components
{
    public class MeleeComponent : Component
    {
        private Entity meleeEntity;

        public MeleeComponent()
        {

        }
        
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
        public List<Entity> targetsHit = new List<Entity>();
    }
}
