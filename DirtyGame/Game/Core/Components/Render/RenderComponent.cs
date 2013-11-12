using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics;
using EntityFramework;


namespace DirtyGame.game.Core.Components
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
