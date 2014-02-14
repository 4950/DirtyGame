using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Util;
using EntityFramework;
using EntityFramework.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Core.Systems.Monster
{
    public class AISystem : EntitySystem
    {
        Random r = new Random();
        private Dirty game;
        private EntityFactory entityFactory;
        public float totaltime;
        private Physics physics;

        //Current goal: Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.

        public AISystem(Dirty game, EntityFactory entityFactory, Physics physics)
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            this.game = game;
            this.entityFactory = entityFactory;
            this.physics = physics;
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

                    if (wc.WeaponName == "SnipWeapon")
                    {
                        SnipComponent snip = e.GetComponent<SnipComponent>();

                        if (snip.Locked == true)
                        {
                            snip.Locked = false;

                            game.weaponSystem.FireWeapon(weapon, e, playerPos);
                        }
                    }

                    else if (wc.WeaponName == "GrenadeLauncher")
                    {
                        double dist = getDistance(monsterPos.X, monsterPos.Y, playerPos.X, playerPos.Y);
                        if (dist < wc.Range)
                        {
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
            if (e.HasComponent<SnipComponent>())
            {
                SnipComponent snip = e.GetComponent<SnipComponent>();
                if (snip.LaserPres == true)
                {
                    game.world.DestroyEntity(game.world.EntityMgr.GetEntity(snip.Laser));
                }
            }

        }


        //Sniper Movement
        public Vector2 snipMovement(Entity e, float dt)
        {
            SpatialComponent spatial = e.GetComponent<SpatialComponent>();
            SnipComponent snip = e.GetComponent<SnipComponent>();
            Boolean playerFound = false;
            double[] vel = new double[2];
            double dist;

            Vector2 Target = game.player.GetComponent<SpatialComponent>().Center;
            if ((dist = getDistance(Target.X, Target.Y, spatial.Center.X, spatial.Center.Y)) <= snip.Range) //Player Within Range
            {
                playerFound = true;

                Vector2 dirNoOff = (Target - spatial.Center);
                dirNoOff.Normalize();
                Vector2 dir = (Target - spatial.Center + snip.Offset);
                dir.Normalize();

                Entity Laser = game.world.EntityMgr.GetEntity(snip.Laser);

                if (snip.LaserPres == true)
                {
                    Laser.GetComponent<SpriteComponent>().Scale = (float)dist / (float)snip.Range;

                    if (Laser.GetComponent<LaserComponent>().PlayerPres == true)
                    {
                        laserFollow(e, Laser, dirNoOff);
                    }


                }

                if (snip.LaserPres == false && snip.IsRunning == false)
                {
                    snip.LaserPres = true;
                    Entity laser = game.entityFactory.CreateLaserEntity("laser", "sniplaser", spatial.Center, dir, (float)dist / (float)snip.Range);
                    laser.Refresh();
                    snip.Laser = laser.Id;

                    SpatialComponent laserSpatial = laser.GetComponent<SpatialComponent>();

                    laser.GetComponent<LaserComponent>().Offset = snip.LaserOffset;
                    laserSpatial.DefaultRotationCons = snip.DefaultRotation;
                    laserSpatial.ConstantRotation = laserSpatial.DefaultRotationCons;

                }


                else if (dist <= snip.FleeDistance) //Snip Run
                {
                    snip.IsRunning = true;
                    if (snip.LaserPres == true)
                    {
                        game.world.DestroyEntity(Laser);
                        snip.LaserPres = false;
                    }

                    vel = getChaseVector(Target.X, Target.Y, spatial.Center.X, spatial.Center.Y);




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


            setIdleDirection(getChaseVector(spatial.Center.X, spatial.Center.Y, Target.X, Target.Y), vel, e);
            setDirection(vel, e);
            return new Vector2((float)vel[0], (float)vel[1]);
        }



        //Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.
        public Vector2 calculateMoveVector(IEnumerable<Entity> entities, Entity m, float dt)
        {
            MovementComponent mc = m.GetComponent<MovementComponent>();
            SpatialComponent s = m.GetComponent<SpatialComponent>();
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

            else if (type == "WallHugger")
            {
                return WallHuggerMovement(m) * 5 * (m.GetComponent<StatsComponent>().MoveSpeed / 100.0f);
            }

            else if (m.HasComponent<InventoryComponent>())//has weapon
            {
                Entity weapon = m.GetComponent<InventoryComponent>().CurrentWeapon;
                WeaponComponent wc = weapon.GetComponent<WeaponComponent>();
                if (wc.Type == WeaponComponent.WeaponType.Landmine)
                {
                    vel = seekPlayer(entities, m, 0, 200, false);//if player is close, run

                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                    {
                        float theta = mc.WanderTheta;
                        vel = Wander(s.Position, mc.Velocity, ref theta);
                        mc.WanderTheta = theta;
                    }
                }
                if (wc.Type == WeaponComponent.WeaponType.Ranged)
                {
                    vel = seekPlayer(entities, m, 0, 200, false);//if player is close, run
                    if (vel[0] == vel[1] && vel[0] == 0)
                        seekPlayer(entities, m, (int)wc.Range - 50, 600, true);//if player is not within weapon range but in sight range, chase
                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                    {
                        float theta = mc.WanderTheta;
                        vel = Wander(s.Position, mc.Velocity, ref theta);
                        mc.WanderTheta = theta;
                    }
                }
                else if (wc.Type == WeaponComponent.WeaponType.Melee)
                {
                    vel = seekPlayer(entities, m, (int)wc.Range, 600, true);//if player is not within weapon range but in sight range, chase
                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                    {
                        float theta = mc.WanderTheta;
                        vel = Wander(s.Position, mc.Velocity, ref theta);
                        mc.WanderTheta = theta;
                    }
                }
            }
            else//old ai
            {
                vel = seekPlayer(entities, m, 0, 600, true);

                if (vel[0] == vel[1] && vel[0] == 0)
                {
                    float theta = mc.WanderTheta;
                    vel = Wander(s.Position, mc.Velocity, ref theta);
                    mc.WanderTheta = theta;
                }

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
                        {
                            bool wall = false;
                            List<Entity> rayCast = physics.RayCast(new Vector2(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y), new Vector2(otherX, otherY));
                            foreach (Entity w in rayCast)
                            {
                                if (w.GetComponent<BorderComponent>() != null)
                                {
                                    wall = true;
                                    break;
                                }
                                else
                                {


                                }
                            }

                            chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY);
                            //if (m.GetComponent<MovementComponent>().prevHorizontal != 0)
                            //{
                            //    m.GetComponent<MovementComponent>().prevVelocity = new Vector2(0, 0);
                            //}
                            MovementComponent oldVector = m.GetComponent<MovementComponent>();

                            if (wall)
                            {
                                if (Math.Abs(chaseVector[0]) > Math.Abs(chaseVector[1]))
                                {
                                    if (chaseVector[1] > 0)
                                    {
                                        chaseVector = WalkAroundWallVertical(m, oldVector, "up");
                                    }
                                    else
                                    {
                                        chaseVector = WalkAroundWallVertical(m, oldVector, "down");
                                    }

                                }
                                else
                                {
                                    if (chaseVector[0] > 0)
                                    {
                                        chaseVector = WalkAroundWallHorizontal(m, oldVector, "left");

                                    }
                                    else
                                    {
                                        chaseVector = WalkAroundWallHorizontal(m, oldVector, "right");
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            chaseVector = getChaseVector(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);
                        }
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

        private void setIdleDirection(double[] dir, double[] velocity, Entity m)
        {
            DirectionComponent direction = m.GetComponent<DirectionComponent>();


            if (velocity[0] == 0 && velocity[1] == 0)
            {


                if (Math.Abs(dir[0]) > Math.Abs(dir[1]))
                {
                    if (dir[0] > 0)
                    {
                        direction.Heading = "Right";

                    }
                    else if (dir[0] < 0)
                    {
                        direction.Heading = "Left";

                    }
                }

                else
                {
                    if (dir[1] > 0)
                    {
                        direction.Heading = "Down";

                    }
                    else if (dir[1] < 0)
                    {
                        direction.Heading = "Up";

                    }
                }

            }
        }

        private void laserFollow(Entity Enemy, Entity e, Vector2 dir)
        {

            TimeComponent time = e.GetComponent<TimeComponent>();
            LaserComponent laser = e.GetComponent<LaserComponent>();
            SnipComponent snip = Enemy.GetComponent<SnipComponent>();
            SpatialComponent spatial = e.GetComponent<SpatialComponent>();


            float rotation = spatial.DefaultRotationCons;

            if (time.timeofLock == 0)
            {
                time.timeofLock = totaltime;
            }

            if (laser.LockedOn == true)
            {
                rotation = 0;
                spatial.Rotation = 0;
                e.GetComponent<SpriteComponent>().Angle = (float)Math.Atan2(dir.X, -dir.Y) + 3.14159265f; //180 degrees

                if (totaltime - time.timeofLock > (1000 * snip.TimeDelay))
                {
                    //fire
                    time.timeofLock = 0;
                    rotation = spatial.DefaultRotationCons;
                    laser.PlayerPres = false;
                    laser.Reset = true;
                    snip.Locked = true;
                    laser.LockedOn = false;
                }

            }

            spatial.ConstantRotation = rotation;
        }
        private double[] Wander(Vector2 pos, Vector2 curVel, ref float theta)
        {
            const float wanderR = 16.0f;
            const float wanderD = 60.0f;
            const float change = 0.5f;

            float negChange = r.Next(2);
            float randomNum = r.Next(1) * change;
            if (negChange == 2)
                theta = theta - randomNum;
            else
                theta = theta + randomNum;


            Vector2 circleLoc = curVel;

            circleLoc.Normalize();
            circleLoc *= wanderD;
            circleLoc += pos;

            Vector2 circleOffset = new Vector2((float)(wanderR * Math.Cos(theta)), (float)(wanderR * Math.Sin(theta)));
            Vector2 target = circleLoc;
            target += circleOffset;

            Vector2 dir = target - pos;
            dir.Normalize();
            double[] rr = new double[2];
            rr[0] = dir.X;
            rr[1] = dir.Y;
            return rr;
        }
        //private double[] randDir()
        //{
        //    int randInt;
        //    randInt = r.Next(0, 101);
        //    double[] randDir = new double[2];
        //    if (randInt < 26)
        //    {
        //        randDir[0] = 1.0;
        //        randDir[1] = 0.0;
        //    }
        //    else if (randInt < 51)
        //    {
        //        randDir[0] = -1.0;
        //        randDir[1] = 0.0;
        //    }
        //    else if (randInt < 76)
        //    {
        //        randDir[0] = 0.0;
        //        randDir[1] = 1.0;
        //    }
        //    else
        //    {
        //        randDir[0] = 0.0;
        //        randDir[1] = -1.0;
        //    }

        //    return randDir;
        //}


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

        private double[] WalkAroundWallVertical(Entity m, MovementComponent oldVector, string direction)
        {
            double[] chaseVector = new double[2];
            if (direction == "up") //Target is above us
            {
                if (oldVector.prevVertical <= 0) //We were moving down before
                {
                    //keep moving down
                    chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y - 32);
                    oldVector.prevHorizontal = (float)chaseVector[0];
                    oldVector.prevVertical = (float)chaseVector[1];
                }
                else
                {
                    // Use Previous Movement Vector
                    chaseVector[0] = oldVector.prevHorizontal;
                    chaseVector[1] = oldVector.prevVertical;
                }
            }
            if (direction == "down") //Tearget is below us
            {
                if (oldVector.prevVertical >= 0) //We were moving up before
                {
                    //Keep moving up
                    chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y + 32);
                    oldVector.prevHorizontal = (float)chaseVector[0];
                    oldVector.prevVertical = (float)chaseVector[1];
                }
                else
                {
                    // Use Previous Movement Vector
                    chaseVector[0] = oldVector.prevHorizontal;
                    chaseVector[1] = oldVector.prevVertical;
                }
            }

            return chaseVector;
        }

        private double[] WalkAroundWallHorizontal(Entity m, MovementComponent oldVector, string direction)
        {
            double[] chaseVector = new double[2];
            if (direction == "left") //Target is to our left
            {
                if (oldVector.prevHorizontal >= 0) //We were moving right before
                {
                    //Keep moving right
                    chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, m.GetComponent<SpatialComponent>().Position.X + 32, m.GetComponent<SpatialComponent>().Position.Y);
                    oldVector.prevHorizontal = (float)chaseVector[0];
                    oldVector.prevVertical = (float)chaseVector[1];
                }
                else
                {
                    // Use Previous Movement Vector
                    chaseVector[0] = oldVector.prevHorizontal;
                    chaseVector[1] = oldVector.prevVertical;
                }

            }
            if (direction == "right") //Target is to our right
            {
                if (oldVector.prevHorizontal <= 0) //We were moving left before
                {
                    //keep moving left
                    chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, m.GetComponent<SpatialComponent>().Position.X - 32, m.GetComponent<SpatialComponent>().Position.Y);
                    oldVector.prevHorizontal = (float)chaseVector[0];
                    oldVector.prevVertical = (float)chaseVector[1];
                }
                else
                {
                    // Use Previous Movement Vector
                    chaseVector[0] = oldVector.prevHorizontal;
                    chaseVector[1] = oldVector.prevVertical;
                }
            }
            return chaseVector;
        }

        private Vector2 WallHuggerMovement(Entity m)
        {
            Vector2 velocity = new Vector2();


            Vector2 top = new Vector2(m.GetComponent<SpatialComponent>().Center.X, m.GetComponent<SpatialComponent>().Center.Y - 32);
            Vector2 bottom = new Vector2(m.GetComponent<SpatialComponent>().Center.X, m.GetComponent<SpatialComponent>().Center.Y + 32);
            Vector2 left = new Vector2(m.GetComponent<SpatialComponent>().Center.X - 32, m.GetComponent<SpatialComponent>().Center.Y);
            Vector2 right = new Vector2(m.GetComponent<SpatialComponent>().Center.X + 32, m.GetComponent<SpatialComponent>().Center.Y);
            List<Entity> topList = physics.RayCast(m.GetComponent<SpatialComponent>().Center, top);
            List<Entity> bottomList = physics.RayCast(m.GetComponent<SpatialComponent>().Center, bottom);
            List<Entity> leftList = physics.RayCast(m.GetComponent<SpatialComponent>().Center, left);
            List<Entity> rightList = physics.RayCast(m.GetComponent<SpatialComponent>().Center, right);
            bool t = WallCheck(topList);
            bool b = WallCheck(bottomList);
            bool l = WallCheck(leftList);
            bool r = WallCheck(rightList);
            Random rand = new Random();

            if (r && l)
            {
                if (b)
                {
                    //Move up
                    velocity = new Vector2(0.0f, -1.0f);
                }
                if (t)
                {
                    //Move Down
                    velocity = new Vector2(0.0f, 1.0f);
                }
                //Move up or down
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, -1.0f);
                }
                else
                {
                    velocity = new Vector2(0.0f, 1.0f);
                }
            }
            else if (t && b)
            {
                if (l)
                {
                    //Move right
                    velocity = new Vector2(1.0f, 0.0f);
                }
                if (r)
                {
                    //Move left
                    velocity = new Vector2(-1.0f, 0.0f);
                }
                //Move left or right
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(-1.0f, 0.0f);
                }
                else
                {
                    velocity = new Vector2(1.0f, 0.0f);
                }
            }
            else if (b && l)
            {
                //Move up or right
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, -1.0f);
                }
                else
                {
                    velocity = new Vector2(1.0f, 0.0f);
                }
            }
            else if (b && r)
            {
                //Move up or left
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, -1.0f);
                }
                else
                {
                    velocity = new Vector2(-1.0f, 0.0f);
                }
            }
            else if (t && l)
            {
                //Move down or right
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, 1.0f);
                }
                else
                {
                    velocity = new Vector2(1.0f, 0.0f);
                }
            }
            else if (t && r)
            {
                //Move down or left
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, 1.0f);
                }
                else
                {
                    velocity = new Vector2(-1.0f, 0.0f);
                }
            }
            else if (l || r)
            {

                //Move up or down
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(0.0f, -1.0f);
                }
                else
                {
                    velocity = new Vector2(0.0f, 1.0f);
                }
            }
            else if (t || b)
            {
                //Move left or right
                if (rand.NextDouble() > .5)
                {
                    velocity = new Vector2(-1.0f, 0.0f);
                }
                else
                {
                    velocity = new Vector2(1.0f, 0.0f);
                }
            }
            else
            {
                Vector2 topleft = new Vector2(m.GetComponent<SpatialComponent>().Center.X - 32, m.GetComponent<SpatialComponent>().Center.Y - 32);
                Vector2 topright = new Vector2(m.GetComponent<SpatialComponent>().Center.X + 32, m.GetComponent<SpatialComponent>().Center.Y - 32);
                Vector2 bottomleft = new Vector2(m.GetComponent<SpatialComponent>().Center.X - 32, m.GetComponent<SpatialComponent>().Center.Y + 32);
                Vector2 bottomright = new Vector2(m.GetComponent<SpatialComponent>().Center.X + 32, m.GetComponent<SpatialComponent>().Center.Y + 32);
                List<Entity> topRight = physics.RayCast(m.GetComponent<SpatialComponent>().Center, topright);
                List<Entity> topLeft = physics.RayCast(m.GetComponent<SpatialComponent>().Center, topleft);
                List<Entity> bottomRight = physics.RayCast(m.GetComponent<SpatialComponent>().Center, bottomright);
                List<Entity> bottomLeft = physics.RayCast(m.GetComponent<SpatialComponent>().Center, bottomleft);
                bool tr = WallCheck(topRight);
                bool tl = WallCheck(topLeft);
                bool bl = WallCheck(bottomLeft);
                bool br = WallCheck(bottomRight);
                if (tr)
                {
                    //Move Up or right
                    if (rand.NextDouble() > .5)
                    {
                        velocity = new Vector2(0.0f, -1.0f);
                    }
                    else
                    {
                        velocity = new Vector2(1.0f, 0.0f);
                    }
                }
                else if (tl)
                {
                    //Move up or left
                    if (rand.NextDouble() > .5)
                    {
                        velocity = new Vector2(0.0f, -1.0f);
                    }
                    else
                    {
                        velocity = new Vector2(-1.0f, 0.0f);
                    }
                }
                else if (bl)
                {
                    //Move down or left
                    if (rand.NextDouble() > .5)
                    {
                        velocity = new Vector2(0.0f, 1.0f);
                    }
                    else
                    {
                        velocity = new Vector2(-1.0f, 0.0f);
                    }
                }
                else if (br)
                {
                    //Move down or right
                    if (rand.NextDouble() > .5)
                    {
                        velocity = new Vector2(0.0f, 1.0f);
                    }
                    else
                    {
                        velocity = new Vector2(1.0f, 0.0f);
                    }
                }
                else
                {
                    //go find a wall//go find a wall
                    velocity = new Vector2(-1.0f, 0.0f);
                }
            }

            return velocity;
        }
        private bool WallCheck(List<Entity> list)
        {
            if (list.Count != 0)
            {
                foreach (Entity e in list)
                {
                    if (e.GetComponent<BorderComponent>() != null)
                    {
                        return true;

                    }
                }
                return false;
            }
            return false;
        }
    }
}
