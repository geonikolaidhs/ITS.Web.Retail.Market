using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ITS.POS.Tools.FormDesigner.Main
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] arg)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();
#endif

            Arguments args = new Arguments(arg);

            if (arg.Length == 0 || arg[0] == "")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmDesigner());
            }
            else if (args["build"] != null)
            {
                if (args["main"] == null && args["secondary"] == null)
                {
                    //example -build -tempfolder="C:\Projects\ITS.Retail.Platform\ITS\Retail\WebClient\ITS.Retail.WebClient\Temp" -secondary="C:\Projects\ITS.Retail.Platform\ITS\Retail\WebClient\ITS.Retail.WebClient\Temp\9505e5e1df5e4550b4b91f136bb1480d.itssform"

                    Console.WriteLine("Invalid parameters; usage:\n ITS.POS.Tools.FormDesigner.exe -build [-tempfolder=\"c:\\temp\"] [-main=\"c:\\path_to_form.itsform\"] [-secondary=\"c:\\path_to_secondary_form.itssform\"]");
                    Environment.Exit(-4);
                    return;
                }
                string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                using (StreamWriter logger = new StreamWriter(directory + "\\log.txt", true))
                {

                    try
                    {
                        string mainFormFile = args["main"];
                        string secondaryFormFile = args["secondary"];
                        string tempFolder = args["tempfolder"];

                        FormBuilder builder = new FormBuilder(mainFormFile, secondaryFormFile, tempFolder,logger);
                        int exitCode;
                        string result = builder.BuildAll(out exitCode);

                        Console.WriteLine(result);
                        Environment.Exit(exitCode);
                    }
                    catch(Exception ex)
                    {
                        logger.WriteLine(DateTime.Now + " - " + GetFullMessage(ex));
                    }
                }
            }

        }

        public static string GetFullMessage(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return ex.Message;
            }
            else
            {
                return ex.Message + GetFullMessage(ex.InnerException);
            }
        }
    }
}
