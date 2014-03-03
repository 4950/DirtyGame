using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using EntityFramework;
using CleanGame.Game.Core.Components.Movement;

namespace CleanGame.Game.Core.Systems.Util
{
    public class SystemDescriptions
    {
        public static SystemParams SpriteRenderSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {typeof (SpriteComponent), typeof (SpatialComponent)}), 1000);

        public static SystemParams MeleeSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MeleeComponent) , typeof(TimeComponent)}), 1);

        public static SystemParams PhysicsSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PhysicsComponent) }), 1);

        public static SystemParams AnimationSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(SpriteComponent), typeof(AnimationComponent) }), 1);

        public static SystemParams PlayerControlSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PlayerComponent), typeof (SpatialComponent), typeof(DirectionComponent)}), 1);

        public static SystemParams CameraUpdateSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PlayerComponent) }), 1);

        public static SystemParams MapBoundrySystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(SpatialComponent), typeof(SpriteComponent) }), 1);

        public static SystemParams SpawnerSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof (SpatialComponent), typeof(SpawnerComponent) }), 1);

        public static SystemParams GameLogicSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(StatsComponent) }), 1);

        public static SystemParams TutorialSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(StatsComponent) }), 1);

        public static SystemParams MonsterSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MonsterComponent), typeof(TimeComponent), typeof(SpatialComponent), typeof(SpriteComponent), typeof(DirectionComponent) }), 1);

        public static SystemParams MovementSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MovementComponent), typeof(SpatialComponent), typeof(SpriteComponent) }), 1);

        public static SystemParams HUDSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> {  typeof(SpatialComponent), typeof(StatsComponent) }), 1);

        public static SystemParams SeparationSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(MovementComponent), typeof(SeparationComponent) }), 1);

        public static SystemParams ProjectileSystem =
            new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(ProjectileComponent) }), 1);

        public static SystemParams WeaponSystem =
           new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(WeaponComponent) }), 1);

        public static SystemParams AOESystem =
           new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(AOEComponent) }), 1);

        public static SystemParams GrenadeSystem =
           new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(GrenadeComponent) }), 1);

        public static SystemParams PropertySystem =
           new SystemParams(Aspect.CreateAspectFor(new List<Type> { typeof(PropertyComponent) }), 0);
    }
}
