using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using ShittyPrototype.src.core;
using ShittyPrototype.src.graphics;
using ShittyPrototype.src.util;
using ShittyPrototype.src.Map;
using ShittyPrototype.src.application;
using System.Diagnostics;
using ShittyPrototype.src.application.core;
using ShittyPrototype.src;

namespace ShittyPrototype.src.graphics
{
    class Gamestate
    {

        public bool Over = false;

        public Gamestate()
        {
        }

        public bool Gameover()
        {
            return Over;
        }
    }
}
