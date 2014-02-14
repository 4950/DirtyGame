using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.SGraphics;
using EntityFramework;


namespace CleanGame.Game.Core.Components
{
    public abstract class RenderComponent : Component
    {
        protected RenderComponent()
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
