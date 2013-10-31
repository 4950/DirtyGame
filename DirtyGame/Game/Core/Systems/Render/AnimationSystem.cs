using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.SGraphics;
using DirtyGame.game.SGraphics.Commands;
using DirtyGame.game.SGraphics.Commands.DrawCalls;
using EntityFramework;
using EntityFramework.Systems;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;


namespace DirtyGame.game.Systems
{
    class AnimationSystem : EntitySystem
    {
        public AnimationSystem()
            : base(SystemDescriptions.AnimationSystem.Aspect, SystemDescriptions.AnimationSystem.Priority)
        {
            // Register the EventCallback function with the EventManager
            EventManager.Instance.AddListener("EntityAdded", EventCallback);
        }

        public override void OnEntityAdded(Entity e)
        {           
            Event evt = new Event();
            evt.name = "EntityAdded";
            EventManager.Instance.TriggerEvent(evt);
            // I don't know how to use the logging system yet, sry
            Console.WriteLine("------- EVENT FIRED -------");
        }
        public void EventCallback(Event e)
        {
            Console.WriteLine("------- I HEARD THAT -------");
        }

        public override void OnEntityRemoved(Entity e)
        {           
            
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
                //Getting components for this entity
                Animation animation = e.GetComponent<Animation>();
                Sprite sprite = e.GetComponent<Sprite>();

                //Setting the next frame
                sprite.SpriteSheet.NextFrame(animation.CurrentAnimation, dt);
                //Setting the rectangle of the sprite sheet to draw
                sprite.SrcRect = sprite.SpriteSheet.Animation[animation.CurrentAnimation][sprite.SpriteSheet.CurrentFrame];
            }
        }
    }
}
