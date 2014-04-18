using CleanGame.Game.Core.Components;
using CleanGame.Game.Core.Components.Render;
using CleanGame.Game.Core.Systems.Util;
using CleanGame.Game.SGraphics;
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
        public Renderer renderer;


        //Current goal: Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.

        public AISystem(Dirty game, EntityFactory entityFactory, Physics physics, Renderer renderer)
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            this.game = game;
            this.entityFactory = entityFactory;
            this.physics = physics;
            this.renderer = renderer;
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

                    else if (wc.WeaponName == "GrenadeLauncher" || wc.WeaponName == "Monsterbow")
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
        public double[] snipMovement(Entity e, float dt)
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
            return vel;
        }



        //Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.
        public Vector2 calculateMoveVector(IEnumerable<Entity> entities, Entity m, float dt, TimeSpan totalTime)
        {
            MovementComponent mc = m.GetComponent<MovementComponent>();
            SpatialComponent s = m.GetComponent<SpatialComponent>();
            double[] vel = new double[2];
            String type = m.GetComponent<PropertyComponent<String>>("MonsterType").value;

            bool[,] collMap = renderer.ActiveMap.getPassabilityMap();
            int mapWidth = renderer.ActiveMap.getPixelWidth() / 32;
            int mapHeight = renderer.ActiveMap.getPixelHeight() / 32;
            Entity player = game.player;



            if (type == "Flametower")
            {
                //don't move
            }

            else if (type == "SnipMonster")
            {
                vel = snipMovement(m, dt);
            }

            else if (type == "WallHugger")
            {
                return WallHuggerMovement(m, collMap, mapWidth, mapHeight) * 5 * (m.GetComponent<StatsComponent>().MoveSpeed / 100.0f);
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
                    TimeComponent mTime = m.GetComponent<TimeComponent>();
                    vel = seekPlayer(entities, m, 0, 150, false);//if player is close, run

                    if (mTime.timeOfWeaponCheck <= totalTime)
                    {
                        //Set what the monster thinks the player's weapon is for AI purpooses
                        m.GetComponent<MonsterComponent>().PlayerWeapon = player.GetComponent<InventoryComponent>().CurrentWeapon.GetComponent<WeaponComponent>().WeaponName;

                        //Set the next time the monster should check the player's weapon (randomized)
                        m.GetComponent<TimeComponent>().timeOfWeaponCheck = mTime.timeOfWeaponCheck + new TimeSpan(0, 0, 0, 0, 1000 + r.Next(4) * 1000);
                    }
                    if (vel[0] == vel[1] && vel[0] == 0)
                    {

                        // If Player Weapon is Sword
                        if (m.GetComponent<MonsterComponent>().PlayerWeapon == "Basic Sword")
                        {

                            //  Move to half range
                            vel = seekPlayer(entities, m, (int)wc.Range / 2, 600, true);
                        }
                        else // If Player Weapon is Bow
                        {
                            //  Move to full range
                            vel = seekPlayer(entities, m, (int)wc.Range, 600, true);
                        }


                        //seekPlayer(entities, m, (int)wc.Range - 50, 600, true);//if player is not within weapon range but in sight range, chase
                    }

                    if (vel[0] == vel[1] && vel[0] == 0)//player not in sight or in range, wander
                    {
                        float theta = mc.WanderTheta;
                        vel = Wander(s.Position, mc.Velocity, ref theta);
                        mc.WanderTheta = theta;
                    }
                }
                if (wc.WeaponName == "GrenadeLauncher")
                {
                    TimeComponent mTime = m.GetComponent<TimeComponent>();
                    vel = seekPlayer(entities, m, 0, 150, false);//if player is close, run

                    if (mTime.timeOfWeaponCheck <= totalTime)
                    {
                        //Set what the monster thinks the player's weapon is for AI purpooses
                        m.GetComponent<MonsterComponent>().PlayerWeapon = player.GetComponent<InventoryComponent>().CurrentWeapon.GetComponent<WeaponComponent>().WeaponName;

                        //Set the next time the monster should check the player's weapon (randomized)
                        m.GetComponent<TimeComponent>().timeOfWeaponCheck = mTime.timeOfWeaponCheck + new TimeSpan(0, 0, 0, 0, 1000 + r.Next(4) * 1000);
                    }
                    if (vel[0] == vel[1] && vel[0] == 0)
                    {

                        // If Player Weapon is Sword
                        if (m.GetComponent<MonsterComponent>().PlayerWeapon == "Basic Sword")
                        {

                            //  Move to half range
                            vel = seekPlayer(entities, m, (int)wc.Range / 2, 600, true);
                        }
                        else // If Player Weapon is Bow
                        {
                            //  Move to full range
                            vel = seekPlayer(entities, m, (int)wc.Range, 600, true);
                        }


                        //seekPlayer(entities, m, (int)wc.Range - 50, 600, true);//if player is not within weapon range but in sight range, chase
                    }

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

            if (type != "Flametower")
                setDirection(vel, m);

            return new Vector2((float)vel[0], (float)vel[1]) * 5 * (m.GetComponent<StatsComponent>().MoveSpeed / 100.0f);
        }


        private double[] seekPlayer(IEnumerable<Entity> entities, Entity m, int minrange, int maxrange, bool seek)
        {
            int mapWidth = renderer.ActiveMap.getPixelWidth() / 32;
            int mapHeight = renderer.ActiveMap.getPixelHeight() / 32;
            foreach (Entity e in entities)
            {
                if (e.HasComponent<PlayerComponent>())
                {

                    int otherX = (int)e.GetComponent<SpatialComponent>().Center.X;
                    int otherY = (int)e.GetComponent<SpatialComponent>().Center.Y;
                    //bool inSight = !WallCheck(physics.RayCast(new Vector2(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y), new Vector2(otherX, otherY)));
                    if (getDistance(m.GetComponent<SpatialComponent>().Center.X, m.GetComponent<SpatialComponent>().Center.Y, otherX, otherY) < maxrange &&
                        getDistance(m.GetComponent<SpatialComponent>().Center.X, m.GetComponent<SpatialComponent>().Center.Y, otherX, otherY) > minrange)
                    {
                        double[] chaseVector;

                        if (seek)
                        {
                            // A*
                            ////Monster Tile Position
                            //int monsterTileX = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.X / 32);
                            //int monsterTileY = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.Y / 32);

                            ////Player Tile Position
                            //int goalTileX = (int)Math.Floor(e.GetComponent<SpatialComponent>().Center.X / 32);
                            //int goalTileY = (int)Math.Floor(e.GetComponent<SpatialComponent>().Center.Y / 32);

                            //chaseVector = aStarPath(monsterTileX, monsterTileY, goalTileX, goalTileY, mapWidth, mapHeight, m);
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

                            chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Center.X, m.GetComponent<SpatialComponent>().Center.Y, otherX, otherY);
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
                            //chaseVector = flee(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);
                            //chaseVector = getChaseVector(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);
                            chaseVector = fleeToNearest(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, entities);
                        }
                        return chaseVector;
                    }
                }
            }
            return new double[2];
        }

        private double[] fleeToNearest(float monsterX, float monsterY, IEnumerable<Entity> entities)
        {
            float allyX;
            float allyY;
            float goalX = 0;
            float goalY = 0;
            double distanceToAlly;
            double minDistance = double.MaxValue;
            double[] moveVector = new double[2];
            foreach (Entity ally in entities)
            {
                allyX = ally.GetComponent<SpatialComponent>().Position.X;
                allyY = ally.GetComponent<SpatialComponent>().Position.Y;

                if (allyX == monsterX && allyY == monsterY || (ally.GetComponent<PlayerComponent>() != null))
                {
                    continue;
                }

                distanceToAlly = getManhattanDistance(monsterX, monsterY, allyX, allyY);
                if (distanceToAlly < minDistance)
                {
                    minDistance = distanceToAlly;
                    goalX = allyX;
                    goalY = allyY;
                }
            }
            moveVector = getChaseVector(monsterX, monsterY, goalX, goalY);
            return moveVector;
        }

        private double[] flee(int playerX, int playerY, float monsterX, float monsterY)
        {
            double[] pToM = getChaseVector(playerX, playerY, monsterX, monsterY);
            throw new NotImplementedException();
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
            const float wanderR = 32.0f;
            const float wanderD = 64.0f;
            const float change = 0.5f;

            float negChange = (int)Math.Round(r.NextDouble()) + 1;
            float randomNum = (float)(r.NextDouble() * change);
            if (negChange == 2)
                theta = theta - randomNum;
            else
                theta = theta + randomNum;


            Vector2 circleLoc = curVel;
            if (circleLoc.Length() == 0)
                circleLoc = new Vector2(0, 1);

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


        private double getDistance(double x, double y, double ox, double oy)
        {
            return Math.Sqrt(
                (Math.Pow(ox - x, 2.0))
                + (Math.Pow(oy - y, 2.0))
                );
        }

        private double getManhattanDistance(double x, double y, double ox, double oy)
        {
            return Math.Abs(ox - x) + Math.Abs(oy - y);
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

        //depricated
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
        //depricated
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

        private Vector2 WallHuggerMovement(Entity m, bool[,] collMap, int mapWidth, int mapHeight)
        {
            int monsterX = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.X / 32);
            int monsterY = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.Y / 32);

            Vector2 velocity = new Vector2();
            bool t = collMap[monsterX, Math.Max(monsterY - 1, 0)];
            bool b = collMap[monsterX, Math.Min(monsterY + 1, mapHeight - 1)];
            bool l = collMap[Math.Max(monsterX - 1, 0), monsterY];
            bool r = collMap[Math.Min(monsterX + 1, mapWidth - 1), monsterY];
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
                //Move up
                velocity = new Vector2(0.0f, -1.0f);
            }
            else if (b && r)
            {
                //Moveleft
                velocity = new Vector2(-1.0f, 0.0f);
            }
            else if (t && l)
            {
                //Move right
                velocity = new Vector2(1.0f, 0.0f);
            }
            else if (t && r)
            {
                //Move down 
                velocity = new Vector2(0.0f, 1.0f);
            }
            else if (l)
            {

                //Move up
                velocity = new Vector2(0.0f, -1.0f);
            }
            else if (r)
            {
                //Move Down
                velocity = new Vector2(0.0f, 1.0f);

            }
            else if (b)
            {
                //Move left 
                velocity = new Vector2(-1.0f, 0.0f);
            }
            else if (t)
            {
                //Move Right
                velocity = new Vector2(1.0f, 0.0f);
            }
            else
            {
                bool tr = collMap[Math.Min(monsterX + 1, mapWidth - 1), Math.Max(monsterY - 1, 0)];
                bool tl = collMap[Math.Max(monsterX - 1, 0), Math.Max(monsterY - 1, 0)];
                bool bl = collMap[Math.Max(monsterX - 1, 0), Math.Min(monsterY + 1, mapHeight - 1)];
                bool br = collMap[Math.Min(monsterX + 1, mapWidth - 1), Math.Min(monsterY + 1, mapHeight - 1)];
                MovementComponent oldMovement = m.GetComponent<MovementComponent>();
                if (tr)
                {
                    //Move Up or right
                    if (oldMovement.prevVertical == 0)
                    {
                        velocity = new Vector2(0.0f, -1.0f);
                    }
                    else //if(oldMovement.prevVertical>0)
                    {
                        velocity = new Vector2(1.0f, 0.0f);
                    }
                }
                else if (tl)
                {
                    //Move up or left
                    if (oldMovement.prevVertical == 0)
                    {
                        velocity = new Vector2(0.0f, -1.0f);
                    }
                    else //if(oldMovement.prevVertical>0)
                    {
                        velocity = new Vector2(-1.0f, 0.0f);
                    }
                }
                else if (bl)
                {
                    //Move down or left
                    if (oldMovement.prevVertical == 0)
                    {
                        velocity = new Vector2(0.0f, 1.0f);
                    }
                    else //if(oldMovement.prevVertical<0)
                    {
                        velocity = new Vector2(-1.0f, 0.0f);
                    }
                }
                else if (br)
                {
                    //Move down or right
                    if (oldMovement.prevVertical == 0)
                    {
                        velocity = new Vector2(0.0f, 1.0f);
                    }
                    else //if (oldMovement.prevVertical < 0)
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

            m.GetComponent<MovementComponent>().prevHorizontal = velocity.X;
            m.GetComponent<MovementComponent>().prevVertical = velocity.Y;
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

        private class Node
        {
            public int xPos;
            public int yPos;
            public float gScore;
            public float fScore;
            public Node cameFrom;
            private Renderer renderer;

            public Node(int xPos, int yPos, float gScore, float fScore, Node cameFrom)
            {
                this.xPos = xPos;
                this.yPos = yPos;
                this.gScore = gScore;
                this.fScore = fScore;
                this.cameFrom = cameFrom;
            }

            public Node(Renderer r)
            {
                renderer = r;
            }

            public Node()
            {

            }

            public Node(int xPos, int yPos, Node cameFrom, Renderer renderer)
            {
                this.xPos = xPos;
                this.yPos = yPos;
                this.cameFrom = cameFrom;
                this.renderer = renderer;
            }

            public LinkedList<Node> neighbors()
            {
                LinkedList<Node> neighbors = new LinkedList<Node>();
                Map.Map map = renderer.ActiveMap;
                bool[,] collMap = map.getPassabilityMap();

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                        {

                        }
                        else if ((this.xPos + i < map.getPixelHeight() / 32) && (this.yPos + j < map.getPixelWidth() / 32))
                        {
                            if (!collMap[this.xPos + i, this.yPos + j])
                            {
                                neighbors.AddLast(new Node(this.xPos + i, this.yPos + j, this, renderer));

                            }
                        }
                    }
                }

                return neighbors;
            }

            public override bool Equals(object obj)
            {
                Node n = (Node)obj;
                if (n.xPos != this.xPos || n.yPos != this.yPos)
                {
                    return false;
                }
                return true;
            }

            //int IComparer<Node>.Compare(object a, object b)
            //{
            //    Node n1 = (Node)a;
            //    Node n2 = (Node)b;

            //    if (n1.fScore > n2.fScore)
            //        return 1;

            //    if (n1.fScore < n2.fScore)
            //        return -1;

            //    else
            //        return 0;
            //}

            public static Boolean operator >(Node n1, Node n2)
            {
                if (n1.fScore > n2.fScore)
                {
                    return true;
                }
                return false;
            }

            public static Boolean operator >=(Node n1, Node n2)
            {
                if (n1.fScore >= n2.fScore)
                {
                    return true;
                }
                return false;
            }

            public static Boolean operator <(Node n1, Node n2)
            {
                if (n1.fScore < n2.fScore)
                {
                    return true;
                }
                return false;
            }

            public static Boolean operator <=(Node n1, Node n2)
            {
                if (n1.fScore <= n2.fScore)
                {
                    return true;
                }
                return false;
            }

            public Node getPath()
            {
                Node n = this;
                while (n.cameFrom.cameFrom != null)
                {
                    n = n.cameFrom;
                }
                return n;
            }
        }

        private double[] aStarPath(int monsterTileX, int monsterTileY, int goalTileX, int goalTileY, int mapWidth, int mapHeight, Entity m)
        {
            double[] chaseVector = new double[2];
            LinkedList<Node> openList = new LinkedList<Node>();
            //Dictionary<int, int> closedList = new Dictionary<int, int>();

            bool[,] closedList = new bool[mapWidth, mapHeight];

            Node start = new Node(renderer);
            start.xPos = monsterTileX;
            start.yPos = monsterTileY;
            start.gScore = 0;
            start.fScore = (float)getManhattanDistance(start.xPos, start.yPos, goalTileX, goalTileY) + start.gScore;
            start.cameFrom = null;

            openList.AddFirst(start);
            chaseVector = null;
            while (openList.Count != 0)
            {
                Node current = null;
                //Find best node to evaluate
                foreach (Node n in openList)
                {
                    if (current == null)
                    {
                        current = n;
                    }
                    else
                    {

                        if (current.fScore > n.fScore)
                        {
                            current = n;
                        }
                    }
                }



                //Finish if we find goal node
                if (current.xPos == goalTileX && current.yPos == goalTileY)
                {
                    //Generate Vector
                    Node next = current.getPath();
                    chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, next.xPos * 32, next.yPos * 32);
                }

                //Update open/closed list
                openList.Remove(current);
                closedList[current.xPos, current.yPos] = true;

                foreach (Node n in current.neighbors())
                {
                    if (closedList[n.xPos, n.yPos])
                    {
                        continue;
                    }

                    float tentativeGScore = current.gScore + (float)getManhattanDistance(current.xPos, current.yPos, n.xPos, n.yPos);

                    LinkedListNode<Node> old = openList.Find(n);
                    if (old != null)
                    {
                        Node oldN = old.Value;

                        if (tentativeGScore < oldN.gScore)
                        {
                            openList.Remove(n);
                            n.cameFrom = current;
                            n.gScore = tentativeGScore;
                            n.fScore = n.gScore + (float)getManhattanDistance(n.xPos, n.yPos, goalTileX, goalTileY);
                            openList.AddFirst(n);
                        }
                    }
                    else
                    {
                        n.cameFrom = current;
                        n.gScore = tentativeGScore;
                        n.fScore = n.gScore + (float)getManhattanDistance(n.xPos, n.yPos, goalTileX, goalTileY);
                        openList.AddFirst(n);
                    }
                }
            }

            return chaseVector;
        }

    }
}
