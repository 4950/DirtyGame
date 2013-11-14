using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using EntityFramework;
using DirtyGame.game.Core.Components.Movement;

namespace DirtyGame.game.Core.Systems.Util
{
    public class SystemDescriptions
    {
        public static SystemParams SpriteRenderSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {typeof (SpriteComponent), typeof (SpatialComponent)}), 1000);

        public static SystemParams PhysicsSystem =
          new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(SpatialComponent) , typeof(PhysicsComponent) }), 1);

        public static SystemParams AnimationSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(SpriteComponent), typeof(AnimationComponent), typeof(DirectionComponent) }), 1);

        public static SystemParams PlayerControlSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PlayerComponent), typeof (SpatialComponent), typeof(DirectionComponent)}), 1);

        public static SystemParams CameraUpdateSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PlayerComponent) }), 1);

        public static SystemParams MapBoundrySystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(SpatialComponent), typeof(SpriteComponent) }), 1);

        public static SystemParams SpawnerSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof (SpatialComponent), typeof(SpawnerComponent) }), 1);

        public static SystemParams GameLogicSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(HealthComponent) }), 1);

        public static SystemParams MonsterSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MonsterComponent), typeof(TimeComponent), typeof(SpatialComponent), typeof(SpriteComponent), typeof(DirectionComponent) }), 1);

        public static SystemParams MovementSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MovementComponent), typeof(TimeComponent), typeof(SpatialComponent), typeof(SpriteComponent) }), 1);

        public static SystemParams HUDSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {  typeof(SpatialComponent), typeof(HealthComponent) }), 1);

    }
}
