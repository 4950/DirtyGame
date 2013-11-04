using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;

namespace DirtyGame.game.Core.Components.Movement
{
    public class SeekMover : Mover
    {
        public Entity Target
        {
            get;
            set;
        }

        public float SlowingDistance
        {
            get;
            set;
        }

        public SeekMover()
        {
            Target = null;
            SlowingDistance = 100;
        }
    }
}
