using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace DirtyGame.game.Core.Components
{
    public class PhysicsComponent : Component
    {

        

        #region Constructors
        public PhysicsComponent()
        {
            Origin = new Vector2(.5f, .5f);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Origin for rotation. Default is (.5f, .5f)
        /// </summary>
        public Vector2 Origin { get; set; }
        #endregion

        #region Functions
        
        #endregion
    }
}
