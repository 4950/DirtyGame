using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace CleanGame.Game.Core.Components
{
    public class PhysicsComponent : Component
    {

        

        #region Constructors
        public PhysicsComponent()
        {
            Origin = new Vector2(.5f, .5f);
            movePlayer = false;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Origin for rotation. Default is (.5f, .5f)
        /// </summary>
        public Vector2 Origin { get; set; }
        public bool movePlayer { get; set; }
        public bool collidingWithPlayer { get; set; }
        #endregion

        #region Functions
        
        #endregion
    }
}
