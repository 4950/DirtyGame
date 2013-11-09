using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using EntityFramework.Systems;
using FarseerPhysics.Factories;
using EntityFramework;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Systems
{
    class PhysicsSystem : EntitySystem 
    {

        private Physics physics;
        private FarseerPhysics.Dynamics.World physicsWorld;

        public PhysicsSystem(Physics  physics) 
            : base(SystemDescriptions.PhysicsSystem.Aspect, SystemDescriptions.PhysicsSystem.Priority)
        {
            this.physics = physics;
            physicsWorld = physics.World;

            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
        }


        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {

            foreach (Entity e in entities)
            {
                Spatial spatial = e.GetComponent<Spatial>();
                spatial.Position = ConvertUnits.ToDisplayUnits(physics.Body[e.Id].Position);
                
               if(e.HasComponent<MovementComponent>()) //Some could be static
               {
                    
                    MovementComponent movement = e.GetComponent<MovementComponent>();
                    physics.Body[e.Id].LinearVelocity = movement.Velocity;
                
                }
                


                
             
                

            }
        }

        public override void OnEntityAdded(Entity e)
        {
            Spatial spatial = e.GetComponent<Spatial>();
            Body Body = new Body(physicsWorld);

            Body = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits
                                                                                  (spatial.Width), ConvertUnits.ToSimUnits(spatial.Height), 1f, 
                                                                                  ConvertUnits.ToSimUnits(spatial.Position));

           
            
                Body.BodyType = BodyType.Dynamic;
                Body.Restitution = 0.3f;
            

            if (e.HasComponent<Collidable>())
            {
                Body.OnCollision += BodyOnCollision;
            }

            CollisionCategory(e, Body);

            physics.AddEntity(Body.BodyId, e);
            physics.AddBody(e.Id, Body);
        }

        private bool BodyOnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            List<Entity> e = new List<Entity>();
            int count = 0;
            if (physics.Entity.ContainsKey(fixtureA.Body.BodyId) && physics.Entity.ContainsKey(fixtureB.Body.BodyId))
            {
                if (physics.Entity[fixtureA.Body.BodyId].HasComponent<Player>())
                {

                    if (physics.Entity[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }

                    else if (physics.Entity[fixtureB.Body.BodyId].HasComponent<MonsterComponent>() && !physics.Entity[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {
                        World.RemoveEntity(physics.Entity[fixtureB.Body.BodyId]);
                   
                    }

                    else if (physics.Entity[fixtureB.Body.BodyId].HasComponent<MonsterComponent>() && physics.Entity[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }
                }

                else if (physics.Entity[fixtureB.Body.BodyId].HasComponent<Player>())
                {
                    if (physics.Entity[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }

                    else if (physics.Entity[fixtureA.Body.BodyId].HasComponent<MonsterComponent>() && !physics.Entity[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {
                        World.RemoveEntity(physics.Entity[fixtureA.Body.BodyId]);
                    
                    }

                    else if (physics.Entity[fixtureA.Body.BodyId].HasComponent<MonsterComponent>() && physics.Entity[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }
                }
            }
            return true;
        }

        public override void OnEntityRemoved(Entity e)
        {
            if(physics.Body.ContainsKey(e.Id)) 
            {
                if(physics.Entity.ContainsKey(physics.Body[e.Id].BodyId))
                {
                    physics.Body[e.Id].Dispose();
                    physics.RemoveEntity(physics.Body[e.Id].BodyId);
                }
                physics.RemoveBody(e.Id);
            } 
           
        }


//Cat 1 = Player, Cat2= Player Weapon, Cat3 = Monster, Cat4 = Monster Weapon
        private void CollisionCategory(Entity e, Body body)
        {
            if (e.HasComponent<Player>())
            {
                if (e.HasComponent<WeaponComponent>())
                {
                    body.CollisionCategories = Category.Cat2;
                    body.CollidesWith = Category.Cat3; //Weapon only collides with Monster (Can Change to collide with Monster Weapon too)
                }
                else
                {
                    body.CollisionCategories = Category.Cat1;
                    body.CollidesWith = Category.All; //Player can collide with Monster weapon too
                }
                
            }

            else if (e.HasComponent<MonsterComponent>())
            {
                if (e.HasComponent<WeaponComponent>())
                {
                    body.CollisionCategories = Category.Cat4;
                    body.CollidesWith = Category.Cat1;
                }
                else
                {
                    body.CollisionCategories = Category.Cat3;
                    body.CollidesWith = Category.Cat1 | Category.Cat2;
                }
            }
        }
    }
}
