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
                SpatialComponent spatial = e.GetComponent<SpatialComponent>();
                spatial.Position = ConvertUnits.ToDisplayUnits(bodyDictionary[e.Id].Position);

               if (e.HasComponent<MovementComponent>()) //Some could be static
               {
                    
                    MovementComponent movement = e.GetComponent<MovementComponent>();
                    bodyDictionary[e.Id].LinearVelocity = movement.Velocity;

                }                
             
                

            }
        }

        public override void OnEntityAdded(Entity e)
        {
            SpatialComponent spatial = e.GetComponent<SpatialComponent>();
            Body Body = new Body(physicsWorld);

            Body = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits
                                                                                  (spatial.Width), ConvertUnits.ToSimUnits(spatial.Height), 1f, 
                                                                                  ConvertUnits.ToSimUnits(spatial.Position));

            if (e.HasComponent<MovementComponent>())
            {
                Body.BodyType = BodyType.Dynamic;
                Body.Restitution = 0.3f;
            }

            Body.OnCollision += BodyOnCollision;

            CollisionCategory(e, Body);

            entityDictionary.Add(Body.BodyId, e);
            physics.AddEntityId(Body.BodyId, e.Id);
            bodyDictionary.Add(e.Id, Body);
        }

        private bool BodyOnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
           
            if (entityDictionary.ContainsKey(fixtureA.Body.BodyId) && entityDictionary.ContainsKey(fixtureB.Body.BodyId))
            {
                Entity A = entityDictionary[fixtureA.Body.BodyId];
                Entity B = entityDictionary[fixtureB.Body.BodyId];
                if (A.HasComponent<ProjectileComponent>() || B.HasComponent<ProjectileComponent>())
                {
                    Entity proj = A.HasComponent<ProjectileComponent>() ? A : B;
                    Entity hit = proj == A ? B : A;
                    Fixture hitBody = proj == A ? fixtureB : fixtureA;

                    if (hit.HasComponent<HealthComponent>() && hit != proj.GetComponent<ProjectileComponent>().owner)//valid hit, do dmg
                    {
                        hit.GetComponent<HealthComponent>().CurrentHealth -= proj.GetComponent<ProjectileComponent>().damage;
                        World.RemoveEntity(proj);
                        hitBody.Body.ApplyLinearImpulse(proj.GetComponent<ProjectileComponent>().direction * 10);
                    }
                }
                else if (A.HasComponent<PlayerComponent>() || B.HasComponent<PlayerComponent>())
                {
                    Entity player = A.HasComponent<PlayerComponent>() ? A : B;
                    Entity hit = player == A ? B : A;
                    Fixture hitBody = player == A ? fixtureB : fixtureA;
                    Fixture playerBody = hitBody == fixtureA ? fixtureB : fixtureA;

                    if (player.HasComponent<WeaponComponent>())
                    {

                    }
                    else if (hit.HasComponent<MonsterComponent>() )//&& !hit.HasComponent<WeaponComponent>())
                    {
                        //do damage to player and monster
                        hit.GetComponent<HealthComponent>().CurrentHealth -= 25;
                        player.GetComponent<HealthComponent>().CurrentHealth -= 25;
                        //World.RemoveEntity(entityDictionary[fixtureB.Body.BodyId]);
                        Vector2 a = hit.GetComponent<SpatialComponent>().Position - player.GetComponent<SpatialComponent>().Position;
                        a.Normalize();
                        Vector2 b = player.GetComponent<SpatialComponent>().Position - hit.GetComponent<SpatialComponent>().Position;
                        b.Normalize();
                        playerBody.Body.ApplyLinearImpulse(a * 5);
                        hitBody.Body.ApplyLinearImpulse(b * 5);
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
            if (e.HasComponent<PlayerComponent>())
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
