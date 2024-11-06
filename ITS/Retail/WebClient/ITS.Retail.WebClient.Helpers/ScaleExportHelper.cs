using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ScaleExportHelper
    {
        public static bool Export(IDictionary<ItemBarcode, PriceCatalogPolicyPriceResult> listToExport, string format, string fileName, Encoding encode, out string message, Store CurrentStore)
        {
            message = "";
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, true, encode))
                {
                    foreach (KeyValuePair<ItemBarcode, PriceCatalogPolicyPriceResult> pair in listToExport)
                    {
                        try
                        {
                                string output = string.Format(new PaddedStringFormatInfo(), format,
                                pair.Key.Item.Code,  //FK
                                pair.Key.Item.Name + (pair.Key.Item.ItemExtraInfos.FirstOrDefault(x => x.Store.Oid == CurrentStore.Oid) ==null?"":" "+ pair.Key.Item.ItemExtraInfos.FirstOrDefault(x => x.Store.Oid == CurrentStore.Oid).Description),  //Description
                                pair.Key.PluCode,   //PLU
                                pair.Key.PluPrefix, //PLU Prefix
                                pair.Value == null || pair.Value.PriceCatalogDetail == null ? 0 : pair.Value.PriceCatalogDetail.Value,      //Value
                                ((pair.Key.IsActive && pair.Key.Item.IsActive && pair.Value != null) ? 0 : 1), //Active
                               (pair.Key.MeasurementUnit != null && pair.Key.MeasurementUnit.SupportDecimal) ? 0 : 1 // Weighted
                                ); 
                            writer.WriteLine(output);
                        }
                        catch(Exception ee)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                message = "File creation failed." + ex.Message;
                return false;
            }

            return true;
        }

        public static bool ExportSQL(string format, string fileName, Encoding encode, string query, out string message, Dictionary<ItemBarcode, PriceCatalogPolicyPriceResult> pcds)
        {
            message = "";
            try
            {
                string pcdoids = pcds.Where(x=> x.Value != null && x.Value.PriceCatalogDetail != null).Select(x => x.Value.PriceCatalogDetail.Oid).Distinct().Select(x => "'" + x.ToString() + "'").Aggregate((f, s) => f + "," + s);
                string finalQuery = string.Format("select * from ({0}) a where a.pcdoid in ({1})", query, pcdoids);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    SelectedData result = uow.ExecuteQuery(finalQuery);
                    if (result.ResultSet[0].Rows.Length > 0)
                    {
                        using (StreamWriter writer = new StreamWriter(fileName, true, encode))
                        {
                            foreach (var row in result.ResultSet[0].Rows)
                            {                               
                                try
                                {                                    
                                    writer.WriteLine(String.Format(new PaddedStringFormatInfo(), format, row.Values));
                                }
                                catch (Exception ee)
                                {
                                    string errorMessage = ee.GetFullMessage();
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                message = "File creation failed." + ex.Message;
                return false;
            }
            return true;
        }
    }
}
