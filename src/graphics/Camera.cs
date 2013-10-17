using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShittyPrototype.src.graphics
{
    class Camera
    {
        public Camera()
        {
            Position = Vector2.Zero;
            Zoom = 1f;
            Rotation = 0f;
            MinimumPosition = Vector2.Zero;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 MinimumPosition { get; set; }
        public Vector2 MaximumPosition { get; set; }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom) *
                       Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
            }
        }
    }
}
