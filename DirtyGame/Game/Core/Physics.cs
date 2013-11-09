using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EntityFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace DirtyGame.game.Core
{
    public class Physics
    {
      
        private FarseerPhysics.Dynamics.World physicsWorld;

        public Physics()
        {
            physicsWorld = new FarseerPhysics.Dynamics.World(new Vector2(0, 0));
          
        }

     

        public void Update(GameTime gameTime)
        {
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        public List<Body> Query(Vector2 center, float width, float height)
        {
            List<Body> found = new List<Body>();          
            List<Fixture> fixtures = new List<Fixture>();
                            
            FarseerPhysics.Collision.AABB Box = new FarseerPhysics.Collision.AABB(
                ConvertUnits.ToSimUnits(center),
                ConvertUnits.ToSimUnits(width),
                ConvertUnits.ToSimUnits(height));

            fixtures = physicsWorld.QueryAABB(ref Box);          
            
            foreach (Fixture f in fixtures)
            {
                if (!found.Contains(f.Body))
                {
                    found.Add(f.Body);                    
                }
            }


            return found;
        }

    }
}
