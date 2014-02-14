using EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.Components
{
    public class MovementComponent : Component
    {

       #region Constructors
        public MovementComponent()
        {
            Velocity = new Vector2(0, 0);  
        }
        #endregion

        #region Properties
        public float WanderTheta { get; set; }
        public Vector2 Velocity
        {
            get;
            set;
        }

        public Vector2 prevVelocity
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

        public float prevVertical
        {
            get
            {
                return prevVelocity.Y;
            }
            set
            {
                prevVelocity = new Vector2(prevVelocity.X, value);
            }
        }

        public float prevHorizontal
        {
            get
            {
                return prevVelocity.X;
            }
            set
            {
                prevVelocity = new Vector2(value, prevVelocity.Y);
            }
        }

        #endregion

        #region Functions
        
        #endregion

    }
}
