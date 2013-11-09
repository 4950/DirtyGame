using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace DirtyGame.game.Core
{
    public class Physics
    {
        private Dictionary<int, Entity> entityDictionary;
        private FarseerPhysics.Dynamics.World physicsWorld;

        public Physics()
        {
            physicsWorld = new FarseerPhysics.Dynamics.World(new Vector2(0, 0));
            entityDictionary = new Dictionary<int, Entity>();
        }

        public FarseerPhysics.Dynamics.World World
        {
            get
            {
                return physicsWorld;
            }
        }


        public void Add(int key, Entity e)
        {
            entityDictionary.Add(key, e);
        }

        public void Remove(int key)
        {
            if (entityDictionary.ContainsKey(key))
            {
                entityDictionary.Remove(key);
            }
        }

        public void Update(GameTime gameTime)
        {
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }
    }
}
