using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ITS.Retail.PrintServer.Configurator
{
    public partial class MainForm : XtraForm
    {
        private ServiceController _Service;
        protected ServiceController Service
        {
            get
            {
                if (_Service == null)
                {
                    _Service = ServiceController.GetServices().FirstOrDefault(sv => sv.ServiceName == PrintServerConstants.ServiceName);                    
                }
                else
                {
                    _Service.Refresh();
                }
                return _Service;
            }
        }

        protected static readonly string SettingsFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Service\\config.xml";
        protected PrintServerSettings Settings { get; set; }

        private XmlSerializer xmlPrintServerSettingsSerializer;

        public MainForm()
        {
            InitializeComponent();
            xmlPrintServerSettingsSerializer = new XmlSerializer(typeof(PrintServerSettings));
            try
            {
                using (StreamReader reader = new StreamReader(SettingsFilePath))
                {
                    Settings = xmlPrintServerSettingsSerializer.Deserialize(reader) as PrintServerSettings;
                }
            }
            catch(Exception ex)
            {
                string exceptionMessage = ex.Message;
                Settings = new PrintServerSettings();
            }
            InitiateBindings();
            UpdateServiceStatus();
        }

        private void UpdateServiceStatus()
        {
            btnInstallService.Enabled = false;
            btnUninstallService.Enabled = false;
            btnStartService.Enabled = false;
            btnStopService.Enabled = false;
            if(Service == null)
            {
                btnInstallService.Enabled = true;
                this.lblServiceStatus.Text = "No service";
            }
            else
            {
                btnUninstallService.Enabled = true;
                if (Service.Status == ServiceControllerStatus.Running)
                {
                    btnStopService.Enabled = true;
                }
                else if (Service.Status == ServiceControllerStatus.Stopped)
                {
                    btnStartService.Enabled = true;
                }

                this.lblServiceStatus.Text = Service.Status.ToString();
            }
            
        }

        private void InitiateBindings()
        {
            txtListeningPort.DataBindings.Add("EditValue", Settings, "Port");
            txtStoreControllerUrl.DataBindings.Add("EditValue", Settings, "StoreControllerURL");
            btnedtDefaultPrinter.DataBindings.Add("EditValue", Settings, "DefaultPrinter");
            grdPrinterPosAssociations.DataSource = this.Settings.PrinterPOSAssociations;

            //Settings.PrinterPOSAssociations.AddingNew += PrinterPOSAssociations_AddingNew;
        }

        //private void PrinterPOSAssociations_AddingNew(object sender, AddingNewEventArgs e)
        //{
        //    
        //}

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            int numberOfDefaultPrinters = Settings.Printers.Where(printer => printer.IsDefault).Count();
            if ( numberOfDefaultPrinters > 1 )
            {
                MessageBox.Show(String.Format("You have set {0} printers as default. Please revise your settings.", numberOfDefaultPrinters));
                return;
            }
            else if (numberOfDefaultPrinters <= 0)
            {
                MessageBox.Show("You have not set a default Printer. Please revise your settings.");
                return;
            }

            xmlPrintServerSettingsSerializer = new XmlSerializer(typeof(PrintServerSettings));
            using (StreamWriter writer = new StreamWriter(SettingsFilePath))
            {
                xmlPrintServerSettingsSerializer.Serialize(writer, this.Settings);
            }
        }

        private void btnedtDefaultPrinter_ButtonClick(object sender, 
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            using (PrintDialog pd = new PrintDialog())
            {
                pd.PrinterSettings = new PrinterSettings();
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    btnedtDefaultPrinter.EditValue = pd.PrinterSettings.PrinterName;
                }
            }
        }

        private void repbtnedtSelectPrinter_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            using (PrintDialog pd = new PrintDialog())
            {
                pd.PrinterSettings = new PrinterSettings();
                if (DialogResult.OK == pd.ShowDialog(this))
                {
                    PrinterPOSAssociation assoc = grvPrinterPosAssociations.GetFocusedRow() as PrinterPOSAssociation;
                    ButtonEdit bedit = sender as ButtonEdit;
                    if (assoc != null && bedit !=null)
                    {
                        bedit.EditValue = pd.PrinterSettings.PrinterName;
                    }
                }
            }
        }

        private void repbtnedtDeleteRow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            PrinterPOSAssociation assoc = grvPrinterPosAssociations.GetFocusedRow() as PrinterPOSAssociation;
            if(assoc !=null)
            {
                this.Settings.PrinterPOSAssociations.Remove(assoc);
            }
        }

        private void grvPrinterPosAssociations_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if(e.Column == grcCommand && e.RowHandle <0)
            {
                e.Handled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string serviceExecutable = Path.GetDirectoryName(Application.ExecutablePath) + "\\Service\\ITS.Retail.PrintServer.Service.exe";

        private void btnInstallService_Click(object sender, EventArgs e)
        {            
            if (Service == null) 
            {
                Process.Start(serviceExecutable, "-SERVICE");
            }
        }

        private void btnUninstallService_Click(object sender, EventArgs e)
        {
            if (Service != null)
            {
                if (Service.Status != ServiceControllerStatus.Stopped)
                {
                    Service.Stop();
                }
                Process.Start(serviceExecutable, "-NOSERVICE");
            }
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            if(Service.Status == ServiceControllerStatus.Stopped)
            {
                Service.Start();
            }
        }

        private void btnStopService_Click(object sender, EventArgs e)
        {
            if (Service.Status == ServiceControllerStatus.Running)
            {
                Service.Stop();
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            this.UpdateServiceStatus();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GetSystemPrinters();
        }

        private void GetSystemPrinters()
        {
            ManagementObjectSearcher printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer");
            foreach (var printer in printerQuery.Get())
            {
                string name = printer.GetPropertyValue("Name") as string;
                string status = printer.GetPropertyValue("Status") as string;
                bool isNetworkPrinter = (bool)printer.GetPropertyValue("Network");

                
                PrinterInfo printerInfo = Settings.Printers.Where(printInfo => printInfo.Name == name).FirstOrDefault();
                bool isNewPrinter = printerInfo == null;
                if (isNewPrinter )
                {
                    printerInfo = new PrinterInfo()
                    {
                        Name = name,
                        NickName = name,
                        IsDefault = false,
                        IsNetworkPrinter = isNetworkPrinter,
                        Status = status
                    };
                    Settings.Printers.Add(printerInfo);
                }
                else
                {
                    printerInfo.Status = status;
                }
            }
            gridControlPrinters.DataSource = Settings.Printers;            
        }
    }
}
