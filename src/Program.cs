#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace ShittyPrototype
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static String Path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Content\\";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new ShittyPrototype())
                game.Run();
        }
    }
#endif
}
