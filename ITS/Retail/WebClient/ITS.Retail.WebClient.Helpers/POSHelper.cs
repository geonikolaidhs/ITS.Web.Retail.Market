using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Web;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Helpers
{
    public static class POSHelper
    {
        public static Byte[] ConvertFileToByteArray(string fileName)
        {
            byte[] fileContent = null;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fs);
            long byteLength = new FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }

        private static void ConvertByteArrayToFile(string fileName, Byte[] fileContent)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fs);
            long byteLength = fileContent.Length;
            binaryWriter.Write(fileContent);
            fs.Close();
            fs.Dispose();
            binaryWriter.Close();
        }

        /// <summary>
        /// Returns a dll
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public static string BuildForm(string formFile, string tempFolder, HttpServerUtility srv)
        {
            string formArgument = Path.GetExtension(formFile) == ".itsform" ? "main" : "secondary";

            using (Process p = new Process())
            {
                p.StartInfo.FileName = srv.MapPath("~/Tools/Layout Builder/ITS.POS.Tools.FormBuilder.exe");
                p.StartInfo.Arguments = String.Format("-build -tempfolder=\"{0}\" -{1}=\"{2}\"", tempFolder.TrimEnd('\\'), formArgument, formFile);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    return Path.ChangeExtension(formFile, ".dll");
                }
                else
                {
                    throw new Exception("Form build failed. Exit Code: " + p.ExitCode + Environment.NewLine + "Output: " + p.StandardOutput.ReadToEnd());
                }

            }
        }

        public static string BuildForms(string primaryFormFile, string secondaryFormFile, string tempFolder, HttpServerUtility srv)
        {
            string arguments = "-build -tempfolder=\"" + tempFolder.TrimEnd('\\') + "\"";
            if (String.IsNullOrWhiteSpace(primaryFormFile) == false)
            {
                arguments += " -main=\"" + primaryFormFile + "\"";
            }

            if (String.IsNullOrWhiteSpace(secondaryFormFile) == false)
            {
                arguments += " -secondary=\"" + secondaryFormFile + "\"";
            }

            using (Process p = new Process())
            {
                p.StartInfo.FileName = srv.MapPath("~/Tools/Layout Builder/ITS.POS.Tools.FormBuilder.exe");
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    return Path.ChangeExtension((String.IsNullOrWhiteSpace(primaryFormFile) == false) ? primaryFormFile : secondaryFormFile, ".dll");
                }
                else
                {
                    throw new Exception("Form build failed. Exit Code: " + p.ExitCode + Environment.NewLine + "Output: " + p.StandardOutput.ReadToEnd());
                }

            }
        }

        public static Image GetFormPreview(string assemblyPath, HttpServerUtility srv, string bitmapFilename, out FileStream outFS)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = srv.MapPath("~/POS/ScreenShotTool/ITS.POS.Tools.FormScreenshot.exe");
                p.StartInfo.Arguments = String.Format("\"{0}\" \"{1}\"", assemblyPath, bitmapFilename);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    outFS = new FileStream(bitmapFilename, FileMode.Open, FileAccess.Read);//must keep alive until image is saved
                    Image uploadedImage = Image.FromStream(outFS);
                    return uploadedImage;
                }
                else
                {
                    throw new Exception("Form preview failed. Exit Code: " + p.ExitCode + Environment.NewLine + "Output: " + p.StandardOutput.ReadToEnd());
                }
            }


        }

        public static decimal GetCurrentXAmount(ITS.Retail.Model.POS pos)
        {
            DailyTotals latestZ = new XPCollection<DailyTotals>(pos.Session, new BinaryOperator("POS.Oid", pos.Oid), new SortProperty("FiscalDate", SortingDirection.Descending)).FirstOrDefault();
            if (latestZ == null || latestZ.FiscalDateOpen == false)
            {
                return 0;
            }
            else
            {
                latestZ.UserDailyTotalss.Sorting = new SortingCollection(new SortProperty("CreatedOn", SortingDirection.Ascending));
                UserDailyTotals latestX = latestZ.UserDailyTotalss.Where(g => g.IsOpen == true).FirstOrDefault();
                if (latestX == null)
                {
                    return 0;
                }
                else
                {
                    var paymentsDetail = latestX.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.PAYMENTS);
                    if (paymentsDetail.Count() == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return paymentsDetail.Sum(x => x.Amount);
                    }
                }
            }
        }

        public static decimal GetCurrentXCount(ITS.Retail.Model.POS pos)
        {
            DailyTotals latestZ = new XPCollection<DailyTotals>(pos.Session, new BinaryOperator("POS.Oid", pos.Oid), new SortProperty("FiscalDate", SortingDirection.Descending)).FirstOrDefault();
            if (latestZ == null || latestZ.FiscalDateOpen == false)
            {
                return 0;
            }
            else
            {
                latestZ.UserDailyTotalss.Sorting = new SortingCollection(new SortProperty("CreatedOn", SortingDirection.Ascending));
                UserDailyTotals latestX = latestZ.UserDailyTotalss.Where(g => g.IsOpen == true).FirstOrDefault();
                if (latestX == null)
                {
                    return 0;
                }
                else
                {
                    var paymentsDetail = latestX.UserDailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.INVOICES);
                    if (paymentsDetail.Count() == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return paymentsDetail.Sum(x => x.QtyValue);
                    }
                }
            }
        }

        public static decimal GetCurrentZAmount(ITS.Retail.Model.POS pos)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DailyTotals latestZ = new XPCollection<DailyTotals>(uow, new BinaryOperator("POS.Oid", pos.Oid), new SortProperty("FiscalDate", SortingDirection.Descending)).FirstOrDefault();
                if (latestZ == null || latestZ.FiscalDateOpen == false)
                {
                    return 0;
                }
                else
                {
                    var paymentsDetail = latestZ.DailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.PAYMENTS);
                    if (paymentsDetail.Count() == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return paymentsDetail.Sum(x => x.Amount);
                    }
                }
            }
        }

        public static decimal GetCurrentZCount(ITS.Retail.Model.POS pos)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DailyTotals latestZ = new XPCollection<DailyTotals>(uow, new BinaryOperator("POS.Oid", pos.Oid), new SortProperty("FiscalDate", SortingDirection.Descending)).FirstOrDefault();
                if (latestZ == null || latestZ.FiscalDateOpen == false)
                {
                    return 0;
                }
                else
                {
                    var paymentsDetail = latestZ.DailyTotalsDetails.Where(x => x.DetailType == eDailyRecordTypes.INVOICES);
                    if (paymentsDetail.Count() == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return paymentsDetail.Sum(x => x.QtyValue);
                    }
                }
            }
        }

        static Assembly newDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(args.Name);
        }

        internal class AssemblyLoader : MarshalByRefObject
        {
            public Assembly LoadAssembly(string path)
            {
                return Assembly.LoadFrom(path);
            }
        }

        public static List<eActions> GetExternalActions()
        {
            List<eActions> list = new List<eActions>();
            foreach (eActions action in Enum.GetValues(typeof(eActions)))
            {
                if ((int)action <= 100000 && action != eActions.NONE) //Internal Actions > 100000. None is inserted after the sorting to the top of the list
                {
                    list.Add(action);
                }
            }

            list = list.OrderBy(x => x.ToString()).ToList();
            list.Insert(0, eActions.NONE);
            return list;
        }


        public static void ProcessPOSTransactions(string fullFilename)
        {
            if (!File.Exists(fullFilename))
            {
                throw new Exception(fullFilename);
            }
        }

        private static long GetCurrentMaxVersion(string entityName, UnitOfWork uow)
        {
            Type modelType = typeof(Item).Assembly.GetType("ITS.Retail.Model." + entityName);
            object maxUpdatedOnTicks = uow.Evaluate(modelType, CriteriaOperator.Parse("Max(UpdatedOnTicks)"), null);
            return (maxUpdatedOnTicks is long) ? (long)maxUpdatedOnTicks : 0;
        }

        public static void SendSqlPosCommnad(String sqlCommand, Guid posGuid, ePosCommand posCommand)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Model.POS pos = uow.GetObjectByKey<Model.POS>(posGuid);
                POSCommand poscommand = new POSCommand(uow);
                poscommand.POS = pos;
                poscommand.CommandParameters = sqlCommand;
                poscommand.Command = posCommand;
                poscommand.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        public static void AddPosCommand(Guid posGuid, ePosCommand posCommand, string parameters)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Model.POS pos = uow.GetObjectByKey<Model.POS>(posGuid);
                if (posCommand == ePosCommand.SEND_CHANGES)
                {
                    string[] entities = parameters.Split(',');
                    foreach (string entityName in entities)
                    {
                        POSUpdaterManualVersion manualVersion = uow.FindObject<POSUpdaterManualVersion>(CriteriaOperator.And(new BinaryOperator("POS.Oid", pos.Oid), new BinaryOperator("EntityName", entityName.Trim())));
                        if (manualVersion == null)
                        {
                            manualVersion = new POSUpdaterManualVersion(uow);
                            manualVersion.EntityName = entityName.Trim();
                            manualVersion.POS = pos;
                        }
                        manualVersion.MaxUpdatedOnTicks = GetCurrentMaxVersion(entityName.Trim(), uow);
                        manualVersion.Save();
                    }
                }
                else
                {
                    POSCommand poscommand = new POSCommand(uow);
                    poscommand.POS = pos;
                    poscommand.CommandParameters = parameters;
                    poscommand.Command = posCommand;
                    poscommand.Save();
                }
                XpoHelper.CommitChanges(uow);
            }
        }


        public static void ReloadScales(UnitOfWork uow, out string erroMessage, Store CurrentStore)
        {
            XPCollection<Scale> allScales = new XPCollection<Scale>(uow);
            erroMessage = string.Empty;
            DateTime from = allScales.Min(scale => scale.ExportVersion);
            DateTime until = DateTime.Now;

            LabelItemChangeCriteria criteriaBuilder = new LabelItemChangeCriteria()
            {
                FromDate = from.Date,
                FromDateTime = new DateTime(from.TimeOfDay.Ticks),
                ToDate = until.Date,
                ToDateTime = new DateTime(until.TimeOfDay.Ticks),
                WithValueChangeOnly = false,
                WithTimeValueFilter = true,

                TimeValueFromDate = from.Date,
                TimeValueFromTime = new DateTime(from.TimeOfDay.Ticks),
                TimeValueToDate = until.Date,
                TimeValueToTime = new DateTime(until.TimeOfDay.Ticks),
            };

            CriteriaOperator priceCatalogDetailsFilter = criteriaBuilder.BuildCriteria();


            XPCollection<ItemBarcode> weightedIbcs = new XPCollection<ItemBarcode>(uow,
                                                            CriteriaOperator.And(new NotOperator(new NullOperator("PluCode")),
                                                           new NotOperator(new NullOperator("PluPrefix")),
                                                           new BinaryOperator("PluCode", string.Empty, BinaryOperatorType.NotEqual),
                                                           new BinaryOperator("PluPrefix", string.Empty, BinaryOperatorType.NotEqual),
                                                           new ContainsOperator("Item.PriceCatalogs", priceCatalogDetailsFilter)));


            Dictionary<ItemBarcode, PriceCatalogPolicyPriceResult> itemBarcodesPriceCatalogDetails = weightedIbcs.ToDictionary(y => y, x => PriceCatalogHelper.GetPriceCatalogDetail(StoreControllerAppiSettings.CurrentStore, x.Barcode.Code));

            List<ItemBarcode> itemBarcodesWithoutValues = itemBarcodesPriceCatalogDetails.Where(pair => pair.Value == null).Select(pair => pair.Key).ToList();
            itemBarcodesWithoutValues.ForEach(itemWithoutValue =>
            {
                itemBarcodesPriceCatalogDetails.Remove(itemWithoutValue);
            });

            foreach (Scale scale in allScales)
            {
                if (scale.UseDirectSQL == false)
                {

                    string message;
                    if (ScaleExportHelper.Export(itemBarcodesPriceCatalogDetails, scale.ExportFormatString, scale.ExportFullFilePath, Encoding.GetEncoding(scale.Encoding), out message, CurrentStore))
                    {
                        scale.ExportVersion = until;
                        scale.Save();
                        uow.CommitChanges();
                    }
                    else
                    {
                        erroMessage += message;
                    }
                }
                else if (scale.UseDirectSQL == true)
                {
                    //"select * from z_its_scales"
                    string message;
                    if (ScaleExportHelper.ExportSQL(scale.ExportFormatString, scale.ExportFullFilePath, Encoding.GetEncoding(scale.Encoding), scale.DirectSQL, out message, itemBarcodesPriceCatalogDetails))
                    {
                        scale.ExportVersion = until;
                        scale.Save();
                        uow.CommitChanges();
                    }
                    else
                    {
                        erroMessage += message;
                    }
                }
            }
        }
    }
}
