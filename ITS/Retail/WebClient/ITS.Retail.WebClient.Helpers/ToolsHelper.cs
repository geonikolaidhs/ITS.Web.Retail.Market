using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Shell32;
using System.IO;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;
using System.Text;
using ITS.Retail.Common;

namespace ITS.Retail.WebClient.Helpers
{
    public class ToolsHelper
    {
        public static bool ClearFolder(string Dir)
        {
            if (!Directory.Exists(Dir))
            {               
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(Dir);
            foreach (FileInfo f in dir.GetFiles("*.*"))
            {
                File.Delete(Dir+f);
            }
            return true;
        }


        private static string GetLabelsDirectSQL(DBType? dbtype)
        {
            DBType databaseType = dbtype.HasValue ? dbtype.Value : XpoHelper.databasetype;
            switch (databaseType)
            {
                case DBType.SQLServer:
                    return @"Select intable.*, intable.barcode from ({0}) as intable                                
                                where PCDOID in ({1})                                
                                ORDER BY CHARINDEX(','+CONVERT(varchar(36), intable.PCDOID)+',', ',{2},')";
                case DBType.postgres:
                    return @"Select intable.*, intable.barcode from ({0}) as intable                                
                                where PCDOID in ({1})                                
                                ORDER BY strpos(',{2},',',' || (intable.PCDOID::varchar) ||',')";
                default:
                    return @"Select intable.*, intable.barcode from ({0}) as intable                                
                                where PCDOID in ({1})                                
                                ORDER BY CHARINDEX(','+CONVERT(varchar(36), intable.PCDOID)+',', ',{2},')";
            }
        }

        private static string GetDocumentLabelsDirectSQL(DBType? dbtype)
        {
            DBType databaseType = dbtype.HasValue ? dbtype.Value : XpoHelper.databasetype;
            switch (databaseType)
            {
                case DBType.SQLServer:
                    return @"Select intable.*, b.Code from ({0}) as intable 
                                left join DocumentDetail det on det.Item = intable.ItemOid
                                left join Barcode b on det.Barcode = b.Oid
                                where PCDOID in ({1}) 
                                and det.Oid in ({3}) 
                                ORDER BY CHARINDEX(','+CONVERT(varchar(36), intable.PCDOID)+',', ',{2},')";
                case DBType.postgres:
                    return @"Select intable.*, b.""Code"" from ({0}) as intable 
                                left join ""DocumentDetail"" det on det.""Item"" = intable.ItemOid
                                left join ""Barcode"" b on det.""Barcode"" = b.""Oid""
                                where PCDOID in ({1}) 
                                and det.""Oid"" in ({3}) 
                                ORDER BY strpos(',{2},',',' || (intable.PCDOID::varchar) ||',')";
                default:
                    return @"Select intable.*, b.Code from ({0}) as intable 
                                left join DocumentDetail det on det.Item = intable.ItemOid
                                left join Barcode b on det.Barcode = b.Oid
                                where PCDOID in ({1}) 
                                and det.Oid in ({3}) 
                                ORDER BY CHARINDEX(','+CONVERT(varchar(36), intable.PCDOID)+',', ',{2},')";
            }
        }

        public static string GetLabelsStringToPrint(Store CurrentStore, string type, IEnumerable<PriceCatalogDetail> priceCatalogDetails, string[] dets, int parcedCopies, string output,
            string ejf, Label lbl, UnitOfWork uow,/* PriceCatalog pricecatalog,*/ int bytesOutputEncoding, List<LeafletDetail> leafletDetails, out string message, out byte[] outBytes, DBType? dbType=null )
        {
            message = String.Empty;
            output = "";

            if (lbl.UseDirectSQL)
            {
                IEnumerable<Guid> selectedItems = priceCatalogDetails.Select(pcd => pcd.Oid);

                string query;
                string dbQuery;
                if (dets == null || dets.Length == 0)
                {
                    dbQuery = GetLabelsDirectSQL(dbType);
                    query = String.Format(dbQuery,
                                          lbl.DirectSQL,
                                          selectedItems.Select(x => String.Format("'{0}'", x)).Aggregate((f, s) => f + "," + s),
                                          selectedItems.Select(x => String.Format("{0}", x)).Aggregate((f, s) => f + "," + s)
                                         );
                }
                else
                {
                    dbQuery = GetDocumentLabelsDirectSQL(dbType);
                    query = String.Format(dbQuery,
                                          lbl.DirectSQL, selectedItems.Select(x => String.Format("'{0}'", x)).Aggregate((f, s) => f + "," + s),
                                          selectedItems.Select(x => String.Format("{0}", x)).Aggregate((f, s) => f + "," + s),
                                          dets.Select(x => String.Format("'{0}'", x)).Aggregate((f, s) => f + "," + s)
                                         );
                }
                SelectedData result = uow.ExecuteQuery(query);

                for (int i = 0; i < parcedCopies; i++)
                {
                    foreach (var row in result.ResultSet[0].Rows)
                    {
                        if (row.XmlValues.Contains("sE"))
                        { }
                        output += String.Format(ejf, row.Values);
                    }
                }
            }
            else if (leafletDetails.Count >= 1)
            {
                foreach (LeafletDetail leafletDetail in leafletDetails)
                {
                    ItemBarcode itembar = null;
                    if (leafletDetail != null)
                    {
                        if (itembar == null)
                        {
                            itembar = uow.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Oid", leafletDetail.Barcode.Oid),
                                                                                       new BinaryOperator("Owner", leafletDetail.Item.Owner.Oid)));
                        }
                        for (int i = 0; i < parcedCopies; i++)
                        {
                            //string value = BusinessLogic.RoundAndStringify(pcd.RetailValue, pcd.PriceCatalog.Owner);
                            LinkedItem linkeditemConnection = leafletDetail.Item.LinkedItems.FirstOrDefault();
                            Item linkedItem = linkeditemConnection == null ? null : linkeditemConnection.SubItem;
                            double factor = linkeditemConnection == null ? 0 : linkeditemConnection.QtyFactor;

                            PriceCatalog priceCatalog = uow.FindObject<PriceCatalog>(CriteriaOperator.And(new BinaryOperator("Owner.Oid", CurrentStore),
                                                             new BinaryOperator("IsActive", true)));
                            decimal linkval = (linkedItem != null) ? ItemHelper.GetItemPrice(priceCatalog , linkedItem).RetailValue : 0;
                            string descr = leafletDetail.Item != null ? leafletDetail.Item.Name.Replace("\"", "'") : "";
                            if (leafletDetail.Item.ItemExtraInfos.Count > 0)
                            {
                                descr += " " + leafletDetail.Item.ItemExtraInfos.FirstOrDefault().Description.Replace("\"", "'");
                            }
                            int descrLength = descr.Length;
                            string descr1 = (descrLength > 20) ? descr.Substring(0, 20) : descr;
                            string descr2 = (descrLength > 20) ? descr.Substring(20) : "";
                            decimal UnitValue = leafletDetail.Item.ReferenceUnit > 0 ? (leafletDetail.Item.ContentUnit / leafletDetail.Item.ReferenceUnit) * leafletDetail.Value : 0;
                            
                            Leaflet leaflet = leafletDetail.Leaflet;
                            
                            List<object> arguments = new List<object>
                                        {
                                            descr1,
                                            descr2,
                                            leafletDetail.Item == null ? "" : leafletDetail.Item.Code,
                                            leafletDetail.Barcode == null ? "" : leafletDetail.Barcode.Code,
                                            leaflet.StartTime,
                                            leafletDetail.Item == null || leafletDetail.Item.VatCategory == null ? "" : leafletDetail.Item.VatCategory.Description,
                                            itembar == null || itembar.MeasurementUnit == null ? "": itembar.MeasurementUnit.Description,
                                            leafletDetail.Value,
                                            linkval == 0 ? (object)"" : linkval,
                                            leafletDetail.Item == null || leafletDetail.Item.DefaultSupplier == null ? "" : leafletDetail.Item.DefaultSupplier.Code,
                                            leafletDetail.Item == null ? (object)"" : leafletDetail.Item.ReferenceUnit,
                                            leafletDetail.Item == null ? (object)"" : leafletDetail.Item.ContentUnit,
                                            UnitValue,
                                            leafletDetail.Value + (linkval * (decimal)factor)
                                        };
                            output += String.Format(ejf, arguments.ToArray());
                        }
                    }
                }
            }
            else
            {
                foreach (PriceCatalogDetail pcd in priceCatalogDetails)
                {
                    ItemBarcode itembar = null;
                    if (pcd != null)
                    {
                        if (itembar == null)
                        {
                            itembar = uow.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Oid", pcd.Barcode.Oid),
                                                                                       new BinaryOperator("Owner", pcd.Item.Owner.Oid)));
                        }
                        for (int i = 0; i < parcedCopies; i++)
                        {
                            string value = BusinessLogic.RoundAndStringify(pcd.RetailValue, pcd.PriceCatalog.Owner);
                            LinkedItem linkeditemConnection = pcd.Item.LinkedItems.FirstOrDefault();
                            Item linkedItem = linkeditemConnection == null ? null : linkeditemConnection.SubItem;
                            double factor = linkeditemConnection == null ? 0 : linkeditemConnection.QtyFactor;

                            decimal linkval = (linkedItem != null) ? ItemHelper.GetItemPrice(pcd.PriceCatalog, linkedItem).RetailValue : 0;
                            string descr = pcd.Item != null ? pcd.Item.Name.Replace("\"", "'") : "";
                            if (pcd.Item.ItemExtraInfos != null)
                            {
                                descr += " " + pcd.Item.ItemExtraInfos.FirstOrDefault().Description.Replace("\"", "'");
                            }
                            int descrLength = descr.Length;
                            string descr1 = (descrLength > 20) ? descr.Substring(0, 20) : descr;
                            string descr2 = (descrLength > 20) ? descr.Substring(20) : "";


                            List<object> arguments = new List<object>
                                        {
                                            descr1,
                                            descr2,
                                            pcd.Item == null ? "" : pcd.Item.Code,
                                            pcd.Barcode == null ? "" : pcd.Barcode.Code,
                                            pcd.ValueChangedOnDate,
                                            pcd.Item == null || pcd.Item.VatCategory == null ? "" : pcd.Item.VatCategory.Description,
                                            itembar == null || itembar.MeasurementUnit == null ? "": itembar.MeasurementUnit.Description,
                                            pcd.RetailValue,
                                            linkval == 0 ? (object)"" : linkval,
                                            pcd.Item == null || pcd.Item.DefaultSupplier == null ? "" : pcd.Item.DefaultSupplier.Code,
                                            pcd.Item == null ? (object)"" : pcd.Item.ReferenceUnit,
                                            pcd.Item == null ? (object)"" : pcd.Item.ContentUnit,
                                            pcd.UnitValue,
                                            pcd.RetailValue + (linkval * (decimal)factor)
                                        };
                            output += String.Format(ejf, arguments.ToArray());
                        }
                    }
                    else
                    {
                        message = Resources.PriceCatalogDetailNotExists;
                    }
                }
            }

            int actualEncoding =  bytesOutputEncoding == 0 ? 737 : bytesOutputEncoding;// see also PrinterServiceHelper.PrintLabel()
            outBytes = Encoding.GetEncoding(actualEncoding).GetBytes(output);
            return output;
        }
    }
}