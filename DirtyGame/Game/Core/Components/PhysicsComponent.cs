using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics;
using DirtyGame.game.Core.Events;

namespace DirtyGame.game.Core.Components
{
    class PhysicsComponent : Component
    {
        #region Constructors
        public PhysicsComponent()
        {
            CollisionCallbackString = new List<string>();
        }
        #endregion

        #region Properties

        private List<string> CollisionCallbackString;

        #endregion

        #region Functions

        public void AddCollisionCallback(string name, EventManager.EventCallback collisionCallback)
        {
            EventManager.Instance.AddListener(name, collisionCallback);
            CollisionCallbackString.Add(name);
        }

        public void RemoveCollisionCallback(string name, EventManager.EventCallback collisionCallback)
        {
            EventManager.Instance.RemoveListener(name, collisionCallback);
            CollisionCallbackString.Remove(name);
        }

        public void ExecuteCollisionList(Entity entityA, Entity entityB)
        {
            foreach (string collisionCallback in CollisionCallbackString)
            {
                CollisionEvent collision = new CollisionEvent();
                collision.name = collisionCallback;
                collision.entityA = entityA;
                collision.entityB = entityB;
                EventManager.Instance.TriggerEvent(collision);
            }
        }

        public bool IsCollisionCallbackEmpty()
        {
            if (CollisionCallbackString.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
