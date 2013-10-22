using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using EntityFramework;

namespace DirtyGame.game.Core.Systems.Util
{
    public class SystemDescriptions
    {
        public static SystemParams SpriteRenderSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {typeof (Sprite), typeof (Spatial)}), 1);

        public static SystemParams AnimationSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(Sprite), typeof(Animation) }), 2);
    }
}
