using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DirtyGame.game.Core.Systems
{
    class PlayerControlSystem : EntitySystem
    {
        private KeyboardState KeyboardState;

        public PlayerControlSystem()
            : base(SystemDescriptions.PlayerControlSystem.Aspect, SystemDescriptions.PlayerControlSystem.Priority)
        {
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            foreach (Entity e in entities)
            {
    //            if (!e.HasComponent<Player>()) continue;

                Spatial spatial = e.GetComponent<Spatial>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();

                Vector2 translateVector = new Vector2(0,0); 

                KeyboardState = Keyboard.GetState();
                if (KeyboardState.IsKeyDown(Keys.Left))
                {
                    //Arbitrarily chosen number of pixels... speed can easily be added if we want
                    translateVector.X -= 5;

                    direction.Heading = "Left";
                }
                if (KeyboardState.IsKeyDown(Keys.Right))
                {
                    translateVector.X += 5;

                    direction.Heading = "Right";
                }
                if (KeyboardState.IsKeyDown(Keys.Up))
                {
                    translateVector.Y -= 5;

                    direction.Heading = "Up";
                }
                if (KeyboardState.IsKeyDown(Keys.Down))
                {
                    translateVector.Y += 5;

                    direction.Heading = "Down";
                }

                spatial.Translate(translateVector);
                spatial.isMoving = (translateVector.Length() != 0);

            }
        }

        public override void OnEntityAdded(Entity e)
        {
            // do nothing
        }

        public override void OnEntityRemoved(Entity e)
        {
            // do nothing
        }

    }
}
