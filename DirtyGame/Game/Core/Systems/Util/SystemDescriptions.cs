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
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {typeof (Sprite), typeof (Spatial)}), 1000);

        public static SystemParams AnimationSystem =
    new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(Sprite), typeof(Animation) }), 1);

        public static SystemParams PlayerControlSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(Player), typeof (Spatial)}), 1);

        public static SystemParams CameraUpdateSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(Player) }), 1);

        public static SystemParams MapBoundrySystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(Spatial) }), 1);
    }
}
