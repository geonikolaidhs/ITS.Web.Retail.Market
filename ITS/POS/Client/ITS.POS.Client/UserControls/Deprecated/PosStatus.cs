using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using System.Reflection;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Master;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class PosStatus : DoubleLabel, IObserverStatusDisplayer
    {
        public PosStatus()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.UPDATE_COMMUNICATION_STATUS);
            VisibleStatus = eMachineStatus.UNKNOWN; //visible in all statuses
        }

        public eMachineStatus VisibleStatus { get; set; }

        public eStatusLabelType StatusType { get; set; }

        Type[] paramsTypes = new Type[] { typeof(StatusDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public void Update(StatusDisplayerParams parameters)
        {
            if (this.IsDisposed)
            {
                return;
            }
            Invoke((MethodInvoker)delegate
            {
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                IDocumentService documentService = Kernel.GetModule<IDocumentService>();
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();

                if (VisibleStatus == eMachineStatus.UNKNOWN || VisibleStatus == parameters.MachineStatus)
                {
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }

                DocumentType docType = null;
                DocumentSeries docSeries = null;
                Customer customer = null;
                string nextNumber = "-";
                try
                {
                    docType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
                    docSeries = sessionManager.GetObjectByKey<DocumentSeries>(appContext.CurrentDocument.DocumentSeries);
                    customer = sessionManager.GetObjectByKey<Customer>(appContext.CurrentDocument.Customer);

                    nextNumber = "" + documentService.GetNextDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid);
                }
                catch
                {
                    docType = sessionManager.GetObjectByKey<DocumentType>( config.DefaultDocumentTypeOid);
                    docSeries = sessionManager.GetObjectByKey<DocumentSeries>(config.DefaultDocumentSeriesOid);
                    customer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
                }
                try
                {
                    switch (StatusType)
                    {
                        case eStatusLabelType.APPLICATION_VERSION:

                            string versionText = GetType().Assembly.GetName().Version.ToString(4);
                            if (config.FiscalMethod == eFiscalMethod.ADHME)
                            {
                                FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                                if(printer != null)
                                {
                                    versionText = deviceManager.GetVisibleVersion(config.FiscalMethod);
                               } 
                            }

                            lblValue.Text = versionText;
                            break;
                        case eStatusLabelType.DATE:
                            lblValue.Text = DateTime.Now.ToShortDateString();
                            break;
                        case eStatusLabelType.DOCUMENT_NUMBER:
                            lblValue.Text = nextNumber;
                            break;
                        case eStatusLabelType.DOCUMENT_SERIES:
                            lblValue.Text = docSeries.Description;
                            break;
                        case eStatusLabelType.DOCUMENT_TYPE:
                            lblValue.Text = docType.Description;
                            break;
                        case eStatusLabelType.MACHINE_STATUS:
                            lblValue.Text = appContext.GetMachineStatus().ToString();
                            break;
                        case eStatusLabelType.POSID:
                            lblValue.Text = config.TerminalID.ToString();
                            break;
                        case eStatusLabelType.TIME:
                            lblValue.Text = DateTime.Now.ToShortTimeString();
                            break;
                        case eStatusLabelType.USERID:
                            lblValue.Text = appContext.CurrentUser.POSUserName;
                            break;
                        case eStatusLabelType.USERNAME:
                            lblValue.Text = appContext.CurrentUser.UserName;
                            break;
                        case eStatusLabelType.CHANGE:
                            lblValue.Text = appContext.CurrentDocument != null ?  String.Format("{0:C}", appContext.CurrentDocument.Change.ToString()) : "-";
                            break;
                        case eStatusLabelType.CUSTOMER:
                            lblValue.Text = appContext.CurrentDocument != null ? appContext.CurrentDocument.CustomerDisplayName : customer.CompanyName;
                            break;

                    }
                }
                catch (Exception)
                {
                    lblValue.Text = "-";
                }
            });

        }

    }
}
