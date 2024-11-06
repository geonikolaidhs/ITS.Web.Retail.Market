using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using System.Reflection;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using ITS.POS.Client.Extensions;
using ITS.POS.Hardware;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    [Designer(typeof (ControlDesigner))]
    public partial class ucSimpleDocumentDetailGrid : ucObserver, IObserverGrid
    {
        Type[] paramsTypes = new Type[] { typeof(GridParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        private void UpdateMode()
        {
            
        }

        //object _docDetDS;
        //protected object DocumentDetailDataSource
        //{
        //    get
        //    {
        //        return _docDetDS;
        //    }
        //    set
        //    {
        //        _docDetDS = value;
        //    }
        //}

        private bool _PaymentMode;
        protected bool PaymentMode
        {
            get
            {
                return _PaymentMode;
            }
            set
            {
                _PaymentMode = value;
                UpdateMode();
            }
        }

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //[Editor("System.Windows.Forms.Design.DataGridViewColumnCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        //[Editor(typeof(ExtendedDataGridViewColumnCollectionEditor), typeof(UITypeEditor))]
        public DataGridViewColumnCollection DetailColumnCollection
        {
            get
            {
                return grdDocumentDetails.Columns;
            }
        }



        public ucSimpleDocumentDetailGrid()
        {            
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.MOVE_UP);
            this.ActionsToObserve.Add(eActions.MOVE_DOWN);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO);
            this.grdDocumentDetails.DataSource = new BindingList<DocumentDetail>();
            
            /*source.DataSource = new List<DocumentDetail>();
            this.grdDocumentDetails.DataSource = source;*/
            
            
        }

        public bool HideDeletedLines { get; set; }

        public DataGridView DocumentDetails
        {
            get
            {
                return this.grdDocumentDetails;
            }
        }
        /// <summary>
        /// Expected Parameters: param1 = {DocumentHeader || null || DocumentDetail || DocumentPayment }, [ param2 = eDisplayMode Mode ]
        /// 
        /// DocumentHeader or null : Sets or clears the datasource
        /// DocumentDetail : Selects that document detail
        /// DocumentPayment: Selects that document payment
        /// Mode : Selects displaying between DocumentDetails or DocumentPayments
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void Update(GridParams parameters)
        {
            if (parameters.DocumentHeader != null)
            {
                if (parameters.DisplayMode == eDisplayMode.DETAILS)
                {
                    if (HideDeletedLines)
                    {
                        parameters.DocumentHeader.DocumentDetails.Filter = new BinaryOperator("IsCanceled", false);
                    }
                    else
                    {
                        parameters.DocumentHeader.DocumentDetails.Filter = null;
                    }
                    grdDocumentDetails.DataSource = parameters.DocumentHeader.DocumentDetails;

                    //grdDocumentDetails.MainView = gridViewDocumentDetails;
                }
                else if (parameters.DisplayMode == eDisplayMode.PAYMENTS)
                {
                    grdDocumentDetails.DataSource = parameters.DocumentHeader.DocumentPayments;
                    //grdDocumentDetails.MainView = gridViewDocumentTotals;
                }
            }
            else if (parameters.SelectedDocumentDetail != null)
            {

                int newPosition = grdDocumentDetails.LocateByValue("Oid", parameters.SelectedDocumentDetail.Oid);
                if (grdDocumentDetails.CurrentRow.Index == newPosition && newPosition != 0)
                {
                    //Same position, do nothing
                    return;
                }
                //grdDocumentDetails.in = newPosition;
                if (parameters.SelectedDocumentDetail.CustomDescription != null)
                {
                    //string[] lines = new String[] { parameters.SelectedDocumentDetail.CustomDescription.ToUpperGR(), parameters.SelectedDocumentDetail.Qty + " X " + String.Format("{0:C}", parameters.SelectedDocumentDetail.FinalUnitPriceWithVatBeforeDocumentDiscount) };
                    //GlobalContext.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                    grdDocumentDetails.Rows[newPosition].Selected = true;
                }
            }
            else if (parameters.SelectedDocumentPayment != null)
            {
                int newPosition = grdDocumentDetails.LocateByValue("Oid", parameters.SelectedDocumentPayment.Oid);
                if (grdDocumentDetails.CurrentRow.Index == newPosition)
                {
                    //Same position, do nothing
                    return;
                }
                grdDocumentDetails.Rows[newPosition].Selected = true;
            }
            else if (parameters.Navigation != null)
            {

                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();

                switch (parameters.Navigation)
                {
                    case eNavigation.MOVEUP:
                        if (grdDocumentDetails.CurrentRow.Index > 0)
                        {
                            DocumentDetail detail;
                            grdDocumentDetails.Rows[grdDocumentDetails.CurrentRow.Index - 1].Selected = true;
                            detail = grdDocumentDetails.SelectedRows[0].DataBoundItem as DocumentDetail;
                            if (detail != null) //Update Pole display
                            {
                                string[] customerLines = deviceManager.GetDocumentDetailPoleDisplayLines(detail,false);
                                actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));

                                string[] cashierLines = deviceManager.GetDocumentDetailPoleDisplayLines(detail, true);
                                actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));
                            }
                        }
                        break;
                    case eNavigation.MOVEDOWN:
                        if (grdDocumentDetails.CurrentRow.Index >= 0 && grdDocumentDetails.CurrentRow.Index < (grdDocumentDetails.Rows.Count - 1))
                        {
                            DocumentDetail detail;
                            grdDocumentDetails.Rows[grdDocumentDetails.CurrentRow.Index + 1].Selected = true;
                            detail = grdDocumentDetails.SelectedRows[0].DataBoundItem as DocumentDetail;
                            if (detail != null) //Update Pole display
                            {
                                string[] customerLines = deviceManager.GetDocumentDetailPoleDisplayLines(detail, false);
                                actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(customerLines));

                                string[] cashierLines = deviceManager.GetDocumentDetailPoleDisplayLines(detail, false);
                                actionManager.GetAction(eActions.CASHIER_POLE_DISPLAY_MESSAGE).Execute(new ActionCashierPoleDisplayMessageParams(cashierLines));

                            }
                        }
                        break;
                }
            }
            else if (parameters.DocumentHeader == null)
            {
                
                grdDocumentDetails.DataSource = null;
            }
        }

        private int GetNextRow(int previous, int next, IEnumerable<DocumentDetail> list)
        {
            long motion = next - (long)previous;
            if (motion == 0)
            {
                motion = 1;
            }
            motion /= Math.Abs(motion);
            var l = list.ToList();
            Dictionary<int, DocumentDetail> dict = list.ToDictionary(g => l.IndexOf(g), g => g).Where(x => x.Value.IsCanceled == false && x.Value.IsLinkedLine == false).ToDictionary(i => i.Key, i => i.Value);
            if (dict.Count() == 0)
                return -1;
            int minKey = dict.Min(g => g.Key), maxKey = dict.Max(g => g.Key);
            while (!dict.ContainsKey(next))
            {
                next += (int)motion;
                if (next > maxKey)
                    return previous;
                if (next < minKey)
                    return previous;
            }

            return next;

        }
    }


}
