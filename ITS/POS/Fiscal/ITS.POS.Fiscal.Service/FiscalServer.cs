using ITS.Common.Communication;
using ITS.Common.Utilities.Forms;
using ITS.POS.Fiscal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;
using NLog;
using ITS.POS.Hardware.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.Requests;
using ITS.POS.Fiscal.Service.Listeners;
using System.Diagnostics;
using ITS.POS.FiscalService.RequestResponseLogging.Model;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
//using ITS.POS.Hardware.Common;

namespace ITS.POS.Fiscal.Service
{
    public static class Common
    {
        public static String SettingsFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.xml";
    }

    public class FiscalServer : IDisposable
    {
        public MessageServer MessageServer { get; protected set; }
        public FiscalServiceSettings Settings { get; protected set; }
        public AlgoboxNetESD Algobox { get; protected set; }
        public AlgoboxNetESDV2 AlgoboxV2 { get; protected set; }
        public RBSSign RbsSign { get; protected set; }
        public bool ReloadSettings { get; set; }

        object locking = new object();
        private volatile bool _online;
        public bool Online
        {
            get
            {
                return _online;
            }
            set
            {
                lock (locking)
                {
                    _online = value;
                }
            }
        }

        CustomThread checkingThread;

        public FiscalServer(bool gui)
        {
            Program.Logger.Trace("Loading Settings...");
            Settings = ConfigurationHelper.LoadSettings<FiscalServiceSettings>(Common.SettingsFilePath, gui);
            Program.Logger.Trace("Settings Loaded");

            Program.Logger.Trace("Initialising Request / Response Log if needed.");
            if (Settings.KeepLog)
            {
                string dataBaseFile = "FiscalServer.sqlite";
                XpoHelper.SetTransactionFile(dataBaseFile);
                if (File.Exists(dataBaseFile) == false)
                {
                    XpoHelper.UpdateDatabase();
                }
            }
            Program.Logger.Trace("Request / Response Log initialised");

            switch (Settings.FiscalDevice)
            {
                case Retail.Platform.Enumerations.eFiscalDevice.RBS_NET:
                    Program.Logger.Trace("Creating RBS Device");
                    RbsSign = new RBSSign(Settings.ConnectionType, Settings.DeviceName, Settings);
                    RbsSign.SetLogger(Program.Logger);
                    RbsSign.Settings.COM.PortName = Settings.COM_PortName;
                    RbsSign.Settings.Ethernet.IPAddress = Settings.Ethernet_IPAddress;
                    RbsSign.AfterLoad(null);
                    RbsSign.RegistrationNumber = Settings.RegistrationNumber;
                    Program.Logger.Trace("Getting Serial Number Start");
                    RbsSign.GetSerialNumber(this.Settings.AbcFolder);
                    Program.Logger.Trace("Getting Serial Number End");
                    break;
                case Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NETV2:
                    Program.Logger.Trace("Creating AlgoboxV2 Device");
                    AlgoboxV2 = new AlgoboxNetESDV2(Settings.ConnectionType, Settings.DeviceName, Settings);
                    AlgoboxV2.SetLogger(Program.Logger);
                    AlgoboxV2.Settings.COM.PortName = Settings.COM_PortName;
                    AlgoboxV2.Settings.Ethernet.IPAddress = Settings.Ethernet_IPAddress;
                    break;
                default:
                    Program.Logger.Trace("Creating Algobox Device");
                    Algobox = new AlgoboxNetESD(Settings.ConnectionType, Settings.DeviceName);
                    Algobox.Settings.COM.PortName = Settings.COM_PortName;
                    Algobox.Settings.Ethernet.IPAddress = Settings.Ethernet_IPAddress;
                    break;
            }



            Program.Logger.Trace("Starting Message Server at port " + Settings.Port);
            MessageServer = new MessageServer(Settings.Port, Program.Logger);
            Program.Logger.Trace("Starting Message Server started");
            Program.RequestResponseLogger = new RequestResponseLogger(Settings.KeepLog);
            MessageServer.AddListener(new FiscalServerSignDocumentListener(this, Settings));
            MessageServer.AddListener(new FiscalServerFiscalGetSerialNumberListener(this, Settings));
            MessageServer.AddListener(new FiscalServerCheckBoxSystemListener(this));
            MessageServer.AddListener(new FiscalServerGetVersionListener(this));
            MessageServer.AddListener(new FiscalServerIssueZListener(this, Settings));
            MessageServer.AddListener(new FiscalServerGetOnlineListener(this));
            MessageServer.AddListener(new FiscalServerSetFiscalOnErrorListener(this));

            checkingThread = new CustomThread(CheckOnline);
            Program.Logger.Trace("Starting Check online thread");
            checkingThread.Start();


        }

        private void CheckOnline()
        {


            //#if DEBUG
            //            Debugger.Launch();
            //#endif
            int tries = 0;
            while (checkingThread.IsAborted == false)
            {
                string v = null;
                Program.Logger.Trace("Begin Check Online thread GetSerial Number");
                if (this.Algobox != null)
                {
                    v = this.Algobox.GetSerialNumber();
                }
                else if (this.AlgoboxV2 != null)
                {
                    v = this.AlgoboxV2.GetSerialNumber();
                }
                else
                {
                    v = this.RbsSign.GetSerialNumber(this.Settings.AbcFolder);
                }
                Program.Logger.Trace("End Check Online thread GetSerial Number. Result: " + v);
                if (v == null && this.Online == true && tries < 10)
                {
                    tries++;
                    Program.Logger.Trace("GetSerialNumber Failed. Retrying... (try " + tries + ")");
                    checkingThread.Sleep(100);
                    continue;
                }
                tries = 0;
                this.Online = (v != null);
                if (v != null && v != this.Settings.SerialNumber)
                {
                    this.Settings.SerialNumber = v;
                    ConfigurationHelper.SaveSettingsFile(Settings, Common.SettingsFilePath);
                }
                checkingThread.Sleep(60000);
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (checkingThread != null)
                {
                    checkingThread.Abort();
                }
                if (MessageServer != null)
                {
                    MessageServer.Dispose();
                }
            }

        }

    }

    public partial class FiscalService : ServiceBase
    {
        FiscalServer fiscalserver;

        public FiscalService()
        {
            InitializeComponent();

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
        }

        protected override void OnStart(string[] args)
        {
            if (Program.Logger == null)
            {
                Program.Logger = LogManager.GetLogger("DiSign Service");
            }
            try
            {
                Program.Logger.Trace("Staring Fiscal Server");
                fiscalserver = new FiscalServer(false);
                Program.Logger.Trace("Fiscal Server Started");
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex, "On Start Exception");
                this.Stop();
            }
        }



        protected override void OnStop()
        {
            if (fiscalserver != null)
            {
                fiscalserver.Dispose();
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
            this.ServiceName = "ITS Fiscal Service";
        }

        #endregion
    }
}
