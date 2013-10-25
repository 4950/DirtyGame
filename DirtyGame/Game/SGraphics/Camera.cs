using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.SGraphics
{
    public class Camera
    {    
        //private Matrix projection;
        //private Matrix view;
        
        //private bool dirty;
        private Vector3 lookAt;
        private Vector3 position;
        private float rotation;
        private float zoom;

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(position);
                //return Matrix.CreateLookAt(position, lookAt, Vector3.UnitY);
            }
        }
                                    
        //public Camera(float width, float height, float near, float far)
        public Camera()
        {
            //projection = Matrix.CreateOrthographic(width, height, near, far);
           // projection = Matrix.CreateOrthographicOffCenter(-100, 100, 100, -100, -100, 100);
            position = new Vector3(0, 0, 1);
            lookAt = new Vector3(0, 0, 0);
            //view = Matrix.Identity;
           // dirty = true;

        }

        public void MoveTo(Vector2 pos)
        {
            position.X = pos.X;
            position.Y = pos.Y;
            lookAt.X = pos.X;
            lookAt.Y = pos.Y;
          //  dirty = true;
        }

        public void Translate(Vector2 translation)
        {
            position.X += translation.X;
            position.Y += translation.Y;
            lookAt.X += translation.X;
            lookAt.Y += translation.Y;
          //  dirty = true;
        }      
    }
}
