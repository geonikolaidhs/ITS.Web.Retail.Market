using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Model.Transactions;
using DevExpress.XtraGrid.Views.Grid;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Settings;
using DevExpress.Data.Filtering;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using DevExpress.XtraGrid;
using System.Windows.Forms.Design;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// The original document details grid. Abandoned and not recommended for usage
    /// </summary>
    [Obsolete]
    public partial class ucDocumentDetailsGrid : ucObserver, IObserverGrid
	{
        Type[] paramsTypes = new Type[] { typeof(GridParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }


		public ucDocumentDetailsGrid()
		{
			InitializeComponent();

            this.gridColumn1.Caption = POSClientResources.CODE;//"Κωδ.";
            this.gridColumn2.Caption = POSClientResources.DESCRIPTIONS;//"Περιγραφή";
            this.gridColumn4.Caption = POSClientResources.QTY;//"Ποσ.";
            this.gridColumn6.Caption = POSClientResources.DISCOUNT;//"Εκπτ.";
            this.gridColumn3.Caption = POSClientResources.VALUE;//"Αξία";
            this.gridColumnPaymentMethod.Caption = POSClientResources.PAYMENT_METHOD;//"Τρόπος Πληρωμής";
            this.gridColumnAmount.Caption = POSClientResources.AMOUNT;//"Ποσό";

            this.ActionsToObserve.Add(eActions.MOVE_UP);
            this.ActionsToObserve.Add(eActions.MOVE_DOWN);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO);

            this.gridViewDocumentDetails.RowStyle += gridViewDocumentDetails_RowStyle;
            
		}

        void gridViewDocumentDetails_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (this.HideDeletedLines)
                return;
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                if (((DocumentDetail)View.GetRow(e.RowHandle)).IsCanceled)
                {
                    Font fnt = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Strikeout);
                    e.Appearance.Font = fnt;
                }
            }
        }

        public bool HideDeletedLines { get; set; }

        [Browsable(false)] 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] //DO NOT DELETE FOR BACKWARDS COMPARTIBILITY
        public Color GridBackColor
        {
            get
            {
                return gridViewDocumentDetails.Appearance.Row.BackColor;
            }
            set
            {
                gridViewDocumentDetails.Appearance.Row.BackColor = value;
                gridViewDocumentDetails.Appearance.Empty.BackColor = value;
                gridViewDocumentTotals.Appearance.Row.BackColor = value;
                gridViewDocumentTotals.Appearance.Empty.BackColor = value;
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
			if (parameters.DocumentHeader!=null)
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
                    
                    grdDocumentDetails.MainView = gridViewDocumentDetails;
				}
				else if (parameters.DisplayMode == eDisplayMode.PAYMENTS)
				{
					grdDocumentDetails.DataSource = parameters.DocumentHeader.DocumentPayments;
					grdDocumentDetails.MainView = gridViewDocumentTotals;
				}
			}
			else if (parameters.SelectedDocumentDetail!=null)
			{
                int newPosition = gridViewDocumentDetails.LocateByValue("Oid", parameters.SelectedDocumentDetail.Oid);
                if (gridViewDocumentDetails.FocusedRowHandle == newPosition && newPosition !=0)
                {
                    //Same position, do nothing
                    return;
                }
                gridViewDocumentDetails.FocusedRowHandle = newPosition;
                //if (parameters.SelectedDocumentDetail.CustomDescription != null)
                //{
                //    string[] lines = new String[] { parameters.SelectedDocumentDetail.CustomDescription.ToUpperGR(), parameters.SelectedDocumentDetail.Qty + " X " + String.Format("{0:C}", parameters.SelectedDocumentDetail.FinalUnitPriceWithVatBeforeDocumentDiscount)/*parameters.SelectedDocumentDetail.FinalUnitPriceWithVat + " €"*/};
                //    GlobalContext.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                //}
			}
			else if (parameters.SelectedDocumentPayment!= null)
			{
                int newPosition = gridViewDocumentTotals.LocateByValue("Oid", parameters.SelectedDocumentPayment.Oid);
                if (gridViewDocumentTotals.FocusedRowHandle == newPosition)
                {
                    //Same position, do nothing
                    return;
                }
                gridViewDocumentTotals.FocusedRowHandle = newPosition;
			}
			else if (parameters.Navigation != null)
			{
				GridView view = (grdDocumentDetails.MainView as GridView);
				switch (parameters.Navigation)
				{
					case eNavigation.MOVEUP:
						if (view.FocusedRowHandle > 0)
						{
							DocumentDetail detail;                            
                            view.FocusedRowHandle--;
                            detail = view.GetFocusedRow() as DocumentDetail;
                            //if (detail != null) //Update Pole display
                            //{
                            //    string[] lines = new String[] { detail.CustomDescription.ToUpperGR(), detail.Qty + " X " + String.Format("{0:C}", detail.FinalUnitPriceWithVatBeforeDocumentDiscount)/*detail.FinalUnitPriceWithVat + " €" */};
                            //    GlobalContext.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                            //}
						}
						break;
					case eNavigation.MOVEDOWN:
						if (view.FocusedRowHandle >=0 && view.FocusedRowHandle < (view.RowCount - 1))
						{
                            DocumentDetail detail;
                            view.FocusedRowHandle++;
                            detail = view.GetFocusedRow() as DocumentDetail;
                            //if (detail != null) //Update Pole display
                            //{
                            //    string[] lines = new String[] { detail.CustomDescription.ToUpperGR(), detail.Qty + " X " + String.Format("{0:C}", detail.FinalUnitPriceWithVatBeforeDocumentDiscount)/*detail.FinalUnitPriceWithVat + " €" */};
                            //    GlobalContext.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                            //}
						}
						break;
				}
			}
			else if (parameters.DocumentHeader == null)
			{
				grdDocumentDetails.DataSource = null;
                grdDocumentDetails.MainView = gridViewDocumentDetails;
			}
		}


		private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            DocumentDetail row = gridViewDocumentDetails.GetRow(e.FocusedRowHandle) as DocumentDetail;
			if (row != null)
			{
                if (row.IsCanceled || row.IsLinkedLine)
                {
                    int getNextRow = GridHelper.GetNextRow(e.PrevFocusedRowHandle, e.FocusedRowHandle, gridViewDocumentDetails.DataSource as IEnumerable<DocumentDetail>);

                    if (getNextRow >= 0)
                    {
                        ((GridView)sender).FocusedRowHandle = getNextRow;
                    }

                }
                else
                {
                    //GlobalContext.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(row as DocumentDetail,true,false));
                    appContext.CurrentDocumentLine = (row.IsCanceled) ? null : row;
                }
			}
		}

        private void gridViewDocumentTotals_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            object row = gridViewDocumentTotals.GetRow(e.FocusedRowHandle);
            if (row != null && row is DocumentPayment)
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                actionManager.GetAction(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO).Execute(new ActionPublishDocumentPaymentInfoParams(row as DocumentPayment,appContext.CurrentDocument,true,false));
                appContext.CurrentDocumentPayment = row as DocumentPayment;
            }
        }

        private void gridViewDocumentDetails_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName == "")
            //    e.DisplayText = e.RowHandle.ToString();

        }

        private void gridViewDocumentDetails_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle+1).ToString();
            }
        }
    }
}
