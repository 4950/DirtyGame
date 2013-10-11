using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShittyPrototype.src.graphics
{
    //So far I only have it interacting with entities, not the map.  The pink square is the player, the white one is a fixed entity.
    //I haven't figured out why the white one jumpes away at the start (it's above and to the left of the screen)
    class Camera
    {
        public int x;
        public int y;

        public Camera(int a, int b)
        {
            x = a;
            y = b;
        }
    }
}
