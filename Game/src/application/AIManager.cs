using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShittyPrototype.src.application.core;

namespace ShittyPrototype.src.application
{
    class AIManager
    {
        //Current goal: Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.

        public AIManager()
        {
            //Constructor. It does boring stuff.
        }




        int speedNumber = 10;

        //Make monsters of different types rush towards each other.
        // If no monster of another type is nearby... wander.
        public double[] calculateMoveVector(List<Monster> currentState, Monster m)
        {
            List<Monster> nextState = new List<Monster>();
            Random r = new Random();

            for (int i = 0; i < currentState.Count; i++)
            {
                if (!currentState[i].monsterType.Equals(m.monsterType))
                {
                    int otherX = currentState[i].pos.x; int otherY = currentState[i].pos.y;
                    if (getDistance(m.pos.x, m.pos.y, otherX, otherY) < 400)
                    {
                        //chase that intruder
                        double[] chaseVector = getChaseVector(m.pos.x, m.pos.y, otherX, otherY);
                        return chaseVector;
                    }
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
