
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WinFormsHalloweenProject
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Ghost());
        }
    }
}