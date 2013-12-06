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
        private SpriteSheet meleeSpriteSheet;

        public MeleeComponent()
        {

        }
        
        public Entity MeleeEntity
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
        public SpriteSheet MeleeSpriteSheet
        {
            get
            {
                return meleeSpriteSheet;
            }
            set
            {
                meleeSpriteSheet = value;
            }
        }
    }
}
