using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Util;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Core.Systems.Monster
{
    public class AISystem : EntitySystem
    {
        Random r = new Random();
        private Dirty game;
        private EntityFactory entityFactory;
        public float totaltime;
        //Current goal: Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.

        public AISystem(Dirty game, EntityFactory entityFactory)
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            this.game = game;
            this.entityFactory = entityFactory;
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            Vector2 playerPos = game.player.GetComponent<SpatialComponent>().Center;
            totaltime += (float)Math.Floor(dt * 1000);

            foreach (Entity e in entities)
            {
                if (e.HasComponent<InventoryComponent>())//has weapon
                {
                    Entity weapon = e.GetComponent<InventoryComponent>().CurrentWeapon;
                    WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

                    Vector2 monsterPos = e.GetComponent<SpatialComponent>().Center;

                    if(wc.WeaponName == "SnipWeapon")
                    {
                        if (e.GetComponent<SnipComponent>().Locked == true)
                        {
                            e.GetComponent<SnipComponent>().Locked = false;
                            
                            game.weaponSystem.FireWeapon(weapon, e, playerPos);
                        }
                    }

                    else if (wc.Type != WeaponComponent.WeaponType.AOE)
                    {
                        double dist = getDistance(monsterPos.X, monsterPos.Y, playerPos.X, playerPos.Y);
                        if (wc.WeaponName == "BomberWeapon")
                        {
                            if (dist < wc.Range)
                            {
                                game.weaponSystem.FireWeapon(weapon, e, playerPos);
                            }
                        }
                        else
                        {
                            game.weaponSystem.FireWeapon(weapon, e, playerPos);
                        }

                    }
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


        //Sniper Movement
        public Vector2 snipMovement(Entity e, float dt)
        {
            SpatialComponent spatial = e.GetComponent<SpatialComponent>();
            SnipComponent snip = e.GetComponent<SnipComponent>();
            Boolean playerFound = false;
            double[] vel = new double[2];
            double dist;

            Vector2 Target = new Vector2(game.player.GetComponent<SpatialComponent>().Center.X, game.player.GetComponent<SpatialComponent>().Center.Y);
            if((dist = getDistance(Target.X, Target.Y, spatial.Center.X, spatial.Center.Y)) <= snip.Range) //Player Within Range
            {
                    playerFound = true;
                    
                    Vector2 dir = (Target - spatial.Center + snip.Offset);
                    dir.Normalize();

                    if (snip.LaserPres == true)
                    {
                        game.world.EntityMgr.GetEntity(snip.Laser).GetComponent<SpriteComponent>().Scale = (float)dist / (float)snip.Range;

                        if (game.world.EntityMgr.GetEntity(snip.Laser).GetComponent<LaserComponent>().PlayerPres == true)
                        {
                            game.world.EntityMgr.GetEntity(snip.Laser).GetComponent<SpatialComponent>().ConstantRotation = laserFollow(e, game.world.EntityMgr.GetEntity(snip.Laser), dt, Target - spatial.Center);
                        }

                        
                    }

                    if (snip.LaserPres == false && snip.IsRunning == false)
                    {
                        snip.LaserPres = true;
                        Entity laser = game.entityFactory.CreateLaserEntity("laser", "sniplaser", spatial.Center, dir, (float)dist / (float)snip.Range);
                        laser.Refresh();
                        snip.Laser = laser.Id;
                      
                        
                    }


                    else if (dist <= snip.FleeDistance) //Snip Run
                    {
                        snip.IsRunning = true;
                        if (snip.LaserPres == true)
                        {
                            game.world.DestroyEntity(game.world.EntityMgr.GetEntity(snip.Laser));
                            snip.LaserPres = false;
                        }

                        vel = getChaseVector(-dir.X, dir.Y, spatial.Center.X, spatial.Center.Y);



                    }

                    else if (dist > snip.FleeDistance) //Snip Camp
                    {
                        snip.IsRunning = false;
                        vel[0] = 0;
                        vel[1] = 0;
                    }

                }

            

            if (playerFound == false && snip.LaserPres == true)
            {
                game.world.DestroyEntity(game.world.EntityMgr.GetEntity(snip.Laser));
                snip.LaserPres = false;
            }



            setDirection(vel, e);
            return new Vector2((float)vel[0], (float)vel[1]);
        }



        //Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.
        public Vector2 calculateMoveVector(IEnumerable<Entity> entities, Entity m, float dt)
        {

            double[] vel = new double[2];
            String type = m.GetComponent<PropertyComponent<String>>("MonsterType").value;

            if (type == "Flametower")
            {
                //don't move
            }

            else if (type == "SnipMonster")
            {
                return snipMovement(m, dt);
            }

            else if (m.HasComponent<InventoryComponent>())//has weapon
            {
                Entity weapon = m.GetComponent<InventoryComponent>().CurrentWeapon;
                WeaponComponent wc = weapon.GetComponent<WeaponComponent>();

                if (wc.Type == WeaponComponent.WeaponType.Ranged)
                {
                    vel = seekPlayer(entities, m, 0, 200, false);//if player is close, run
                    if (vel[0] == vel[1] && vel[0] == 0)
                        seekPlayer(entities, m, (int)wc.Range - 50, 600, true);//if player is not within weapon range but in sight range, chase
                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                        vel = randDir();
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {
                    vel = seekPlayer(entities, m, (int)wc.Range, 600, true);//if player is not within weapon range but in sight range, chase
                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                        vel = randDir();
                }
            }
            else//old ai
            {
                vel = seekPlayer(entities, m, 0, 600, true);

                if (vel[0] == vel[1] && vel[0] == 0)
                    vel = randDir();

            }

            setDirection(vel, m);

            return new Vector2((float)vel[0], (float)vel[1]) * 5 * (m.GetComponent<StatsComponent>().MoveSpeed / 100.0f);
        }
        private double[] seekPlayer(IEnumerable<Entity> entities, Entity m, int minrange, int maxrange, bool seek)
        {
            foreach (Entity e in entities)
            {
                if (e.HasComponent<PlayerComponent>())
                {

                    int otherX = (int)e.GetComponent<SpatialComponent>().Position.X;
                    int otherY = (int)e.GetComponent<SpatialComponent>().Position.Y;
                    if (getDistance(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY) < maxrange &&
                        getDistance(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY) > minrange)
                    {
                        double[] chaseVector;

                        if (seek)
                            chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY);
                        else
                            chaseVector = getChaseVector(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);

                        return chaseVector;
                    }
                }
            }

            return new double[2];
        }
        private void setDirection(double[] vel, Entity m)
        {
            DirectionComponent direction = m.GetComponent<DirectionComponent>();


            if (vel[0] == 0 && vel[1] == 0)
            {
                direction.Heading = "Down";
                m.GetComponent<MovementComponent>().Vertical = 0;
                m.GetComponent<MovementComponent>().Horizontal = 0;
                AnimationComponent animation = new AnimationComponent();
                animation.CurrentAnimation = "Idle" + direction.Heading;
                m.AddComponent(animation);
                m.Refresh();
            }

            else if (Math.Abs(vel[0]) > Math.Abs(vel[1]))
            {
                if (vel[0] > 0)
                {
                    direction.Heading = "Right";
                    m.GetComponent<MovementComponent>().Horizontal = 1;
                    AnimationComponent animation = new AnimationComponent();
                    animation.CurrentAnimation = "Walk" + direction.Heading;
                    m.AddComponent(animation);
                    m.Refresh();
                }
                else if (vel[0] < 0)
                {
                    direction.Heading = "Left";
                    m.GetComponent<MovementComponent>().Horizontal = -1;
                    AnimationComponent animation = new AnimationComponent();
                    animation.CurrentAnimation = "Walk" + direction.Heading;
                    m.AddComponent(animation);
                    m.Refresh();
                }
            }
            else
            {
                if (vel[1] > 0)
                {
                    direction.Heading = "Down";
                    m.GetComponent<MovementComponent>().Vertical = 1;
                    AnimationComponent animation = new AnimationComponent();
                    animation.CurrentAnimation = "Walk" + direction.Heading;
                    m.AddComponent(animation);
                    m.Refresh();
                }
                else if (vel[1] < 0)
                {
                    direction.Heading = "Up";
                    m.GetComponent<MovementComponent>().Vertical = -1;
                    AnimationComponent animation = new AnimationComponent();
                    animation.CurrentAnimation = "Walk" + direction.Heading;
                    m.AddComponent(animation);
                    m.Refresh();
                }
            }
        }

        

        private float laserFollow(Entity Enemy, Entity e, float dt, Vector2 target)
        {
            float rotation = 2f;
            float angle = e.GetComponent<SpriteComponent>().Angle;
            float tarangle = (float)Math.Atan2(-target.X, target.Y);

            if (e.GetComponent<TimeComponent>().timeofLock == 0)
            {
                e.GetComponent<TimeComponent>().timeofLock = totaltime;
            }


            if (totaltime - e.GetComponent<TimeComponent>().timeofLock > 4000)
            {

                e.GetComponent<SpatialComponent>().ConstantRotation = e.GetComponent<SpatialComponent>().DefaultRotationCons;
                e.GetComponent<LaserComponent>().PlayerPres = false;
                e.GetComponent<LaserComponent>().Reset = true;
            }

            if (e.GetComponent<LaserComponent>().LockedOn == true)
            {
                //fire

                Enemy.GetComponent<SnipComponent>().Locked = true;
                e.GetComponent<LaserComponent>().LockedOn = false;
                e.GetComponent<SpatialComponent>().ConstantRotation = e.GetComponent<SpatialComponent>().DefaultRotationCons;
                e.GetComponent<LaserComponent>().PlayerPres = false;
                e.GetComponent<LaserComponent>().Reset = true;
            }

            


            return rotation;
        }

        private double[] randDir()
        {
            int randInt;
            randInt = r.Next(0, 101);
            double[] randDir = new double[2];
            if (randInt < 26)
            {
                randDir[0] = 1.0;
                randDir[1] = 0.0;
            }
            else if (randInt < 51)
            {
                randDir[0] = -1.0;
                randDir[1] = 0.0;
            }
            else if (randInt < 76)
            {
                randDir[0] = 0.0;
                randDir[1] = 1.0;
            }
            else
            {
                randDir[0] = 0.0;
                randDir[1] = -1.0;
            }

            return randDir;
        }


        private double getDistance(double x, double y, double ox, double oy)
        {
            return Math.Sqrt(
                (Math.Pow(ox - x, 2.0))
                + (Math.Pow(oy - y, 2.0))
                );
        }

        private double[] getChaseVector(double x, double y, double ox, double oy)
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
