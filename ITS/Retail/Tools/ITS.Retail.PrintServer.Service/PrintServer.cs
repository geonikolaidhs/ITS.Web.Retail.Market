using DevExpress.Pdf;
using DevExpress.XtraPdfViewer;
using ITS.Common.Communication;
using ITS.Common.Utilities.Forms;
using ITS.Retail.PrintServer.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace ITS.Retail.PrintServer.Service
{

    public static class Common
    {
        public static string SettingsFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.xml";        
    }
    public class PrintServer : IDisposable
    {
        /// <summary>
        /// Gets or sets the TCP Message Server
        /// </summary>
        public MessageServer MessageServer { get; protected set; }

        /// <summary>
        /// Gets or sets the Print Server Settings
        /// </summary>
        public PrintServerSettings Settings { get; protected set; }




        /// <summary>
        /// Gets or sets a flag to Reload Settings from the file
        /// </summary>
        public bool ReloadSettings { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (MessageServer != null)
                {
                    MessageServer.Dispose();
                    MessageServer = null;
                }
            }

        }
        
        /// <summary>
        /// Creates a PrintServer object 
        /// </summary>
        /// <param name="gui">Determines if the object will be used in a UI</param>
        public PrintServer(bool gui)
        {
            Program.Logger.Trace("Loading Settings...");

            XmlSerializer xmlser = new XmlSerializer(typeof(PrintServerSettings));
            using (StreamReader reader = new StreamReader(Common.SettingsFilePath))
            {
                Settings = xmlser.Deserialize(reader) as PrintServerSettings;
                //.Serialize(writer, this.Settings);
            }
    //        Settings = ConfigurationHelper.LoadSettings<PrintServerSettings>(Common.SettingsFilePath, gui);
            Program.Logger.Trace("Settings Loaded");           

            Program.Logger.Trace("Starting Message Server at port " + Settings.Port);
            MessageServer = new MessageServer(Settings.Port, Program.Logger);
            Program.Logger.Trace("Starting Message Server started");
            Program.Logger.Trace("Add Listener PrintServerPrintDocumentListener");
            MessageServer.AddListener(new PrintServerPrintDocumentListener(this));
            Program.Logger.Trace("Listener PrintServerPrintDocumentListener added");

            Program.Logger.Trace("Add Listener PrintServerGetPrintersListener");
            MessageServer.AddListener(new PrintServerGetPrintersListener(this));
            Program.Logger.Trace("Listener PrintServerGetPrintersListener added");

            Program.Logger.Trace("Add Listener PrintServerPrintLabelListener");
            MessageServer.AddListener(new PrintServerPrintLabelListener(this));
            Program.Logger.Trace("Listener PrintServerPrintLabelListener added");
        }

        private object locking = new object();
    }


    public partial class PrintService : ServiceBase
    {
        PrintServer printServer;

        /// <summary>
        /// Creates a PrinteService Object
        /// </summary>
        public PrintService()
        {
            InitializeComponent();
            //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;           
        }

        protected override void OnStart(string[] args)
        {
            if (Program.Logger == null)
            {
                Program.Logger = LogManager.GetLogger("Print Service");
            }
            try
            {
                Program.Logger.Trace("Staring Print Server");
                printServer = new PrintServer(false);
                Program.Logger.Trace("Print Server Started");
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, "On Start Exception");
                this.Stop();
            }
        }



        protected override void OnStop()
        {
            if (printServer != null)
            {
                printServer.Dispose();
            }
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "ITS Print Service";
        }

        #endregion
    }
}
