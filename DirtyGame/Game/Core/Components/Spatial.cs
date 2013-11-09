using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Components
{
    class Spatial : Component
    {
        #region Constructors
        public Spatial()
        {
            Position = new Vector2(0, 0);
            Bounds = new Rectangle();
        }
        #endregion

        #region Properties

        public Vector2 Position
        {
            get
            {
                return new Vector2((int)Bounds.X, (int)Bounds.Y);
            }
            set
            {
                Bounds = new Rectangle((int)value.X, (int)value.Y, Bounds.Width, Bounds.Height);
            }
        }

        public Rectangle Bounds
        {
            get;
            set;
        }

        public int Height
        {
            get
            {
               return Bounds.Height;
            }
            set
            {
                Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, value);
            }
        }

        public int Width
        {
            get
            {
                return Bounds.Width;
            }
            set
            {
                Bounds = new Rectangle((int)Position.X, (int)Position.Y, value, Bounds.Height);
            }
        }

        #endregion

        #region Functions

        #endregion
    }
}
