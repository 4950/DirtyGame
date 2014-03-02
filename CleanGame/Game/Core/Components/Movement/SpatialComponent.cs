using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;

namespace CleanGame.Game.Core.Components
{
    public class SpatialComponent : Component
    {

        
        private float defaultRotationCons;
        public Boolean ShouldRotate;

        #region Constructors
        public SpatialComponent()
        {
            ShouldRotate = false;
            defaultRotationCons = 0;
            Position = new Vector2(0, 0);
            Bounds = new Rectangle();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Position of the top-left corner
        /// </summary>
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

        /// <summary>
        /// Size of the spatial
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2((int)Bounds.Width, (int)Bounds.Height);
            }
            set
            {
                Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)value.X, (int)value.Y);
            }
        }


        public float DefaultRotationCons
        {
            get
            {
                return defaultRotationCons;
            }
            set
            {
                defaultRotationCons = value;
            }
        }

        /// <summary>
        /// Rotation in Radians
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Rotates the spatial by specified Radians/sec
        /// </summary>
        public float ConstantRotation { get; set; }

        public bool isMoving
        {
            get;
            set;
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
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
            }
        }
        #endregion



        #region Functions

        #endregion
    }
}
