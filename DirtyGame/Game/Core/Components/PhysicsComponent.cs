using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

namespace DirtyGame.game.Core.Components
{
    class PhysicsComponent : Component
    {
        #region Constructors
        public PhysicsComponent()
        {
            CollisionList = new List<CollisionCallback>();
        }
        #endregion

        #region Properties
     
        public delegate void CollisionCallback(Entity entityA, Entity entityB);

        private List<CollisionCallback> CollisionList;

        #endregion

        #region Functions

        public void AddCollisionCallback(CollisionCallback collisionCallbacK)
        {
            CollisionList.Add(collisionCallbacK);
        }

        public void RemoveCollisionCallback(CollisionCallback collisionCallback)
        {
            CollisionList.Remove(collisionCallback);
        }

        public void ExecuteCollisionList(Entity entityA, Entity entityB)
        {
            foreach (CollisionCallback collisionCallback in CollisionList)
            {
                collisionCallback(entityA, entityB);
            }
        }

        public bool IsCollisionListEmpty()
        {
            if (CollisionList.Count > 0)
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
