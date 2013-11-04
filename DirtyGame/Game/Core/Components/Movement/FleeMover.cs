using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components.Movement
{
    class FleeMover : Mover
    {
        public float FleeRadius
        {
            get;
            set;
        }

        public Entity FleeTarget
        {
            get;
            set;
        }

        public FleeMover()
        {
            FleeRadius = 250;
            FleeTarget = null;

        }
    }
}
