using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanGame.Game.Util
{
    class Logger
    {
        public enum LogLevel
        {
            DEBUG,
            ERROR
        };

        public static void Error(String msg)
        {
            Log(LogLevel.ERROR, null, msg);
        }

        public static void Error(String category, String msg)
        {
            Log(LogLevel.ERROR, category, msg);
        }

        public static void Debug(String msg)
        {
            Log(LogLevel.DEBUG, null, msg);
        }

        public static void Debug(String category, String msg)
        {
            Log(LogLevel.DEBUG, category, msg);
        }

        private static void Log(LogLevel level, String category, String msg)
        {
            String timestamp = DateTime.Now.ToString("HH:mm:ss");                       
            String categoryString = " ";
            if (category != null)
            {
                categoryString = " [" + category + "] ";
            }
            Console.WriteLine(level.ToString() +" "+ timestamp + categoryString + msg);
        }
    }
}
