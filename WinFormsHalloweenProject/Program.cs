
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WinFormsHalloweenProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            Process visualStudios = Process.GetProcessesByName("VsDebugConsole").FirstOrDefault();
            
            if (visualStudios != null)
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Ghost());
            }
        }
    }
}