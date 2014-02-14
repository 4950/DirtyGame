using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using EntityFramework;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Components;
using Microsoft.Xna.Framework;
using CoreUI;
using CleanGame.Game.SGraphics;
using CoreUI.Elements;
using CleanGame.Game.Core.Events;
using CleanGame.Game.Util;

namespace CleanGame.Game.Core.Systems
{
    class GrenadeSystem : EntitySystem
    {
        private Dirty game;
        public GrenadeSystem(Dirty game)
            : base(SystemDescriptions.GrenadeSystem.Aspect, SystemDescriptions.GrenadeSystem.Priority)
        {
            this.game = game;
        }
        public override void OnEntityAdded(EntityFramework.Entity e)
        {
        }

        public override void OnEntityRemoved(EntityFramework.Entity e)
        {
        }
        public override void ProcessEntities(IEnumerable<EntityFramework.Entity> entities, float dt)
        {
            for (int i = 0; i < entities.Count(); i++)
            {
                Entity e = entities.ElementAt(i);
                if (e.HasComponent<GrenadeComponent>())
                {
                    GrenadeComponent gc = e.GetComponent<GrenadeComponent>();
                    if (Vector2.Distance(gc.origin, e.GetComponent<SpatialComponent>().Position) > gc.range)
                    {
                        if (e.HasComponent<MovementComponent>())
                        {
                            MovementComponent moveComp = e.GetComponent<MovementComponent>();
                            if (moveComp.Velocity.Length() > 0)
                            {
                                moveComp.Velocity = new Vector2(0);
                            }
                            else
                            {
                                gc.fuseTime -= dt;
                                if (gc.fuseTime <= 0)
                                {
                                    SoundSystem.Instance.PlayEffect("Explosion");

                                    DetonateEvent detEvt = new DetonateEvent();
                                    detEvt.center = e.GetComponent<SpatialComponent>().Center;
                                    detEvt.size = gc.explosionSize;
                                    detEvt.Owner = new EntityRef(gc.owner);
                                    detEvt.Weapon = new EntityRef(gc.weapon);
                                    detEvt.name = "Detonate";
                                    EventManager.Instance.TriggerEvent(detEvt);

                                    game.world.DestroyEntity(e);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
