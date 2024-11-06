using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Web.Mvc;
using System.Web.Mvc;
using ITS.Retail.Model;
using System.Globalization;
using DevExpress.Data.Filtering;

namespace ITS.Retail.WebClient.Helpers
{
	public static class GridExportHelper
	{
		public delegate ActionResult ExportMethod(GridViewSettings settings, object dataObject);

		public class ExportType
		{
			public string Title { get; set; }
			public ExportMethod Method { get; set; }
		}

		public enum GridViewExportType { None, Pdf, Xls, Xlsx, Rtf, Csv }

		static Dictionary<string, ExportType> exportTypes;
		public static Dictionary<string, ExportType> ExportTypes
		{
			get
			{
				if (exportTypes == null)
					exportTypes = CreateExportTypes();
				return exportTypes;
			}
		}

		static Dictionary<string, ExportType> CreateExportTypes()
		{
			Dictionary<string, ExportType> types = new Dictionary<string, ExportType>();
			types.Add("PDF", new ExportType { Title = "Export to PDF", Method = GridViewExtension.ExportToPdf });
			types.Add("XLS", new ExportType { Title = "Export to XLS", Method = GridViewExtension.ExportToXls });
			types.Add("XLSX", new ExportType { Title = "Export to XLSX", Method = GridViewExtension.ExportToXlsx });
			types.Add("RTF", new ExportType { Title = "Export to RTF", Method = GridViewExtension.ExportToRtf });
			types.Add("CSV", new ExportType { Title = "Export to CSV", Method = GridViewExtension.ExportToCsv });
			return types;
		}


        public static List<string> GetLabelExportLines(IEnumerable<PriceCatalogDetail> priceCatalogDetails, string type, CompanyNew owner)
        {
            NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." };
            List<string> lines = new List<string>();

            VatLevel vatLevel = StoreControllerAppiSettings.CurrentStore.Address.VatLevel;
            if(vatLevel==null)
            {
                //HttpContenxt. Session["Error"] = ResourcesLib.Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                return null;
            }

            foreach (PriceCatalogDetail detail in priceCatalogDetails)
            {                
                VatCategory vatCategory = detail.Item.VatCategory;
                VatFactor vatFactor = detail.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatLevel.Oid", vatLevel.Oid), new BinaryOperator("VatCategory.Oid", vatCategory.Oid)));
                MeasurementUnit mm = detail.Barcode.MeasurementUnit(owner);
                string mmDescription = mm == null ? "" : mm.Description;
                decimal priceWithoutVat = detail.VATIncluded ? (detail.Value / (1 + vatFactor.Factor)) : detail.Value;
                decimal priceWithVat = detail.VATIncluded == false ? (detail.Value * (1 + vatFactor.Factor)) : detail.Value;

                switch (type)
                {
                    case "Pricer":
                        string line = String.Format(nfi, "{0};{1};{2};{3};{4:0.00};{5:0.00};{6:0%};{7};{8};{9:dd/MM/yyyy};{10:dd/MM/yyyy}",
                       detail.Item.Code, detail.Barcode.Code, detail.Item.Name.Replace(';', '?'), mmDescription.Replace(';', '?'), priceWithoutVat,
                       priceWithVat, vatFactor.Factor, detail.PriceCatalog.Code, detail.PriceCatalog.Description.Replace(';', '?'), detail.ValueChangedOnDate, detail.UpdatedOn);
                        lines.Add(line);
                        break;
                    case "Libra":
                        //TODO
                        break;
                }
            }
            return lines;
        }
	}
}
