using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.SGraphics
{
    public class Camera
    {    
        private Vector2 position;
        private float zoom;
        public Matrix Transform
        {
            get
            {
                return Matrix.CreateScale(zoom) * Matrix.CreateTranslation(-position.X, -position.Y, 0);
            }
        }
                                    
        public Camera()
        {
            position = new Vector2(0, 0);
            zoom = 1;
        }

        public void Zoom(float factor)
        {
            zoom = factor;
        }

        public void MoveTo(Vector2 pos)
        {
            position.X = pos.X;
            position.Y = pos.Y;
        }

        public void Translate(Vector2 translation)
        {
            position.X += translation.X;
            position.Y += translation.Y;
        }      
    }
}
