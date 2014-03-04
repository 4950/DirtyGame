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
        public Vector2 calculateMoveVector(IEnumerable<Entity> entities, Entity m, float dt)
        {
            MovementComponent mc = m.GetComponent<MovementComponent>();
            SpatialComponent s = m.GetComponent<SpatialComponent>();
            double[] vel = new double[2];
            String type = m.GetComponent<PropertyComponent<String>>("MonsterType").value;

            bool[,] collMap = renderer.ActiveMap.getPassabilityMap();
            int mapWidth = renderer.ActiveMap.getPixelWidth() / 32;
            int mapHeight = renderer.ActiveMap.getPixelHeight() / 32;

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
            int mapWidth = renderer.ActiveMap.getPixelWidth() / 32;
            int mapHeight = renderer.ActiveMap.getPixelHeight() / 32;
            foreach (Entity e in entities)
            {
                if (e.HasComponent<PlayerComponent>())
                {

                    int otherX = (int)e.GetComponent<SpatialComponent>().Position.X;
                    int otherY = (int)e.GetComponent<SpatialComponent>().Position.Y;
                    //bool inSight = !WallCheck(physics.RayCast(new Vector2(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y), new Vector2(otherX, otherY)));
                    if (getDistance(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY) < maxrange &&
                        getDistance(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY) > minrange)
                    {
                        double[] chaseVector;

                        if (seek)
                        {
                            //Monster Tile Position
                            int monsterTileX = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.X / 32);
                            int monsterTileY = (int)Math.Floor(m.GetComponent<SpatialComponent>().Center.Y / 32);

                            //Player Tile Position
                            int goalTileX = (int)Math.Floor(e.GetComponent<SpatialComponent>().Center.X / 32);
                            int goalTileY = (int)Math.Floor(e.GetComponent<SpatialComponent>().Center.Y / 32);

                            chaseVector = aStarPath(monsterTileX, monsterTileY, goalTileX, goalTileY, mapWidth, mapHeight, m);
                        }
                        else
                        {
                            //chaseVector = flee(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);
                            chaseVector = getChaseVector(otherX, otherY, m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y);
                        }
                        return chaseVector;
                    }
                }
            }
            return new double[2];
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
                bool [,] collMap = map.getPassabilityMap();

                for (int i = -1; i<=1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if(i== 0 && j==0)
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
                Node n = (Node) obj;
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

        private class HeapSort
        {
            static int size = 0;
            public static void MinHeapify(Node[] a, int i)
            {
                Node temp;
                int left = Left(a, i);
                int right = Right(a, i);
                /* System.out.println("i: " + i);
                 System.out.println("L: " + left);
                 System.out.println("R: " + right);
                 System.out.println("len: " + a.length);
                 System.out.println("");*/
                if ((left < (a.Length + size)) && (right < (a.Length + size)))
                {
                    if ((a[left] > a[i]) && (a[right] > a[i]))
                    {

                    }
                    else
                    {
                        if (a[left] < a[right])
                        {
                            temp = a[i];
                            a[i] = a[left];
                            a[left] = temp;
                            MinHeapify(a, left);
                        }
                        else
                        {
                            temp = a[i];
                            a[i] = a[right];
                            a[right] = temp;
                            MinHeapify(a, right);
                        }
                    }
                }
                else
                {
                    if ((left >= a.Length + size) && (right >= a.Length + size))
                    {

                    }
                    else
                    {
                        if ((left >= a.Length + size))
                        {
                            if (a[right] < a[i])
                            {
                                temp = a[i];
                                a[i] = a[right];
                                a[right] = temp;
                                MinHeapify(a, right);
                            }
                        }
                        else
                        {
                            if (a[left] < a[i])
                            {
                                temp = a[i];
                                a[i] = a[left];
                                a[left] = temp;
                                MinHeapify(a, left);
                            }
                        }
                    }
                }
            }

            public static void BuildMinHeap(Node[] a)
            {
                for (int i = (int)Math.Floor(a.Length / 2.0); i >= 1; i--)
                {
                    MinHeapify(a, i);
                }
            }

            public static void HS(Node[] a)
            {
                Node[] b = Body(a);
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = b[b.Length - 1 - i];
                }
            }
            public static Node[] Body(Node[] a)
            {
                Node[] b = new Node[a.Length + 1];
                Node temp;
                int i;
                for (i = 0; i < a.Length; i++)
                {
                    b[i + 1] = a[i];
                }
                BuildMinHeap(b);
                for (i = b.Length - 1; i >= 2; i--)
                {
                    temp = b[1];
                    b[1] = b[i];
                    b[i] = temp;
                    size--;
                    MinHeapify(b, 1);
                }
                MinHeapify(b, 1);
                return b;
            }

            private static int Parent(Node[] a, int i)
            {
                if (i == 1)
                {
                    return i;
                }
                else
                {
                    return (int)Math.Floor(i / 2.0);
                }
            }

            private static int Right(Node[] a, int i)
            {
                if ((2 * i) + 1 > a.Length)
                {
                    return Int32.MaxValue;
                }
                else
                {
                    return (2 * i) + 1;
                }
            }

            private static int Left(Node[] a, int i)
            {
                if (2 * i > a.Length)
                {
                    return Int32.MaxValue;
                }
                else
                {
                    return 2 * i;
                }
            }
        }

        private class MaxHeapQueue
        {
            public class MaxHeapNode {

                public Node data;
                public MaxHeapNode left = null;
                public MaxHeapNode right = null;
                public MaxHeapNode parent = null;

                public MaxHeapNode(Node d) {
                    this.data = d;
                }

                public MaxHeapNode(Node d, MaxHeapNode p) {
                    //Set data = d, parent = p, and have
                    //this node be p.left if p.left is empty,
                    //or else p.right.
                    this.data = d;
                    this.parent = p;
                    if(p.left==null)
                    {
                        p.left = this;
                    }
                    else
                    {
                        p.right = this;
                    }
        
                }

                public void setLeft(MaxHeapNode l) {
                    left = l;
                }

                public void setRight(MaxHeapNode r) {
                    right = r;
                }

                public void heapify() {
                    Node temp = this.data;
            //        this.printSubTree(0);
            //        System.out.println("");
                    if(this.left!=null && this.right!=null)
                    {
                        if(this.data>this.left.data && this.data>this.right.data)
                        {}
                        else
                        {
                            if(this.left.data>=this.right.data)
                            {
                                this.data = this.left.data;
                                this.left.data = temp;
                                this.left.heapify();
                            }
                            else
                            {
                                this.data = this.right.data;
                                this.right.data = temp;
                                this.right.heapify();
                            } 
                        }
                    }
                    else
                    {
                        if(this.left==null && this.right==null)
                        {}
                        else
                        {
                            if(this.left==null)
                            {
                                if(this.data>this.right.data)
                                {}
                                else
                                {
                                    this.data = this.right.data;
                                    this.right.data = temp;
                                    this.right.heapify();
                                }
                            }
                            else
                            {
                                if(this.right==null)
                                {
                                    if(this.data>this.left.data)
                                    {}
                                    else
                                    {
                                        this.data = this.left.data;
                                        this.left.data = temp;
                                        this.left.heapify();
                                    }
                                }
                            }
                        }
                    }        //Determine which value is the largest: this node,
                    //The left node, or the right node. If a child is larger,
                    //Swap the value of the parent and the largest child, then
                    //heapify on the child node.
                }

                public void promote(Node v) 
                {
                    this.data = v;
                    if(this.parent == null)
                    {
                        this.heapify();
                    }
                    else
                    {
                        if(v>this.parent.data)
                        {
                            this.data = this.parent.data;
                            this.parent.data = v;
            //                System.out.println("parent: " + this.parent.data);
                            heapifyUp(this.parent);
                        }
                        else
                        {
            
                        }   
                    }
                    //Increase the data value of this node to v. Then, compare it to the parent node.
                    //if the new data value is higher than the parent data value, swap them.
                    //Continue swapping with higher parents until you've reached the top
                    //Or your parent is a larger value than you.
                }

                public void heapifyUp(MaxHeapNode node)
                {
                    if(node.parent != null)
                    {
                        if(node.data > node.parent.data)
                        {
                            node.parent.heapify();
                            heapifyUp(node.parent);
                        }
                        else
                        {
                            node.heapify();
                        }
                    }
                    else
                    {
                        node.heapify();
                    }
                }
    
                public void printSubTree(int depth) {
                    //Prints the node and its children, using the style developed by Baylor.
                    for (int i = 1; i < depth; i++) {
                        Console.Write("    ");
                    }
                    if (depth != 0) {
                        Console.Write("|~~~");
                    }
                    Console.WriteLine(this.data);
                    if (left != null) {
                        left.printSubTree(depth + 1);
                    }
                    if (right != null) {
                        right.printSubTree(depth + 1);
                    }
                }

                public Boolean isFull() 
                {
                    return (this.left!=null && this.right!=null);
                }

                public Boolean isLeaf()
                {
                    if (left == null && right ==null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
    
                
                public override String ToString()
                {
                    String s = "";
                    s+=this.data;
                    return s;
                }
            }

            MaxHeapNode top = null;
            Queue<MaxHeapNode> q = new Queue<MaxHeapNode>();
            int Size = 0;
            MaxHeapNode addNode;
            private int tempHeight = 0;
            MaxHeapNode[] BF;


            public MaxHeapQueue()
            {
            }

            public MaxHeapQueue(Node[] values) 
            {
                foreach(Node n in values)
                {
                    this.push(n);
                }
            }

            public void push(Node n) {
                //Find the correct node to add to, using getAddNode(). Make the new node a 
                //child of addNode with an initial data value of Integer.MIN_VALUE.
                //Then, promote the new node to data value v.
                if(top == null)
                {
                    top = new MaxHeapNode(n);
                    Size++;
                }
                else
                {
                    MaxHeapNode parent = this.getAddNode();
                    //System.out.println("addNode:" + parent.data);
                    MaxHeapNode newNode = new MaxHeapNode(new Node(), parent);
                    newNode.promote(n);
                    Size++;
                }
            }

            public Node peek()
            {
                if (top == null)
                {
                    return null;
                }
                else
                {
                    return top.data;
                }
            }

            public Node pop()
            {
                if (top == null)
                {
                    return null;
                }
                else
                {
                    if (top.isLeaf())
                    {
                        Node pop = top.data;
                        top = null;
                        Size--;
                        return pop;
                    }
                    else
                    {
                        if (top.right == null)
                        {
                            Node pop = top.data;
                            MaxHeapNode temp = top.left;
                            top.left = null;
                            temp.parent = null;
                            top = temp;
                            Size--;
                            return pop;
                        }
                        else
                        {
                            if (getHeight(top) == 1)
                            {
                                Node pop = top.data;
                                MaxHeapNode temp = popLast();
                                if (temp == top.left)
                                {
                                    top.left = null;
                                }
                                if (temp == top.right)
                                {
                                    top.right = null;
                                }
                                temp.left = top.left;
                                temp.right = top.right;
                                top = temp;
                                top.heapify();
                                Size--;
                                return pop;
                            }
                            else
                            {
                                Node pop = top.data;
                                MaxHeapNode temp = popLast();
                                temp.left = top.left;
                                temp.right = top.right;
                                top = temp;
                                top.heapify();
                                Size--;
                                return pop;
                            }
                        }
                    }
                }

                //Save the value of top.data. Remove the last node from the bottom of the heap,
                //and place its value at the top of the heap. Heapify to fix any issues with
                //Removing the top value.
            }

            public MaxHeapNode popLast() {
                //Breadth-first search of the heap. Works just like BF search of a binary tree.
                //Using a queue, push the first HeapNode into the queue. While the queue isn't empty,
                //add the children of each node. The last node to leave the queue is the last node in
                //the heap. Remove the parent references to that node and return it.
                //System.out.println("pop last");
                MaxHeapNode last = new MaxHeapNode(new Node());
                q.Clear();
                q.Enqueue(top);
                while(q.Count!=0)
                {
                    if(q.Peek().left!= null)
                    {
                        q.Enqueue(q.Peek().left);
                    }
                    if(q.Peek().right!=null)
                    {
                        q.Enqueue(q.Peek().right);
                    }
                    last = q.Dequeue();
                }
                if(last == last.parent.left)
                {
                    last.parent.left = null;
                }
                if(last == last.parent.right)
                {
                    last.parent.right = null;
                }
                last.parent = null;
        //        System.out.println(last.parent);
        //        System.out.println(this.top.left);
        //        System.out.println(this.top.right);
                return last;
            }

            public MaxHeapNode getAddNode() {
                //The addNode is the node under which newly pushed nodes should be added.
                //To find this node, do a breadth-first search of the heap, and return the
                //first node that does not have BOTH its left and right children.
                MaxHeapNode addNode = new MaxHeapNode(new Node());
                q.Clear();
                q.Enqueue(top);
                while(q.Count != 0)
                {
                    Console.WriteLine(q.ToString());
                    if(!q.Peek().isFull())
                    {
                        addNode = q.Peek();
                        break;
                    }
                    if(q.Peek().left!= null)
                    {
                        q.Enqueue(q.Peek().left);
                    }
                    if(q.Peek().right!=null)
                    {
                        q.Enqueue(q.Peek().right);
                    }
                    q.Dequeue();
                }
                return addNode;
            }

            public int size()
            {
                return Size;
            }

            public void clear()
            {
                top = null;
            }

            public Boolean isEmpty()
            {
                //A method you should be somewhat familiar with.
                //Returns true if there's nothing in the HeapQueue.
                if (top != null)
                {
                    return false;
                }
                return true;
            }

            public void printHeap() {
                //Prints the entire heap recursively.
                if (isEmpty()) {
                    return;
                }
                top.printSubTree(0);
                Console.WriteLine();
            }

            public int getHeight(MaxHeapNode node)
            {
                MaxHeapNode current = node;
                heightHelper(current, tempHeight);
                int returnHeight = tempHeight;
                tempHeight = 0;
                return returnHeight;
            }

            private void heightHelper(MaxHeapNode current, int h)
            {
                if (current == null)
                {
                    if (h > tempHeight)
                    {
                        tempHeight = h - 1;
                    }
                }
                else
                {
                    heightHelper(current.left, h + 1);

                    heightHelper(current.right, h + 1);
                }
            }


        }
    }
}
