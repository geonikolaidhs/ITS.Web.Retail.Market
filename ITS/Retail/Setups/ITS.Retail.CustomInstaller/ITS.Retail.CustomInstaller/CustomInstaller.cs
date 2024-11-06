using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.IO;


namespace ITS.Retail.CustomInstaller
{
    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {
        public CustomInstaller()
        {
            InitializeComponent();
            //Debugger.Launch();
            //Debugger.Break();
            //try
            //{
            //    string installPath = this.Context.Parameters["targetdir"];
            //    foreach (string path in Directory.GetDirectories(installPath).Where(x => !x.EndsWith("Log") && !x.EndsWith("Configuration")))
            //    {
            //        this.DeleteDirectory(path);
            //    }
            //}
            //catch
            //{
            //
            //}
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            //-		["assemblypath"]	"C:\\inetpub\\wwwroot\\Retail.Setup\\ITS.Retail.CustomInstaller.dll"	
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
                //Delete all child Directories
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
        }
    }
}
