using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;

namespace CleanGame.Game.Core.Components
{
    public class BorderComponent : Component
    {
        #region Constructors
        private BorderComponent()
        {
        }
        public BorderComponent(Vector2 topLeft, Vector2 bottomLeft, Vector2 topRight, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            BottomRight = bottomRight;

        }
        #endregion

        #region Properties
        public Vector2 TopLeft { get; set; }

        public Vector2 BottomLeft { get; set; }

        public Vector2 TopRight { get; set; }

        public Vector2 BottomRight { get; set; }

        #endregion

        #region Functions
        
        #endregion
    }
}
