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

                SpatialComponent spatial = e.GetComponent<SpatialComponent>();
                DirectionComponent direction = e.GetComponent<DirectionComponent>();
                MovementComponent movement = e.GetComponent<MovementComponent>();
                

                KeyboardState = Keyboard.GetState();
                if (KeyboardState.IsKeyDown(Keys.Left))
                {
                    //Arbitrarily chosen number of pixels... speed can easily be added if we want
                    movement.Horizontal = -5;

                    direction.Heading = "Left";
                }                
                else if (KeyboardState.IsKeyDown(Keys.Right))
                {
                    movement.Horizontal = 5;

                    direction.Heading = "Right";

                }
                else
                {
                    movement.Horizontal = 0;
                }

                if (KeyboardState.IsKeyDown(Keys.Up))
                {
                    movement.Vertical = -5;

                    direction.Heading = "Up";
                }
                else if (KeyboardState.IsKeyDown(Keys.Down))
                {
                    movement.Vertical = 5;

                    direction.Heading = "Down";
                }
                else
                {
                    movement.Vertical = 0;
                }

                if (movement.Velocity == new Vector2(0f, 0f))
                {
                    spatial.isMoving = false;
                }

                else
                {
                    spatial.isMoving = true;
                }
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
