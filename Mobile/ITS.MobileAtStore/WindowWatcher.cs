using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace ITS.DataLogger.Mobile
{
    public class WindowWatcher
    {
        private bool m_enabled;
        private bool m_stopThread;
        private IntPtr m_lastForeWindow;
        private uint m_lastPID;
        private uint m_myPID;
        private int m_interval = 100;
        public event WindowChangedHandler WindowChanged;
        public delegate void WindowChangedHandler(bool processChanged, bool inMyProcess);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(
        IntPtr hwnd, out uint lpdwProcessId);

        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                if (value == m_enabled) return;

                if (value)
                {
                    Thread thread = new Thread(WatcherThreadProc);
                    thread.IsBackground = true;
                    thread.Start();
                }
                else
                {
                    m_stopThread = true;
                }
            }
        }

        public WindowWatcher(int interval)
        {
            m_enabled = false;
            m_lastForeWindow = IntPtr.Zero;
            m_lastPID = 0;
            m_interval = interval;
            // call the SDF because it's not a P/Invoke - it's a kcall and this is less code
            m_myPID = (uint)OpenNETCF.Diagnostics.ProcessHelper.GetCurrentProcessID();
        }

        private void WatcherThreadProc()
        {
            // initialize some values
            m_stopThread = false;
            m_lastForeWindow = GetForegroundWindow();
            GetWindowThreadProcessId(m_lastForeWindow, out m_lastPID);

            while (!m_stopThread)
            {
                IntPtr currentForeWindow = GetForegroundWindow();

                if (currentForeWindow != m_lastForeWindow)
                {
                    uint pid;
                    GetWindowThreadProcessId(currentForeWindow, out pid);

                    // window has changed - is it a new process?
                    bool newProcess = false;
                    if (pid != m_lastPID)
                    {
                        newProcess = true;
                        m_lastPID = pid;
                    }
                    bool inMyProcess = (pid == m_myPID);

                    if (WindowChanged != null)
                    {
                        WindowChanged(newProcess, inMyProcess);
                    }

                    m_lastForeWindow = currentForeWindow;
                }

                Thread.Sleep(m_interval);
            }
            m_enabled = false;
        }
    }
}
