#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
#endregion

namespace CleanGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class App
    {
        public static String Path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Content\\";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Dirty())
#if DEBUG
                game.Run();
#else
            CatchExceptions(game);
#endif
        }
        public static string PublishVersion
        {
            get
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    Version ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    return string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
                }
                else
                    return "Develop";
            }
        }
        static void CatchExceptions(Dirty game)
        {
            try
            {
                game.Run();
            }
            catch(Exception e)
            {
                LogStack(e);
            }
        }
        public static void LogStack(Exception e)
        {
            var trace = new System.Diagnostics.StackTrace();
            string msg = "";
            foreach (var frame in trace.GetFrames())
            {
                var method = frame.GetMethod();
                if (method.Name.Equals("LogStack")) continue;
                string tmp = string.Format("{0}::{1}",
                    method.ReflectedType != null ? method.ReflectedType.Name : string.Empty,
                    method.Name);
                Debug.WriteLine(tmp);
                msg += tmp + "\n";
            }
            MessageBox.Show(e.Message + "\n" + e.StackTrace + "\n" + msg, "Game Error");
        }
    }
#endif
}
