using System;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Common.Keyboards.Compact;
using System.Globalization;
using Common;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Retail.Mobile.Helpers;
using DevExpress.Xpo.Metadata;
using Retail.Mobile_Model;

namespace Retail.Mobile
{
    static class Program
    {
        #region OpenNETCF native interface to mutex generation (version 1.4 of the SDF)

        public const Int32 NATIVE_ERROR_ALREADY_EXISTS = 183;

        #region P/Invoke Commands for Mutexes
        [DllImport("coredll.dll", EntryPoint = "CreateMutex", SetLastError = true)]
        public static extern IntPtr CreateMutex(
            IntPtr lpMutexAttributes,
            bool InitialOwner,
            string MutexName);

        [DllImport("coredll.dll", EntryPoint = "ReleaseMutex", SetLastError = true)]
        public static extern bool ReleaseMutex(IntPtr hMutex);

        [DllImport("coredll.dll")]
        private static extern IntPtr FindWindow(IntPtr className, string windowName);
        [DllImport("coredll.dll")]
        internal static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("coredll.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, int hwnd2, int x, int y, int cx, int cy, int uFlags);
        #endregion

        public static bool IsInstanceRunning()
        {
            IntPtr hMutex = CreateMutex(IntPtr.Zero, true, "ApplicationName");
            if (hMutex == IntPtr.Zero)
                throw new ApplicationException("Failure creating mutex: "
                    + Marshal.GetLastWin32Error().ToString("X"));

            if (Marshal.GetLastWin32Error() == NATIVE_ERROR_ALREADY_EXISTS)
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            if (IsInstanceRunning())
            {
#region open existing thread
                IntPtr h = FindWindow(IntPtr.Zero, "Retail --- "+Resources.Resources.Offline);
                if (h == IntPtr.Zero)
                {
                    h = FindWindow(IntPtr.Zero, "Retail --- " + Resources.Resources.Online);
                }
                SetForegroundWindow(h);
                SetWindowPos(h, 0, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, 0x0040);
#endregion
                return;
            }

            AppSettings.executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (File.Exists(AppSettings.executingPath + "\\config.xml"))
            {
                using (StreamReader sr = new StreamReader(AppSettings.executingPath + "\\config.xml", Encoding.UTF8))
                {
                    CultureInfo culture = new CultureInfo("en");

                    XmlDocument xml = new XmlDocument();
                    xml.Load(sr);
                    XmlNode settingsNode = xml["request"]["settings"];
                    foreach (XmlNode node in settingsNode.ChildNodes)
                    {
                        if (node.Attributes["id"].Value == "ip")
                        {
                            AppSettings.IP = node.InnerText;
                        }
                        else if (node.Attributes["id"].Value == "PDA_ID")
                        {
                            AppSettings.Pda_ID = node.InnerText;
                        }
                        else if (node.Attributes["id"].Value == "LocalDBPath")
                        {
                            AppSettings.databasePath = node.InnerText;
                        }
                        else if (node.Attributes["id"].Value == "Language")
                        {
                            culture = new CultureInfo(node.InnerText);
                        }
                    }

                    CultureHelper.SetCulture(culture);
                    
                    
                    if (AppSettings.databasePath.Length == 0)
                    {
                        AppSettings.databasePath = AppSettings.executingPath;
                    }
                }
            }
            else
            {
                using (SettingsForm frm = new SettingsForm(null))
                {
                    frm.ShowDialog();
                }

            }
            
            string sqlitefile = Path.Combine(AppSettings.databasePath, "Pda-Retail.db");

            KeyboardGateway.Initialize(Common.CultureInfo, true);


            string conn = DevExpress.Xpo.DB.SQLiteConnectionProvider.GetConnectionString(sqlitefile);
            XPDictionary dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(DocLine).Assembly);

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(conn, dict,AutoCreateOption.DatabaseAndSchema,out objectsTodispose);
            XpoDefault.Session = new Session(XpoDefault.DataLayer);
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static IDisposable[] objectsTodispose;
        
    }
}