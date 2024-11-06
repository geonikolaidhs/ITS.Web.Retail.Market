using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Text;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using System.IO;
using System.Threading;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Globalization;
using System.Xml;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using ImageMagick;
using System.Drawing;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering.Helpers;
using ITS.Retail.Platform.Kernel;

#if _RETAIL_WEBCLIENT || _RETAIL_DUAL
namespace ITS.Retail.WebClient
{
    /// <summary>
    /// Summary description for RetailBridgeService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RetailBridgeService : System.Web.Services.WebService
    {
        private static Queue<Tuple<Thread, object>> threadsToRun;
        private static Thread currentThread;
        private static Timer runner;
        private static TimerCallback runnerCallback;

        public enum FileType
        {
            TEXT = 0,
            IMAGE = 1
        }

        /// <summary>
        /// Filetype: 0 = textfile, 1 = image.
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="compressedFileContents"></param>
        /// <param name="encoding"></param>
        /// <param name="agendOid"></param>
        /// <param name="email"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        [WebMethod]
        public string PostFile(FileType fileType, string compressedFileContents, string fileName, string encoding, Guid agendOid, string email, string md5,
            bool createUsersForCustomers, bool createUsersForStores, string customerUserRoleDescription, string storeUserRoleDescription, string locale,
            string ownerTaxCode, bool multipleCategoryTrees, string rootCategoryCode, bool makeZeroPricesUserDefinable, bool sendEmailsForImagesOnlyOnError,
            bool oneStorePerCustomer)  //step 1
        {

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            try
            {
                if (String.IsNullOrWhiteSpace(ownerTaxCode))
                {
                    return "Owner tax code is null or empty. Please provide a valid tax code; Import Aborted";
                }

                CompanyNew owner = uow.FindObject<CompanyNew>(new BinaryOperator("Trader.TaxCode", ownerTaxCode));
                if (owner == null)
                {
                    return "No company found with tax code '" + ownerTaxCode + "'; Import Aborted";
                }

                DataFileReceived newFile = new DataFileReceived(uow);
                newFile.AgendOid = agendOid;
                newFile.Decoded = false;
                newFile.Email = email;
                newFile.MD5 = md5;
                newFile.Tries = 0;
                newFile.FileContext = compressedFileContents;
                newFile.Filename = fileName;
                newFile.Encoding = encoding;
                newFile.CreateUsersForCustomers = createUsersForCustomers;
                newFile.CreateUsersForStores = createUsersForStores;
                newFile.CustomerUserRoleDescription = customerUserRoleDescription;
                newFile.StoreUserRoleDescription = storeUserRoleDescription;
                newFile.Locale = locale;
                newFile.Owner = owner;
                newFile.MultipleItemCategoryTrees = multipleCategoryTrees;
                newFile.RootItemCategoryCode = rootCategoryCode;
                newFile.MakeZeroPricesUserDefinable = makeZeroPricesUserDefinable;
                newFile.SendEmailsForImagesOnlyOnError = sendEmailsForImagesOnlyOnError;
                newFile.OneStorePerCustomer = oneStorePerCustomer;

                if (fileType == FileType.TEXT)
                {
                    newFile.Image = null;
                }
                else if (fileType == FileType.IMAGE)
                {

                    newFile.Image = CompressionHelper.DecompressImageLZMA(compressedFileContents);
                }

                XpoHelper.CommitTransaction(uow);
                QueueImport(newFile);
                return "File upload succeeded;Import queued";
            }
            catch (Exception e)
            {
                uow.RollbackTransaction();
                return e.Message;
            }

        }
        [WebMethod]
        public XmlDocument GetOrders(string username, string password, string storeCode, long versionDateTicks, out long latestVersionTicks)
        {
            latestVersionTicks = versionDateTicks;
            //TODO Potential Memory Problem
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            User user = uow.FindObject<User>(CriteriaOperator.And(new BinaryOperator("UserName", username),
                                                                  new BinaryOperator("Password", UserHelper.EncodePassword(password))));
            if (user == null)
            {
                return null;
            }

            if (!UserHelper.IsCompanyUser(user))
            {
                return null;
            }

            CompanyNew userSupplier = UserHelper.GetCompany(user);

            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                XPCollection<DocumentHeader> orders = null;
                if (string.IsNullOrWhiteSpace(storeCode))
                {
                    orders = new XPCollection<DocumentHeader>(uow, CriteriaOperator.And(new BinaryOperator("Store.Owner.Oid", userSupplier.Oid),
                                                                                        new BinaryOperator("UpdatedOnTicks", versionDateTicks, BinaryOperatorType.Greater)),
                                                                                        new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                }
                else
                {
                    orders = new XPCollection<DocumentHeader>(uow, CriteriaOperator.And(new BinaryOperator("Store.Owner.Oid", userSupplier.Oid),
                                                                                        new BinaryOperator("Store.Code", storeCode),
                                                                                        new BinaryOperator("UpdatedOnTicks", versionDateTicks, BinaryOperatorType.Greater)),
                                                                                        new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                }
                writer.WriteStartDocument();
                writer.WriteStartElement("Documents");
                foreach (DocumentHeader order in orders)
                {
                    writer.WriteStartElement("Document");

                    writer.WriteStartElement("DocumentHeader");

                    writer.WriteStartElement("Oid");
                    writer.WriteString(order.Oid.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Customer");
                    writer.WriteString(order.Customer == null ? "" : order.Customer.Code);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Store");
                    writer.WriteString(order.Store == null ? "" : order.Store.Code);
                    writer.WriteEndElement();

                    writer.WriteStartElement("DeliveryAddress");
                    writer.WriteString(order.DeliveryAddress);
                    writer.WriteEndElement();

                    writer.WriteStartElement("CreatedOn");
                    writer.WriteString(order.CreatedOn.ToShortDateString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("DocumentNumber");
                    writer.WriteString(order.DocumentNumber.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("DocumentSeries");
                    writer.WriteString(order.DocumentSeries == null || order.DocumentSeries.Description == null ? "" : order.DocumentSeries.Description);
                    writer.WriteEndElement();

                    writer.WriteStartElement("DocumentType");
                    writer.WriteString(order.DocumentType == null || order.DocumentType.Description == null ? "" : order.DocumentType.Description);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Status");
                    writer.WriteString(order.Status == null || order.Status.Description == null ? "" : order.Status.Description);
                    writer.WriteEndElement();

                    writer.WriteStartElement("FinalizedDate");
                    writer.WriteString(order.FinalizedDate == DateTime.MinValue ? "" : order.FinalizedDate.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("TotalDiscountAmount");
                    writer.WriteString(order.TotalDiscountAmount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("TotalVatAmount");
                    writer.WriteString(order.TotalVatAmount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("NetTotal");
                    writer.WriteString(order.NetTotal.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("GrossTotal");
                    writer.WriteString(order.GrossTotal.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Remarks");
                    writer.WriteString(order.Remarks);
                    writer.WriteEndElement();

                    writer.WriteStartElement("TotalVatAmountBeforeDiscount");
                    writer.WriteString(order.TotalVatAmountBeforeDiscount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("NetTotalBeforeDiscount");
                    writer.WriteString(order.NetTotalBeforeDiscount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("GrossTotalBeforeDiscount");
                    writer.WriteString(order.GrossTotalBeforeDiscount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("GrossTotalBeforeDocumentDiscount");
                    writer.WriteString(order.GrossTotalBeforeDocumentDiscount.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("ExecutionDate");
                    writer.WriteString(order.ExecutionDate == null || order.ExecutionDate == DateTime.MinValue ? "" : order.ExecutionDate.Value.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("DocumentTypeOid");
                    writer.WriteString(order.DocumentType == null ? Guid.Empty.ToString() : order.DocumentType.Oid.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("StatusOid");
                    writer.WriteString(order.Status == null ? Guid.Empty.ToString() : order.Status.Oid.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("UpdatedOn");
                    writer.WriteString(order.UpdatedOn.ToShortDateString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteStartElement("DocumentDetails");
                    foreach (DocumentDetail detail in order.DocumentDetails)
                    {
                        writer.WriteStartElement("Detail");

                        writer.WriteStartElement("Oid");
                        writer.WriteString(detail.Oid.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("DocumentHeaderOid");
                        writer.WriteString(detail.DocumentHeader.Oid.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("Item");
                        writer.WriteString(detail.Item == null ? "" : detail.Item.Code);
                        writer.WriteEndElement();

                        writer.WriteStartElement("Barcode");
                        writer.WriteString(detail.Barcode == null ? "" : detail.Barcode.Code);
                        writer.WriteEndElement();

                        writer.WriteStartElement("UnitPrice");
                        writer.WriteString(detail.UnitPrice.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("UnitPriceAfterDiscount");
                        writer.WriteString(detail.UnitPriceAfterDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("VatAmount");
                        writer.WriteString(detail.TotalVatAmount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("VatFactor");
                        writer.WriteString(detail.VatFactor.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("FinalUnitPrice");
                        writer.WriteString(detail.FinalUnitPrice.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("FirstDiscount");
                        writer.WriteString(detail.PriceCatalogDiscountPercentage.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("SecondDiscount");
                        writer.WriteString(detail.CustomDiscountsPercentageWholeSale.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("TotalDiscount");
                        writer.WriteString(detail.TotalDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("TotalVatAmount");
                        writer.WriteString(detail.TotalVatAmount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("UnitPrice");
                        writer.WriteString(detail.UnitPrice.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("NetTotal");
                        writer.WriteString(detail.NetTotal.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("CentralStore");
                        writer.WriteString(detail.CentralStore == null ? "" : detail.CentralStore.Code);
                        writer.WriteEndElement();

                        writer.WriteStartElement("MeasurementUnit");
                        writer.WriteString(detail.MeasurementUnit == null || detail.MeasurementUnit.Description == null ? "" : detail.MeasurementUnit.Description);
                        writer.WriteEndElement();

                        writer.WriteStartElement("Qty");
                        writer.WriteString(detail.Qty.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("GrossTotal");
                        writer.WriteString(detail.GrossTotal.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("TotalVatAmountBeforeDiscount");
                        writer.WriteString(detail.TotalVatAmountBeforeDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("NetTotalBeforeDiscount");
                        writer.WriteString(detail.NetTotalBeforeDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("GrossTotalBeforeDiscount");
                        writer.WriteString(detail.GrossTotalBeforeDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("GrossTotalBeforeDocumentDiscount");
                        writer.WriteString(detail.GrossTotalBeforeDocumentDiscount.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("CreatedOn");
                        writer.WriteString(order.CreatedOn.ToShortDateString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("UpdatedOn");
                        writer.WriteString(order.UpdatedOn.ToShortDateString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("LineNumber");
                        writer.WriteString(detail.LineNumber.ToString());
                        writer.WriteEndElement();

                        writer.WriteEndElement();

                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    latestVersionTicks = order.UpdatedOnTicks;
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sb.ToString());
                return xmlDocument;
            }
        }

        public static void Initialize()
        {
            threadsToRun = new Queue<Tuple<Thread, object>>();
            currentThread = null;
            runnerCallback = new TimerCallback(ThreadRunner);
            runner = new Timer(runnerCallback, null, 1000, 2000);
        }

        protected static void ThreadRunner(object state)
        {
            lock (threadsToRun)
            {
                if ((currentThread == null || !currentThread.IsAlive) && threadsToRun.Count > 0)
                {
                    Tuple<Thread, object> threadPair = null;

                    threadPair = threadsToRun.Dequeue();

                    currentThread = threadPair.Item1;
                    currentThread.Start(threadPair.Item2);
                }
            }
        }

        protected void QueueImport(DataFileReceived file)
        {
            Thread startImportThread = new Thread(ImportThread);
            Tuple<Thread, object> temp = new Tuple<Thread, object>(startImportThread, file);
            lock (threadsToRun)
            {
                threadsToRun.Enqueue(temp);
            }
        }

        protected void ImportThread(object file)
        {
            MvcApplication.IsImportRunning = true;
            try
            {
                //ApplicationLog logAtDB;
                DataFileReceived datafileRecieved = (file as DataFileReceived);

                int codepage = 0;
                Encoding encoding = null;

                if (!String.IsNullOrWhiteSpace(datafileRecieved.Encoding))
                {
                    if (int.TryParse(datafileRecieved.Encoding, out codepage))
                    {
                        encoding = Encoding.GetEncoding(codepage);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(datafileRecieved.Encoding);
                    }
                }

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    //Guid logAtDBGuid;
                    //logAtDB = new ApplicationLog(uow);
                    //logAtDBGuid = logAtDBGuid = logAtDB.Oid;
                    //logAtDB.Controller = "RetailBridgeWebservice";
                    //logAtDB.Action = "ImportThread";
                    //logAtDB.Save();
                    XpoHelper.CommitChanges(uow);
                    string serverLogPath = Server.MapPath("~/Log/");
                    if (!Directory.Exists(serverLogPath))
                    {
                        Directory.CreateDirectory(serverLogPath);
                    }
                    string fileName = serverLogPath + String.Format("\\BridgeLog-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);

                    using (StreamWriter writer = new StreamWriter(fileName, true, encoding ?? Encoding.UTF8))
                    {
                        //LOCALE CHANGE
                        try
                        {
                            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(datafileRecieved.Locale);

                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine(DateTime.Now.ToString() + ": Locale error: " + ex.Message);
                        }

                        DeleteTempRecords(writer); //Delete any garbage

                        writer.WriteLine(DateTime.Now.ToString() + ": Import Started for file with ID='" + datafileRecieved.Oid + "', Owner ID='" + datafileRecieved.Owner.Oid + "', Owner TaxCode ='" + datafileRecieved.Owner.Trader.TaxCode + "'");
                        string backupFileName = null;
                        try
                        {
                            if (datafileRecieved.Image == null)
                            {
                                try //File backup
                                {
                                    string fileContent = CompressionHelper.DecompressLZMA(datafileRecieved.FileContext).ToString();
                                    backupFileName = serverLogPath + String.Format("\\_{0}-{1:yyyy-MM-dd_hh-mm-ss-tt}.txt", datafileRecieved.Filename, DateTime.Now);

                                    if (encoding != null)
                                    {
                                        using (StreamWriter backupwriter = new StreamWriter(backupFileName, false, encoding))
                                        {
                                            backupwriter.Write(fileContent);
                                        }
                                    }
                                    else
                                    {
                                        using (StreamWriter backupwriter = new StreamWriter(backupFileName, false))
                                        {
                                            backupwriter.Write(fileContent);
                                        }
                                    }
                                    fileContent = null;
                                    GC.Collect();
                                }
                                catch (Exception e)
                                {
                                    writer.WriteLine(DateTime.Now.ToString() + ": Backup of file '" + datafileRecieved.Filename + "' failed. Error: " + e.Message);
                                }
                            }
                            datafileRecieved.Save();
                            XpoHelper.CommitChanges(uow);
                            bool step2Success = CreateTempRows(datafileRecieved.Oid, backupFileName, writer);
                            GC.Collect();
                            if (!step2Success) throw new Exception("Creation of Temp Records failed.");
                            bool step3Success = InsertTempRowsIntoDB(datafileRecieved.Oid, writer);
                            if (!step3Success) throw new Exception("Insertion of Temp Records into db failed.");
                        }
                        catch (Exception e)
                        {
                            uow.RollbackTransaction();
                            writer.WriteLine(DateTime.Now.ToString() + ";ImportThread;" + "Error: " + e.Message + ";StackTrace:" + e.StackTrace);
                        }
                        finally
                        {
                            DeleteTempRecords(writer);
                            writer.WriteLine(DateTime.Now.ToString() + ": Import end");
                        }

                    }
                    try
                    {

                        using (StreamReader sr = new StreamReader(fileName))
                        {
                            string emailContent = "";
                            while (!sr.EndOfStream)
                            {
                                emailContent += sr.ReadLine() + "<br>";
                            }
                            if (datafileRecieved.Image == null ||
                                (datafileRecieved.SendEmailsForImagesOnlyOnError == false && emailContent.Contains("Error:") == false) ||
                                emailContent.Contains("Error:") == true)
                            {
                                MailHelper.SendMailMessage(BridgeHelper.EmailFrom, datafileRecieved.Email.Split(',').ToList(), "Import Results: " + datafileRecieved.Filename, emailContent, BridgeHelper.SMTPHost, null, null, BridgeHelper.EmailUsername, BridgeHelper.EmailPassword, BridgeHelper.Domain, BridgeHelper.EnableSSL, BridgeHelper.Port);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        using (StreamWriter writer = new StreamWriter(fileName, true))
                        {
                            writer.WriteLine(DateTime.Now.ToString() + ": Error sending email;" + e.Message);
                        }
                    }

                    try
                    {
                        using (StreamReader sr = new StreamReader(fileName))
                        {
                            string filelog = sr.ReadToEnd();
                            if(filelog.Length > 250)
                            {
                                filelog = "Cannot save the whole result (Probably huge)";
                            }
                            MvcApplication.WRMLogModule.Log(null, filelog, "RetailBridgeService", "ImportThread", kernelLogLevel: KernelLogLevel.Info);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MvcApplication.WRMLogModule.Log(ex, "Cannot write the result: See separate log files", "RetailBridgeService", "ImportThread", kernelLogLevel: KernelLogLevel.Warn);
                    }

                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }
            MvcApplication.IsImportRunning = false;
        }

        protected bool CreateTempRows(Guid dataFileOid, string filePath, StreamWriter writer) //step 2
        {
            try
            {
                ReadDataFile(dataFileOid, filePath, writer);
            }
            catch (Exception e)
            {
                writer.WriteLine(DateTime.Now.ToString() + ";CreateTempRecords;" + "Error: " + e.Message + ";StackTrace: " + e.StackTrace);
                return false;
            }
            return true;
        }

        protected bool InsertTempRowsIntoDB(Guid fileOid, StreamWriter writer)  //step 3
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);

                CompanyNew owner = file.Owner;
                OwnerApplicationSettings appiSettings = owner.OwnerApplicationSettings;
                Store supplierCentralStore = uow.FindObject<Store>(CriteriaOperator.And(new BinaryOperator("IsCentralStore", true), new BinaryOperator("Owner.Oid", owner.Oid)));


                if (file.Image == null)     //Temp rows are txt lines
                {
                    int i = 0;
                    XPCollection<DataFileRecordHeader> headers = new XPCollection<DataFileRecordHeader>(uow, new BinaryOperator("Owner.Oid", owner.Oid));
                    headers.Sorting = new SortingCollection(new SortProperty("Order", SortingDirection.Ascending));
                    foreach (DataFileRecordHeader header in headers)
                    {
                        XPCursor tempRows = new XPCursor(uow, typeof(DecodedRawData),
                                           CriteriaOperator.And(new BinaryOperator("Head.Oid", header.Oid), new BinaryOperator("Owner.Oid", owner.Oid)),
                                           new SortProperty("CreatedOnTicks", SortingDirection.Ascending),
                                           new SortProperty("Counter", SortingDirection.Ascending));

                        tempRows.PageSize = 500;
                        foreach (DecodedRawData row in tempRows)
                        {
                            try
                            { 
                                InsertTempRowIntoDB(row.Oid, writer, supplierCentralStore == null ? Guid.Empty : supplierCentralStore.Oid, file.Oid);
                                i++;
                                if (i % 5000 == 0)
                                {
                                    GC.Collect();
                                }
                            }
                            catch (Exception e)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertTempRecordsIntoDB;" + "Error: " + e.Message + ";StackTrace:" + e.StackTrace);
                                continue;
                            }
                        }
                    }
                    XPCursor itemRows = new XPCursor(uow, typeof(DecodedRawData), CriteriaOperator.And(new BinaryOperator("Head.EntityName", "Item"), new BinaryOperator("Owner.Oid", owner.Oid)));
                    itemRows.PageSize = 1000;
                    SetMotherCodes(uow, itemRows, owner);

                }
                else   //Temp row is image
                {
                    XPCollection<DecodedRawData> tempRows = new XPCollection<DecodedRawData>(uow, CriteriaOperator.And(new NullOperator("Head"), new BinaryOperator("Owner.Oid", owner.Oid)));
                    foreach (DecodedRawData image in tempRows)
                    {
                        try
                        {
                            //JObject jsonItem = JObject.Parse(image.jsonDataRecord);
                            string itemCode = image.Item;
                            itemCode = appiSettings.PadItemCodes ? itemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]) : itemCode;
                            Item item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", itemCode), new BinaryOperator("Owner.Oid", owner.Oid)));
                            if (item != null)
                            {
                                //image.Image

                                if (image.Image == null)
                                {
                                    throw new Exception("Failed: Image is null. Item code '" + itemCode + "'");
                                }

                                using (Bitmap bitmap = new Bitmap(image.Image))
                                using (MagickImage uploadedImage = new MagickImage(bitmap))
                                {
                                    item.ImageLarge = ItemHelper.PrepareImage(uploadedImage, 600, 600);
                                    item.ImageMedium = ItemHelper.PrepareImage(uploadedImage, 300, 300);
                                    item.ImageSmall = ItemHelper.PrepareImage(uploadedImage, 150, 150);
                                    item.Save();
                                    XpoHelper.CommitTransaction(uow);
                                }
                            }
                            else
                            {
                                throw new Exception("Failed: Item with code '" + itemCode + "' not found.");
                            }
                        }
                        catch (Exception e)
                        {
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertTempRecordsIntoDB;Import of Image;" + "Error: " + e.Message + ";StackTrace:" + e.StackTrace);
                            continue;
                        }

                    }
                }

                return true;
            }
        }

        protected bool DeleteTempRecords(/*UnitOfWork uow, List<DecodedRawData> rowsToDelete,*/ StreamWriter writer)  //step 4
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    //XPCollection<DecodedRawData> rowsToDelete = new XPCollection<DecodedRawData>(uow);
                    //int i = 0;
                    //foreach (DecodedRawData row in rowsToDelete)
                    //{
                    //    row.Delete();
                    //    i++;
                    //    if (i % 1000 == 0)
                    //    {
                    //        uow.CommitTransaction();

                    //    }
                    //}
                    //uow.CommitTransaction();
                    //uow.PurgeDeletedObjects();


                    //uow.Delete(rowsToDelete);         //throws out of memory exception when deleting many rows (e.x. 200000)
                    //uow.CommitTransaction();
                    //uow.PurgeDeletedObjects();

                    switch (XpoHelper.databasetype)
                    {
                        case DBType.postgres:
                            uow.ExecuteNonQuery("TRUNCATE \"DecodedRawData\";");
                            break;
                        default:
                            uow.ExecuteNonQuery("TRUNCATE TABLE DecodedRawData");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                writer.WriteLine(DateTime.Now.ToString() + ";DeleteTempRecords;" + "Error: " + e.Message + "StackTrace:" + e.StackTrace);
            }
            return true;
        }

        protected void ReadDataFile(Guid dataFileRecievedOid, string filePath, StreamWriter writer)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DataFileReceived dataFile = uow.GetObjectByKey<DataFileReceived>(dataFileRecievedOid);
                OwnerApplicationSettings appiSettings = dataFile.Owner.OwnerApplicationSettings;
                dataFile.Tries += 1;
                if (dataFile.Image == null)
                {
                    XPCollection<DataFileRecordHeader> headers = new XPCollection<DataFileRecordHeader>(uow, new BinaryOperator("Owner.Oid", dataFile.Owner.Oid));
                    headers.Sorting = new SortingCollection(new SortProperty("Order", SortingDirection.Ascending));
                    Encoding encoding;
                    int codepage;
                    if (int.TryParse(dataFile.Encoding, out codepage))
                    {
                        encoding = Encoding.GetEncoding(codepage);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(dataFile.Encoding);
                    }

                    using (StreamReader reader = new StreamReader(filePath, encoding))
                    {
                        string line;
                        UnitOfWork tempUnitOfWork = XpoHelper.GetNewUnitOfWork();
                        int counter = 0;
                        while ((line = reader.ReadLine()) != null)
                        {
                            counter++;
                            if (counter % 1000 == 0)
                            {
                                XpoHelper.CommitTransaction(tempUnitOfWork);
                                tempUnitOfWork.Dispose();
                                tempUnitOfWork = XpoHelper.GetNewUnitOfWork();
                            }
                            bool headerFound = false;
                            foreach (DataFileRecordHeader header in headers)
                            {
                                string characters = BridgeHelper.SpecialCharacterReplacement(header.TabDelimitedString);
                                try
                                {
                                    bool validLine = false;
                                    if (header.IsTabDelimited)
                                    {
                                        string[] splitedLine = line.Split(characters.ToCharArray());
                                        if (splitedLine.Length > header.Position && splitedLine[header.Position] == header.HeaderCode)
                                        {
                                            validLine = true;
                                            headerFound = true;
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(line) && line.Substring(header.Position, header.Length) == header.HeaderCode)
                                        {
                                            validLine = true;
                                            headerFound = true;
                                        }
                                    }
                                    if (validLine)
                                    {
                                        try
                                        {
                                            DecodedRawData tempRow = CreateTempRow(line, header.Oid, tempUnitOfWork, counter);
                                            if (tempRow == null)
                                            {
                                                throw new Exception("Creation of temp row from line '" + line + "' failed");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            writer.WriteLine(DateTime.Now.ToString() + ";ReadDataFile;CreateTempRow;" + "Error: " + e.Message + ";StackTrace: " + e.StackTrace);
                                            continue;
                                        }
                                        //break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    writer.WriteLine(DateTime.Now.ToString() + ";ReadDataFile;" + "Error: " + e.Message + ";StackTrace: " + e.StackTrace);
                                    continue;
                                }
                            }
                            if (!headerFound)
                            {
                                writer.WriteLine(DateTime.Now.ToString() + "Creation of temp row from line '" + line + "' failed. No matching header found.");
                            }
                        }
                        XpoHelper.CommitTransaction(tempUnitOfWork);
                        tempUnitOfWork.Dispose();
                        System.GC.Collect();
                    }
                }
                else  //Image file
                {
                    string itemCode = dataFile.Filename;

                    int fileExtPos = itemCode.LastIndexOf(".");
                    if (fileExtPos >= 0)
                    {
                        itemCode = itemCode.Substring(0, fileExtPos);
                    }

                    if (appiSettings.PadItemCodes)
                    {
                        itemCode = itemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                    }

                    Item item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", itemCode, BinaryOperatorType.Like), new BinaryOperator("Owner.Oid", dataFile.Owner.Oid)));
                    if (item == null)
                    {
                        dataFile.Decoded = false;
                        dataFile.Save();
                        //uow.CommitTransaction();
                        XpoHelper.CommitTransaction(uow);
                        throw new Exception("Image Import: Error at file '" + dataFile.Filename + "'. Item with code '" + itemCode + "' not found. Image will not be inserted.");
                    }
                    DecodedRawData tempRow = new DecodedRawData(uow);
                    tempRow.Head = null;                //Marked as Item Image
                    string compressedImage = dataFile.FileContext;
                    tempRow.Item = itemCode;
                    tempRow.Owner = dataFile.Owner;
                    tempRow.Image = CompressionHelper.DecompressImageLZMA(compressedImage);
                    tempRow.Save();
                    //uow.CommitTransaction();
                    XpoHelper.CommitTransaction(uow);
                }
                dataFile.Decoded = true;
                dataFile.Save();
                //uow.CommitTransaction();
                XpoHelper.CommitTransaction(uow);
            }
        }

        protected DecodedRawData CreateTempRow(string line, Guid headerOid, UnitOfWork uow, int counter)
        {
            {
                DataFileRecordHeader header = uow.GetObjectByKey<DataFileRecordHeader>(headerOid);
                string[] tabDelimitedFields = null;

                if (header.IsTabDelimited)
                {
                    line = line.Trim();
                    tabDelimitedFields = line.Split(BridgeHelper.SpecialCharacterReplacement(header.TabDelimitedString).ToCharArray());
                    int expectedFieldsCount = header.DataFileRecordDetails.Where(detail => detail.ConstantValue == "").Count();
                    int lineFieldsCount = tabDelimitedFields.Length - 1;
                    if (lineFieldsCount < expectedFieldsCount)
                    {
                        throw new Exception(@"Creation of temp row of line '" + line + "' failed. Line does not match the DataFileRecordDetails of Header '" + header.HeaderCode + "','" + header.EntityName + "', Line Fields Count = " + lineFieldsCount + ", Expected Fields Count = " + expectedFieldsCount);
                    }
                }

                DecodedRawData tempRow = new DecodedRawData(uow);
                tempRow.Owner = header.Owner;
                tempRow.Head = header;
                tempRow.IsActive = true;
                tempRow.Counter = counter;

                foreach (DataFileRecordDetail detail in header.DataFileRecordDetails)
                {
                    string value = detail.DefaultValue;
                    if (String.IsNullOrWhiteSpace(detail.ConstantValue))
                    {
                        if (header.IsTabDelimited)
                        {
                            value = tabDelimitedFields[detail.Position];
                        }
                        else
                        {
                            value = line.Substring(detail.Position, detail.Length);
                        }

                        if (detail.Trim)
                        {
                            value = value.Trim();
                        }
                        if (detail.Padding)
                        {
                            value = value.PadLeft(detail.Length, detail.PaddingCharacter[0]);
                        }
                    }
                    else
                    {
                        value = detail.ConstantValue;
                    }

                    if (tempRow.GetType().GetProperty(detail.PropertyName) == null)
                    {
                        throw new Exception("DecodedRawData Property with name '" + detail.PropertyName + "' not found.");
                    }
                    else
                    {
                        if (detail.Multiplier != 0 && tempRow.GetType().GetProperty(detail.PropertyName).GetCustomAttributes(typeof(RawDataNumericAttribute), false).FirstOrDefault() != null)
                        {
                            RawDataNumericAttribute attrib = tempRow.GetType().GetProperty(detail.PropertyName).GetCustomAttributes(typeof(RawDataNumericAttribute), false).FirstOrDefault() as RawDataNumericAttribute;
                            switch (attrib.Type)
                            {
                                case NumericType.INTEGER:
                                    int intVal = 0;
                                    if (int.TryParse(value, out intVal))
                                    {
                                        value = ((int)(intVal * detail.Multiplier)).ToString();
                                    }
                                    break;
                                case NumericType.DOUBLE:
                                    double doubleVal = 0;
                                    if (double.TryParse(value, out doubleVal))
                                    {
                                        value = (doubleVal * detail.Multiplier).ToString();
                                    }
                                    break;
                            }
                        }

                        if (tempRow.GetType().GetProperty(detail.PropertyName).PropertyType == typeof(bool))
                        {
                            bool parcedValue = (value == "1");
                            tempRow.GetType().GetProperty(detail.PropertyName).SetValue(tempRow, parcedValue, null);
                        }
                        else
                        {
                            tempRow.GetType().GetProperty(detail.PropertyName).SetValue(tempRow, value, null);
                        }
                    }
                }
                tempRow.Save();

                return tempRow;
            }
        }

        protected bool InsertTempRowIntoDB(Guid tempRowOid, StreamWriter writer, Guid centralStoreOid, Guid fileOid)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DecodedRawData row = uow.GetObjectByKey<DecodedRawData>(tempRowOid);
                Assembly assembly = typeof(Item).Assembly;
                Type T = assembly.GetType("ITS.Retail.Model." + row.Head.EntityName);

                if (T == typeof(ItemCategory))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    InsertOrUpdateItemCategory(uow, row, writer, file.MultipleItemCategoryTrees, file.RootItemCategoryCode);
                }
                else if (T == typeof(SupplierNew))
                {
                    InsertOrUpdateSupplier(uow, row, writer);
                }
                else if (T == typeof(PriceCatalog))
                {
                    InsertOrUpdatePriceCatalog(uow, row, writer);
                }
                else if (T == typeof(Store))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    Store centralStore = uow.GetObjectByKey<Store>(centralStoreOid);
                    Role storeUserRole = uow.FindObject<Role>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Description", file.StoreUserRoleDescription), typeof(Role), row.Owner));
                    InsertOrUpdateStore(uow, row, writer, true, storeUserRole, centralStore);
                }
                else if (T == typeof(Item))
                {
                    InsertOrUpdateItem(uow, row, writer);
                }
                else if (T == typeof(Barcode))
                {
                    InsertOrUpdateBarcode(uow, row, writer);
                }
                else if (T == typeof(LinkedItem))
                {
                    InsertOrUpdateLinkedItem(uow, row, writer);
                }
                else if (T == typeof(PriceCatalogDetail))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    InsertOrUpdatePriceCatalogDetail(uow, row, writer, file.MakeZeroPricesUserDefinable);
                }
                else if (T == typeof(PriceCatalogDetailTimeValue))
                {
                    InsertOrUpdatePriceCatalogDetailTimeValue(uow, row, writer);
                }
                else if (T == typeof(Customer))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    Role customerUserRole = uow.FindObject<Role>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Description", file.CustomerUserRoleDescription), typeof(Role), row.Owner));
                    InsertOrUpdateCustomer(uow, row, writer, file.CreateUsersForCustomers, customerUserRole, file.OneStorePerCustomer);
                }
                else if (T == typeof(Offer))
                {
                    InsertOrUpdateOffer(uow, row, writer);
                }
                else if (T == typeof(OfferDetail))
                {
                    InsertOrUpdateOfferDetail(uow, row, writer);
                }
                else if (T == typeof(MeasurementUnit))
                {
                    InsertOrUpdateMeasurentUnit(uow, row, writer);
                }
                else if (T == typeof(BarcodeType))
                {
                    InsertOrUpdateBarcodeType(uow, row, writer);
                }
                else if (T == typeof(ItemCategoryImportData))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    InsertOrUpdateItemCategoryImportData(uow, row, writer,file.RootItemCategoryCode);
                }
                else if(T==typeof(ItemAnalyticTree))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    InsertOrUpdateItemAnaylticTree(uow, row, writer);
                }
                else if(T==typeof(ItemExtraInfo))
                {
                    DataFileReceived file = uow.GetObjectByKey<DataFileReceived>(fileOid);
                    InsertOrUpdateItemExtraInfo(uow, row, writer);
                }
                else if (T == typeof(Leaflet))
                {
                    InsertOrUpdateLeaflet(uow, row, writer);
                }
                else if (T == typeof(LeafletDetail))
                {
                    InsertOrUpdateLeafletDetail(uow, row, writer);
                }
                else if (T == typeof(LeafletStore))
                {
                    InsertOrUpdateLeafletStore(uow, row, writer);
                }
                return true;
            }
        }

        

        private void InsertOrUpdatePriceCatalogDetailTimeValue(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", row.Code));//row.Code;
            decimal timeValue = 0;//row.TimeValue
            DateTime validFrom = DateTime.MinValue;//row.TimeValueValidFromDate
            DateTime validUntil = DateTime.MaxValue;//row.TimeValueValidUntilDate
            PriceCatalog priceCatalog = null;//row.PriceCatalog
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            
            if (barcode == null)
            {
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot find Barcode {0}.", row.Code));
                return;
            }
            if (!decimal.TryParse(row.TimeValue, out timeValue))
            {
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot parse to decimal timevalue {0}.Please provide a valid time value.", row.TimeValue));
                return;
            }
            if (!DateTime.TryParse(row.TimeValueValidFromDate, out validFrom))
            {
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot parse to date time (TimeValueValidFromDate) value {0}.Please provide a valid date time value.", row.TimeValueValidFromDate));
                return;
            }
            if (!DateTime.TryParse(row.TimeValueValidUntilDate, out validUntil))
            {
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot parse to date time (TimeValueValidUntilDate) value {0}.Please provide a valid time value.", row.TimeValueValidUntilDate));
                return;
            }
            priceCatalog = uow.FindObject<PriceCatalog>(new BinaryOperator("Code", row.PriceCatalog));
            if (priceCatalog == null)
            {
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot find PriceCatalog with Code {0}.", row.PriceCatalog));
                return;
            }
            CriteriaOperator priceCatalogDetailTimeValueCriteria =
                string.IsNullOrWhiteSpace(row.RemoteReferenceId) ?
                CriteriaOperator.And(new BinaryOperator("PriceCatalogDetail.PriceCatalog", priceCatalog.Oid),
                                                                                        new BinaryOperator("PriceCatalogDetail.Barcode", barcode.Oid),
                                                                                        new BinaryOperator("TimeValueValidFrom", validFrom.Ticks),
                                                                                        new BinaryOperator("TimeValueValidUntil", validUntil.Ticks)
                                                                                       )
                                                                                       :
                                                                                       new BinaryOperator("ReferenceId", row.RemoteReferenceId);
            PriceCatalogDetailTimeValue priceCatalogDetailTimeValue = uow.FindObject<PriceCatalogDetailTimeValue>(priceCatalogDetailTimeValueCriteria);
            if (priceCatalogDetailTimeValue == null)
            {
                priceCatalogDetailTimeValue = new PriceCatalogDetailTimeValue(uow);
                if (string.IsNullOrWhiteSpace(row.RemoteReferenceId)== false)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                        {
                            priceCatalogDetailTimeValue.ReferenceId = row.RemoteReferenceId;
                        }
                    }
                }
                    CriteriaOperator priceCatalogDetailCriteria = CriteriaOperator.And(new BinaryOperator("PriceCatalog", priceCatalog.Oid),
                                                                                   new BinaryOperator("Barcode", barcode.Oid)
                                                                                  );
                PriceCatalogDetail priceCatalogDetail = uow.FindObject<PriceCatalogDetail>(priceCatalogDetailCriteria);
                if (priceCatalogDetail == null)
                {
                    writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Cannot find the PriceCatalogDetail for Barcode {0}.", row.Code));
                    return;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                    {
                        priceCatalogDetailTimeValue.PriceCatalogDetail = priceCatalogDetail;
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValue")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValue")).Count() == 1)
                {
                    priceCatalogDetailTimeValue.TimeValue = timeValue;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValueValidFromDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValueValidFromDate")).Count() == 1)
                {
                    priceCatalogDetailTimeValue.TimeValueValidFrom = validFrom.Ticks;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValueValidUntilDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TimeValueValidUntilDate")).Count() == 1)
                {
                    priceCatalogDetailTimeValue.TimeValueValidUntil = validUntil.Ticks;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    priceCatalogDetailTimeValue.IsActive = row.IsActive;
                }
            }
            PlatformPriceCatalogDetailService platformPriceCatalogDetailService = new PlatformPriceCatalogDetailService();
            ValidationPriceCatalogDetailTimeValuesResult validationPriceCatalogDetailTimeValuesResult = platformPriceCatalogDetailService.ValidatePriceCatalogDetailTimeValues(priceCatalogDetailTimeValue.PriceCatalogDetail.TimeValues);
            if (validationPriceCatalogDetailTimeValuesResult != null)
            {
                string errorMessage = string.Empty;
                if (validationPriceCatalogDetailTimeValuesResult.FromGreaterThanTo)
                {
                    errorMessage = string.Format("Time value cannot be set to be active from {0} until {1}", validationPriceCatalogDetailTimeValuesResult.From.ToString(), validationPriceCatalogDetailTimeValuesResult.To.ToString());
                }
                else if (validationPriceCatalogDetailTimeValuesResult.PartialOverlap)
                {
                    errorMessage = string.Format("There is a partial overlapping for date time from {0} until {1}", validationPriceCatalogDetailTimeValuesResult.From.ToString(), validationPriceCatalogDetailTimeValuesResult.To.ToString());
                }
                writer.Write(string.Format("InsertOrUpdatePriceCatalogDetailTimeValue | Error: Time Limits are not valid {0}.", errorMessage));
                return;
            }

            priceCatalogDetailTimeValue.Save();
            XpoHelper.CommitChanges(uow);
        }

        private void InsertOrUpdateBarcodeType(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string code = row.Code;
            string description = row.Description;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            if (string.IsNullOrEmpty(code))
            {
                writer.Write(string.Format("Error: Cannot create Barcode Type with code {0}.Please provide a non empty code.", code));
                return;
            }

            CriteriaOperator criteria = CriteriaOperator.Or(new BinaryOperator("Description", description), new BinaryOperator("Code", code));
            criteria = RetailHelper.ApplyOwnerCriteria(criteria, typeof(BarcodeType), row.Owner);
            BarcodeType barcodeType = uow.FindObject<BarcodeType>(criteria);
            if (barcodeType == null)
            {
                barcodeType = new BarcodeType(uow);
            }

            barcodeType.Owner = row.Owner;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).FirstOrDefault().AllowNull == false)
                {
                    barcodeType.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).FirstOrDefault().AllowNull == false)
                {
                    barcodeType.Description = description;
                }
            }
            barcodeType.Save();
            XpoHelper.CommitChanges(uow);
        }

        private void InsertOrUpdateMeasurentUnit(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string code = row.Code;
            string description = row.Description;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            if (string.IsNullOrEmpty(code))
            {
                writer.Write(string.Format("Error: Cannot create Measurement Unit with code {0}.Please provide a non empty code.", code));
                return;
            }

            CriteriaOperator criteria = CriteriaOperator.Or(new BinaryOperator("Description", description), new BinaryOperator("Code", code));
            criteria = RetailHelper.ApplyOwnerCriteria(criteria, typeof(MeasurementUnit), row.Owner);
            MeasurementUnit measurementUnit = uow.FindObject<MeasurementUnit>(criteria);
            if (measurementUnit == null)
            {
                measurementUnit = new MeasurementUnit(uow);
            }

            measurementUnit.Owner = row.Owner;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    measurementUnit.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    measurementUnit.Description = description;
                }
            }
            measurementUnit.Save();
            XpoHelper.CommitChanges(uow);
        }

        protected void SetMotherCodes(UnitOfWork uow, XPCursor tempRows, CompanyNew owner)
        {
            foreach (DecodedRawData row in tempRows)
            {
                string motherCode = row.MotherCode;
                string code = row.Code;
                OwnerApplicationSettings appiSettings = row.Owner.OwnerApplicationSettings;
                var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

                if (!String.IsNullOrWhiteSpace(motherCode))
                {
                    if (appiSettings.PadItemCodes)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                        {
                            code = code.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MotherCode")).Count() == 1)
                        {
                            motherCode = motherCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                        }
                        
                    }
                    if (code != motherCode)
                    {
                        Item item = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", code), new BinaryOperator("Owner.Oid", owner.Oid)));
                        if (item.MotherCode == null)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MotherCode")).Count() == 1)
                            {
                                item.MotherCode = uow.FindObject<Item>(CriteriaOperator.And(new BinaryOperator("Code", motherCode), new BinaryOperator("Owner.Oid", owner.Oid)));
                            }
                            item.Save();
                            XpoHelper.CommitChanges(uow);
                        }

                    }
                }
            }
        }

        protected void InsertOrUpdateItemCategory(UnitOfWork uow, DecodedRawData row, StreamWriter writer, bool multipleCategoryTrees, string rootCategoryCode)
        {
            string level1 = row.Level1;
            string level2 = row.Level2;
            string level3 = row.Level3;
            string level4 = row.Level4;
            string description = row.Description;
            string points = row.Points;
            bool isActive = row.IsActive;
            CompanyNew owner = row.Owner;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            ItemCategory rootCategory = null;

            if (!multipleCategoryTrees)
            {
                rootCategory = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", rootCategoryCode), typeof(ItemCategory), row.Owner));
                if (rootCategory == null)
                {
                    rootCategory = new ItemCategory(uow);
                    rootCategory.Owner = owner;
                    rootCategory.Code = rootCategoryCode;
                    rootCategory.Description = rootCategoryCode;
                    rootCategory.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }


            string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            decimal parsedPoints = -1;

            if (!ParseDecimalValue(points, dec_seperator[0], out parsedPoints))
            {
                string message = "Item Category: '" + level1 + "-" + level2 + "-" + level3 + "-" + level4 + "';Invalid Points: '" + points + "'";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemCategory;Warning: " + message);
            }

            if (level4 == "\0" || String.IsNullOrWhiteSpace(level4))
            {
                if (level3 == "\0" || String.IsNullOrWhiteSpace(level3))
                {
                    if (level2 == "\0" || String.IsNullOrWhiteSpace(level2))
                    {

                        string currentCode = level1.Trim();
                        if (!String.IsNullOrWhiteSpace(currentCode))
                        {
                            ItemCategory ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.Parse("Code='" + currentCode + "'", ""), typeof(ItemCategory), row.Owner));
                            if (ic == null)
                            {
                                ic = new ItemCategory(uow);
                                ic.Owner = owner;
                            }
                            ic.Parent = rootCategory;
                            ic.Code = currentCode;
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                            {
                                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).FirstOrDefault().AllowNull == false)
                                {
                                    ic.Description = description.Trim();
                                }
                            }
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                            {
                                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).FirstOrDefault().AllowNull == false)
                                {
                                    ic.IsActive = isActive;
                                }
                            }
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
                            {
                                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).FirstOrDefault().AllowNull == false)
                                {
                                    ic.Points = parsedPoints != -1 ? parsedPoints : ic.Points;
                                }
                            }
                            ic.Save();
                        }
                    }
                    else
                    {

                        string currentCode = level1.Trim() + "-" + level2.Trim();
                        string parentCode = level1.Trim();
                        ItemCategory parent = null;

                        if (!multipleCategoryTrees)
                        {
                            parent = uow.FindObject<ItemCategory>((RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", parentCode),
                                                                                                                    new BinaryOperator("Parent", rootCategory)),
                                                                                                                    typeof(ItemCategory), row.Owner)));
                        }
                        else
                        {
                            parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCode), typeof(ItemCategory), owner));
                        }
                        ItemCategory ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", currentCode)),
                                                                                                                            typeof(ItemCategory), owner));
                        if (ic == null)
                        {
                            ic = new ItemCategory(uow);
                            ic.Owner = owner;
                        }
                        ic.Parent = parent;
                        ic.Code = currentCode;
                        ic.Description = description.Trim();
                        ic.IsActive = isActive;
                        ic.Points = parsedPoints != -1 ? parsedPoints : ic.Points;
                        ic.Save();
                    }

                }
                else
                {
                    string currentCode = level1.Trim() + "-" + level2.Trim() + "-" + level3.Trim();
                    string parentCode = level1.Trim() + "-" + level2.Trim();
                    string parentOfParentCode = level1.Trim();

                    ItemCategory parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", parentCode),
                                                                                                                            new BinaryOperator("Parent.Code", parentOfParentCode)),
                                                                                                                            typeof(ItemCategory), owner));

                    ItemCategory ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", currentCode)), typeof(ItemCategory), owner));
                    if (ic == null)
                    {
                        ic = new ItemCategory(uow);
                        ic.Owner = owner;
                    }
                    ic.Parent = parent;
                    ic.Code = currentCode;
                    ic.Description = description.Trim();
                    ic.IsActive = isActive;
                    ic.Points = parsedPoints != -1 ? parsedPoints : ic.Points;
                    ic.Save();
                }
            }
            else
            {
                string currentCode = level1.Trim() + "-" + level2.Trim() + "-" + level3.Trim() + "-" + level4.Trim();
                string parentCode = level1.Trim() + "-" + level2.Trim() + "-" + level3.Trim();
                string parentOfParentCode = level1.Trim() + "-" + level2.Trim();
                ItemCategory parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", parentCode),
                                                                                                                        new BinaryOperator("Parent.Code", parentOfParentCode)), typeof(ItemCategory), owner));
                ItemCategory ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", currentCode)), typeof(ItemCategory), owner));
                if (ic == null)
                {
                    ic = new ItemCategory(uow);
                    ic.Owner = owner;
                }
                ic.Parent = parent;
                ic.Code = currentCode;
                ic.Description = description.Trim();
                ic.IsActive = isActive;
                ic.Points = parsedPoints != -1 ? parsedPoints : ic.Points;
                ic.Save();
            }
            XpoHelper.CommitChanges(uow);
        }

        protected void InsertOrUpdateSupplier(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string code = row.Code;
            string taxCode = row.TaxCode;
            string companyName = row.CompanyName;
            string address = row.Address;
            string area = row.Region;
            string city = row.City;
            string poCode = row.ZipCode;
            string fax = row.Fax;
            bool isActive = row.IsActive;
            string phoneNumber = row.Phone;
            string profession = row.Profession;
            string taxOffice = row.TaxOffice;
            string vatLevel = row.VatCategory;
            bool isDefaultAddress = row.IsDefaultAddress;
            string addressProfession = row.AddressProfession;
            string thirdPartNum = row.ThirdPartNumber;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            if (String.IsNullOrWhiteSpace(taxCode))
            {
                string message = "Supplier Code: '" + code + "'; Tax Code is empty. Suplier will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateSupplier;Error: " + message);
                return;
            }
                        
            SupplierNew supplier = uow.FindObject<SupplierNew>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(SupplierNew), row.Owner));

            if (supplier == null)
            {
                supplier = new SupplierNew(uow);
                //supplier.Trader = new Trader(uow);
                supplier.Owner = row.Owner;
            }

            Trader trader = uow.FindObject<Trader>(new BinaryOperator("TaxCode", taxCode));
            if (trader == null)
            {
                trader = new Trader(uow);
            }
            supplier.Trader = trader;

            if (!String.IsNullOrEmpty(row.Address))
            {
                Address traderAddress = null;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).FirstOrDefault().UseThirdPartNum)
                    {
                        traderAddress = uow.FindObject<Address>(CriteriaOperator.And(new BinaryOperator("ThirdPartNum", thirdPartNum),
                                                                             new BinaryOperator("Trader", trader)));
                    }

                }
                else
                {
                    traderAddress = uow.FindObject<Address>(CriteriaOperator.And(new BinaryOperator("Street", address),
                                                                             new BinaryOperator("Trader", trader),
                                                                             new BinaryOperator("City", city),
                                                                             new BinaryOperator("PostCode", poCode)));
                }

                if (traderAddress == null)
                {
                    traderAddress = new Address(uow);
                }

                Phone phone = null;
                if (!String.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != "\0")
                {
                    phone = uow.FindObject<Phone>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Number", phoneNumber),
                                                                    new BinaryOperator("Address", traderAddress)), typeof(Phone), row.Owner));
                    if (phone == null)
                    {
                        phone = new Phone(uow);
                        phone.Owner = row.Owner;
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).FirstOrDefault().AllowNull == false)
                        {
                            phone.Number = phoneNumber;
                        }
                    }
                    phone.PhoneType = uow.FindObject<PhoneType>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(PhoneType), row.Owner));
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                        {
                            phone.Address = traderAddress;
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(fax) && fax != "\0")
                {
                    Phone phoneFax = uow.FindObject<Phone>(CriteriaOperator.And(new BinaryOperator("Number", fax), new BinaryOperator("Address", traderAddress)));
                    if (phoneFax == null)
                    {
                        phoneFax = new Phone(uow);
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Fax")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Fax")).FirstOrDefault().AllowNull == false)
                        {
                            phoneFax.Number = fax;
                        }
                    }
                    phoneFax.PhoneType = uow.FindObject<PhoneType>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", false), typeof(PhoneType), row.Owner));
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                        {
                            phoneFax.Address = traderAddress;
                        }
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.City = city;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.DefaultPhone = phone;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ZipCode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ZipCode")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.PostCode = poCode;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.Street = address;
                    }
                }
                traderAddress.Region = area;
                traderAddress.Trader = trader;

                if (isDefaultAddress)
                {
                    supplier.DefaultAddress = traderAddress;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("AddressProfession")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("AddressProfession")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.Profession =addressProfession;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).FirstOrDefault().AllowNull == false)
                    {
                        traderAddress.ThirdPartNum = thirdPartNum;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(vatLevel))
            {
                supplier.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", vatLevel), typeof(VatLevel), row.Owner));

                if (supplier.VatLevel == null)
                {
                    supplier.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(VatLevel), row.Owner));
                }
            }
            else
            {
                supplier.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(VatLevel), row.Owner));
            }

            string message2 = "Supplier Code: " + code;
            if (supplier.VatLevel != null)
            {

                if (vatLevel == null)
                {
                    message2 += "; Vat Level with Code: '" + vatLevel + "' does not exist;The Default Vat Level (" + supplier.VatLevel.Code +
                               ", " + supplier.VatLevel.Description + ") was used.";
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error:" + message2);
                }
            }
            else
            {
                message2 += ";InsertOrUpdateCustomer;Error: Vat Level with Code: '" + vatLevel + " does not exist; A Default Vat Level does not exist; Customer Vat Level was NOT set.";
                writer.WriteLine(DateTime.Now.ToString() + message2);
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    supplier.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CompanyName")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CompanyName")).Count() == 1)
                {
                    supplier.CompanyName = companyName;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Profession")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Profession")).Count() == 1)
                {
                    supplier.Profession = profession;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxCode")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxCode")).Count() == 1)
                {
                    supplier.Trader.TaxCode = row.TaxCode;  //Δεν έχει οριστεί (Βασικός προμηθευτής ειδών).
                }
            }
            //string taxOffice = row.TaxOffice; //Δεν έχει οριστεί (Βασικός προμηθευτής ειδών).
            
            TaxOffice txOffice = uow.FindObject<TaxOffice>(new BinaryOperator("Code", taxOffice));
            if (txOffice != null)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).Count() == 1)
                    {
                        supplier.Trader.TaxOfficeLookUp = txOffice;
                    }
                }
            }
            else
            {
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateSupplier;Warning: Tax Office with code" + taxOffice + " not found");
            }
            supplier.Owner = row.Owner;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).Count() == 1)
                {
                    supplier.IsActive = isActive;
                }
            }
            supplier.Save();
            XpoHelper.CommitChanges(uow);
        }

        protected void InsertOrUpdatePriceCatalog(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string code = row.Code;
            string description = row.Description;
            string parentCatalog = row.PriceCatalog;
            bool isActive = row.IsActive;
            string endDate = row.EndDate;
            string startDate = row.FromDate;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            
            CompanyNew owner = row.Owner;

            PriceCatalog pc = uow.FindObject<PriceCatalog>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(PriceCatalog), owner));

            if (pc == null)
            {
                pc = new PriceCatalog(uow);
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    pc.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    pc.Description = description;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
                {
                    if (String.IsNullOrWhiteSpace(startDate))
                    {
                        pc.StartDate = new DateTime(2000, 01, 01);
                    }
                    else
                    {
                        pc.StartDate = DateTime.Parse(startDate);
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
                {
                    if (String.IsNullOrWhiteSpace(endDate))
                    {
                        pc.EndDate = DateTime.MaxValue;
                    }
                    else
                    {
                        pc.EndDate = DateTime.Parse(endDate);
                    }
                }
            }
            pc.Owner = owner;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                {
                    pc.ParentCatalog = uow.FindObject<PriceCatalog>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCatalog), typeof(PriceCatalog), owner)); //parentCatalogObj;
                    pc.IsRoot = pc.ParentCatalog == null;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    pc.IsActive = isActive;
                }
            }
            pc.Save();
            XpoHelper.CommitChanges(uow);
        }

        protected void InsertOrUpdateStore(UnitOfWork uow, DecodedRawData row, StreamWriter writer, bool createStoreUsers, Role storeUserRole, Store centralStore)
        {
            string code = row.Code;
            string name = row.Description;
            string city = row.City;
            bool isActive = row.IsActive;
            string defaultPriceCatalogPolicyCode = row.DefaultPriceCatalogPolicy;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            PriceCatalogPolicy defaultPriceCatalogPolicy = uow.FindObject<PriceCatalogPolicy>(new BinaryOperator("Code", defaultPriceCatalogPolicyCode));
            if (string.IsNullOrEmpty(defaultPriceCatalogPolicyCode) == false && defaultPriceCatalogPolicy == null)
            {
                string message = "Store Code: '" + code + "'; PriceCatalogPolicy with code '" + defaultPriceCatalogPolicyCode + "' was not found. Store will not be inserted/updated.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                return;
            }

            Store store = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(Store), row.Owner));

            if (store == null)
            {
                store = new Store(uow);
                Address address = new Address(uow);
                address.Store = store;
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).Count() == 1)
                {
                    store.Address.City = city;
                }
            }
            store.Address.Trader = row.Owner.Trader;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    store.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    store.Name = name;
                }
            }
            store.Owner = row.Owner;
            store.Central = centralStore;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    store.IsActive = isActive;
                }
            }
            store.Save();
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("DefaultPriceCatalogPolicy")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("DefaultPriceCatalogPolicy")).Count() == 1)
                {
                    store.DefaultPriceCatalogPolicy = defaultPriceCatalogPolicy;
                }
            }
            XpoHelper.CommitChanges(uow);

            if (createStoreUsers)  //Create User for this Store
            {
                User storeUser = uow.FindObject<User>(new BinaryOperator("UserName", store.Code));
                if (storeUser == null)  //If user exists don't change
                {
                    storeUser = new User(uow);
                    storeUser.UserName = store.Code;
                    storeUser.Password = UserHelper.EncodePassword(store.Code);
                    storeUser.FullName = store.Name;
                    storeUser.Role = storeUserRole;
                    storeUser.IsApproved = isActive;
                    storeUser.Save();

                    UserTypeAccess association = uow.FindObject<UserTypeAccess>(CriteriaOperator.And(new BinaryOperator("User.Oid", storeUser.Oid),
                                                                                                     new BinaryOperator("EntityOid", store.Owner.Oid)));
                    if (association == null)
                    {
                        association = new UserTypeAccess(uow);
                    }
                    association.IsActive = true;
                    association.User = storeUser;
                    association.EntityOid = store.Owner.Oid;
                    association.EntityType = typeof(CompanyNew).FullName;
                    association.Save();

                    UserTypeAccess storeAccess = uow.FindObject<UserTypeAccess>(CriteriaOperator.And(new BinaryOperator("User.Oid", storeUser.Oid),
                                                                                                     new BinaryOperator("EntityOid", store.Oid)));
                    if (storeAccess == null)
                    {
                        storeAccess = new UserTypeAccess(uow);
                    }
                    storeAccess.IsActive = true;
                    storeAccess.User = storeUser;
                    storeAccess.EntityOid = store.Oid;
                    storeAccess.EntityType = "ITS.Retail.Model.Store";
                    storeAccess.Save();

                    XpoHelper.CommitTransaction(uow);

                }
            }
        }


        protected void InsertOrUpdateItem(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string itemSupplierCode = row.SupplierCode;
            string itemCategoryCode = "";
            string itemCategoryParentCode = "";
            string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            OwnerApplicationSettings appiSettings = row.Owner.OwnerApplicationSettings;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            if (!String.IsNullOrWhiteSpace(row.Level4))
            {
                itemCategoryCode = row.Level1 + "-" + row.Level2 + "-" + row.Level3 + "-" + row.Level4;
                itemCategoryParentCode = row.Level1 + "-" + row.Level2 + "-" + row.Level3;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(row.Level3))
                {
                    itemCategoryCode = row.Level1 + "-" + row.Level2 + "-" + row.Level3;
                    itemCategoryParentCode = row.Level1 + "-" + row.Level2;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(row.Level2))
                    {
                        itemCategoryCode = row.Level1 + "-" + row.Level2;
                        itemCategoryParentCode = row.Level1;
                    }
                    else
                    {
                        itemCategoryCode = row.Level1;
                        itemCategoryParentCode = "";
                    }
                }
            }

            string code = row.Code;
            if (appiSettings.PadItemCodes)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    code = code.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                }
            }

            string barcodeFromItemCode = code;
            if (appiSettings.PadBarcodes)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    barcodeFromItemCode = barcodeFromItemCode.PadLeft(appiSettings.BarcodeLength, appiSettings.BarcodePaddingCharacter[0]);
                }
            }

            string packingQty = "";
            bool isActive=true;
            string name = "";
            string buyerCode = "";
            string seasonalityCode = "";
            string vatCategory = "";
            string measurementUnit = "";
            string packingMeasurementUnit = "";
            string insertedDate = "";
            string centralStoreCode = "";
            string orderQty = "";
            string maxOrderQty = "";
            string points = "";
            string customPriceOptions = "";
            string contentUnit = "";
            string referenceUnit = "";

            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingQuantity")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingQuantity")).FirstOrDefault().AllowNull == false)
                {
                    packingQty = row.PackingQuantity;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).FirstOrDefault().AllowNull == false)
                {
                    isActive = row.IsActive;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).FirstOrDefault().AllowNull == false)
                {
                    name = row.Description;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Buyer")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Buyer")).FirstOrDefault().AllowNull == false)
                {
                    buyerCode = row.Buyer;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Seasonality")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Seasonality")).FirstOrDefault().AllowNull == false)
                {
                    seasonalityCode = row.Seasonality;
                }
            }           
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("VatCategory")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("VatCategory")).FirstOrDefault().AllowNull == false)
                {
                    vatCategory = row.VatCategory;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MeasurmentUnit")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MeasurmentUnit")).FirstOrDefault().AllowNull == false)
                {
                    measurementUnit = row.MeasurmentUnit;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingMeasurmentUnit")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingMeasurmentUnit")).FirstOrDefault().AllowNull == false)
                {
                    packingMeasurementUnit = row.PackingMeasurmentUnit;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("InsertedDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("InsertedDate")).FirstOrDefault().AllowNull == false)
                {
                    insertedDate = row.InsertedDate;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CentralStoreCode")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CentralStoreCode")).FirstOrDefault().AllowNull == false)
                {
                    centralStoreCode = row.CentralStoreCode;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MinOrderQty")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MinOrderQty")).FirstOrDefault().AllowNull == false)
                {
                    orderQty = row.MinOrderQty;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MaxItemOrderQty")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MaxItemOrderQty")).FirstOrDefault().AllowNull == false)
                {
                    maxOrderQty = row.MaxItemOrderQty;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).FirstOrDefault().AllowNull == false)
                {
                    points = row.Points;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CustomPriceOptions")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CustomPriceOptions")).FirstOrDefault().AllowNull == false)
                {
                    customPriceOptions = row.CustomPriceOptions;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ContentUnit")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ContentUnit")).FirstOrDefault().AllowNull == false)
                {
                    contentUnit = row.ContentUnit;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ReferenceUnit")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ReferenceUnit")).FirstOrDefault().AllowNull == false)
                {
                    referenceUnit = row.ReferenceUnit;
                }
            }

            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(Item), row.Owner));

            if (item == null)
            {
                item = new Item(uow);
            }

            if (itemSupplierCode != null)
            {
                item.DefaultSupplier = uow.FindObject<SupplierNew>(new BinaryOperator("Code", itemSupplierCode));
            }
            item.Owner = row.Owner;

            ItemCategory ic = null;

            if (String.IsNullOrWhiteSpace(itemCategoryParentCode))
            {
                ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCategoryCode), typeof(ItemCategory), row.Owner));
            }
            else
            {
                ic = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Code", itemCategoryCode), new BinaryOperator("Parent.Code", itemCategoryParentCode)), typeof(ItemCategory), row.Owner));
            }

            if (ic != null)
            {
                CategoryNode root = ic.GetRoot(uow);
                ItemAnalyticTree iat = uow.FindObject<ItemAnalyticTree>(CriteriaOperator.And(new BinaryOperator("Object.Code", code), new BinaryOperator("Root", root)));
                if (iat == null)
                {
                    iat = new ItemAnalyticTree(uow);
                }
                iat.Object = item;
                iat.Node = ic;
                iat.Root = root;
                iat.Save();
            }
            else if (!string.IsNullOrWhiteSpace(itemCategoryCode))
            {
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: Item Code: '" + code + "';Cannot find Item Category with Code: '" + itemCategoryCode + "'");
            }

            //barcode from itemcode
            //--
            ItemBarcode existingIbc = uow.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Code", barcodeFromItemCode), new BinaryOperator("Item.Owner.Oid", row.Owner.Oid)));
            if (existingIbc == null)
            {
                Barcode bc = uow.FindObject<Barcode>(new BinaryOperator("Code", barcodeFromItemCode));
                if (bc == null)
                {
                    bc = new Barcode(uow);
                }
                bc.Code = barcodeFromItemCode;

                item.DefaultBarcode = bc;
                ItemBarcode ibc = uow.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Item.Oid", item.Oid), new BinaryOperator("Barcode.Oid", bc.Oid)));
                if (ibc == null)
                {
                    ibc = new ItemBarcode(uow);
                    ibc.Owner = item.Owner;
                    ibc.MeasurementUnit = uow.FindObject<MeasurementUnit>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And
                                                                         (new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.PACKING, BinaryOperatorType.NotEqual),
                                                                         CriteriaOperator.Or(new BinaryOperator("Description", measurementUnit),
                                                                                             new BinaryOperator("Code", measurementUnit))),
                                                                                                                typeof(MeasurementUnit), row.Owner));
                    if (ibc.MeasurementUnit == null)
                    {
                        string message = "Item Code: '" + code + "';Cannot find Measurement Unit with Code: '" + measurementUnit + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Error: " + message);
                        return;
                    }
                    ibc.RelationFactor = 1;
                }
                ibc.Barcode = bc;
                ibc.Item = item;
                ibc.Save();
            }
            else
            {
                if (existingIbc.Item != null && existingIbc.Item.Oid != item.Oid)
                {
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Error: Item Code: '" + code + "';Could not create barcode from Item Code. Barcode '" + barcodeFromItemCode + "' is already assigned to Item '" + existingIbc.Item.Code + "'");
                }
            }
            //-----

            item.VatCategory = uow.FindObject<VatCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", vatCategory), typeof(VatCategory), row.Owner));

            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("VatCategory")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("VatCategory")).FirstOrDefault().AllowNull == false)
                {
                    if (item.VatCategory == null)
                    {
                        string message = "Item Code: '" + code + "';Cannot find Vat Category with Code: '" + vatCategory + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Error: " + message);
                        return;
                    }
                    
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).FirstOrDefault().AllowNull == false)
                {
                    item.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).FirstOrDefault().AllowNull == false)
                {
                    item.Name = name;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("InsertedDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("InsertedDate")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(insertedDate))
                    {
                        try
                        {
                            item.InsertedDate = DateTime.Parse(insertedDate);
                        }
                        catch (Exception ex)
                        {
                            string exceptionMessage = ex.GetFullMessage();
                            string message = "Item Code: '" + code + ";Could not parse inserted date '" + insertedDate + "'";
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                        }
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CentralStoreCode")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CentralStoreCode")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(centralStoreCode))
                    {
                        Store centralStore = uow.FindObject<Store>(new BinaryOperator("Code", centralStoreCode));
                        if (centralStore != null)
                        {
                            ItemStore itemStore = uow.FindObject<ItemStore>(CriteriaOperator.And(new BinaryOperator("Store.Code", centralStoreCode),
                                                                                                 new BinaryOperator("Item.Code", code)));
                            if (itemStore == null)
                            {
                                itemStore = new ItemStore(uow);
                            }
                            itemStore.Store = centralStore;
                            itemStore.Item = item;
                            itemStore.Save();
                        }
                        else
                        {
                            string message = "Item Code: '" + code + "';Cannot find Store with Code: '" + centralStoreCode + "'. ItemStore will not be created.";
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                        }
                        item.IsCentralStored = true;
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MinOrderQty")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MinOrderQty")).FirstOrDefault().AllowNull == false)
                {
                    decimal parsedOrderQty = 0;
                    if (orderQty != null && ParseDecimalValue(orderQty, dec_seperator[0], out parsedOrderQty))
                    {
                        item.OrderQty = (double)parsedOrderQty;
                    }
                    else if (orderQty != null)
                    {
                        string message = "ItemCode: '" + code + "';Invalid Order Quantity: '" + orderQty + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MaxItemOrderQty")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("MaxItemOrderQty")).FirstOrDefault().AllowNull == false)
                {
                    decimal parsedMaxOrderQty = 0;
                    if (maxOrderQty != null && ParseDecimalValue(maxOrderQty, dec_seperator[0], out parsedMaxOrderQty))
                    {
                        item.MaxOrderQty = (double)parsedMaxOrderQty;
                    }
                    else if (maxOrderQty != null)
                    {
                        string message = "ItemCode: '" + code + "';Invalid Max Order Quantity: '" + maxOrderQty + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingQuantity")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingQuantity")).FirstOrDefault().AllowNull == false)
                {
                    decimal parsedPackingQty = 0;
                    if (packingQty != null && ParseDecimalValue(packingQty, dec_seperator[0], out parsedPackingQty))
                    {
                        item.PackingQty = (double)parsedPackingQty;
                    }
                    else if (packingQty != null)
                    {
                        string message = "ItemCode: '" + code + "';Invalid Packing Quantity: '" + packingQty + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).FirstOrDefault().AllowNull == false)
                {
                    decimal parsedPoints = 0;
                    if (points != null && ParseDecimalValue(points, dec_seperator[0], out parsedPoints))
                    {
                        item.Points = parsedPoints;
                    }
                    else if (points != null)
                    {
                        string message = "ItemCode: '" + code + "';Invalid Points: '" + points + "'";
                        writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CustomPriceOptions")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CustomPriceOptions")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(customPriceOptions))
                    {
                        int parsedCustomPriceOptions = 0;
                        if (int.TryParse(customPriceOptions, out parsedCustomPriceOptions))
                        {
                            try
                            {
                                eItemCustomPriceOptions finalCustomPriceOptions = (eItemCustomPriceOptions)parsedCustomPriceOptions;
                                item.CustomPriceOptions = finalCustomPriceOptions;
                            }
                            catch
                            {
                                string message = "ItemCode: '" + code + "';Invalid CustomPriceOptions: '" + customPriceOptions + "'.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                            }

                        }
                        else
                        {
                            string message = "ItemCode: '" + code + "';Invalid CustomPriceOptions: '" + customPriceOptions + "'";
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                        }
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Seasonality")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Seasonality")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(seasonalityCode))
                    {
                        Seasonality seasonality = uow.FindObject<Seasonality>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", seasonalityCode), typeof(Seasonality), row.Owner));
                        if (seasonality == null)
                        {
                            seasonality = new Seasonality(uow);
                            seasonality.Owner = row.Owner;
                        }
                        seasonality.Code = seasonalityCode;
                        seasonality.Description = seasonalityCode;
                        item.Seasonality = seasonality;
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Buyer")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Buyer")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(buyerCode))
                    {
                        Buyer buyer = uow.FindObject<Buyer>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", buyerCode), typeof(Buyer), row.Owner));
                        if (buyer == null)
                        {
                            buyer = new Buyer(uow);
                            buyer.Owner = row.Owner;
                        }
                        buyer.Code = buyerCode;
                        buyer.Description = buyerCode;
                        item.Buyer = buyer;
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingMeasurmentUnit")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackingMeasurmentUnit")).FirstOrDefault().AllowNull == false)
                {
                    if (!String.IsNullOrWhiteSpace(packingMeasurementUnit))
                    {
                        MeasurementUnit pmm = uow.FindObject<MeasurementUnit>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And
                                                                            (new BinaryOperator("MeasurementUnitType", eMeasurementUnitType.ORDER, BinaryOperatorType.NotEqual),
                                                                            CriteriaOperator.Or(new BinaryOperator("Description", packingMeasurementUnit),
                                                                                                new BinaryOperator("Code", packingMeasurementUnit))),
                                                                                                                                  typeof(MeasurementUnit), row.Owner));
                        if (pmm == null)
                        {
                            pmm = new MeasurementUnit(uow);
                            pmm.Code = packingMeasurementUnit;
                            pmm.Description = packingMeasurementUnit;
                            pmm.MeasurementUnitType = eMeasurementUnitType.PACKING;
                            pmm.Owner = row.Owner;
                            pmm.SupportDecimal = true;
                            string message = "Creating Packing Measurment Unit: Code or Description '" + packingMeasurementUnit + "' not found;";
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                        }
                        item.PackingMeasurementUnit = pmm;
                    }
                }
            }
            decimal contentUnitValue;
            if (string.IsNullOrWhiteSpace(contentUnit) == false && decimal.TryParse(contentUnit, out contentUnitValue))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ContentUnit")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ContentUnit")).FirstOrDefault().AllowNull == false)
                    {
                        item.ContentUnit = contentUnitValue;
                    }
                }
            }

            decimal referenceUnitValue;
            if (string.IsNullOrWhiteSpace(referenceUnit) == false && decimal.TryParse(referenceUnit, out referenceUnitValue))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ReferenceUnit")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ReferenceUnit")).FirstOrDefault().AllowNull == false)
                    {
                        item.ReferenceUnit = referenceUnitValue;
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).FirstOrDefault().AllowNull == false)
                {
                    item.IsActive = isActive;
                }
            }
            item.Save();
            XpoHelper.CommitChanges(uow);
        }

        private void ManageItemBarcode(Item item, Barcode barcode, string measurementUnit, StreamWriter streamWriter)
        {
            if (barcode == null)
            {
                barcode = new Barcode(item.Session);
            }
            barcode.Code = item.Code;

            item.DefaultBarcode = barcode;
            ItemBarcode ibc = item.Session.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Item.Oid", item.Oid), new BinaryOperator("Barcode.Oid", barcode.Oid)));
            if (ibc == null)
            {
                ibc = new ItemBarcode(item.Session);
                ibc.Owner = item.Owner;
                ibc.MeasurementUnit = item.Session.FindObject<MeasurementUnit>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.Or(new BinaryOperator("Description", measurementUnit),
                                                                                            new BinaryOperator("Code", measurementUnit)), typeof(MeasurementUnit), item.Owner));

                if (ibc.MeasurementUnit == null)
                {
                    string message = "Item Code: '" + item.Code + "';Cannot find Measurement Unit with Code: '" + measurementUnit + "'";
                    streamWriter.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Warning: " + message);
                    return;
                }
                ibc.RelationFactor = 1;
            }
            ibc.Barcode = barcode;
            ibc.Item = item;
            ibc.Save();
        }

        protected void InsertOrUpdateBarcode(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string itemCode = row.Item;
            string barcode = row.Barcode;
            string relationFactor = row.FactorQty;
            bool isActive = row.IsActive;
            string barcodeType = row.BarcodeTypeCode;
            OwnerApplicationSettings appiSettings = row.Owner.OwnerApplicationSettings;
            string pluCode = row.PluCode;
            string pluPrefix = row.PluPrefix;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            if (String.IsNullOrWhiteSpace(barcode))
            {
                string message = "Barcode is null or whitespace. Cannot create";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateBarcode;Error: " + message);
                return;
            }

            if (appiSettings.PadItemCodes)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).FirstOrDefault().AllowNull == false)
                    {
                        itemCode = itemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                    }
                }
            }

            string barcodeFromItemCode = itemCode;

            if (appiSettings.PadBarcodes)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).FirstOrDefault().AllowNull == false)
                    {
                        barcode = barcode.PadLeft(appiSettings.BarcodeLength, appiSettings.BarcodePaddingCharacter[0]);
                        barcodeFromItemCode = barcodeFromItemCode.PadLeft(appiSettings.BarcodeLength, appiSettings.BarcodePaddingCharacter[0]);
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(relationFactor))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).FirstOrDefault().AllowNull == false)
                    {
                        for (int j = 0; j < relationFactor.Length; j++)
                        {
                            if (!Char.IsDigit(relationFactor, j))
                            {
                                relationFactor = relationFactor.Remove(j, 1);
                                j--;
                            }
                        }
                    }
                }
            }
            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCode), typeof(Item), row.Owner));
            if (item != null)
            {
                Barcode bc = uow.FindObject<Barcode>(new BinaryOperator("Code", barcode));
                if (bc == null)
                {
                    bc = new Barcode(uow);
                }

                CriteriaOperator duplicateCriteria = new BinaryOperator("Barcode.Code", barcode);

                ItemBarcode existingIbc = uow.FindObject<ItemBarcode>(RetailHelper.ApplyOwnerCriteria(duplicateCriteria, typeof(ItemBarcode), row.Owner));
                ItemBarcode ibc;
                if (existingIbc == null)
                {
                    ibc = new ItemBarcode(uow);
                    ibc.Owner = item.Owner;
                }
                else
                {
                    ibc = existingIbc;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).FirstOrDefault().AllowNull == false)
                    {
                        ibc.Item = item;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).FirstOrDefault().AllowNull == false)
                    {
                        ibc.Barcode = bc;
                    }
                }
                if (string.IsNullOrWhiteSpace(pluCode) == false)
                {
                    
                        CriteriaOperator duplicatePLUCriteria = CriteriaOperator.And(
                                                                                    new BinaryOperator("PluCode", pluCode),
                                                                                    new BinaryOperator("Oid", ibc.Oid, BinaryOperatorType.NotEqual)
                                                                                );

                        XPCollection<ItemBarcode> existingItemBarcodesWithSamePLU = new XPCollection<ItemBarcode>(uow,
                                                                                                                   RetailHelper.ApplyOwnerCriteria(duplicatePLUCriteria,
                                                                                                                                                    typeof(ItemBarcode),
                                                                                                                                                    row.Owner
                                                                                                                                                   )
                                                                                                                 );
                        if (existingItemBarcodesWithSamePLU.Count > 0)
                        {
                            foreach (ItemBarcode existingPLUItemBarcode in existingItemBarcodesWithSamePLU)
                            {

                                writer.WriteLine(string.Format("{0} : ( {1} - {2} ) {3}",
                                                                Resources.BarcodeAlreadyAttached,
                                                                existingPLUItemBarcode.Item.Code,
                                                                existingPLUItemBarcode.Item.Name,
                                                                Environment.NewLine
                                                              )
                                                );
                            //TODO:if implements and save command...
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluCode")).Count() == 1)
                            {
                                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluCode")).FirstOrDefault().AllowNull == false)
                                {
                                    existingPLUItemBarcode.PluCode = string.Empty;
                                }
                            }
                            existingPLUItemBarcode.Save();
                            }
                        }
                    
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluCode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluCode")).FirstOrDefault().AllowNull == false)
                    {
                        ibc.PluCode = pluCode;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluPrefix")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PluPrefix")).FirstOrDefault().AllowNull == false)
                    {
                        ibc.PluPrefix = pluPrefix;
                    }
                }
                string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                decimal parsedRelationFactor = 1;
                if (ParseDecimalValue(relationFactor, dec_seperator[0], out parsedRelationFactor))
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).FirstOrDefault().AllowNull == false)
                        {
                            ibc.RelationFactor = (double)parsedRelationFactor;
                        }
                    }
                }
                else
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FactorQty")).FirstOrDefault().AllowNull == false)
                        {
                            ibc.RelationFactor = 1;
                        }
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("BarcodeMeasurmentUnit")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("BarcodeMeasurmentUnit")).FirstOrDefault().AllowNull == false)
                    {
                        ibc.MeasurementUnit = uow.FindObject<MeasurementUnit>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.Or(new BinaryOperator("Description", row.BarcodeMeasurmentUnit),
                                                      new BinaryOperator("Code", row.BarcodeMeasurmentUnit)), typeof(MeasurementUnit), row.Owner));
                    }
                }
                if (ibc.MeasurementUnit == null)
                {
                    string message = "Barcode Code: '" + barcode + "';Cannot find Measurement Unit with Code: '" + row.BarcodeMeasurmentUnit + "';DefaultBarcode measurement unit will be assigned";
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItem;Error: " + message);
                    return;
                }

                BarcodeType type = uow.FindObject<BarcodeType>(new BinaryOperator("Code", barcodeType));
                if (type != null)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("BarcodeTypeCode")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("BarcodeTypeCode")).FirstOrDefault().AllowNull == false)
                        {
                            ibc.Type = type;
                        }
                    }
                }
                else
                {
                    string message = "Barcode: '" + barcode + "';Cannot find BarcodeType with Code: '" + barcodeType + "'." + Environment.NewLine;
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateBarcode;Warning: " + message);
                }

                ibc.Save();
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).FirstOrDefault().AllowNull == false)
                    {
                        bc.Code = barcode;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).FirstOrDefault().AllowNull == false)
                    {
                        bc.IsActive = isActive;
                    }
                }
                bc.Save();

                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "Barcode: '" + barcode + "';Cannot find Item with Code: '" + itemCode + "'. Barcode will not be inserted/updated." + Environment.NewLine;
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateBarcode;Error: " + message);
            }
        }

        protected void InsertOrUpdateLinkedItem(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string itemCode = row.Item;
            string subItemCode = row.SubItemCode;
            string qtyFactor = row.SubItemQty;
            bool isActive = row.IsActive;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCode), typeof(Item), row.Owner));
            Item subItem = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", subItemCode), typeof(Item), row.Owner));

            if (item != null && subItem != null)
            {
                LinkedItem linkedItem = uow.FindObject<LinkedItem>(CriteriaOperator.And(new BinaryOperator("Item.Oid", item.Oid),
                                                                                        new BinaryOperator("SubItem.Oid", subItem.Oid)));
                if (linkedItem == null)
                {
                    linkedItem = new LinkedItem(uow);
                }
                string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                decimal parsedQuantityFactor = 0;
                if (ParseDecimalValue(qtyFactor, dec_seperator[0], out parsedQuantityFactor))
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("SubItemQty")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("SubItemQty")).Count() == 1)
                        {
                            linkedItem.QtyFactor = (double)parsedQuantityFactor;
                        }
                    }
                }

                OwnerApplicationSettings appiSettings = row.Owner.OwnerApplicationSettings;
                if (appiSettings.PadItemCodes)
                {
                    itemCode = itemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                    subItemCode = subItemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                    {
                        linkedItem.Item = item;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("SubItemCode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("SubItemCode")).Count() == 1)
                    {
                        linkedItem.SubItem = subItem;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        linkedItem.IsActive = isActive;
                    }
                }
                linkedItem.Save();

                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "";
                if (item == null)
                {
                    message += "Cannot find Item with Code: '" + itemCode + "'. ";
                }
                if (subItem == null)
                {
                    message += "Cannot find Item with Code: '" + subItemCode + "'. ";
                }
                message += "LinkedItem will not be inserted/updated.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLinkedItem;Error: " + message);
            }
        }

        protected void InsertOrUpdatePriceCatalogDetail(UnitOfWork uow, DecodedRawData row, StreamWriter writer, bool makeZeroPricesUserDefinable)
        {
            string itemCode = row.Item;
            string priceCatalogCode = row.PriceCatalog;
            string priceWithVat = row.GrossPrice;
            string priceWithoutVat = row.NetPrice;
            string discountString = row.Discount;

            string remoteReferenceId = row.RemoteReferenceId;
            //string timeValue = row.TimeValue;
            //string timeValueValidFromDateStr = row.TimeValueValidFromDate;
            //string timeValueValidUntilDateStr = row.TimeValueValidUntilDate;

            bool isActive = row.IsActive;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            OwnerApplicationSettings appiSettings = row.Owner.OwnerApplicationSettings;
            decimal discount = 0;
            string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (!ParseDecimalValue(discountString, dec_seperator[0], out discount))
            {
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdatePriceCatalogDetail;Error: Invalid discount " + discountString);
                return;
            }

            bool vatIncluded = false;
            decimal finalPrice = 0;
            
            if (!String.IsNullOrWhiteSpace(priceWithVat))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("GrossPrice")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("GrossPrice")).Count() == 1)
                    {
                        if (!ParseDecimalValue(priceWithVat, dec_seperator[0], out finalPrice))
                        {
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdatePriceCatalogDetail;Error: Invalid price with vat " + priceWithVat);
                            return;
                        }
                        vatIncluded = true;
                    }
                }
            }
            else if (!String.IsNullOrWhiteSpace(priceWithoutVat))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("NetPrice")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("NetPrice")).Count() == 1)
                    {
                        if (!ParseDecimalValue(priceWithoutVat, dec_seperator[0], out finalPrice))
                        {
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdatePriceCatalogDetail;Error: Invalid price without vat " + priceWithoutVat);
                            return;
                        }
                        vatIncluded = false;
                    }
                }
            }
            else
            {
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdatePriceCatalogDetail;Error: Price is empty");
                return;
            }


            if (appiSettings.PadItemCodes)
            {
                itemCode = itemCode.PadLeft(appiSettings.ItemCodeLength, appiSettings.ItemCodePaddingCharacter[0]);
            }

            string barcodeFromItemCode = itemCode;
            if (appiSettings.PadBarcodes)
            {
                barcodeFromItemCode = barcodeFromItemCode.PadLeft(appiSettings.BarcodeLength, appiSettings.BarcodePaddingCharacter[0]);
            }

            Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", barcodeFromItemCode));
            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCode), typeof(Item), row.Owner));
            PriceCatalog priceCatalog = uow.FindObject<PriceCatalog>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", priceCatalogCode), typeof(PriceCatalog), row.Owner));
            if (barcode != null && item != null & priceCatalog != null)
            {
                PriceCatalogDetail pcd = uow.FindObject<PriceCatalogDetail>(CriteriaOperator.And((new BinaryOperator("Barcode.Oid", barcode.Oid)),
                                                                                                  new BinaryOperator("Item.Oid", item.Oid),
                                                                                                  new BinaryOperator("PriceCatalog.Oid", priceCatalog.Oid)));
                if (pcd == null)
                {
                    pcd = new PriceCatalogDetail(uow);
                }

                if (makeZeroPricesUserDefinable)
                {
                    if (finalPrice == 0)
                    {
                        item.CustomPriceOptions = eItemCustomPriceOptions.CUSTOM_PRICE_IS_OPTIONAL;
                        item.Save();
                    }
                }

                pcd.Barcode = barcode;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                    {
                        pcd.Item = item;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                    {
                        pcd.PriceCatalog = priceCatalog;
                    }
                }
                pcd.DatabaseValue = finalPrice;
                pcd.VATIncluded = vatIncluded;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Discount")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Discount")).Count() == 1)
                    {
                        pcd.Discount = discount / 100;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        pcd.IsActive = isActive;
                    }
                }

                //if (!string.IsNullOrEmpty(timeValue))
                //{
                //    pcd.TimeValue = parsedTimeValue;
                //}
                //if (!string.IsNullOrEmpty(timeValueValidFromDateStr))
                //{
                //    pcd.TimeValueValidFromDate = timeValueValidFrom;
                //}
                //if (!string.IsNullOrEmpty(timeValueValidUntilDateStr))
                //{
                //    pcd.TimeValueValidUntilDate = timeValueValidUntil;
                //}

                pcd.Save();
                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "";
                if (barcode == null)
                {
                    message += "Cannot find Barcode with Code: '" + barcodeFromItemCode + "'. ";
                }
                if (item == null)
                {
                    message += "Cannot find Item with Code: '" + itemCode + "'. ";
                }
                if (priceCatalog == null)
                {
                    message += "Cannot find PriceCatalog with Code: '" + priceCatalogCode + "'. ";
                }
                message += "PriceCatalogDetail will not be inserted/updated.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdatePriceCatalogDetail;Error: " + message);
            }
        }

        protected void InsertOrUpdateCustomer(UnitOfWork uow, DecodedRawData row, StreamWriter writer, bool createCustomerUsers, Role customerUserRole, bool oneStorePerCustomer)
        {
            string code = row.Code;
            string taxCode = row.TaxCode;
            string companyName = row.CompanyName;
            string address = row.Address;
            string area = row.Region;
            string city = row.City;
            string poCode = row.ZipCode;
            string phoneNumber = row.Phone;
            string profession = row.Profession;
            string taxOffice = row.TaxOffice;
            string storeCode = row.BaseStore;
            string cardID = row.CardID;
            string priceCatalogCode = row.PriceCatalog;
            string companyBrandName = row.Firm;
            string fax = row.Fax;
            string breakOrderToCentral = row.SplitOrder;
            bool isActive = row.IsActive;
            bool isLicensed = row.IsLicenced;
            string vatLevel = row.VatCategory;
            string points = row.Points;
            string refundStore = row.RefundStore;
            bool isDefaultAddress = row.IsDefaultAddress;
            string priceCatalogPolicyCode = row.PriceCatalogPolicy;
            string addressProfession = row.AddressProfession;
            string thirdPartNum = row.ThirdPartNumber;
            string addressvatCategory = row.AddressVatCategory;
            PriceCatalogPolicy priceCatalogPolicy = uow.FindObject<PriceCatalogPolicy>(new BinaryOperator("Code", priceCatalogPolicyCode));
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            if (String.IsNullOrWhiteSpace(taxCode))
            {
                string message = "Customer Code: '" + code + "'; Tax Code is empty. Customer will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                return;
            }

            if (String.IsNullOrWhiteSpace(storeCode))
            {
                string message = "Customer Code: '" + code + "'; Store Code is empty. Customer will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                return;
            }

            if (string.IsNullOrEmpty(priceCatalogPolicyCode) == false && priceCatalogPolicy == null)
            {
                string message = "Customer Code: '" + code + "'; PriceCatalogPolicy with code '" + priceCatalogPolicyCode + "' was not found. Customer will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                return;
            }

            Customer customer = uow.FindObject<Customer>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(Customer), row.Owner));
            if (customer == null)
            {
                customer = new Customer(uow);
                customer.Owner = row.Owner;
            }
            Trader trd = uow.FindObject<Trader>(new BinaryOperator("TaxCode", taxCode));
            if (trd == null)
            {
                trd = new Trader(uow);
            }

            if (!String.IsNullOrEmpty(address))
            {
                Address adr = null;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).FirstOrDefault().UseThirdPartNum)
                    {
                        adr = uow.FindObject<Address>(CriteriaOperator.And(new BinaryOperator("ThirdPartNum", thirdPartNum),
                                                                             new BinaryOperator("Trader", trd)));
                    }
                    
                }
                else
                {
                    adr = uow.FindObject<Address>(CriteriaOperator.And(new BinaryOperator("Street", address),
                                                                             new BinaryOperator("Trader", trd),
                                                                             new BinaryOperator("City", city),
                                                                             new BinaryOperator("PostCode", poCode)));
                }
                if (adr == null)
                {
                    adr = new Address(uow);
                }
                Phone phone = null;
                if (!String.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != "\0")
                {
                    phone = uow.FindObject<Phone>(RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("Number", phoneNumber),
                                                                    new BinaryOperator("Address", adr)), typeof(Phone), row.Owner));
                    if (phone == null)
                    {
                        phone = new Phone(uow);
                        phone.Owner = row.Owner;
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).FirstOrDefault().AllowNull == false)
                        {
                            phone.Number = phoneNumber;
                        }
                    }
                    phone.PhoneType = uow.FindObject<PhoneType>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(PhoneType), row.Owner));
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                        {
                            phone.Address = adr;
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(fax) && fax != "\0")
                {
                    Phone phoneFax = uow.FindObject<Phone>(CriteriaOperator.And(new BinaryOperator("Number", fax), new BinaryOperator("Address", adr)));
                    if (phoneFax == null)
                    {
                        phoneFax = new Phone(uow);
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Fax")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Fax")).FirstOrDefault().AllowNull == false)
                        {
                            phoneFax.Number = fax;
                        }
                    }
                    phoneFax.PhoneType = uow.FindObject<PhoneType>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", false), typeof(PhoneType), row.Owner));
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                        {
                            phoneFax.Address = adr;
                        }
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("City")).FirstOrDefault().AllowNull == false)
                    {
                        adr.City = city;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Phone")).FirstOrDefault().AllowNull == false)
                    {
                        adr.DefaultPhone = phone;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ZipCode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ZipCode")).FirstOrDefault().AllowNull == false)
                    {
                        adr.PostCode = poCode;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Address")).FirstOrDefault().AllowNull == false)
                    {
                        adr.Street = address;
                    }
                }
                adr.Region = area;
                adr.Trader = trd;
                
                if (isDefaultAddress)
                {
                    customer.DefaultAddress = adr;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("AddressProfession")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("AddressProfession")).FirstOrDefault().AllowNull == false)
                    {
                        adr.Profession = addressProfession;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ThirdPartNumber")).FirstOrDefault().AllowNull == false)
                    {
                        adr.ThirdPartNum = thirdPartNum;
                    }
                }
                if (!String.IsNullOrWhiteSpace(addressvatCategory))
                {
                    adr.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", addressvatCategory), typeof(VatLevel), row.Owner));

                    if (adr.VatLevel == null)
                    {
                        adr.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(VatLevel), row.Owner));
                    }
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxCode")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxCode")).FirstOrDefault().AllowNull == false)
                {
                    trd.TaxCode = taxCode;
                }
            }
            TaxOffice txOffice = uow.FindObject<TaxOffice>(new BinaryOperator("Code", taxOffice));
            if (txOffice != null)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("TaxOffice")).FirstOrDefault().AllowNull == false)
                    {
                        trd.TaxOfficeLookUp = txOffice;
                    }
                }
            }
            else
            {
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Warning: Tax Office with code" + taxOffice + " not found");
            }

            customer.Trader = trd;
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CompanyName")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CompanyName")).FirstOrDefault().AllowNull == false)
                {
                    customer.CompanyName = companyName;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Profession")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Profession")).FirstOrDefault().AllowNull == false)
                {
                    customer.Profession = profession;
                    
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalogPolicy")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalogPolicy")).FirstOrDefault().AllowNull == false)
                {
                    customer.PriceCatalogPolicy = priceCatalogPolicy;
                }
            }
            Store customerStore = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", storeCode), typeof(Store), row.Owner));
            Store customerRefundStore = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", refundStore), typeof(Store), row.Owner));
            customer.Discount = 0;
            customer.PaymentMethod = uow.FindObject<PaymentMethod>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(PaymentMethod), row.Owner));

            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RefundStore")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RefundStore")).FirstOrDefault().AllowNull == false)
                {
                    if (customerRefundStore != null)
                    {

                        customer.RefundStore = customerRefundStore;
                    }
                    else if (customerStore != null)
                    {
                        customer.RefundStore = customerStore;
                    }
                }
            }
            if (!String.IsNullOrWhiteSpace(vatLevel))
            {
                customer.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", vatLevel), typeof(VatLevel), row.Owner));

                if (customer.VatLevel == null)
                {
                    customer.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(VatLevel), row.Owner));
                }
            }
            else
            {
                customer.VatLevel = uow.FindObject<VatLevel>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("IsDefault", true), typeof(VatLevel), row.Owner));
            }

            string message2 = "Customer Code: " + code;
            if (customer.VatLevel != null)
            {
                if (vatLevel == null)
                {
                    message2 += "; Vat Level with Code: '" + vatLevel + "' does not exist;The Default Vat Level (" + customer.VatLevel.Code +
                               ", " + customer.VatLevel.Description + ") was used.";
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error:" + message2);
                }
            }
            else
            {
                message2 += ";InsertOrUpdateCustomer;Error: Vat Level with Code: '" + vatLevel + " does not exist; A Default Vat Level does not exist; Customer Vat Level was NOT set.";
                writer.WriteLine(DateTime.Now.ToString() + message2);
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).FirstOrDefault().AllowNull == false)
                {
                    customer.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CardID")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("CardID")).FirstOrDefault().AllowNull == false)
                {
                    customer.CardID = row.CardID;
                }
            }
            customer.Trader = trd;
            customer.CompanyBrandName = companyBrandName == "\0" ? "" : companyBrandName;
            customer.BreakOrderToCentral = (breakOrderToCentral == "1");
           
            if (!String.IsNullOrWhiteSpace(points))
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).FirstOrDefault().AllowNull == false)
                    {
                        decimal parsedPoints = 0;
                        if (decimal.TryParse(points, out parsedPoints))
                        {
                            customer.CollectedPoints = parsedPoints;
                        }
                        else
                        {
                            string message = "Customer Code: '" + code + "';Invalid Points: '" + points + "'";
                            writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Warning: " + message);
                        }
                    }
                }
            }

            customer.IsActive = isActive;
            customer.Save();

            if (createCustomerUsers)  //Create User for this Customer
            {
                User customerUser = uow.FindObject<User>(new BinaryOperator("UserName", customer.Code.TrimStart('0')));
                if (customerUser == null) //If user exists don't change
                {
                    customerUser = new User(uow);
                    customerUser.UserName = customer.Code.TrimStart('0'); //customia gia masouti
                    customerUser.Password = UserHelper.EncodePassword(customer.Code.TrimStart('0')); //customia gia masouti
                    customerUser.FullName = customer.CompanyName;
                    //customerUser.TaxCode = customer.Trader.TaxCode;
                    customerUser.Role = customerUserRole;
                    UserTypeAccess association = uow.FindObject<UserTypeAccess>(CriteriaOperator.And(new BinaryOperator("User.Oid", customerUser.Oid), new BinaryOperator("EntityOid", customer.Oid)));
                    customerUser.Save();

                    if (association == null)
                    {
                        association = new UserTypeAccess(uow);
                    }
                    association.IsActive = true;
                    association.User = customerUser;
                    association.EntityOid = customer.Oid;
                    association.EntityType = "ITS.Retail.Model.Customer";
                    association.Save();
                    XpoHelper.CommitTransaction(uow);
                }
                try //Store UserTypeAccess
                {
                    bool canCreate = false;

                    foreach (CustomerStorePriceList cspl in customer.CustomerStorePriceLists)
                    {
                        if (cspl.StorePriceList != null && cspl.StorePriceList.Store != null && customerStore != null && cspl.StorePriceList.Store.Oid == customerStore.Oid)
                        {
                            canCreate = true;
                            break;
                        }
                    }

                    if (canCreate)
                    {
                        UserTypeAccess storeAccess = uow.FindObject<UserTypeAccess>(CriteriaOperator.And(new BinaryOperator("User.Oid", customerUser.Oid),
                                                                                         new BinaryOperator("EntityOid", customerStore.Oid)));
                        if (storeAccess == null)
                        {
                            storeAccess = new UserTypeAccess(uow);
                        }
                        storeAccess.IsActive = true;
                        storeAccess.User = customerUser;
                        storeAccess.EntityOid = customerStore.Oid;
                        storeAccess.EntityType = "ITS.Retail.Model.Store";
                        storeAccess.Save();
                        customerUser.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                catch (Exception e)
                {
                    string message = "Customer Code: '" + code + "'; Error while trying to assign a store to customer: " + e.Message;
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                }

                try
                {
                    customerUser.IsApproved = isLicensed;
                    customerUser.Save();
                }
                catch (Exception e)
                {
                    string message = "Customer Code: '" + code + "'; Error while trying to change User License key: " + e.Message;
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateCustomer;Error: " + message);
                }
            }
            XpoHelper.CommitChanges(uow);
        }

        protected void InsertOrUpdateOffer(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string startDate = row.FromDate;
            string endDate = row.EndDate;
            string code = row.Code;
            string description = row.Description;
            string description2 = row.Description2;
            string priceCatalogCode = row.PriceCatalog;
            bool isActive = row.IsActive;

            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            PriceCatalog priceCatalog = uow.FindObject<PriceCatalog>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", priceCatalogCode), typeof(PriceCatalog), row.Owner));
            if (priceCatalog != null)
            {
                Offer offer = uow.FindObject<Offer>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(Offer), row.Owner));
                if (offer == null)
                {
                    offer = new Offer(uow);
                    offer.Owner = row.Owner;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                    {
                        offer.Code = code;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
                    {
                        offer.StartDate = DateTime.Parse(startDate);
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
                    {
                        offer.EndDate = DateTime.Parse(endDate);
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                    {
                        offer.Description = description;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description2")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description2")).Count() == 1)
                    {
                        offer.Description2 = description2;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PriceCatalog")).Count() == 1)
                    {
                        offer.PriceCatalog = priceCatalog;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        offer.IsActive = isActive;
                    }
                }
                offer.Save();
                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "Offer Code: '" + code + "';Cannot find PriceCatalog with Code: '" + priceCatalogCode + "'. Offer will not be inserted/updated." + Environment.NewLine;
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateOffer;Error: " + message);
            }
        }

        protected void InsertOrUpdateOfferDetail(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string offerCode = row.Code;
            string itemCode = row.Item;
            bool isActive = row.IsActive;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            Offer offer = uow.FindObject<Offer>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", offerCode), typeof(Offer), row.Owner));
            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCode), typeof(Item), row.Owner));

            if (offer != null && item != null)
            {

                OfferDetail offerDetail = uow.FindObject<OfferDetail>(CriteriaOperator.And(new BinaryOperator("Offer.Oid", offer.Oid),
                                                                                            new BinaryOperator("Item.Oid", item.Oid)));
                if (offerDetail == null)
                {
                    offerDetail = new OfferDetail(uow);
                }

                offerDetail.Offer = offer;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                    {
                        offerDetail.Item = item;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        offerDetail.IsActive = isActive;
                    }
                }
                offerDetail.Save();
                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "";
                if (offer == null)
                {
                    message += "Cannot find Offer with Code: '" + offerCode + "'. ";
                }
                if (item == null)
                {
                    message += "Cannot find Item with Code: '" + itemCode + "'. ";
                }
                message += "OfferDetail will not be inserted/updated.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateOfferDetail;Error: " + message);
            }
        }

        /// <summary>
        /// Sets a property within a propery. Ex: Item.DefaultBarcode.Code
        /// </summary>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected void SetProperty(object source, string property, object value)
        {
            string[] bits = property.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo prop = source.GetType().GetProperty(bits[i]);
                if (prop.GetValue(source, null) == null) //if null then initialize
                {
                    if (prop.GetType().IsSubclassOf(typeof(BaseObj)))
                    {
                        Session uow = (source as BaseObj).Session;
                        prop.SetValue(source, Activator.CreateInstance(prop.PropertyType, new object[] { uow }), null);
                    }
                    else
                    {
                        prop.SetValue(source, Activator.CreateInstance(prop.PropertyType), null);
                    }
                }

                source = prop.GetValue(source, null);
            }
            PropertyInfo propertyToSet = source.GetType().GetProperty(bits[bits.Length - 1]);
            object convertedValue = Convert.ChangeType(value.ToString(), propertyToSet.PropertyType);
            propertyToSet.SetValue(source, convertedValue, null);
        }

        private bool ParseDecimalValue(string input, char separator, out decimal val)
        {
            List<char> validSeparators = new List<char>() { ',', '.' };
            if (!String.IsNullOrEmpty(input))
            {

                if (validSeparators.Contains(input[0]))
                {
                    input = input.Replace(input[0].ToString(), "0" + separator);
                }

                validSeparators.Remove(separator);

                foreach (char sep in validSeparators)
                {
                    input = input.Replace(sep, separator);
                }

                return decimal.TryParse(input, out val);
            }
            val = 0;
            return true;
        }

        protected void InsertOrUpdateItemCategoryImportData(UnitOfWork uow, DecodedRawData row, StreamWriter writer, string rootCategoryCode)
        {
            string code = row.Code;
            string parentCode = row.ParentCode;
            string description = row.Description;
            string points = row.Points;
            bool isActive = row.IsActive;
            decimal ParsedPoints = 0;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            decimal.TryParse(points, out ParsedPoints);
            CompanyNew owner = row.Owner;
            ItemCategory itemCategory = null;
            ItemCategory parent = null;
            if (!String.IsNullOrWhiteSpace(parentCode))
            {
                parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCode), typeof(ItemCategory), row.Owner));
                if (parent != null)
                {
                    itemCategory = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(ItemCategory), row.Owner));
                    if (itemCategory == null)
                    {
                        itemCategory = new ItemCategory(uow);
                        itemCategory.Owner = owner;
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ParentCode")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ParentCode")).FirstOrDefault().AllowNull == false)
                        {
                            parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCode), typeof(ItemCategory), row.Owner));
                            if (parent != null)
                            {
                                itemCategory.Parent = parent;
                            }
                            else
                            {
                                string message = "No parent category found. Category Item will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemCategoryImportData;Error: " + message);
                                return;
                            }
                        }
                    }
                    if (SelectedHeaderRows.Where(x=>x.PropertyName.Equals("Description")).Count()==1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).FirstOrDefault().AllowNull == false)
                        {
                            itemCategory.Description = description;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).FirstOrDefault().AllowNull == false)
                        {
                            itemCategory.Points = ParsedPoints;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).FirstOrDefault().AllowNull == false)
                        {
                            itemCategory.Code = code;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).FirstOrDefault().AllowNull == false)
                        {
                            itemCategory.IsActive = isActive;
                        }
                    }

                    itemCategory.Save();
                    XpoHelper.CommitChanges(uow);
                }
                else
                {
                    string message = "No parent category found. Category Item will not be inserted.";
                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemCategoryImportData;Error: " + message);
                    return;
                }
            }
            else
            {
                itemCategory = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(ItemCategory), row.Owner));
                //parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCode), typeof(ItemCategory), row.Owner));

                if (itemCategory == null)
                {
                    itemCategory = new ItemCategory(uow);
                    itemCategory.Owner = owner;
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ParentCode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ParentCode")).FirstOrDefault().AllowNull == false)
                    {
                        if (String.IsNullOrWhiteSpace(parentCode))
                        {
                            itemCategory.Parent = null;
                        }
                        else
                        {
                            parent = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", parentCode), typeof(ItemCategory), row.Owner));
                            if (parent != null)
                            {
                                itemCategory.Parent = parent;
                            }
                            else
                            {
                                string message = "No parent category found. Category Item will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemCategoryImportData;Error: " + message);
                                return;
                            }
                        }
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                    {
                        itemCategory.Description = description;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Points")).Count() == 1)
                    {
                        itemCategory.Points = ParsedPoints;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                    {
                        itemCategory.Code = code;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        itemCategory.IsActive = isActive;
                    }
                }
                itemCategory.Save();
                XpoHelper.CommitChanges(uow);
            }
        }
        private void InsertOrUpdateItemAnaylticTree(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string Item = row.Item;
            string Node = row.Node;
            string Root = row.Root;
            string RemodeID = row.RemoteReferenceId;
            bool isActive = row.IsActive;
            

            if (String.IsNullOrWhiteSpace(Node))
            {
                string message = "Category Node code is empty. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            if (String.IsNullOrWhiteSpace(Root))
            {
                string message = "Category Root code is empty. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            if (String.IsNullOrWhiteSpace(Item))
            {
                string message = "Item code is empty. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            if (String.IsNullOrWhiteSpace(RemodeID))
            {
                string message = "RemodeID of Item is empty.  Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            
            Item item= uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", Item), typeof(Item), row.Owner));
            ItemCategory root = uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", Root), typeof(ItemCategory), row.Owner));
            ItemCategory node= uow.FindObject<ItemCategory>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", Node), typeof(ItemCategory), row.Owner));
            

            if (item ==null)
            {
                string message = "Item not found. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            if (node == null)
            {
                string message = "Category node not found. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }
            if (root == null)
            {
                string message = "Root not found. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }

            CategoryNode nodeRoot = node.GetRoot(uow);
            if (nodeRoot == null)
            {
                string message = "No Category node found in this Root. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }

            if(nodeRoot.Oid != root.Oid)
            {
                string message = "Category node root is different than given Root. Item Analytic Tree will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemAnalysticTree;Error: " + message);
                return;
            }

            ItemAnalyticTree itemAnalyticTree = uow.FindObject<ItemAnalyticTree>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("ReferenceId", RemodeID), typeof(ItemAnalyticTree), row.Owner));
            if (itemAnalyticTree==null)
            {
                itemAnalyticTree = new ItemAnalyticTree(uow);                
            }
            itemAnalyticTree.Object = item;
            itemAnalyticTree.Node = node;
            itemAnalyticTree.Root = root;
            itemAnalyticTree.ReferenceId = RemodeID;
            itemAnalyticTree.Save();
            XpoHelper.CommitChanges(uow);
        }
        protected void InsertOrUpdateItemExtraInfo(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string item = row.Item;
            string store = row.Store;
            string description = row.Description;
            string packedAt = row.PackedAt;
            string expiresAt = row.ExpiresAt;
            string origin = row.Origin;
            string lot = row.Lot;
            string RemodeID = row.RemoteReferenceId;

            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();


            if (String.IsNullOrWhiteSpace(store))
            {
                string message = "Item Extra Info Store is empty. Item Extra Info will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                return;
            }
            if (String.IsNullOrWhiteSpace(item))
            {
                string message = "Item Extra Info Item is empty. Item Extra Info will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                return;
            }
            
            ItemExtraInfo itemExtraInfo = null;
            Item itemInfo = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", item), typeof(Item), row.Owner));
            Store storeInfo = null;
            List<Store> storeList = new List<Store>();
            //τιμή 'store=9999' για να εφαρμοστεί σε όλα τα καταστήματα
            if (store.Equals("9999"))
            {
                storeInfo = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", store), typeof(Store), row.Owner));
                storeList = new XPCollection<Store>(uow).Where(x=>x.Owner==row.Owner).ToList();
            }
            else
            {
                 storeInfo = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", store), typeof(Store), row.Owner));
                 storeList.Add(storeInfo);
            }

            if(itemInfo == null)
            {
                string message = "Item with code: "+item+" not found. Item Extra Info will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                return;
            }
            if (storeList.Count == 0)
            {
                string message = "Store "+store+" not found. Item Extra Info will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                return;
            }

            itemExtraInfo = uow.FindObject<ItemExtraInfo>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("ReferenceId", RemodeID), typeof(ItemExtraInfo), row.Owner));
            if(itemExtraInfo==null)
            {   
                             
                if (itemInfo != null && storeList.Count>0)
                {
                    foreach (var currentStore in storeList)
                    {
                        CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Store", currentStore.Oid), new BinaryOperator("Item", itemInfo.Oid));
                        //criteria = RetailHelper.ApplyOwnerCriteria(criteria, typeof(ItemExtraInfo) ,row.Owner);
                        itemExtraInfo = uow.FindObject<ItemExtraInfo>(criteria);
                        if (itemExtraInfo == null)
                        {
                            itemExtraInfo = new ItemExtraInfo(uow);
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                            {
                                itemExtraInfo.Description = description;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                            {
                                itemExtraInfo.Item = itemInfo;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                            {
                                itemExtraInfo.Store = currentStore;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ExpiresAt")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ExpiresAt")).Count() == 1)
                            {
                                if (String.IsNullOrWhiteSpace(expiresAt))
                                {
                                    string message = "Item Extra Info ExpiresAt is empty. Item Extra Info will not be inserted.";
                                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                    return;
                                }
                                DateTime DateExpiresAt = DateTime.Now;
                                if (DateTime.TryParse(expiresAt, out DateExpiresAt) == true)
                                {
                                    itemExtraInfo.ExpiresAt = DateExpiresAt;
                                }
                                else
                                {
                                    string message = "Item Extra Info ExpiresAt wrong format date. Item Extra Info will not be inserted.";
                                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                    return;
                                }
                            }

                        }

                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackedAt")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackedAt")).Count() == 1)
                            {
                                if (String.IsNullOrWhiteSpace(packedAt))
                                {
                                    string message = "Item Extra Info PackedAt is empty. Item Extra Info will not be inserted.";
                                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                    return;
                                }
                                DateTime DatePackedAt = DateTime.Now;
                                if (DateTime.TryParse(packedAt, out DatePackedAt) == true)
                                {
                                    itemExtraInfo.PackedAt = DatePackedAt;
                                }
                                else
                                {
                                    string message = "Item Extra Info packedsAt wrong format date. Item Extra Info will not be inserted.";
                                    writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                    return;
                                }
                            }

                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Origin")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Origin")).Count() == 1)
                            {
                                itemExtraInfo.Origin = origin;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Lot")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Lot")).Count() == 1)
                            {
                                itemExtraInfo.Lot = lot;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                            {
                                itemExtraInfo.ReferenceId = RemodeID;
                            }
                        }
                        itemExtraInfo.Save();
                        XpoHelper.CommitChanges(uow);
                    }
                }
                
            }
            else
            {

                foreach (var currentStore in storeList)
                {
                    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Store", currentStore.Oid), new BinaryOperator("Item", itemInfo.Oid));
                    itemExtraInfo = uow.FindObject<ItemExtraInfo>(criteria);

                    if(itemExtraInfo==null)
                    {
                        itemExtraInfo = new ItemExtraInfo(uow);
                        itemExtraInfo.ReferenceId = "";
                    }
                    else
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                            {
                                itemExtraInfo.ReferenceId = RemodeID;
                            }
                        }
                    }

                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                        {
                            itemExtraInfo.Description = description;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                        {
                            if (itemInfo != null)
                            {
                                itemExtraInfo.Item = itemInfo;
                            }
                            else
                            {
                                string message = "Item Extra Info item is empty. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                        }

                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                        {
                            if (currentStore != null)
                            {
                                itemExtraInfo.Store = currentStore;
                            }
                            else
                            {
                                string message = "Item Extra Info store is empty. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ExpiresAt")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("ExpiresAt")).Count() == 1)
                        {
                            if (String.IsNullOrWhiteSpace(expiresAt))
                            {
                                string message = "Item Extra Info ExpiresAt is empty. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                            DateTime DateExpiresAt = DateTime.Now;
                            if (DateTime.TryParse(expiresAt, out DateExpiresAt) == true)
                            {
                                itemExtraInfo.ExpiresAt = DateExpiresAt;
                            }
                            else
                            {
                                string message = "Item Extra Info ExpiresAt wrong format date. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                        }

                    }

                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackedAt")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("PackedAt")).Count() == 1)
                        {
                            if (String.IsNullOrWhiteSpace(packedAt))
                            {
                                string message = "Item Extra Info PackedAt is empty. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                            DateTime DatePackedAt = DateTime.Now;
                            if (DateTime.TryParse(packedAt, out DatePackedAt) == true)
                            {
                                itemExtraInfo.PackedAt = DatePackedAt;
                            }
                            else
                            {
                                string message = "Item Extra Info packedsAt wrong format date. Item Extra Info will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateItemExtraInfo;Error: " + message);
                                return;
                            }
                        }

                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Origin")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Origin")).Count() == 1)
                        {
                            itemExtraInfo.Origin = origin;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Lot")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Lot")).Count() == 1)
                        {
                            itemExtraInfo.Lot = lot;
                        }
                    }
                    
                    itemExtraInfo.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
        }

        protected void InsertOrUpdateLeaflet(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string startDate = row.FromDate;
            string endDate = row.EndDate;
            string code = row.Code;
            string description = row.Description;
            string description2 = row.Description2;
            bool isActive = row.IsActive;

            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            
            Leaflet leaflet = uow.FindObject<Leaflet>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", code), typeof(Leaflet), row.Owner));
            if (leaflet == null)
            {
                leaflet = new Leaflet(uow);
                leaflet.Owner = row.Owner;
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                {
                    leaflet.Code = code;
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("FromDate")).Count() == 1)
                {
                    leaflet.StartDate = DateTime.Parse(startDate);
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("EndDate")).Count() == 1)
                {
                    leaflet.EndDate = DateTime.Parse(endDate);
                }
            }
            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
            {
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Description")).Count() == 1)
                {
                    leaflet.Description = description;
                }
            }
            DateTime validFrom = leaflet.StartDate.Date;
            DateTime validUntil = leaflet.StartDate.Date + new TimeSpan(23,59,59);

            leaflet.StartTime = validFrom;
            leaflet.EndTime = validUntil;

            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        leaflet.IsActive = isActive;
                    }
                }
                leaflet.Save();
                XpoHelper.CommitChanges(uow);
        }

        protected void InsertOrUpdateLeafletDetail(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string leafletCode = row.Code;
            string itemCode = row.Item;
            string barcodeCode = row.Barcode;
            string price = row.GrossPrice;
            bool isActive = row.IsActive;
            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();
            string dec_seperator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            Leaflet leaflet = uow.FindObject<Leaflet>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", leafletCode), typeof(Leaflet), row.Owner));
            Item item = uow.FindObject<Item>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", itemCode), typeof(Item), row.Owner));
            Barcode barcode = uow.FindObject<Barcode>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", barcodeCode), typeof(Barcode), row.Owner));

            if (leaflet != null && item != null && barcode != null)
            {

                LeafletDetail leafletDetail = uow.FindObject<LeafletDetail>(CriteriaOperator.And(new BinaryOperator("Leaflet.Oid", leaflet.Oid),
                                                                                            new BinaryOperator("Item.Oid", item.Oid), new BinaryOperator("Barcode.Oid", barcode.Oid)));
                if (leafletDetail == null)
                {
                    leafletDetail = new LeafletDetail(uow);
                }

                leafletDetail.Leaflet = leaflet;
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Item")).Count() == 1)
                    {
                        leafletDetail.Item = item;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Barcode")).Count() == 1)
                    {
                        leafletDetail.Barcode = barcode;
                    }
                }
                if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("IsActive")).Count() == 1)
                    {
                        leafletDetail.IsActive = isActive;
                    }
                }
                decimal value = 0;
                if (!String.IsNullOrWhiteSpace(price))
                {
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("GrossPrice")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("GrossPrice")).Count() == 1)
                        {
                            if (!ParseDecimalValue(price, dec_seperator[0], out value))
                            {
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLeafletDetail;Error: Invalid price with vat " + price);
                                return;
                            }
                            leafletDetail.Value = value;
                        }
                    }
                }

                leafletDetail.Save();
                XpoHelper.CommitChanges(uow);
            }
            else
            {
                string message = "";
                if (leaflet == null)
                {
                    message += "Cannot find Leaflet with Code: '" + leafletCode + "'. ";
                }
                if (item == null)
                {
                    message += "Cannot find Item with Code: '" + itemCode + "'. ";
                }
                if (barcode == null)
                {
                    message += "Cannot find Barcode with Code: '" + barcodeCode + "'. ";
                }
                message += "LeafletDetail will not be inserted/updated.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLeafletDetail;Error: " + message);
            }
        }

        protected void InsertOrUpdateLeafletStore(UnitOfWork uow, DecodedRawData row, StreamWriter writer)
        {
            string leafletCode = row.Code;
            string storeCode = row.Store;
            string RemoteID = row.RemoteReferenceId;

            var SelectedHeaderRows = row.Head.DataFileRecordDetails.ToList();

            if (String.IsNullOrWhiteSpace(storeCode))
            {
                string message = "Leaflet Store is empty. LeafletStore  will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLeafletStore;Error: " + message);
                return;
            }

            Leaflet leaflet = uow.FindObject<Leaflet>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", leafletCode), typeof(Leaflet), row.Owner));

            Store storeInfo = null;
            List<Store> storeList = new List<Store>();
            //τιμή 'store=9999' για να εφαρμοστεί σε όλα τα καταστήματα
            if (storeCode.Equals("9999"))
            {
                storeInfo = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", storeCode), typeof(Store), row.Owner));
                storeList = new XPCollection<Store>(uow).Where(x => x.Owner == row.Owner).ToList();
            }
            else
            {
                storeInfo = uow.FindObject<Store>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("Code", storeCode), typeof(Store), row.Owner));
                storeList.Add(storeInfo);
            }

            if (storeList.Count == 0)
            {
                string message = "Store " + storeCode + " not found. LeafletStore will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLeafletStore;Error: " + message);
                return;
            }

            if (leaflet == null)
            {
                string message = "Leaflet " + leafletCode + " not found. LeafletStore will not be inserted.";
                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateLeafletStore;Error: " + message);
                return;
            }

            LeafletStore leafletStore = uow.FindObject<LeafletStore>(RetailHelper.ApplyOwnerCriteria(new BinaryOperator("ReferenceId", RemoteID), typeof(LeafletStore), row.Owner));

            if (leafletStore == null)
            {
                if (leaflet != null && storeList.Count > 0)
                {
                    foreach (var currentStore in storeList)
                    {
                        CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Store", currentStore.Oid), new BinaryOperator("Leaflet", leaflet.Oid));

                        leafletStore = uow.FindObject<LeafletStore>(criteria);

                        if (leafletStore == null)
                        {
                            leafletStore = new LeafletStore(uow);
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                            {
                                leafletStore.Store = currentStore;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                            {
                                leafletStore.Leaflet = leaflet;
                            }
                        }
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                            {
                                leafletStore.ReferenceId = RemoteID;
                            }
                        }
                        leafletStore.Save();
                        XpoHelper.CommitChanges(uow);
                    }
                }
            }
            else
            {
                foreach (var currentStore in storeList)
                {
                    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Store", currentStore.Oid), new BinaryOperator("Leaflet", leaflet.Oid));
                    leafletStore = uow.FindObject<LeafletStore>(criteria);

                    if (leafletStore == null)
                    {
                        leafletStore = new LeafletStore(uow);
                        leafletStore.ReferenceId = "";
                    }
                    else
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                        {
                            if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("RemoteReferenceId")).Count() == 1)
                            {
                                leafletStore.ReferenceId = RemoteID;
                            }
                        }
                    }

                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Code")).Count() == 1)
                        {
                            leafletStore.Leaflet = leaflet;
                        }
                    }
                    if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                    {
                        if (SelectedHeaderRows.Where(x => x.PropertyName.Equals("Store")).Count() == 1)
                        {
                            if (currentStore != null)
                            {
                                leafletStore.Store = currentStore;
                            }
                            else
                            {
                                string message = "LeafletStore store is empty. LeafletStore will not be inserted.";
                                writer.WriteLine(DateTime.Now.ToString() + ";InsertOrUpdateleafletStore;Error: " + message);
                                return;
                            }
                        }
                    }
                    leafletStore.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
        }
    }
}
#endif