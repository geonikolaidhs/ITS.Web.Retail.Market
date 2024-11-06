using ITS.POS.Client.Kernel;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmNotIncludedItems : frmMainBase
    {
        List<DocumentDetail> cancelledItems = new List<DocumentDetail>();
        List<DocumentDetail> notIncludedItems = new List<DocumentDetail>();

        public frmNotIncludedItems(IPosKernel kernel, List<DocumentDetail> notIncluded) : base()
        {
            notIncludedItems = notIncluded;
            InitializeComponent();
            //Attach(this.ucInput);
            this.paymentDetailsGrid1.DataSource = notIncluded;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            foreach(DocumentDetail detail in cancelledItems)
            {
                if(detail.Qty < notIncludedItems.FirstOrDefault(det => det.Item == detail.Item).Qty)
                {
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.NOT_ALL_ITEMS_CANCELLED);
                    return;
                }
            }
            //if (this.Customers.Count > 0)
            //{
            //    ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            //    InsertedCustomerViewModel selectedCustomer = bsCustomers.Current as InsertedCustomerViewModel;
            //    if (selectedCustomer.CustomerOid == Guid.Empty && validationProviderCustomer.Validate() == false)
            //    {
            //        return;
            //    }

            //    this.SelectedCustomer = sessionManager.GetObjectByKey<Customer>(selectedCustomer.CustomerOid);
            //    this.SelectedAddress = sessionManager.GetObjectByKey<Address>(selectedCustomer.AddressOid);
            //    this.CustomerViewModel = selectedCustomer;
            //}
            //this.CustomerLookupCode = edtCustomerCode.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void frmNotIncludedItems_Activated(object sender, EventArgs e)
        {
            //this.ucInput.Focus();
            //IAppContext AppContext = Kernel.GetModule<IAppContext>();
            //AppContext.CurrentFocusedScannerInput = this.ucInput;
        }
    }
}
