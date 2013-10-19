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
            Position = new Vector2(0f, 0f);
        }
        #endregion

        #region Properties
        public Vector2 Position
        {
            get;
            set;
        }
        #endregion

        #region Functions
        public void MoveTo(Vector2 pos)
        {
            Position = pos;
        }

        public void MoveTo(float x, float y)
        {
            MoveTo(new Vector2(x, y));
        }

        public void Translate(Vector2 tanslation)
        {
            Position += tanslation;
        }

        public void Translate(float x, float y)
        {
            Translate(new Vector2(x, y));
        }
        #endregion
    }
}
