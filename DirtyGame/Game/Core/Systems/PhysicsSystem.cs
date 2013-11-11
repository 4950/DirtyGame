using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using EntityFramework.Systems;
using FarseerPhysics.Factories;
using DirtyGame.game.Core.Components.Movement;
using EntityFramework;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using Microsoft.Xna.Framework;

namespace DirtyGame.game.Core.Systems
{
    class PhysicsSystem : EntitySystem 
    {
        public Dictionary<int, Entity> entityDictionary;
        public Dictionary<uint, Body> bodyDictionary;
        private Physics physics;
        private FarseerPhysics.Dynamics.World physicsWorld;

        public PhysicsSystem(Physics  physics) 
            : base(SystemDescriptions.PhysicsSystem.Aspect, SystemDescriptions.PhysicsSystem.Priority)
        {
            this.physics = physics;
            physicsWorld = physics.World;
            entityDictionary = new Dictionary<int, Entity>();
            bodyDictionary = new Dictionary<uint, Body>();
           
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

               if (e.HasComponent<AIMovementComponent>())
               {
                   AIMovementComponent aimovement = e.GetComponent<AIMovementComponent>();
                   bodyDictionary[e.Id].LinearVelocity = aimovement.Velocity;
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

            if (e.HasComponent<MovementComponent>() || e.HasComponent<AIMovementComponent>())
            {
                Body.BodyType = BodyType.Dynamic;
                Body.Restitution = 0.3f;
            }

            if (e.HasComponent<Collidable>())
            {
                Body.OnCollision += BodyOnCollision;
            }

            CollisionCategory(e, Body);

            entityDictionary.Add(Body.BodyId, e);
            physics.AddEntityId(Body.BodyId, e.Id);
            bodyDictionary.Add(e.Id, Body);
        }

        private bool BodyOnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
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
                if(entityDictionary.ContainsKey(bodyDictionary[e.Id].BodyId))
                {
                    physicsWorld.RemoveBody(bodyDictionary[e.Id]);
                    entityDictionary.Remove(bodyDictionary[e.Id].BodyId);
                    physics.RemoveEntityId(bodyDictionary[e.Id].BodyId);
                }
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
