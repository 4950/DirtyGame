using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace BasicSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory;
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = path + "TowerOffense\\xnafx40_redist.msi";
            Process p = Process.Start(pInfo);
            p.WaitForInputIdle();
            p.WaitForExit();
            Process.Start(path + "TowerOffense\\setup.exe");
        }
    }
}
