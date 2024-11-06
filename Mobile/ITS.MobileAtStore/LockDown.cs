using System;
using System.Runtime.InteropServices;

namespace App
{
    public class LockDown
    {
        public LockDown()
        {
        }
        [DllImport("coredll.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("coredll.dll", EntryPoint = "EnableWindow")]
        private static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

        public static bool Execute(bool enabled)
        {
            IntPtr hwnd = FindWindow("HHTaskBar", null);
            if (!hwnd.Equals(IntPtr.Zero))
            {
                if (enabled)
                {
                    return EnableWindow(hwnd, false);
                }
                else
                {
                    return EnableWindow(hwnd, true);
                }
            }
            return true;
        }
    }
}
