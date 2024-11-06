using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using DevExpress.Xpo;
using System.Collections.Generic;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using System.Web;

namespace ITS.Retail.ReportsLibrary
{
    public partial class PriceCatalogDetailsReport : DevExpress.XtraReports.UI.XtraReport
    {
        public PriceCatalogDetailsReport()
        {
            InitializeComponent();
        }

        private void PriceCatalogDetailsReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            PriceCatalog priceCatalog = ((XPCollection<PriceCatalog>)DataSource)[0];

            xrLabelPriceCatalogInfo.Text = priceCatalog.Description;

            
            xrTableCellCodeLbl.Text = Resources.Code;
            xrTableCellBarcodelbl.Text = Resources.Barcode;
            xrTableCellDescriptionlbl.Text = Resources.ItemName;
            xrTableCellBuyerlbl.Text = Resources.Buyer;
            xrTableCellItemSupplierlbl.Text = Resources.ItemSupplier;
            xrTableCellInsertedAtlbl.Text = Resources.InsertedDate;
            xrTableCellUpdatedOnlbl.Text = Resources.UpdatedOn;
            xrTableCellSeasonalitylbl.Text = Resources.Seasonality;
            xrTableCellMotherCodelbl.Text = Resources.MotherCode;
            xrTableCellValuelbl.Text = Resources.Value;
            xrTableCellDiscountlbl.Text = Resources.Discount;
            xrTableCellIncludesVatlbl.Text = Resources.VatIncluded;
            CriteriaOperator crop = CriteriaOperator.TryParse(Filter.Value as string);
            IEnumerable<PriceCatalogDetail> details = PriceCatalogHelper.GetTreePriceCatalogDetails(priceCatalog, crop);
                //HttpContext.Current.Session["PriceCatalogDetailViewFilter"] as CriteriaOperator);

            DetailReport.DataSource = details;
            xrTableCell13.DataBindings.Add("Text", DetailReport.DataSource, "Item.Code");
            xrTableCell14.DataBindings.Add("Text", DetailReport.DataSource, "Barcode.Code");
            xrTableCell15.DataBindings.Add("Text", DetailReport.DataSource, "Item.Name");
            xrTableCell16.DataBindings.Add("Text", DetailReport.DataSource, "Item.Buyer.Description");
            xrTableCell17.DataBindings.Add("Text", DetailReport.DataSource, "Item.DefaultSupplier.CompanyName");
            xrTableCell18.DataBindings.Add("Text", DetailReport.DataSource, "Item.InsertedDate", @"{0:dd/MM/yyyy}");
            xrTableCell19.DataBindings.Add("Text", DetailReport.DataSource, "Item.UpdatedOn", @"{0:dd/MM/yyyy}");
            xrTableCell20.DataBindings.Add("Text", DetailReport.DataSource, "Item.Seasonality.Description");
            xrTableCell21.DataBindings.Add("Text", DetailReport.DataSource, "Item.MotherCode.Code");
            xrTableCell22.DataBindings.Add("Text", DetailReport.DataSource, "Value","{0:C2}");
            xrTableCell23.DataBindings.Add("Text", DetailReport.DataSource, "Discount","{0:p}");
            xrTableCell24.DataBindings.Add("Text", DetailReport.DataSource, "VATIncluded");
            
        }

    }
}
