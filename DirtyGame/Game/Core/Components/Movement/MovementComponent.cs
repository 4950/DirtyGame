using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components
{
    class MovementComponent : Component
    {

       #region Constructors
        public MovementComponent()
        {
            Velocity = new Vector2(0, 0);  
        }
        #endregion

        #region Properties

        public Vector2 Velocity
        {
            get;
            set;
        }

        public float Vertical
        {
            get
            {
                return Velocity.Y;
            }
            set
            {
                Velocity = new Vector2(Velocity.X, value);
            }
        }

        public float Horizontal
        {
            get
            {
                return Velocity.X;
            }
            set
            {
                Velocity = new Vector2(value, Velocity.Y);
            }
        }

        #endregion

        #region Functions
        
        #endregion

    }
}
