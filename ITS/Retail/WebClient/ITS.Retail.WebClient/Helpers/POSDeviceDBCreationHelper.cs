using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Ionic.Zip;
using DevExpress.Data.Filtering;

namespace ITS.Retail.WebClient.Helpers
{
    public class POSDeviceDBCreationHelper
    {
        private readonly static object lockObject = new object();
        private readonly static object lockObjectGeneral = new object();

        public static bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
        }

        private static volatile bool _IsProcessing;

        public static string CreateFile(Guid VatLevelOID, string path)
        {
            //string POSMasterFile = path + "\\PosMaster";
            //string POSVersionFile = null;//the filename is the output of the relevant function call
            string zipFile = path + "\\POSDeviceItems.zip";
            //MvcApplication.Log.Info("POSMasterDBPreparationHelper PreparePOSMaster Start");
            MvcApplication.WRMLogModule.Log("POSDeviceDBCreationHelper Start", KernelLogLevel.Info);
            try
            {
                _IsProcessing = true;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                lock (lockObjectGeneral)
                {

                    long tora = DateTime.Now.Ticks;
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    var Categories = new XPCollection<Model.POSDevice>(uow)
                        .Where(x => x.DeviceSettings.DeviceType == Platform.Enumerations.DeviceType.RBSElioWebCashRegister)
                        .Select(s => new { s.ItemCategory, s.BarcodeType, s.PriceCatalog }).Distinct().ToList();
                    Int32 ctr = 0;
                    var vatcats = new XPCollection<VatFactor>(uow, CriteriaOperator.And(new BinaryOperator("VatLevel", VatLevelOID))).Select(s => new { VatCategoryOid = s.VatCategory.Oid, Factor = s.Factor * 100 }).ToList();
                    List<ItemDeviceIndex> DeviceIndexesToDelete =(new XPCollection<ItemDeviceIndex>(uow)).ToList();
                    for (int i = DeviceIndexesToDelete.Count - 1; i > -1; i--)
                    {
                        DeviceIndexesToDelete[i].Delete();
                    }
                    foreach (var item in Categories)
                    {
                        int decimaldigits = (int)item.BarcodeType.Owner.OwnerApplicationSettings.DisplayDigits;
                        
                        List<Item> eidix = (from eidi in new XPCollection<Item>(uow, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal), CriteriaOperator.Or(new BinaryOperator("IsTax", false, BinaryOperatorType.Equal), new NullOperator("IsTax")))) select eidi).ToList();
                        List<ItemBarcode> AllBarcodes = (from u in new XPCollection<ItemBarcode>(uow, CriteriaOperator.And(new BinaryOperator("Type.Oid", item.BarcodeType.Oid, BinaryOperatorType.Equal)))
                                                         select u).ToList();
                        var grouppedBarCodes = (
                                                        from u in AllBarcodes
                                                        group u by u.Item into g
                                                        select new { Item = g.Key, MaxCode = g.Max(x => x.Barcode.Code) }
                                                     ).ToList();
                        List<ItemBarcode> barx = (from b in grouppedBarCodes
                                                  join z in AllBarcodes on new { b.Item, b.MaxCode } equals new { z.Item, MaxCode = z.Barcode.Code }
                                                  select z).ToList();
                        var itemsInCats = (from iat in item.ItemCategory.GetAllNodeTreeItems<ItemAnalyticTree>() select new { Oid = iat.Object.Oid });
                        List<Item> ItemsToUpdate = (from eidi in eidix
                                                    join bar in barx on eidi equals bar.Item
                                                    where bar != null
                                                    join iat in itemsInCats on eidi.Oid equals iat.Oid
                                                    where iat != null

                                                    select eidi
                                                      ).ToList();

                        foreach (Item ItemToUpdate in ItemsToUpdate)
                        {
                            ItemDeviceIndex x = new ItemDeviceIndex(uow) { BarcodeType = item.BarcodeType, ItemCategory = item.ItemCategory, Item = ItemToUpdate, IsActive = true, DeviceIndex = ++ctr,  PriceCatalog=item.PriceCatalog};
                            ItemToUpdate.CashierDeviceIndex = x.DeviceIndex.ToString();
                            x.Save();
                        }
                        List<CashDeviceItem> Items = (from eidi in eidix
                                                      join bar in barx on eidi equals bar.Item
                                                      where bar != null
                                                      join iat in itemsInCats on eidi.Oid equals iat.Oid
                                                      where iat != null
                                                      join vats in vatcats on eidi.VatCategory.Oid equals vats.VatCategoryOid
                                                      select new CashDeviceItem()
                                                      {
                                                          Code = bar.Barcode.Code,
                                                          Description = eidi.Name,
                                                          deviceIndex = eidi.CashierDeviceIndex != null ? int.Parse(eidi.CashierDeviceIndex) : 0,
                                                          IsAvaledToSale = true,
                                                          Qty = (decimal)bar.RelationFactor,
                                                          VatPercent = vats.Factor,
                                                          price = Math.Round(eidi.GetUnitPriceWithVat(item.PriceCatalog, bar.Barcode), decimaldigits, MidpointRounding.AwayFromZero)
                                                      }).ToList();
                        string filename = path + "\\" + item.ItemCategory.Oid.ToString() + "_" + item.BarcodeType.Oid.ToString() + "_" + item.PriceCatalog.Oid.ToString() + ".json";
                        if (File.Exists(filename)) File.Delete(filename);
                        File.WriteAllText(filename, JsonConvert.SerializeObject(Items));
                    }
                    uow.CommitChanges();
                    //Create Zip File
                    if (File.Exists(zipFile))
                    {
                        File.Delete(zipFile);
                    }
                    if (File.Exists(path + "\\PosDeviceVersion.txt")) File.Delete(path + "\\PosDeviceVersion.txt");
                    System.IO.File.WriteAllText(path + "\\PosDeviceVersion.txt", tora.ToString());
                    // Step 1 Prepare master database connection
                    using (ZipFile zip = new ZipFile(zipFile))
                    {
                        Byte[] FileContent = File.ReadAllBytes(path + "\\PosDeviceVersion.txt");
                        zip.AddEntry("PosDeviceVersion.txt", FileContent);
                        foreach (var item in Categories)
                        {
                            FileContent = File.ReadAllBytes(path + "\\" + item.ItemCategory.Oid.ToString() + "_" + item.BarcodeType.Oid.ToString() + "_" + item.PriceCatalog.Oid.ToString() + ".json");
                            zip.AddEntry(item.ItemCategory.Oid.ToString() + "_" + item.BarcodeType.Oid.ToString() + "_" + item.PriceCatalog.Oid.ToString() + ".json", FileContent);
                        }
                        zip.Save();
                        foreach (var item in Categories)
                        {
                            File.Delete(path + "\\" + item.ItemCategory.Oid.ToString() + "_" + item.BarcodeType.Oid.ToString() + "_" + item.PriceCatalog.Oid.ToString() + ".json");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //MvcApplication.Log.Error(e, "POSMasterDBPreparationHelper: PreparePOSMaster");
                MvcApplication.WRMLogModule.Log(e);
                //Logger.Error("POSMasterDBPreparationHelper", "PreparePOSMaster", e.Message, e);
                return e.Message;
            }
            finally
            {
                _IsProcessing = false;
            }

            //MvcApplication.Log.Info("POSMasterDBPreparationHelper: PreparePOSMaster End");
            MvcApplication.WRMLogModule.Log("POSMasterDBPreparationHelper: PreparePOSMaster End", KernelLogLevel.Info);
            return zipFile;
        }
    }
}