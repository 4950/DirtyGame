using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components.Movement
{
    public abstract class Mover : Component
    {
        public Vector2 Velocity
        {
            get;
            set;
        }

        public Vector2 DesiredVelocity
        {
            get;
            set;
        }

        public Vector2 MaxVelocity
        {
            get;
            set;
        }

        public Vector2 SteerForce
        {
            get;
            set;

        }

        public Vector2 MaxForce
        {
            get;
            set;
        }

        public Mover()
        {
            Velocity = new Vector2();
            DesiredVelocity = new Vector2();
            MaxVelocity = new Vector2(75, 75);
            SteerForce = new Vector2();
            MaxForce = new Vector2(5, 5);
        }
    }
}
