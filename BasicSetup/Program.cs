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
            pInfo.FileName = path + "4950\\xnafx40_redist.msi";
            Process p = Process.Start(pInfo);
            p.WaitForInputIdle();
            p.WaitForExit();
            Process.Start(path + "4950\\setup.exe");
        }
    }
}
