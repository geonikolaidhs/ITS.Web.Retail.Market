using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.ReportsLibrary
{
    public partial class OffersReport : DevExpress.XtraReports.UI.XtraReport
    {
        public OffersReport()
        {
            InitializeComponent();
        }

        private void OffersReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DetailReport.DataMember = "OfferDetails";
            xrLabelOfferInfo.DataBindings.Add("Text",DataSource,"Description");

            
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
            xrTableCellStartDate.DataBindings.Add("Text", DataSource, "StartDate",@"{0:dd/MM/yyyy}");
            xrTableCellEndDate.DataBindings.Add("Text", DataSource, "EndDate",@"{0:dd/MM/yyyy}");
            #endregion
            
            #region set Offer Details labels
            xrTableCellItemCodelbl.Text = Resources.Code;
            xrTableCellItemDesclbl.Text = Resources.ItemName;
            #endregion

            #region set Offer Detail Data
            xrTableCellItemCode.DataBindings.Add("Text", null, "OfferDetails.Item.Code");
            xrTableCellItemDescription.DataBindings.Add("Text", null, "OfferDetails.Item.Name");
            #endregion
        }

        private void BottomMargin_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

    }
}
