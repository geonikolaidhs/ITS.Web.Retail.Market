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
using ITS.POS.Hardware.Common;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays various info about the status of the application.
    /// </summary>
    public partial class ucPosStatus : ucDoubleLabel, IObserverStatusDisplayer
    {
        public ucPosStatus()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.UPDATE_COMMUNICATION_STATUS);
            VisibleStatus = eMachineStatus.UNKNOWN; //visible in all statuses
        }

        /// <summary>
        /// Optional. Sets the status that the control will be shown
        /// </summary>
        public eMachineStatus VisibleStatus { get; set; }

        /// <summary>
        /// Determines what info to show
        /// </summary>
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
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                IDocumentService documentService = Kernel.GetModule<IDocumentService>();
                try
                {
                    if (appContext.CurrentDocument != null)
                    {
                        docType = sessionManager.GetObjectByKey<DocumentType>(appContext.CurrentDocument.DocumentType);
                        docSeries = sessionManager.GetObjectByKey<DocumentSeries>(appContext.CurrentDocument.DocumentSeries);
                        nextNumber = "" + documentService.GetNextDocumentNumber(appContext.CurrentDocument, config.CurrentTerminalOid, appContext.CurrentUser.Oid);
                    }
                    else
                    {
                        if (config.DefaultDocumentTypeOid != Guid.Empty && config.DefaultDocumentTypeOid != null && config.DefaultDocumentSeriesOid != Guid.Empty && config.DefaultDocumentSeriesOid != null)
                        {
                            docType = sessionManager.GetObjectByKey<DocumentType>(config.DefaultDocumentTypeOid);
                            docSeries = sessionManager.GetObjectByKey<DocumentSeries>(config.DefaultDocumentSeriesOid);
                        }
                        else
                        {
                            lblValue.Text = "-";
                        }

                    }
                    if (appContext.CurrentDocument != null && appContext.CurrentDocument.Customer != Guid.Empty)
                    {
                        customer = sessionManager.GetObjectByKey<Customer>(appContext.CurrentDocument.Customer);
                    }
                    else
                    {
                        if (config.DefaultCustomerOid != Guid.Empty)
                        {
                            customer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
                        }
                    }
                }
                catch (Exception exception)
                {
                    string errorMessage = exception.GetFullMessage();
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
                                if (printer != null)
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
                            lblValue.Text = appContext?.CurrentUser?.UserName;
                            break;
                        case eStatusLabelType.CHANGE:
                            lblValue.Text = appContext.CurrentDocument != null ? String.Format("{0:C}", appContext.CurrentDocument.Change.ToString()) : "-";
                            break;
                        case eStatusLabelType.CUSTOMER:
                            string customerName = "";
                            string pointsText = "";
                            if (appContext.CurrentDocument != null)
                            {
                                customerName = appContext.CurrentDocument.CustomerDisplayName;
                                if (appContext.CurrentDocument.Customer != config.DefaultCustomerOid)
                                {
                                    Customer currentCustomer = sessionManager.GetObjectByKey<Customer>(appContext.CurrentDocument.Customer);
                                    if (currentCustomer != null)
                                    {
                                        pointsText = String.Format(" ({0:0.###})", currentCustomer.CollectedPoints);
                                    }
                                }
                            }
                            else
                            {
                                customerName = "";
                                if (customer != null)
                                {
                                    if (customer.CompanyName != null)
                                    {
                                        customerName = customer.CompanyName;
                                    }
                                }
                            }

                            lblValue.Text = customerName + pointsText;
                            break;
                        case eStatusLabelType.DOCUMENTS_ON_HOLD:
                            lblValue.Text = appContext.DocumentsOnHold.Count.ToString();
                            break;
                        case eStatusLabelType.ACITVE_FISCAL_DEVICE:
                            switch (config.FiscalDevice)
                            {
                                case eFiscalDevice.DATASIGN:
                                case eFiscalDevice.DISIGN:
                                case eFiscalDevice.ALGOBOX_NET:
                                    List<Device> eafDevices = deviceManager.GetEAFDSSDevicesByPriority(config.FiscalDevice);
                                    lblValue.Text = eafDevices.Count > 0 ? eafDevices[0].DeviceName : Resources.POSClientResources.NOT_FOUND;
                                    break;
                                default:
                                    lblValue.Text = config.FiscalDevice.ToLocalizedString();
                                    break;
                            }
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
