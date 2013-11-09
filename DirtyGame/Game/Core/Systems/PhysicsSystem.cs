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
        private Dictionary<uint, Body> bodyDictionary;
        private Dictionary<int, Entity> entityDictionary;
        public FarseerPhysics.Dynamics.World physicsWorld;

        public PhysicsSystem(FarseerPhysics.Dynamics.World  physicsWorld) 
            : base(SystemDescriptions.PhysicsSystem.Aspect, SystemDescriptions.PhysicsSystem.Priority)
        {
            this.physicsWorld = physicsWorld;
            bodyDictionary = new Dictionary<uint,Body>();
            entityDictionary = new Dictionary<int, Entity>();
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
        }


        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {

            foreach (Entity e in entities)
            {
                Spatial spatial = e.GetComponent<Spatial>();
                spatial.Position = ConvertUnits.ToDisplayUnits(bodyDictionary[e.Id].Position);
                
                if(e.HasComponent<MovementComponent>()) //Some could be static
                {
                    
                    MovementComponent movement = e.GetComponent<MovementComponent>();
                    bodyDictionary[e.Id].LinearVelocity = movement.Velocity;
                
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
                Body.OnCollision += Body_OnCollision;
            }

            CollisionCategory(e, Body);

            entityDictionary.Add(Body.BodyId, e);
            bodyDictionary.Add(e.Id, Body);
        }

        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (entityDictionary.ContainsKey(fixtureA.Body.BodyId) && entityDictionary.ContainsKey(fixtureB.Body.BodyId))
            {
                if (entityDictionary[fixtureA.Body.BodyId].HasComponent<Player>())
                {
                    if (entityDictionary[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }

                    else if (entityDictionary[fixtureB.Body.BodyId].HasComponent<MonsterComponent>() && !entityDictionary[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {
                        World.RemoveEntity(entityDictionary[fixtureB.Body.BodyId]);
                        entityDictionary.Remove(fixtureB.Body.BodyId);
                        fixtureB.Body.Dispose();
                    }

                    else if (entityDictionary[fixtureB.Body.BodyId].HasComponent<MonsterComponent>() && entityDictionary[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }
                }

                else if (entityDictionary[fixtureB.Body.BodyId].HasComponent<Player>())
                {
                    if (entityDictionary[fixtureB.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }

                    else if (entityDictionary[fixtureA.Body.BodyId].HasComponent<MonsterComponent>() && !entityDictionary[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {
                        World.RemoveEntity(entityDictionary[fixtureA.Body.BodyId]);
                        entityDictionary.Remove(fixtureA.Body.BodyId);
                        fixtureA.Body.Dispose();
                    }

                    else if (entityDictionary[fixtureA.Body.BodyId].HasComponent<MonsterComponent>() && entityDictionary[fixtureA.Body.BodyId].HasComponent<WeaponComponent>())
                    {

                    }
                }
            }
            return true;
        }

        public override void OnEntityRemoved(Entity e)
        {
            if(bodyDictionary.ContainsKey(e.Id)) 
            {
                bodyDictionary.Remove(e.Id);
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
