using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Threading;
using NLog;

namespace ITS.Retail.WrmDbTransfer
{
    static class Program
    {
       
        static Mutex mutex = new Mutex(true, "{9F6F0AC4-B9A1-45fd-A8CF-72F8446BDE9F}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                BonusSkins.Register();
                SkinManager.EnableFormSkins();
                UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");                
                Application.Run(new Main());
            }

        }


        public static Logger LogFile { get; set; }
    }
}
