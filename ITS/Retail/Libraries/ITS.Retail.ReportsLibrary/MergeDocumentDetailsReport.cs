using ITS.Retail.ResourcesLib;

namespace ITS.Retail.ReportsLibrary
{
    public partial class MergeDocumentDetailsReport : DevExpress.XtraReports.UI.XtraReport
    {
        public MergeDocumentDetailsReport()
        {
            InitializeComponent();
        }

        private void OffersReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            lblDescription.Text = Resources.Description;
            lblBarcodeCode.Text = Resources.Barcode;
            lblItemCode.Text = Resources.ItemCode;
            lblLinkedItem.Text = Resources.LinkedItem;
            lblQty.Text = Resources.SummedQty;



            #region set Offer table labels
            xrTableCellCodeLbl.Text = Resources.Code;
            xrTableCellDescriptionlbl.Text = Resources.Description;
            xrTableCellDescription2lbl.Text = Resources.Description2;
            xrTableCellStartDatelbl.Text = Resources.StartDate;
            xrTableCellEndDatelbl.Text = Resources.EndDate;
            #endregion

            #region set Offer table data
            xrTableCellCode.DataBindings.Add("Text", DataSource, "Code");
            xrTableCellDescription.DataBindings.Add("Text", DataSource, "Description");
            xrTableCellDescription2.DataBindings.Add("Text", DataSource, "Description2");
            xrTableCellStartDate.DataBindings.Add("Text", DataSource, "StartDate", @"{0:dd/MM/yyyy}");
            xrTableCellEndDate.DataBindings.Add("Text", DataSource, "EndDate", @"{0:dd/MM/yyyy}");
            #endregion

            #region set Offer Details labels
            xrTableCellItemCodelbl.Text = Resources.Code;
            xrTableCellItemDesclbl.Text = Resources.ItemName;
            #endregion

            #region set Offer Detail Data

            #endregion
        }

        private void BottomMargin_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

    }
}
