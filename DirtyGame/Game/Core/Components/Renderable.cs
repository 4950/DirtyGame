using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using EntityFramework;


namespace DirtyGame.game.Core.Components
{
    abstract class Renderable : Component
    {
        protected Renderable()
        {
            RenderLayer = RenderLayer.INVALIDE;
        }

        public RenderLayer RenderLayer
        {
            get;
            set;
        }
    }
}
