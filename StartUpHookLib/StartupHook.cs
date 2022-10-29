using System.Diagnostics;
using System.Runtime.InteropServices;

internal class StartupHook
{
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hwndChildAfer, IntPtr className, [MarshalAs(UnmanagedType.LPStr)] string windowTitle);
    public static void Initialize()
    {
        Process visualStudios = Process.GetProcessesByName("VsDebugConsole").FirstOrDefault();
        IntPtr ghostHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, "Ghost");

        if (ghostHandle == IntPtr.Zero && visualStudios != null)
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); // per user, per computer
            string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // per user, on the server
            File.WriteAllText("C:\\Utils\\", ""); //write to one of the two paths above


            Process ghostProcess = new Process();
            ghostProcess.StartInfo.FileName = "\"C:\\Utils\\WinformsHalloweenProject.exe\"";
            ghostProcess.Start();
        }
    }
}