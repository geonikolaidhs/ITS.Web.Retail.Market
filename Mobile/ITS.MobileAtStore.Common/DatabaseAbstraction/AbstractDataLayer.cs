using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using ITS.MobileAtStore.Common.DatabaseAbstraction.AuxilliaryClasses;
using ITS.Common.Logging;
using System.Data;
using System.Reflection;
using System.Linq;
using ITS.MobileAtStore.ObjectModel.Attributes;
using DevExpress.Data.Filtering;
using ITS.MobileAtStore.Common.DatabaseAbstraction.Enumerations;
using System.IO;
using System.Diagnostics;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction
{
    /// <summary>
    /// Abstract DataLayer implementation
    /// Contains all  important functions that are required by the Web service
    /// </summary>
    public abstract class AbstractDataLayer
    {
        /// <summary>
        /// Data layer for storing the exported Documents
        /// </summary>
        private static IDataLayer mainStoreDatalayer;

        /// <summary>
        /// A lock object that is required during the modification of the <see cref="mainStoreDatalayer"/>
        /// </summary>
        private static object lockObject = new object();

        /// <summary>
        /// Resets the data layer <see cref="mainStoreDatalayer"/>
        /// </summary>
        public static void ResetMainStoreDataLayer()
        {
            if (mainStoreDatalayer != null)
            {
                lock (lockObject)
                {
                    mainStoreDatalayer.Dispose();
                    mainStoreDatalayer = null;
                }
            }
        }

        public virtual string ProductQuery
        {
            get
            {
                return "select * from {0} WHERE  BARCODE = '{1}' {2} {3} union all ( select * from {0} where  CODE = '{1}' {2} {3})";
            }
        }

        public virtual string ReceiptProductQuery
        {
            get
            {
//                return @"(select * from {0} WHERE SUPPLIERCODE  = '{1}' {2} {3} and receiptSupplierid = {4} )
//                         union all ( select * from {0} where  CODE = '{1}' {2} {3}  ) 
//                         union all ( select * from {0} where  BARCODE = '{1}' {2} {3} )";
                return @"(select top 1 * from {0} WHERE SUPPLIER  = '{1}' {2} {3} ) 
                        union all ( select top 1 * from {0} where  CODE = '{1}' {2} {3} ) 
                        union all ( select top 1 * from {0} where  BARCODE = '{1}' {2} {3} )";
            }
        }


        public virtual string GetCode(string code)
        {
            return code;
        }

        /// <summary>
        /// Gets the data layer for storing the exported Documents
        /// </summary>
        public static IDataLayer MainStoreDatalayer
        {
            get
            {
                if (mainStoreDatalayer == null)
                {
                    lock (lockObject)
                    {
                        AutoCreateOption option = AutoCreateOption.DatabaseAndSchema;
                        String connectionString = DataLayerSettings.GetConnectionString(Settings.StoreConnectionSettings, ref option);
                        XPDictionary dict = new ReflectionDictionary();
                        dict.GetDataStoreSchema(typeof(Header).Assembly);

                        XPClassInfo headerClassInfo = dict.GetClassInfo(typeof(Header));
                        headerClassInfo.AddAttribute(new PersistentAttribute(Settings.HeaderPersistanceProperty));
                        XPClassInfo lineClassInfo = dict.GetClassInfo(typeof(Line));
                        lineClassInfo.AddAttribute(new PersistentAttribute(Settings.LinePersistanceProperty));

                        mainStoreDatalayer = XpoDefault.GetDataLayer(connectionString, dict, option);
                    }
                }
                return mainStoreDatalayer;
            }
        }

        /// <summary>
        /// Pads the <paramref name="code"/> according to <paramref name="settings"/>
        /// </summary>
        /// <param name="code">The code to be padded</param>
        /// <param name="settings">The padding settings</param>
        /// <returns>The padded code</returns>
        public static string PadCode(string code, PaddingSettings settings)
        {
            switch (settings.Mode)
            {
                case PaddingMode.LEFT:
                    return code.PadLeft(settings.Length, settings.Character);
                case PaddingMode.RIGHT:
                    return code.PadRight(settings.Length, settings.Character);
            }
            return code;
        }

        /// <summary>
        /// It generates the Database Connection object
        /// </summary>
        /// <returns>the Database Connection object</returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// The initialization function
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Create a UnitOfWork in the <see cref="MainStoreDatalayer"/>
        /// </summary>
        /// <returns>Active UnitOfWork</returns>
        public UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(MainStoreDatalayer);
        }

        /// <summary>
        /// Create a UnitOfWork in the specified data layer
        /// </summary>
        /// <param name="layer">Data layer to use in the UnitOfWork. If this parameter is null the <see cref="MainStoreDatalayer"/> will be used.</param>
        /// <returns>The unit of work</returns>
        public UnitOfWork GetNewUnitOfWork(IDataLayer layer)
        {
            if (layer == null)
            {
                return GetNewUnitOfWork();
            }
            return new UnitOfWork(layer);
        }

        /// <summary>
        /// Get Default object of specific Type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns>The default object</returns>
        public object GetDefault(Type t)
        {
            return this.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
        }

        /// <summary>
        /// Get Default object of specific Type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The default object</returns>
        public T GetDefaultGeneric<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDataLayer"/> class.
        /// </summary>
        public AbstractDataLayer()
        {
            Initialize();
        }
                
        /// <summary>
        /// Returns the suppliers of the database where their code or tax code is <see cref="searchString"/>
        /// </summary>
        /// <param name="searchString">The input criteria for code or tax code</param>
        /// <returns>A list of suppliers</returns>
        public virtual Customer GetSupplier(string searchString)
        {
            string query = string.Format("SELECT * FROM {0} WHERE (CODE='{1}' OR AFM = '{2}')", Settings.DatabaseViewSettings.SuppliersView, PadCode(searchString, Settings.CustomerPaddingSettings), searchString);
            return ExecuteReader<Customer>(query);
        }

        /// <summary>
        /// Get customer based on Code and TaxCode
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public virtual Customer GetCustomer(string searchString)
        {
            string query = string.Format("SELECT * FROM {0} WHERE (CODE='{1}' OR AFM = '{2}')", Settings.DatabaseViewSettings.CustomersView, PadCode(searchString, Settings.CustomerPaddingSettings), searchString);
            return ExecuteReader<Customer>(query);
        }

        /// <summary>
        /// Returns the offers from the appropriate view for the product with id <paramref name="productId"/>
        /// and the store with code <paramref name="compCode"/>.
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <param name="compCode">The store code</param>
        /// <returns>A list of offers</returns>
        public virtual Offer[] GetOffers(string productId, string compCode)
        {
            string productIdPadded = PadCode(productId);

            string query = string.Format("SELECT * FROM {0} WHERE (productid='{1}' and compcode = '{2}')", Settings.DatabaseViewSettings.OffersView, productIdPadded, compCode);
            List<Offer> list = ExecuteReaderList<Offer>(query);
            list.ForEach(x => x.SetProcessedDescription());
            return list.ToArray();
        }

        /// <summary>
        /// Returns the product that fulfils the specified criteria
        /// </summary>
        /// <param name="code">The product code</param>
        /// <param name="suppcode">The supplier's code (currently this parameter is ignored)</param>
        /// <param name="compCode">The store's code (empty to deactivate this filter)</param>
        /// <param name="priceList">The price list (-1 to deactivate this filter)</param>
        /// <returns></returns>
        public virtual Product GetProduct(string code, string suppcode, string compCode, int priceList)
        {
            string timeFormat = "HH:mm:ss.fff";
            Trace.WriteLine("GetProduct Started at : " + DateTime.Now.ToString(timeFormat));

            string initialCode = code;
            code = GetCode(code);
            code = DecodeBarcode(code);
            code = PadCode(code);

            Trace.WriteLine("Code padding ended at : " + DateTime.Now.ToString(timeFormat));

            String whereCompCode = string.IsNullOrEmpty(compCode) ? string.Empty : string.Format(" AND (COMPCODE = '{0}') ", compCode);
            String wherePriceList = priceList <= 0 ? string.Empty : string.Format(" AND (Pricelist  = {0}) ", priceList);
            string query = string.Format(this.ProductQuery, Settings.DatabaseViewSettings.ProductsView, code, whereCompCode, wherePriceList);
            Trace.WriteLine("Query Started at : " + DateTime.Now.ToString(timeFormat));
            Product product = ExecuteReader<Product>(query);
            Trace.WriteLine("Query Finished at : " + DateTime.Now.ToString(timeFormat));
            if (product == null)
            {
                return null;
            }
            DecodedBarcode dc = DecodeBarcodeFull(initialCode);
            if (dc == null)
            {
                product.CalculatedTotalPrice = product.Price;
            }
            else if (dc is WeightedDecodedBarcode)
            {
                product.CalculatedTotalPrice = ((WeightedDecodedBarcode)dc).Weight * product.PricePerUnit;
            }
            else if (dc is PriceDecodedBarcode)
            {
                product.CalculatedTotalPrice = ((PriceDecodedBarcode)dc).Price;
            }
            Trace.WriteLine("GetProduct Completet at : " + DateTime.Now.ToString(timeFormat));
            return product;
        }


        public virtual Product GetReceiptProduct(string code, string suppcode, string compCode, int priceList)
        {
            string timeFormat = "HH:mm:ss.fff";
            Trace.WriteLine("GetProduct Started at : " + DateTime.Now.ToString(timeFormat));

            string initialCode = code;
            code = GetCode(code);
            code = DecodeBarcode(code);
            code = PadCode(code);

            Trace.WriteLine("Code padding ended at : " + DateTime.Now.ToString(timeFormat));

            String whereCompCode = string.IsNullOrEmpty(compCode) ? string.Empty : string.Format(" AND (COMPCODE = '{0}') ", compCode);
            String wherePriceList = priceList <= 0 ? string.Empty : string.Format(" AND (Pricelist  = {0}) ", priceList);
            string query = string.Format(this.ReceiptProductQuery, Settings.DatabaseViewSettings.ProductsView, code, whereCompCode, wherePriceList,suppcode);
            Trace.WriteLine("Query Started at : " + DateTime.Now.ToString(timeFormat));
            Product product = ExecuteReader<Product>(query);
            Trace.WriteLine("Query Finished at : " + DateTime.Now.ToString(timeFormat));
            if (product == null)
            {
                return null;
            }
            DecodedBarcode dc = DecodeBarcodeFull(initialCode);
            if (dc == null)
            {
                product.CalculatedTotalPrice = product.Price;
            }
            else if (dc is WeightedDecodedBarcode)
            {
                product.CalculatedTotalPrice = ((WeightedDecodedBarcode)dc).Weight * product.PricePerUnit;
            }
            else if (dc is PriceDecodedBarcode)
            {
                product.CalculatedTotalPrice = ((PriceDecodedBarcode)dc).Price;
            }
            Trace.WriteLine("GetProduct Completet at : " + DateTime.Now.ToString(timeFormat));
            return product;
        }

        /// <summary>
        /// Returns the Warehouses of the database
        /// </summary>
        /// <returns>A list of Warehouses</returns
        public List<Warehouse> GetWarehouses()
        {
            String query = String.Format("SELECT * FROM {0}", Settings.DatabaseViewSettings.WarehousesView);
            return ExecuteReaderList<Warehouse>(query);
        }

        /// <summary>
        /// Update Inventory Line. 
        /// </summary>
        /// <returns>the result of the update</returns>
        public bool UpdateInvLine()
        {
            try
            {
                IDataLayer dataLayer = DataLayerSettings.DataLayers.ContainsKey(DOC_TYPES.INVENTORY) ? DataLayerSettings.DataLayers[DOC_TYPES.INVENTORY] : null;
                using (UnitOfWork uow = GetNewUnitOfWork(dataLayer))
                {
                    XPCollection<InvLine> invLines = new XPCollection<InvLine>(uow);
                    /* •—————————————————————————•
                       | 0 = Oracle  new record  |
                       | 1 = Mysql new record    |
                       | 2 = Prepei ginei export |
                       | 3 = Prepei faei delete  |
                       •—————————————————————————• */
                    invLines.Criteria = CriteriaOperator.Or(new BinaryOperator("Export", 0, BinaryOperatorType.Equal),
                                                            new BinaryOperator("Export", 1, BinaryOperatorType.Equal) 
                                                           );
                    for (int i = 0; i < invLines.Count; i++)
                    {
                        //InvLines[i].outputPath = outputPath;
                        invLines[i].Export = 2;
                    }

                    uow.CommitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(this, "UpdateInvLine", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Perfomrs an invoice export. 
        /// </summary>
        /// <returns>the result of the export</returns>
        public bool PerformInvoiceExport()
        {
            try
            {
                IDataLayer dataLayer = DataLayerSettings.DataLayers.ContainsKey(DOC_TYPES.INVENTORY) ? DataLayerSettings.DataLayers[DOC_TYPES.INVENTORY] : null;
                using (UnitOfWork uow = GetNewUnitOfWork(dataLayer))
                {
                    try
                    {
                        Header header = new Header(uow);
                        header.DocType = DOC_TYPES.INVENTORY;
                        XPCollection<InvLine> invLines = new XPCollection<InvLine>(uow);
                        invLines.Criteria = new BinaryOperator("Export", 2);
                        if (invLines.Count > 0)
                        {
                            foreach (InvLine invLine in invLines)
                            {
                                Line line = new Line(uow);
                                line.ProdBarcode = invLine.ProdBarcode;
                                line.ProdCode = invLine.ProdCode;
                                line.Qty1 = invLine.Qty;
                                header.Lines.Add(line);
                                invLine.Export = 3;//Mark lines for deletion //prepei na faei delete
                            }
                            ////header.Save();
                            PerformExport(header,"");
                            //They must be executed seperately because the list is being modified internally
                            //uow.Delete(invLines);
                            //header.Delete();
                            //uow.CommitTransaction();
                            uow.RollbackTransaction();
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        uow.RollbackTransaction();
                        uow.ReloadChangedObjects();
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(this, "PerformInvoiceExport()", ex.Message);
                return false;
            }
        }

        public ExportResult[] PerformExport(Header documentHeader,string remoteIP)
        {
            if (Settings.ExportSettingsDictionary[documentHeader.DocType].RemoveZeroQtyLines)
            {
                documentHeader.Lines.RemoveAll(x => x.Qty1 == 0);
            }
#if DEBUG
            List<Line> lines = documentHeader.Lines.Where(x => String.IsNullOrEmpty(x.ProdCode)).ToList();
            if (lines.Count > 0)
            {
                int i = 0;
            }
#endif
            documentHeader.DocDate = documentHeader.CreatedOn = documentHeader.UpdatedOn = DateTime.Now;
            ExportResult exportFileResult = PerformExportFile(documentHeader);
            ExportResult exportDatabaseResult = PerformExportDatabase(documentHeader);
            ExportResult exportApplicationResult = PerformExportApplication(documentHeader,remoteIP);
            return new ExportResult[] { exportFileResult, exportDatabaseResult, exportApplicationResult };
        }

        public List<PriceList> GetPriceLists()
        {
            String query = String.Format("SELECT * FROM {0}", Settings.DatabaseViewSettings.PriceListsView);
            return ExecuteReaderList<PriceList>(query);
        }

        protected virtual string PadCode(string code)
        {
            return PadCode(code, Settings.ProductPaddingSettings);
        }

        protected virtual string DecodeBarcode(string code)
        {
            if (Settings.DecodingSettings.DecodeBarcodes == false || string.IsNullOrEmpty(code) || Settings.DecodingSettings.MinimumDecodingLength > code.Length)
            {
                return code;
            }
            foreach (DecodingPattern pattern in Settings.DecodingSettings.DecodingPrefixes)
            {
                string trimmedCode = Settings.ProductPaddingSettings.Mode == PaddingMode.LEFT ? code.TrimStart(Settings.ProductPaddingSettings.Character) : code;
                if (trimmedCode.StartsWith(pattern.Prefix))
                {
                    //return PadCode(code.Substring(pattern.StartIndex, pattern.Length), pattern.PaddingSettings);
                    string codePart = string.Empty;

                    if ((pattern.DecodingRule.Length - pattern.StartIndex) != trimmedCode.Length)
                    {
                        throw new Exception("Έχει οριστεί λάθος κανόνας για τα barcodes που ξεκινούν από " + trimmedCode);
                    }
                    for (int character = 0; character < trimmedCode.Length; character++)
                    {
                        if (pattern.DecodingRule[character + pattern.StartIndex] == 'C')
                        {
                            codePart += trimmedCode[character];
                        }
                        else
                        {
                            codePart += pattern.PaddingSettingsCharacter;
                        }
                    }
                    return codePart;
                }
            }
            return code;
        }

        protected virtual DecodedBarcode DecodeBarcodeFull(string code)
        {
            if (Settings.DecodingSettings.DecodeBarcodes == false || string.IsNullOrEmpty(code) || Settings.DecodingSettings.MinimumDecodingLength > code.Length)
            {
                return null;
            }
            foreach (DecodingPattern pattern in Settings.DecodingSettings.DecodingPrefixes)
            {
                string withoutsStartingCharacters = Settings.ProductPaddingSettings.Mode == PaddingMode.LEFT ? code.TrimStart(Settings.ProductPaddingSettings.Character) : code;
                if (withoutsStartingCharacters.StartsWith(pattern.Prefix))
                {
                    if (pattern.DecodingRule.Contains("W"))
                    {
                        return WeightedDecodedBarcode.Decode(code, pattern);
                    }
                    else if (pattern.DecodingRule.Contains("P"))
                    {
                        return PriceDecodedBarcode.Decode(code, pattern);
                    }
                }
            }
            return null;
        }

        protected virtual List<T> ExecuteReaderList<T>(String query)
        {
            List<T> result = new List<T>();
            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        using (DbDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader != null && dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    T obj = Activator.CreateInstance<T>();
                                    AssignModelProperties(dataReader, obj);
                                    result.Add(obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(this, "SQL Command Error :" + Environment.NewLine + query + Environment.NewLine, ex.Message, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        protected virtual ExportResult PerformExportFile(Header documentHeader)
        {
            if (Settings.ExportSettingsDictionary.ContainsKey(documentHeader.DocType) == false || Settings.ExportSettingsDictionary[documentHeader.DocType].ExportMode != ExportMode.FILE)
            {
                Logger.Info(GetType().Name, "PerformExportFile", "Document with Code=" + documentHeader.Code + " does not need File export");
                return ExportResult.NOT_NEEDED;
            }
            try
            {
                foreach (FileExportSetting settings in Settings.ExportSettingsDictionary[documentHeader.DocType].FileExportSettings)
                {
                    if (settings.FileExportHasBeenSet)
                    {
                        String filename = documentHeader.ToString(settings.FullFileName);
                        bool append = settings.OverwriteMode == OverwriteMode.APPEND;
                        if (settings.OverwriteMode == OverwriteMode.RENAME && File.Exists(filename))
                        {
                            File.Move(filename, filename + "-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss"));
                        }
                        using (StreamWriter stream = new StreamWriter(filename, append))
                        {
                            String toWrite = documentHeader.ToString(settings.HeaderStringFormat);
                            if (String.IsNullOrEmpty(toWrite) == false)
                            {
                                stream.WriteLine(toWrite);
                            }
                            foreach (Line line in documentHeader.Lines)
                            {
                                toWrite = line.ToString(settings.LineStringFormat);
                                if (String.IsNullOrEmpty(toWrite) == false)
                                {
                                    stream.WriteLine(toWrite);
                                }
                            }
                        }
                        Logger.Info(GetType().Name, "PerformExportFile", "Succesfully saved Header with Code=" + documentHeader.Code);
                        return ExportResult.SUCCESFULL;
                    }
                }
                throw new Exception("Invalid Output Path ID");
            }
            catch (Exception ex)
            {
                Logger.Info(GetType().Name, "PerformExportFile", ex.Message, ex);
                return ExportResult.FAILURE;
            }
        }

        protected virtual ExportResult PerformExportDatabase(Header documentHeader)
        {
            if (Settings.ExportSettingsDictionary.ContainsKey(documentHeader.DocType) == false || Settings.ExportSettingsDictionary[documentHeader.DocType].ExportMode != ExportMode.DATABASE)
            {
                Logger.Info(GetType().Name, "PerformExportDatabase", "Document with Code=" + documentHeader.Code + "is not configured to be saved on database.");
                return ExportResult.NOT_NEEDED;
            }
            try
            {
                IDataLayer layer = DataLayerSettings.DataLayers.ContainsKey(documentHeader.DocType) ? DataLayerSettings.DataLayers[documentHeader.DocType] : null;
                using (UnitOfWork uow = GetNewUnitOfWork(layer))
                {
                    Header header = uow.GetObjectByKey<Header>(documentHeader.Oid);
                    if (header == null)
                    {
                        header = new Header(uow);
                        header.GetData(documentHeader);                        
                    }
                    foreach (Line documentLine in documentHeader.Lines)
                    {
                        Line line = uow.GetObjectByKey<Line>(documentLine.Oid);
                        if (line == null)
                        {
                            line = new Line(uow);
                            line.GetData(documentLine);
                            line.Save();
                        }
                    }
                    // Just Trigger the property to update the relevant field
                    int r = header.LineCount;
                    header.Save();
                    uow.CommitChanges();//uow.CommitTransaction();
                }
                Logger.Info(GetType().Name, "PerformExportDatabase", "Document with Code " + documentHeader.Code + "has been succesfully saved on database.");
                return ExportResult.SUCCESFULL;
            }
            catch (Exception ex)
            {
                Logger.Info(GetType().Name, "PerformExportDatabase", ex.Message, ex);
                return ExportResult.FAILURE;
            }
        }

        protected virtual bool IsApplication(ExportMode exportMode)
        {
            return !(exportMode.Equals(ExportMode.DATABASE) || exportMode.Equals(ExportMode.FILE));
        }

        protected virtual ExportResult PerformExportApplication(Header documentHeader, string remoteIP)
        {
            if (Settings.ExportSettingsDictionary.ContainsKey(documentHeader.DocType) == false
                || !IsApplication(Settings.ExportSettingsDictionary[documentHeader.DocType].ExportMode))
            {
                Logger.Info(GetType().Name, "PerformExportApplication", "Document with Code=" + documentHeader.Code + "is not configured to be saved directly on an application.");
                return ExportResult.NOT_NEEDED;
            }
            try
            {
                string logMessage = String.Empty;
                bool exportResult = false;
                switch (Settings.ExportSettingsDictionary[documentHeader.DocType].ExportMode)
                {
                    case ExportMode.REFLEXIS:
                        exportResult = Settings.ReflexisExportSettings.PerformExport(documentHeader, remoteIP,out logMessage);
                        Logger.Info(Settings.ReflexisExportSettings, "PerformExport", logMessage);
                        break;
                    case ExportMode.WRM:
                        exportResult = Settings.WRMExportSettings.PerformExport(documentHeader, remoteIP, out logMessage);
                        Logger.Info(Settings.WRMExportSettings, "PerformExport", logMessage);
                        break;
                    default:
                        return ExportResult.NOT_NEEDED;
                }

                if(!exportResult)
                {
                    //throw new Exception();
                    return ExportResult.FAILURE;
                }

                Logger.Info(GetType().Name, "PerformExportApplication", "Document with Code " + documentHeader.Code + "has been succesfully exported.");
                return ExportResult.SUCCESFULL;
            }
            catch (Exception ex)
            {
                Logger.Info(GetType().Name, "PerformExportApplication", ex.Message, ex);
                return ExportResult.FAILURE;
            }
        }

        protected virtual bool ExecuteQuery(String query)
        {
            //if (Settings.TraceSQL)
            //{
            //    File.AppendAllText("%tmp%\\mob@sto-trace.log", query + Environment.NewLine);
            //}
            bool result = false;
            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(this, "SQL Command Error :" + Environment.NewLine + query + Environment.NewLine, ex.Message, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        protected virtual T ExecuteReader<T>(String query)
        {
            T result = default(T);
            string timeFormat = "HH:mm:ss.fff";
            Trace.WriteLine("ExecuteReader Started at : "+DateTime.Now.ToString(timeFormat));
            using (DbConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    Trace.WriteLine("Connection Opened at: " + DateTime.Now.ToString(timeFormat));
                    using (DbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        
                        using (DbDataReader dataReader = command.ExecuteReader())
                        {
                            Trace.WriteLine("Query completed at: " + DateTime.Now.ToString(timeFormat));
                            if (dataReader != null && dataReader.HasRows)
                            {
                                result = Activator.CreateInstance<T>();
                                dataReader.Read();
                                Trace.WriteLine("Data fetch Completed at : " + DateTime.Now.ToString(timeFormat));
                                AssignModelProperties(dataReader, result);
                                Trace.WriteLine("Model object Created at : " + DateTime.Now.ToString(timeFormat));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(this, "SQL Command Error :" + Environment.NewLine + query + Environment.NewLine, ex.Message, ex);
                }
                finally
                {
                    connection.Close();
                    Trace.WriteLine("Connection Closed at : " + DateTime.Now.ToString(timeFormat));
                }
            }

            return result;
        }

        private void AssignModelProperties<T>(DbDataReader dataReader, T result)
        {
            DataTable schema = dataReader.GetSchemaTable();
            Dictionary<String, String> fieldNames = new Dictionary<String, String>();
            foreach (DataRow column in schema.Rows)
            {
                fieldNames.Add(column["ColumnName"].ToString().ToUpper(), column["ColumnName"].ToString());
            }
            foreach (PropertyInfo pInfo in typeof(T).GetProperties().Where(x => x.CanWrite && x.GetCustomAttributes(typeof(IgnoreFieldFromView), true).Count() == 0))
            {
                if (fieldNames.ContainsKey(pInfo.Name.ToUpper()))
                {
                    try
                    {
                        object dbValue = dataReader[fieldNames[pInfo.Name.ToUpper()]];
                        object value;
                        if (Convert.IsDBNull(dbValue))
                        {
                            value = GetDefault(pInfo.PropertyType);
                        }
                        else
                        {
                            value = Convert.ChangeType(dbValue.ToString(), pInfo.PropertyType);
                        }
                        pInfo.SetValue(result, value, null);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }        

        public bool DeleteExportedInventoryLines()
        {
            try
            {
                IDataLayer dataLayer = DataLayerSettings.DataLayers.ContainsKey(DOC_TYPES.INVENTORY) ? DataLayerSettings.DataLayers[DOC_TYPES.INVENTORY] : null;
                using (UnitOfWork uow = GetNewUnitOfWork(dataLayer))
                {
                    try
                    {
                        XPCollection<InvLine> invLines = new XPCollection<InvLine>(uow);
                        invLines.Criteria = new BinaryOperator("Export", 3);
                        if (invLines.Count > 0)
                        {
                            uow.Delete(invLines);
                            uow.CommitTransaction();
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        uow.RollbackTransaction();
                        uow.ReloadChangedObjects();
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(this, "DeleteExportedInventoryLines()", ex.Message);
                return false;
            }
        }

        public bool MarkExportedInventoryLinesForDeletion()
        {
            try
            {
                IDataLayer dataLayer = DataLayerSettings.DataLayers.ContainsKey(DOC_TYPES.INVENTORY) ? DataLayerSettings.DataLayers[DOC_TYPES.INVENTORY] : null;
                using (UnitOfWork uow = GetNewUnitOfWork(dataLayer))
                {
                    try
                    {
                        XPCollection<InvLine> invLines = new XPCollection<InvLine>(uow);
                        invLines.Criteria = new BinaryOperator("Export", 2);
                        if (invLines.Count > 0)
                        {
                            foreach (InvLine invLine in invLines)
                            {
                                invLine.Export = 3;
                                invLine.Save();
                            }
                            uow.CommitTransaction();
                            return true;
                        }
                    }
                    catch (Exception exception)
                    {
                        uow.RollbackTransaction();
                        uow.ReloadChangedObjects();
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(this, "DeleteExportedInventoryLines()", ex.Message);
                return false;
            }
        }
    }
}
