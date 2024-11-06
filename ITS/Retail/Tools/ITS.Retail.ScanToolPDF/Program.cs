using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ITS.Common.Utilities.Forms;

namespace ITS.Retail.ScanTool.PDF
{
    static class Program
    {

        public static Form mainWindow;
        public static String ConfigurationFile
        {
            get
            {
                String appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                String configFileName = appPath + "\\ITS.Retail.ScanTool.config.xml";
                return configFileName;
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainWindow = new MainForm();
            mainWindow.Show();
            Application.DoEvents();
            //mainWindow.Hide();
            try
            {
                if (ReadDesignerParametersFromFile())
                //if(true)
                {
                    Application.Run(mainWindow);
                }
                else
                {
                    MessageBox.Show("Please specify Settings");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private static bool ReadDesignerParametersFromFile()
        {


            return ConfigurationHelper.LoadSettingsStatic(typeof(ApplicationSettings), ConfigurationFile, true);


        }
    }
}
