using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace BasicSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String path = AppDomain.CurrentDomain.BaseDirectory;
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.FileName = path + "4950\\xnafx40_redist.msi";
                Process p = Process.Start(pInfo);
                p.WaitForInputIdle();
                p.WaitForExit();
                Process.Start(path + "4950\\setup.exe");
            }
            catch (Exception e)
            {
                LogStack(e);
            }
        }
        public static void LogStack(Exception e)
        {
            var trace = new System.Diagnostics.StackTrace();
            MessageBox.Show(e.Message + "\n" + e.StackTrace, "Setup Error");
        }
    }
}
