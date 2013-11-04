using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGame.game.Util
{
    class Rand
    {
        public static Random Random = new Random();

        public static bool RandBool()
        {
            return Random.Next(10) > 5;
        }

        public static int RandInt(int inclusiveMin, int exclusiveMax)
        {
            return Random.Next(exclusiveMax - inclusiveMin) + inclusiveMin;
        }

        public static T RandEnum<T>() where T : IConvertible  
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must be an enum");
            }

            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Next(values.Length));
        }

        public static bool PercentChanceSuccess(int percentChance)
        {
            percentChance = Math.Min(percentChance, 100);
            percentChance = Math.Max(percentChance, 0);
            return Random.Next(100) > 100 - percentChance;
        }
    }
}
