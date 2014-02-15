using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.SGraphics
{
    public class Camera
    {    
        private Vector2 position;


        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        public Matrix Transform
        {
            get
            {
               // return Matrix.CreateTranslation(-position.X, -position.Y, 0);
                return View * Proj;
            }
        }

        public Matrix Proj
        {
            get
            {
                return Matrix.CreateOrthographic(2, 2, 0, 10);
            }
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(new Vector3(position.X, position.Y, 1), new Vector3(position.X, position.Y, 0), Vector3.Up);
            }
        }
        public Vector2 size { get; set; }
                                    
        public Camera(Vector2 size)
        {
            position = new Vector2(0, 0);
            this.size = size;
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
