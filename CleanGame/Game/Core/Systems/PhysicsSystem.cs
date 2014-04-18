using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using EntityFramework.Systems;
using FarseerPhysics.Factories;
using CleanGame.Game.Core.Components.Movement;
using EntityFramework;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.Core.Components;
using Microsoft.Xna.Framework;
using CleanGame.Game.SGraphics;
using CleanGame.Game.SGraphics.Commands;
using CleanGame.Game.SGraphics.Commands.DrawCalls;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using CleanGame.Game.Util;
using CleanGame.Game.Core.Components.Render;
using GameService;

namespace CleanGame.Game.Core.Systems
{
    class PhysicsSystem : EntitySystem
    {

    public Dictionary<int, Entity> entityDictionary;
    public Dictionary<uint, Body> bodyDictionary;
    private Physics physics;
    private FarseerPhysics.Dynamics.World physicsWorld;
    private Renderer renderer;
    private bool PhysicsDebug = false;
    uint playerId;
    private Dirty game;

	public PhysicsSystem(Physics physics, Renderer renderer, Dirty game)
				: base(SystemDescriptions.PhysicsSystem.Aspect, SystemDescriptions.PhysicsSystem.Priority)
			{
				this.physics = physics;
				this.game = game;
				physicsWorld = physics.World;
				entityDictionary = new Dictionary<int, Entity>();
				bodyDictionary = new Dictionary<uint, Body>();

				ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

				this.renderer = renderer;
			}

	public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
			{
				RenderGroup renderGroup = null;
				if (PhysicsDebug)
				{
					renderGroup = new RenderGroup();
					renderGroup.AddCommand(new BeginBatchDraw(renderer.ActiveCamera.Transform));
				}
				foreach (Entity e in entities)
				{
					if (e.HasComponent<SpatialComponent>())
					{
						SpatialComponent spatial = e.GetComponent<SpatialComponent>();
						PhysicsComponent pc = e.GetComponent<PhysicsComponent>();
						//body position is (.5f, .5f), while spatial position is (0, 0)

                        if (pc.movePlayer)
                        {
                            //move the player to the new spatial location
                            // bodyDictionary[playerId].Position = spatial.Position;
                            bodyDictionary[playerId].Position = ConvertUnits.ToSimUnits(spatial.Position - spatial.Size * pc.Origin);
                            pc.movePlayer = false;
                        }

						spatial.Position = ConvertUnits.ToDisplayUnits(bodyDictionary[e.Id].Position) - spatial.Size * pc.Origin;

                        if (spatial.ShouldRotate == false)
                        {
                            bodyDictionary[e.Id].Rotation = spatial.Rotation;
                        }

						if (spatial.ConstantRotation != 0 && !e.HasComponent<LaserComponent>())
						{
							spatial.Rotation += spatial.ConstantRotation * dt;
							bodyDictionary[e.Id].Rotation = spatial.Rotation;
						}

                        if (e.HasComponent<PropertyComponent<String>>())
                        {

                            if (e.GetComponent<PropertyComponent<String>>("MonsterType").value == "Flametower" && bodyDictionary[e.Id].IsStatic == false)
                            {
                                bodyDictionary[e.Id].IsStatic = true;
                            }

                        }

                        if (e.HasComponent<SnipComponent>())
                        {
                            SnipComponent snip = e.GetComponent<SnipComponent>();

                            if (snip.IsRunning == false)
                            {
                                bodyDictionary[e.Id].IsStatic = true;
                            }

                            if (snip.IsRunning == true)
                            {
                                bodyDictionary[e.Id].IsStatic = false;

                            }

                        }

                        if (e.HasComponent<LaserComponent>())
                        {

                            LaserComponent laser = e.GetComponent<LaserComponent>();

                            if (laser.LockedOn == true)
                            {
                                bodyDictionary[e.Id].Rotation = e.GetComponent<SpriteComponent>().Angle;

                            }

                            else
                            {
                                bodyDictionary[e.Id].Rotation += spatial.ConstantRotation * dt;
                                spatial.Rotation += spatial.ConstantRotation * dt;
                            }

                            if (laser.Reset == true)
                            {


                                laser.Reset = false;
                                spatial.Rotation = laser.Offset;
                                bodyDictionary[e.Id].Rotation = e.GetComponent<SpriteComponent>().Angle + laser.Offset;

                            }

                        }


						if (PhysicsDebug)
						{
							//spatial box
							RenderInstance instance = new RenderInstance();
							instance.DrawCall = new BatchDrawLine(spatial.Position, new Vector2(spatial.Position.X + spatial.Width, spatial.Position.Y), Color.Blue);
							renderGroup.AddInstance(instance);
							instance = new RenderInstance();
							instance.DrawCall = new BatchDrawLine(new Vector2(spatial.Position.X + spatial.Width, spatial.Position.Y), new Vector2(spatial.Position.X + spatial.Width, spatial.Position.Y + spatial.Height), Color.Blue);
							renderGroup.AddInstance(instance);
							instance = new RenderInstance();
							instance.DrawCall = new BatchDrawLine(new Vector2(spatial.Position.X, spatial.Position.Y + spatial.Height), new Vector2(spatial.Position.X + spatial.Width, spatial.Position.Y + spatial.Height), Color.Blue);
							renderGroup.AddInstance(instance);
							instance = new RenderInstance();
							instance.DrawCall = new BatchDrawLine(new Vector2(spatial.Position.X, spatial.Position.Y + spatial.Height), spatial.Position, Color.Blue);
							renderGroup.AddInstance(instance);

							//physics box
							foreach (Fixture f in bodyDictionary[e.Id].FixtureList)
							{
								Transform t;
								f.Body.GetTransform(out t);
								AABB aabb;
								f.Shape.ComputeAABB(out aabb, ref t, 0);

								if (aabb.Vertices.Count > 1)
								{
									instance = new RenderInstance();
									instance.DrawCall = new BatchDrawLine(ConvertUnits.ToDisplayUnits(aabb.Vertices[0]), ConvertUnits.ToDisplayUnits(aabb.Vertices[aabb.Vertices.Count - 1]), Color.Red);
									//instance.SortKey.SetRenderLayer(sprite.RenderLayer);

									renderGroup.AddInstance(instance);
								}
								for (int i = 1; i < aabb.Vertices.Count; i++)
								{
									instance = new RenderInstance();
									instance.DrawCall = new BatchDrawLine(ConvertUnits.ToDisplayUnits(aabb.Vertices[i - 1]), ConvertUnits.ToDisplayUnits(aabb.Vertices[i]), Color.Red);
									//instance.SortKey.SetRenderLayer(sprite.RenderLayer);

									renderGroup.AddInstance(instance);
								}
							}
						}

					}

					if (e.HasComponent<MovementComponent>()) //Some could be static
					{

						MovementComponent movement = e.GetComponent<MovementComponent>();
              
                        if (bodyDictionary[e.Id].IsStatic == false)
                        {
                            bodyDictionary[e.Id].LinearVelocity = movement.Velocity;
                            if (e.HasComponent<MeleeComponent>())
                            {
                                Entity owner = e.GetComponent<MeleeComponent>().Owner;

                                if (bodyDictionary.ContainsKey(owner.Id))
                                {

                                    bodyDictionary[e.Id].Position = bodyDictionary[owner.Id].Position;
                                }
                                

                            }
                        }

					}



				}

				if (PhysicsDebug)
					renderer.Submit(renderGroup);
			}

	public override void OnEntityAdded(Entity e)
			{
                
				Body Body = new Body(physicsWorld);

				if (e.HasComponent<PlayerComponent>())
				{
					playerId = e.Id;
				}

				if (e.HasComponent<SpatialComponent>())
				{
					SpatialComponent spatial = e.GetComponent<SpatialComponent>();
					PhysicsComponent p = e.GetComponent<PhysicsComponent>();

					//body position is (.5f, .5f), while spatial position is (0, 0)

					//if (p.Origin == new Vector2(0, 1))
					//{
						Vector2 oMod = p.Origin - new Vector2(.5f, .5f);
						Vector2 size = new Vector2(ConvertUnits.ToSimUnits(spatial.Width), ConvertUnits.ToSimUnits(spatial.Height));
						Body = BodyFactory.CreateBody(physicsWorld);
						Vector2[] v = new Vector2[4];
						v[0] = new Vector2(0, 0) - size * p.Origin;//top left
						v[1] = new Vector2(0, size.Y) - size * p.Origin;//bottom left
						v[2] = new Vector2(size.X, size.Y) - size * p.Origin;//bottom right
						v[3] = new Vector2(size.X, 0) - size * p.Origin;//top right
						PolygonShape ps = new PolygonShape(new Vertices(v.ToArray()), 1f);

						Body.CreateFixture(ps);
						Body.Position = ConvertUnits.ToSimUnits(spatial.Position - spatial.Size * p.Origin);
						
						Body.Rotation = spatial.Rotation;
					//}
					//else
					//{
					//    //Body.CreateFixture(Shape.
					//    Body = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(spatial.Width), ConvertUnits.ToSimUnits(spatial.Height), 1f, ConvertUnits.ToSimUnits(spatial.Position + spatial.Size/2));

					//    Body.Rotation = spatial.Rotation;
					//}
				}

				if (e.HasComponent<LaserComponent>())
				{

					Body.Rotation = e.GetComponent<SpriteComponent>().Angle;
					
				}

                if (e.HasComponent<BorderComponent>())
                {
                    BorderComponent border = e.GetComponent<BorderComponent>();
                    Vertices borders = new Vertices(4);
                    borders.Add(ConvertUnits.ToSimUnits(border.TopLeft));
                    borders.Add(ConvertUnits.ToSimUnits(border.TopRight));
                    borders.Add(ConvertUnits.ToSimUnits(border.BottomRight));
                    borders.Add(ConvertUnits.ToSimUnits(border.BottomLeft));

                    Body = BodyFactory.CreateLoopShape(physicsWorld, borders);
                    Body.CollisionCategories = Category.All;
                    Body.CollidesWith = Category.All;
                }

				if (e.HasComponent<MovementComponent>())
				{
					Body.BodyType = BodyType.Dynamic;
					Body.Restitution = 0.3f;
				}

                

				Body.OnCollision += BodyOnCollision;

                if (e.HasComponent<PropertyComponent<String>>())
                {
                    if (e.GetComponent<PropertyComponent<String>>("MonsterType").value == "MeleeMonster")
                    {
                        Body.OnSeparation += BodyOnSeparation;
                    }
                }
				CollisionCategory(e, Body);

				entityDictionary.Add(Body.BodyId, e);
				physics.AddEntityId(Body.BodyId, e.Id);
				bodyDictionary.Add(e.Id, Body);

                
			}

    void BodyOnSeparation(Fixture fixtureA, Fixture fixtureB)
    {

        if (entityDictionary.ContainsKey(fixtureA.Body.BodyId) && entityDictionary.ContainsKey(fixtureB.Body.BodyId))
        {

            Entity A = entityDictionary[fixtureA.Body.BodyId];
            Entity B = entityDictionary[fixtureB.Body.BodyId];

            if (A.HasComponent<PlayerComponent>() || B.HasComponent<PlayerComponent>())
            {
                Entity player = A.HasComponent<PlayerComponent>() ? A : B;
                Entity hit = player == A ? B : A;

                hit.GetComponent<PhysicsComponent>().collidingWithPlayer = false;
            }
        }
    }
			
	private bool BodyOnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
			{
				bool Collide = true;

				if (entityDictionary.ContainsKey(fixtureA.Body.BodyId) && entityDictionary.ContainsKey(fixtureB.Body.BodyId))
				{
					Entity A = entityDictionary[fixtureA.Body.BodyId];
					Entity B = entityDictionary[fixtureB.Body.BodyId];

					if (A.HasComponent<GrenadeComponent>() || B.HasComponent<GrenadeComponent>())
					{
						Collide = false;
					}


					if (A.HasComponent<ProjectileComponent>() || B.HasComponent<ProjectileComponent>())//Projectiles
					{
						Entity proj = A.HasComponent<ProjectileComponent>() ? A : B;
						Entity hit = proj == A ? B : A;
						Fixture hitBody = proj == A ? fixtureB : fixtureA;

						ProjectileComponent pc = proj.GetComponent<ProjectileComponent>();

						if (hit != pc.owner)
						{
							if (hit.HasComponent<StatsComponent>())//valid hit, do dmg
							{
								if (pc.owner.HasComponent<PlayerComponent>())
									GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerWeaponFirstHit, pc.weapon.GetComponent<WeaponComponent>().WeaponName);

								game.weaponSystem.DealDamage(proj.GetComponent<ProjectileComponent>().weapon, hit);
								//hit.GetComponent<HealthComponent>().CurrentHealth -= proj.GetComponent<ProjectileComponent>().damage;
								hitBody.Body.ApplyLinearImpulse(proj.GetComponent<ProjectileComponent>().direction * 1f);
								World.DestroyEntity(proj);
							}
							else if (hit.HasComponent<BorderComponent>())//hit map bounds, remove
							{
								World.DestroyEntity(proj);
							}
						}
					}
					else if (A.HasComponent<MeleeComponent>() || B.HasComponent<MeleeComponent>())//Melee
					{

                        Collide = false;

						Entity melee = A.HasComponent<MeleeComponent>() ? A : B;
						Entity hit = melee == A ? B : A;
                        Fixture hitBody = melee == A ? fixtureB : fixtureA;

						MeleeComponent mc = melee.GetComponent<MeleeComponent>();
						if (mc.Owner != hit)//don't hit owner (later this needs to be don't hit team to turn off friendly fire)
						{

							if (hit.HasComponent<StatsComponent>())//valid hit, do dmg
							{
								if (!mc.targetsHit.Contains(hit))//have not already hit target
								{
									if (mc.Owner.HasComponent<PlayerComponent>() && mc.targetsHit.Count == 0)
										GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerWeaponFirstHit, mc.Weapon.GetComponent<WeaponComponent>().WeaponName);

									mc.targetsHit.Add(hit);

									game.weaponSystem.DealDamage(mc.Weapon, hit);
                                    double[] dir = new double[2];
                                    dir = getDirection(melee.GetComponent<SpatialComponent>().Center.X, melee.GetComponent<SpatialComponent>().Center.Y,
                                                                          hit.GetComponent<SpatialComponent>().Center.X, hit.GetComponent<SpatialComponent>().Center.Y);
                                    Vector2 direction = new Vector2((float)dir[0], (float)dir[1]);
                                    hitBody.Body.ApplyLinearImpulse(direction * 11f);
                                    
								}
							}
						}

					}
					else if (A.HasComponent<AOEComponent>() || B.HasComponent<AOEComponent>())//aoe
					{
						Entity aoe = A.HasComponent<AOEComponent>() ? A : B;
						Entity hit = aoe == A ? B : A;

						AOEComponent ac = aoe.GetComponent<AOEComponent>();

						if (ac.Owner.entity != null && ac.Owner.entity.HasComponent<PlayerComponent>() && ac.HitList.Count == 0)
							GameplayDataCaptureSystem.Instance.LogEvent(CaptureEventType.PlayerWeaponFirstHit, ac.Weapon.entity.GetComponent<WeaponComponent>().WeaponName);

						if (ac.Owner.entity != hit && hit.HasComponent<PlayerComponent>())//player hit
						{
							if (!ac.HitList.Contains(hit))
							{
								ac.HitList.Add(hit);

								game.weaponSystem.DealDamage(ac.Weapon.entity, hit);
							}
						}
                    else if (ac.Weapon.entity.GetComponent<WeaponComponent>().WeaponName == "BomberWeapon")
                    {
                        if (!ac.HitList.Contains(hit))
                        {
                            ac.HitList.Add(hit);

                            game.weaponSystem.DealDamage(ac.Weapon.entity, hit);
                        }
                    }

                    Collide = false;
					}

                    else if (A.HasComponent<LaserComponent>() || B.HasComponent<LaserComponent>())
                    {
                        Entity laser = A.HasComponent<LaserComponent>() ? A : B;
                        Entity player = laser == A ? B : A;

                        if (player.HasComponent<PlayerComponent>())
                        {

                            LaserComponent laserComp = laser.GetComponent<LaserComponent>();

                            laserComp.LockedOn = true;
                            laserComp.PlayerPres = true;

                        }

                        Collide = false;
                    }

                else if (A.HasComponent<PlayerComponent>() || B.HasComponent<PlayerComponent>())
                {
                    Entity player = A.HasComponent<PlayerComponent>() ? A : B;
                    Entity hit = player == A ? B : A;
                    Fixture hitBody = player == A ? fixtureB : fixtureA;
                    Fixture playerBody = hitBody == fixtureA ? fixtureB : fixtureA;

                    if (hit.HasComponent<PropertyComponent<String>>())
                    {
                        if (hit.GetComponent<PropertyComponent<String>>("MonsterType").value == "MeleeMonster")
                        {
                            if (!hit.GetComponent<PhysicsComponent>().collidingWithPlayer)
                            {
                                int Damage = hit.GetComponent<StatsComponent>().BaseDamage;
                                hit.GetComponent<PhysicsComponent>().collidingWithPlayer = true;

                                game.weaponSystem.DealDamage(Damage, player);

                                double[] dir = new double[2];
                                dir = getDirection(hit.GetComponent<SpatialComponent>().Center.X, hit.GetComponent<SpatialComponent>().Center.Y,
                                                                      player.GetComponent<SpatialComponent>().Center.X, player.GetComponent<SpatialComponent>().Center.Y);
                                Vector2 direction = new Vector2((float)dir[0], (float)dir[1]);
                                playerBody.Body.ApplyLinearImpulse(direction * 10f);
                            }
                        }
                    }
                    if (player.HasComponent<WeaponComponent>())
                    {

                    }
                    else if (hit.HasComponent<MonsterComponent>())//&& !hit.HasComponent<WeaponComponent>())
                    {
                        //do damage to player and monster
                        //hit.GetComponent<HealthComponent>().CurrentHealth -= 25;
                        //player.GetComponent<HealthComponent>().CurrentHealth -= 25;
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
            return Collide;
        }

        public override void OnEntityRemoved(Entity e)
        {
            if (bodyDictionary.ContainsKey(e.Id))
            {
                if (entityDictionary.ContainsKey(bodyDictionary[e.Id].BodyId))
                {
                    physicsWorld.RemoveBody(bodyDictionary[e.Id]);
                    entityDictionary.Remove(bodyDictionary[e.Id].BodyId);
                    physics.RemoveEntityId(bodyDictionary[e.Id].BodyId);
                }
                bodyDictionary.Remove(e.Id);
            }

        }


        //Cat 1 = Player, Cat 2= Player Weapon, Cat 3 = Monster, Cat4 = Monster Weapon, Cat5 = AOE
        private void CollisionCategory(Entity e, Body body)
        {
            if (e.HasComponent<PlayerComponent>())//is player
            {
                body.CollisionCategories = Category.Cat1;
                body.CollidesWith = Category.Cat3 | Category.Cat4 | Category.Cat5; //Player collides with monsters and monster weapons
            }
            else if (e.HasComponent<MonsterComponent>())//is monster
            {
                body.CollisionCategories = Category.Cat3;
                body.CollidesWith = Category.Cat1 | Category.Cat2 | Category.Cat5;//Monster collides with player and player weapons
            }
            else if (e.HasComponent<LaserComponent>())//is monster
            {
                body.CollisionCategories = Category.Cat3;
                body.CollidesWith = Category.Cat1; //Player
            }
            else if (e.HasComponent<AOEComponent>())//AOE Damage
            {
                if (e.GetComponent<AOEComponent>().Owner.entity.Id == playerId)//player aoe
                {

                }
                else//monster aoe
                {
                    body.CollisionCategories = Category.Cat5;
                    body.CollidesWith = Category.Cat1 | Category.Cat3; //Monster AOE only collides with player and monsters
                }
            }
            else if (e.HasComponent<ProjectileComponent>())//Projectiles
            {
                if (e.GetComponent<ProjectileComponent>().owner.Id == playerId)//player projectile
                {
                    body.CollisionCategories = Category.Cat2;
                    body.CollidesWith = Category.Cat3;
                }
                else
                {
                    body.CollisionCategories = Category.Cat4;
                    body.CollidesWith = Category.Cat1;
                }
            }


            else if (e.HasComponent<MeleeComponent>())
            {
                if (e.GetComponent<MeleeComponent>().Owner.Id == playerId)//player melee
                {
                    body.CollisionCategories = Category.Cat2;
                    body.CollidesWith = Category.Cat3; //Weapon only collides with Monster (Can Change to collide with Monster Weapon too)
                }
                else//monster melee
                {
                    body.CollisionCategories = Category.Cat4;
                    body.CollidesWith = Category.Cat1;
                }
            }
        }

        private double[] getDirection(double x, double y, double ox, double oy)
        {
            double[] vect = new double[2];
            double dx = ox - x;
            double dy = oy - y;
            double angle = Math.Atan2(dy, dx); // This is opposite y angle.
            vect[0] = Math.Cos(angle);
            vect[1] = Math.Sin(angle);
            return vect;
        }


    }
}
