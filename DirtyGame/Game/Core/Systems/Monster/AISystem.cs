using DirtyGame.game.Core.Components;
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
    public class AISystem
    {
        //Current goal: Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.

        public AISystem()
        {
            //Constructor. It does boring stuff.
        }

        //public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        //{
        //    //Do Nothing
        //}

        //public override void OnEntityAdded(Entity e)
        //{
        //    // do nothing
        //}

        //public override void OnEntityRemoved(Entity e)
        //{
        //    // do nothing
        //}


        int speedNumber = 10;

        //Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.
        public double[] calculateMoveVector(IEnumerable<Entity> entities, Entity m)
        {
            List<Entity> nextState = new List<Entity>();
            Random r = new Random();
            DirectionComponent direction;

            foreach (Entity e in entities)
            {
                if (e.HasComponent<PlayerComponent>())
                {
                    //if (!m.GetComponent<MonsterComponent>().monsterType.Equals(e.GetComponent<MonsterComponent>().monsterType))
                    //{
                        int otherX = (int)e.GetComponent<SpatialComponent>().Position.X;
                        int otherY = (int)e.GetComponent<SpatialComponent>().Position.Y;
                        if (getDistance(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY) < 400)
                        {
                            double[] chaseVector = getChaseVector(m.GetComponent<SpatialComponent>().Position.X, m.GetComponent<SpatialComponent>().Position.Y, otherX, otherY);

                            //Maybe put the monster animation here
                            direction = m.GetComponent<DirectionComponent>();
                            if (Math.Abs(chaseVector[0]) > Math.Abs(chaseVector[1]))
                            {
                                if (chaseVector[0] > 0)
                                {
                                    direction.Heading = "Right";
                                    m.GetComponent<MovementComponent>().Horizontal = 1;
                                    AnimationComponent animation = new AnimationComponent();
                                    animation.CurrentAnimation = "Walk" + direction.Heading;
                                    m.AddComponent(animation);
                                    m.Refresh();
                                }
                                else if (chaseVector[0] < 0)
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
                                if (chaseVector[1] > 0)
                                {
                                    direction.Heading = "Down";
                                    m.GetComponent<MovementComponent>().Vertical = 1;
                                    AnimationComponent animation = new AnimationComponent();
                                    animation.CurrentAnimation = "Walk" + direction.Heading;
                                    m.AddComponent(animation);
                                    m.Refresh();
                                }
                                else if (chaseVector[1] < 0)
                                {
                                    direction.Heading = "Up";
                                    m.GetComponent<MovementComponent>().Vertical = -1;
                                    AnimationComponent animation = new AnimationComponent();
                                    animation.CurrentAnimation = "Walk" + direction.Heading;
                                    m.AddComponent(animation);
                                    m.Refresh();
                                }
                            }

                            return chaseVector;
                        }
                        //}
                }
           }

                //else, Random walk 
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

            //Maybe put the monster movement animation here too
                direction = m.GetComponent<DirectionComponent>();
                if (Math.Abs(randDir[0]) > Math.Abs(randDir[1]))
                {
                    if (randDir[0] > 0)
                    {
                        direction.Heading = "Right";
                        m.GetComponent<MovementComponent>().Horizontal = 1;
                        AnimationComponent animation = new AnimationComponent();
                        animation.CurrentAnimation = "Walk" + direction.Heading;
                        m.AddComponent(animation);
                        m.Refresh();
                    }
                    else if (randDir[0] < 0)
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
                    if (randDir[1] > 0)
                    {
                        direction.Heading = "Down";
                        m.GetComponent<MovementComponent>().Vertical = 1;
                        AnimationComponent animation = new AnimationComponent();
                        animation.CurrentAnimation = "Walk" + direction.Heading;
                        m.AddComponent(animation);
                        m.Refresh();
                    }
                    else if (randDir[1] < 0)
                    {
                        direction.Heading = "Up";
                        m.GetComponent<MovementComponent>().Vertical = -1;
                        AnimationComponent animation = new AnimationComponent();
                        animation.CurrentAnimation = "Walk" + direction.Heading;
                        m.AddComponent(animation);
                        m.Refresh();
                    }
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
