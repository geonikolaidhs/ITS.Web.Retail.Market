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
            Settings = ConfigurationHelper.LoadSettings<FiscalServiceSettings>(Common.SettingsFilePath, gui);
            switch (Settings.FiscalDevice)
            {
                case Retail.Platform.Enumerations.eFiscalDevice.RBS_NET:
                    RbsSign = new RBSSign(Settings.ConnectionType, Settings.DeviceName);
                    RbsSign.Settings.COM.PortName = Settings.COM_PortName;
                    RbsSign.Settings.Ethernet.IPAddress = Settings.Ethernet_IPAddress;
                    RbsSign.AfterLoad(null);
                    RbsSign.RegistrationNumber = Settings.RegistrationNumber;
                    RbsSign.GetSerialNumber(this.Settings.AbcFolder);
                    break;
                default:
                    Algobox = new AlgoboxNetESD(Settings.ConnectionType, Settings.DeviceName);
                    Algobox.Settings.COM.PortName = Settings.COM_PortName;
                    Algobox.Settings.Ethernet.IPAddress = Settings.Ethernet_IPAddress;
                    break;
            }


            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            MessageServer = new MessageServer(Constants.ApplicationIdentifier, Settings.Port);
            FiscalServerFiscalGetSerialNumberListener getVersionListener = new FiscalServerFiscalGetSerialNumberListener(this, Settings);
            MessageServer.AddListener(new FiscalServerSignDocumentListener(this, Settings));
            MessageServer.AddListener(getVersionListener);
            MessageServer.AddListener(new FiscalServerCheckBoxSystemListener(this));
            MessageServer.AddListener(new FiscalServerGetVersionListener(this));
            MessageServer.AddListener(new FiscalServerIssueZListener(this));
            MessageServer.AddListener(new FiscalServerGetOnlineListener(this));
            MessageServer.AddListener(new FiscalServerSetFiscalOnErrorListener(this));

            MessageServer.StartServer();
            checkingThread = new CustomThread(CheckOnline);
            checkingThread.Start();


        }

        private void CheckOnline()
        {
            int tries = 0;
            while (checkingThread.IsAborted == false)
            {
                string v = null;
                if (this.Algobox != null)
                {
                    v = this.Algobox.GetSerialNumber();

                }
                else
                {
                    Program.Logger.Trace("Begin Check Online thread GetSerial Number");
                    v = this.RbsSign.GetSerialNumber(this.Settings.AbcFolder);
                    Program.Logger.Trace("End Check Online thread GetSerial Number. Result: " + v);
                }
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
                    MessageServer.StopServer("Disposing");
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
        }

        protected override void OnStart(string[] args)
        {
            if (Program.Logger == null)
            {
                Program.Logger = LogManager.GetLogger("DiSign Service");
            }
            try
            {
                fiscalserver = new FiscalServer(false);
            }
            catch (Exception ex)
            {
                Program.Logger.ErrorException("On Start Exception", ex);
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
