using System;
using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.XtraPrinting;
using System.Web;
using System.Linq;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Reports
{
    public partial class CustomerDocumentReport : DevExpress.XtraReports.UI.XtraReport
    {
        public CustomerDocumentReport()
        {
            InitializeComponent();
        }

        private void DocumentReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                pictureBoxLogo.Image = Image.FromFile(HttpContext.Current.Server.MapPath("../Content/small_logo.png"));
            }
            catch (Exception)
            {
                //using default image
            }
            DocumentHeader documentHeader = ((XPCollection<DocumentHeader>)DataSource)[0];
            xrLabelTraderCompanyName.Text = documentHeader.Store.Owner.CompanyName ?? "";
            xrLabelTraderProfession.Text = documentHeader.Store.Owner.Profession ?? "";
            xrLabelTraderAddress.Text = Resources.Address.ToUpper() + ":" + (documentHeader.Store.Address.Street ?? "") + ", " + Resources.City.ToUpper() + ":" + (documentHeader.Store.Address.City ?? "") + "," + Resources.PostCode.ToUpper() + ":" + (documentHeader.Store.Address.PostCode ?? "");
            if (documentHeader.Store.Address.DefaultPhone == null)
            {
                try
                {
                    xrLabelTraderPhone.Text = Resources.Phone.ToUpper() + ":" + (documentHeader.Store.Address.Phones[0].PhoneType.Description ?? "") + " - " + (documentHeader.Store.Address.Phones[0].Number ?? "");
                }
                catch 
                {
                    xrLabelTraderPhone.Text = "";
                }
            }
            else
            {
                xrLabelTraderPhone.Text = Resources.Phone.ToUpper() + ":" + (documentHeader.Store.Address.DefaultPhone.PhoneType.Description ?? "") + ": " + (documentHeader.Store.Address.DefaultPhone.Number ?? "");
            }

            string OwnerTaxOfficeDescription = documentHeader.Store.Owner.Trader.TaxOfficeLookUp != null ?
                                               documentHeader.Store.Owner.Trader.TaxOfficeLookUp.Description : "";

            Trader trader;
            if (documentHeader.Customer != null)
            {
                trader = documentHeader.Customer.Trader;
            }
            else if (documentHeader.Supplier != null)
            {
                trader = documentHeader.Supplier.Trader;
            }
            else
            {
                trader = documentHeader.Owner.Trader;
            }


            string TraderTaxOfficeDescription = trader.TaxOfficeLookUp != null ? trader.TaxOfficeLookUp.Description : "";

            xrLabelTaxCodeOffice.Text = 
                Resources.TaxCode.ToUpper() + ":" + (documentHeader.Store.Owner.Trader.TaxCode ?? "") + ", " + Resources.TaxOffice.ToUpper() + ":" + OwnerTaxOfficeDescription;
            xrLabelDocumentType.Text = documentHeader.DocumentType.Description ?? "";
            xrLabelDocumentSeries.Text = documentHeader.DocumentSeries.Description ?? "";
            xrLabelDocumentStatus.Text = documentHeader.Status.Description ?? "";
            xrLabelDocumentNumber.Text = documentHeader.DocumentNumber.ToString() ?? "";
            xrLabelDocumentDate.Text = documentHeader.FinalizedDate.ToShortDateString() ?? "";

            xrLabelDocumentDate2.Text = documentHeader.FinalizedDate.ToShortDateString() ?? "";
            xrLabelCustCompanyName.Text = xrLabelCustCompanyName2.Text = documentHeader.Customer != null ? documentHeader.Customer.CompanyName : "";
            xrLabelDocumentNumber2.Text = documentHeader.DocumentNumber.ToString() ?? "";


            if (documentHeader.Customer == null || documentHeader.Customer.PaymentMethod == null)
            {
                xrLabelDocumentPayment.Text = "";
            }
            else
            {
                xrLabelDocumentPayment.Text = documentHeader.Customer.PaymentMethod.Description ?? "";
            }

            xrLabelCustCode.Text = documentHeader.Customer.Code ?? "";
            xrLabelCustProfession.Text = documentHeader.Customer.Profession ?? "";
            if (documentHeader.Customer.DefaultAddress == null)
            {
                try
                {
                    xrLabelCustAddress.Text = documentHeader.Customer.Trader.Addresses[0].Street ?? "";
                    xrLabelCustCityPOCode.Text = (documentHeader.Customer.Trader.Addresses[0].City ?? "") + "/" + (documentHeader.Customer.Trader.Addresses[0].PostCode ?? "");
                    if (documentHeader.Customer.Trader.Addresses[0].DefaultPhone == null)
                    {
                        try
                        {
                            xrLabelCustPhoneNumber.Text = documentHeader.Customer.Trader.Addresses[0].Phones[0].Number ?? "";
                        }
                        catch
                        {
                            xrLabelCustPhoneNumber.Text = "";
                        }

                    }
                    else
                    {
                        xrLabelCustPhoneNumber.Text = documentHeader.Customer.Trader.Addresses[0].DefaultPhone.Number ?? "";
                    }
                }
                catch 
                {
                    xrLabelCustAddress.Text = "";
                    xrLabelCustCityPOCode.Text = "";
                }
            }
            else
            {
                xrLabelCustAddress.Text = documentHeader.Customer.DefaultAddress.Street ?? "";
                xrLabelCustCityPOCode.Text = (documentHeader.Customer.DefaultAddress.City ?? "") + " " + (documentHeader.Customer.DefaultAddress.PostCode ?? "");
                if (documentHeader.Customer.DefaultAddress.DefaultPhone == null)
                {
                    try
                    {
                        xrLabelCustPhoneNumber.Text = documentHeader.Customer.DefaultAddress.Phones[0].Number ?? "";
                    }
                    catch
                    {
                        xrLabelCustPhoneNumber.Text = "";
                    }

                }
                else
                {
                    xrLabelCustPhoneNumber.Text = documentHeader.Customer.DefaultAddress.DefaultPhone.Number ?? "";
                }
            }
            xrLabelCustTaxCode.Text = trader.TaxCode ?? "";
            xrLabelCustTaxOffice.Text = TraderTaxOfficeDescription;
            xrLabelDocumentDeliveryAddress.Text = documentHeader.DeliveryAddress ?? "";
            xrLabelDocNetTotal.Text = String.Format("{0:c2}", BusinessLogic.RoundAndStringify(documentHeader.NetTotal,documentHeader.Owner)) + " €";
            xrLabelDocTotalDiscAmount.Text = String.Format("{0:C2}", BusinessLogic.RoundAndStringify(documentHeader.TotalDiscountAmount, documentHeader.Owner)) + " €";
            xrLabelDocNetPlusDisc.Text = String.Format("{0:C2}", BusinessLogic.RoundAndStringify(documentHeader.NetTotal - documentHeader.TotalDiscountAmount, documentHeader.Owner)) + " €";
            xrLabelDocVatAmount.Text = String.Format("{0:C2}", BusinessLogic.RoundAndStringify(documentHeader.TotalVatAmount, documentHeader.Owner)) + " €";
            xrLabelDocGrossTotal.Text = String.Format("{0:C2}", BusinessLogic.RoundAndStringify(documentHeader.GrossTotal, documentHeader.Owner)) + " €";
            xrLabelDocQtySum.Text = getQtySum(documentHeader).ToString();
            xrLabelDocRemarks.Text = Resources.Comments.ToUpper() + ": " + (documentHeader.Remarks == "null" || documentHeader.Remarks == null ? "" : documentHeader.Remarks);
            xrLabelSignature.Text = documentHeader.Signature;

            var customizedDetails = from detail in ((XPCollection<DocumentHeader>)DataSource)[0].DocumentDetails
                                    select new
                                    {
                                        itemCategory = detail.Item.ItemAnalyticTrees.FirstOrDefault() != null ?
                                                                       (detail.Item.ItemAnalyticTrees.FirstOrDefault().Node != null ?
                                                                       ((detail.Item.ItemAnalyticTrees.FirstOrDefault().Node.Code ?? "") + " " + (detail.Item.ItemAnalyticTrees.FirstOrDefault().Node.Description ?? ""))
                                                                       : "")
                                                                       : "",
                                        itemCode = documentHeader.Owner.OwnerApplicationSettings.PadItemCodes ? detail.Item.Code.TrimStart(documentHeader.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]) : detail.Item.Code,
                                        itemCodeForSorting = detail.Item.Code,
                                        barcodeCode = detail.Barcode.Code,
                                        itemName = detail.Item.Name,
                                        measurmentUnit = detail.Barcode.MeasurementUnit(documentHeader.Owner) != null ?
                                                                     (detail.Barcode.MeasurementUnit(documentHeader.Owner).Description ?? "")
                                                                       : "",
                                        qty = detail.Qty,
                                        unitPrice = detail.UnitPrice,
                                        totalDiscount = detail.TotalDiscount,
                                        grossTotal = detail.GrossTotal,
                                        vatFactor = detail.VatFactor,

                                    };
            
            DetailReport.DataSource = customizedDetails.OrderBy(x => x.itemCodeForSorting).OrderBy(y => y.itemCategory);

            xrTableCellCode.DataBindings.Add("Text", DetailReport.DataSource, "itemCode");
            xrTableCellBarcode.DataBindings.Add("Text", DetailReport.DataSource, "barcodeCode");
            xrTableCellItemName.DataBindings.Add("Text", DetailReport.DataSource, "itemName");
            xrTableCellBarcodeMeasurementUnit.DataBindings.Add("Text", DetailReport.DataSource, "measurmentUnit");
            xrTableCellQty.DataBindings.Add("Text", DetailReport.DataSource, "qty", "{0:0.00}");
            xrTableCellUnitPrice.DataBindings.Add("Text", DetailReport.DataSource, "unitPrice", "{0:N2}");
            xrTableCellTotalDiscount.DataBindings.Add("Text", DetailReport.DataSource, "totalDiscount", "{0:N2}");
            xrTableCellGrossTotal.DataBindings.Add("Text", DetailReport.DataSource, "grossTotal", "{0:N2}");
            xrTableCellVatFactor.DataBindings.Add("Text", DetailReport.DataSource, "vatFactor", "{0:0.0%}");

            //Resources
            //---------------------------------------
            xrLabelStaticDocumentType.Text = Resources.DocumentType.ToUpper() + ":";
            xrLabelStaticDocumentSeries.Text = Resources.DocumentSeries.ToUpper() + ":";
            xrLabelStaticDocumentNumber.Text = Resources.DocumentNumber.ToUpper() + ":";
            xrLabelStaticDocumentStatus.Text = Resources.DocumentStatus.ToUpper() + ":";
            xrLabelStaticDocumentDate.Text = Resources.Date.ToUpper() + ":";
            xrLabelStaticDocumentPaymentMethods.Text = Resources.PaymentMethods.ToUpper() + ":";
            xrLabelStaticDocumentDeliveryAddress.Text = Resources.DeliveryAddress.ToUpper() + ":";
            xrLabelStaticCustomerInfo.Text = Resources.CustomerInfo.ToUpper() + ":";
            xrLabelStaticCustomerCode.Text = Resources.CustomerCode.ToUpper() + ":";
            xrLabelStaticCustomerCompanyName.Text = Resources.CompanyName.ToUpper() + ":";
            xrLabelStaticCustomerProfession.Text = Resources.Profession.ToUpper() + ":";
            xrLabelStaticCustomerAddress.Text = Resources.Address.ToUpper() + ":";
            xrLabelStaticCustomerCityPOCode.Text = Resources.City.ToUpper() + "/" +
                                                   Resources.PostCode.ToUpper() + ":";
            xrLabelStaticCustomerPhone.Text = Resources.Phone.ToUpper() + ":";
            xrLabelStaticCustomerTaxCode.Text = Resources.TaxCode.ToUpper() + ":";
            xrLabelStaticCustomerTaxOffice.Text = Resources.TaxOffice.ToUpper() + ":";
            xrTableCellStaticItemCode.Text = Resources.ItemCode;
            xrTableCellStaticBarcodeCode.Text = Resources.Barcode;
            xrTableCellStaticItemName.Text = Resources.ItemDescription;
            xrTableCellStaticMesurmentUnit.Text = Resources.MesurmentUnit;
            xrTableCellStaticQty.Text = Resources.Quantity;
            xrTableCellStaticUnitPrice.Text = Resources.UnitPrice;
            xrTableCellTotalDiscount.Text = Resources.DiscountShort;
            xrTableCellStaticGrossTotal.Text = Resources.Value;
            xrTableCellStaticVatTotal.Text = Resources.VAT;
            xrLabelStaticDocumentQtySum.Text = Resources.TotalQuantity.ToUpper() + ":";
            xrLabelStaticDocumentTerms.Text = Resources.Terms.ToUpper() + ":";
            xrLabelStaticDocNetTotal.Text = Resources.TotalValue.ToUpper() + ":";
            xrLabelStaticDocTotalDiscAmount.Text = Resources.Discount.ToUpper() + ":";
            xrLabelStaticDocNetPlusDisc.Text = Resources.TotalNetValue.ToUpper() + ":";
            xrLabelStaticDocVatAmount.Text = Resources.VAT.ToUpper() + ":";
            xrLabelStaticDocGrossTotal.Text = Resources.Payable.ToUpperInvariant() + ":";
            xrPageInfo1.Format = Resources.Page+" {0} "+Resources.From+" {1}";

        }

        decimal getQtySum(DocumentHeader documentHeader)
        {
            decimal qtySum = 0;
            foreach (DocumentDetail dd in documentHeader.DocumentDetails)
            {
                qtySum += dd.Qty;
            }
            return qtySum;
        }

        private void xrPanel1_Draw(object sender, DrawEventArgs e)
        {

        }

        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }

        private void PageFooter_AfterPrint(object sender, EventArgs e)
        {

        }

        private void DocumentReport_PrintProgress(object sender, PrintProgressEventArgs e)
        {
        }

    }
}
