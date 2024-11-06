using System;
using System.IO;
using System.Windows.Forms;
using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Common.Utilities.Forms;

namespace ITS.Retail.ReportDesigner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                if (ReadDesignerParametersFromFile())
                {
                    Application.Run(new DesignerForm());
                }
                else
                {
                    MessageBox.Show("Please specify Designer Settings");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool ReadDesignerParametersFromFile()
        {
            string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + "\\ITS\\ReportDesigner";
            if(Directory.Exists(configPath) == false)
            {
                Directory.CreateDirectory(configPath);
            }
            String configFileName = configPath + "\\Reportdesigner.xml";
            XpoDefault.Dictionary = XpoHelper.dict;
            return ConfigurationHelper.LoadSettingsStatic(typeof(XpoHelper),configFileName, true);

            
        }
    }
}
