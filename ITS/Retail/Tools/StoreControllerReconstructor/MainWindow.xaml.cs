using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StoreControllerReconstructor.AuxilliaryClasses;
using StoreControllerReconstructor.ConnectionHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace StoreControllerReconstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StoreControllerRecontructorSettings Settings { get; set; }

        private Store Store { get; set; }

        private XpoLayerHelper SourceLayer { get; set; }

        private XpoLayerHelper DestinationLayer { get; set; }

        public LogHelper Logger { get; set; }

        private List<Type> MASTER_TO_STORE_CONTROLLER_TYPES
        {
            get
            {
                return typeof(Item).Assembly
                                           .GetTypes()
                                           .Where(t => typeof(BasicObj).IsAssignableFrom(t)
                                               && t.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() <= 0
                                               && t.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0
                                               )
                                           .OrderBy(t => ((UpdaterAttribute)t.GetCustomAttributes(typeof(UpdaterAttribute), false).First()).Order)
                                           .ToList();
            }
        }

        private Dictionary<Type, CriteriaOperator> ADDITIONAL_TYPES_BASED_ON_STORE
        {
            get
            {
                CriteriaOperator storeCriteria = new BinaryOperator("Store", this.Store.Oid);
                CriteriaOperator posStoreCriteria = new BinaryOperator("POS.Store", this.Store.Oid);
                CriteriaOperator docStoreCriteria = new BinaryOperator("DocumentHeader.Store", this.Store.Oid);
                CriteriaOperator docDetStoreCriteria = new BinaryOperator("DocumentDetail.DocumentHeader.Store", this.Store.Oid);
                CriteriaOperator actionStoreCriteria = new BinaryOperator("ActionTypeEntity.Store", this.Store.Oid);
                CriteriaOperator customerStoreCriteria = new BinaryOperator("PriceCatalog.IsEditableAtStore", this.Store.Oid);
                CriteriaOperator dataStoreCriteria = new BinaryOperator("Owner.Stores", this.Store.CentralOid);
                CriteriaOperator dataCategoryStoreCriteria = new BinaryOperator("Owner.Stores", this.Store.CentralOid);
                CriteriaOperator decodedStoreCriteria = new BinaryOperator("Store", this.Store.Oid);         
                CriteriaOperator docTypeMapStoreCriteria = new BinaryOperator("DocumentType.Owner.Stores", this.Store.CentralOid);
                CriteriaOperator docTypeRoleStoreCriteria = new BinaryOperator("DocumentType.Owner.Stores", this.Store.CentralOid);
                CriteriaOperator memTypeStoreCriteria = new BinaryOperator("Owner.Stores", this.Store.CentralOid);
                CriteriaOperator ownerCatStoreCriteria = new BinaryOperator("Owner.Stores", this.Store.CentralOid);
                CriteriaOperator sdrdtStoreCriteria = new BinaryOperator("StoreDailyReport.Store", this.Store.Oid);
                CriteriaOperator sdrdhStoreCriteria = new BinaryOperator("StoreDailyReport.Store", this.Store.Oid);
                CriteriaOperator sdrpStoreCriteria = new BinaryOperator("DailyReport.Store", this.Store.Oid);
                CriteriaOperator varActionTypeStoreCriteria = new BinaryOperator("Variable.Owner.Stores", this.Store.CentralOid);
                CriteriaOperator verEmailTokenStoreCriteria = new BinaryOperator("User.CentralStore", this.Store.CentralOid);
                CriteriaOperator widManStoreCriteria = new BinaryOperator("User.CentralStore", this.Store.CentralOid);
                CriteriaOperator relDocStoreCriteria = new BinaryOperator("InitialDocument.Store", this.Store.Oid);
                CriteriaOperator relDocDetStoreCriteria = new BinaryOperator("InitialDocumentDetail.DocumentHeader.Store", this.Store.Oid);

                return new Dictionary<Type, CriteriaOperator>()
                {
                    { typeof(MobileTerminal), storeCriteria },
                    { typeof(POS), storeCriteria },
                    { typeof(POSDocumentSeries),  posStoreCriteria},
                    { typeof(ZReportSequence), posStoreCriteria },
                    { typeof(DailyTotals), posStoreCriteria },
                    { typeof(ElectronicJournalFilePackage), storeCriteria },
                    { typeof(EdpsBatchTotal), storeCriteria },
                    { typeof(StoreDailyReport), storeCriteria },
                    { typeof(ActionTypeDocStatus), actionStoreCriteria },
                    { typeof(CustomerCategoryDiscount), customerStoreCriteria },
                    { typeof(DataView), dataStoreCriteria },
                    { typeof(DataViewCategory), dataCategoryStoreCriteria },
                    { typeof(DecodedRawData), decodedStoreCriteria },
                    { typeof(DocumentTypeMapping), docTypeMapStoreCriteria },
                    { typeof(DocumentTypeRole), docTypeRoleStoreCriteria },
                    { typeof(MemberType), memTypeStoreCriteria },
                    { typeof(OwnerCategories), ownerCatStoreCriteria },
                    { typeof(StoreDailyReportDailyTotal), sdrdtStoreCriteria },
                    { typeof(StoreDailyReportDocumentHeader), sdrdhStoreCriteria },
                    { typeof(StoreDailyReportPayment), sdrpStoreCriteria },
                    { typeof(VariableActionType), varActionTypeStoreCriteria },
                    { typeof(VerifyEmailToken), verEmailTokenStoreCriteria },
                    { typeof(WidgetManager), widManStoreCriteria },
                    { typeof(RelativeDocument), relDocStoreCriteria },
                    { typeof(RelativeDocumentDetail), relDocDetStoreCriteria }                 
                    //{ typeof(DocumentSequence),  }
                };
            }
        }

        private Dictionary<Type, CriteriaOperator> DOCUMENT_TYPES
        {
            get
            {
                CriteriaOperator storeCriteria = new BinaryOperator("Store", this.Store.Oid);
                CriteriaOperator docStoreCriteria = new BinaryOperator("DocumentHeader.Store", this.Store.Oid);
                CriteriaOperator docDetStoreCriteria = new BinaryOperator("DocumentDetail.DocumentHeader.Store", this.Store.Oid);

                return new Dictionary<Type, CriteriaOperator>()
                {
                    { typeof(DocumentHeader), storeCriteria },
                    { typeof(DocumentDetail), docStoreCriteria },
                    { typeof(DocumentDetailDiscount), docDetStoreCriteria },
                    { typeof(DocumentPromotion), docStoreCriteria },
                    { typeof(DocumentPayment), docStoreCriteria },
                    { typeof(DocumentPaymentEdps), docStoreCriteria }
                };
            }
        }

        private int numberOfTotalTables;
        private int NUMBER_OF_TOTAL_TABLES
        {
            get
            {
                if (numberOfTotalTables <= 0)
                {
                    numberOfTotalTables = MASTER_TO_STORE_CONTROLLER_TYPES.Count + ADDITIONAL_TYPES_BASED_ON_STORE.Count;
                }
                return numberOfTotalTables;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Settings = new StoreControllerRecontructorSettings();
            this.Settings.SourceConnectionSettings.PasswordHandler = () => SourcePassword.Password;
            this.Settings.DestinationConnectionSettings.PasswordHandler = () => DestinationPassword.Password;

            this.Logger = new LogHelper(this.LogRichTextBox, StoreControllerReconstructorContants.LOG_FILE);

            this.DataContext = this.Settings;
        }

        private bool InputIsValid()
        {
            this.Settings.SourceConnectionSettings.Password = this.Settings.SourceConnectionSettings.PasswordHandler();
            this.Settings.SourceConnectionSettings.DatabaseType = (DatabaseType)this.SourceServerType.SelectedIndex;

            this.Settings.DestinationConnectionSettings.Password = this.Settings.DestinationConnectionSettings.PasswordHandler();
            this.Settings.DestinationConnectionSettings.DatabaseType = (DatabaseType)this.DestinationServerType.SelectedIndex;

            return this.Settings.IsValid;
        }

        private int DateIsValid()
        {
            this.Settings.StartDate = (DateTime)this.DatePickerStart.SelectedDate;
            this.Settings.EndDate = (DateTime)this.DatePickerEnd.SelectedDate;     

            return this.Settings.IsDateValid;
        }

        private bool DataSourceIsAvailable()
        {
            try
            {
                using (UnitOfWork uow = this.SourceLayer.GetNewUnitOfWork())
                {
                    XPCollection<StoreControllerSettings> storeControllerSettings = new XPCollection<StoreControllerSettings>(uow);
                }
                return true;
            }
            catch (Exception exception)
            {
                this.Logger.Error("Unable to connect to Source" + Environment.NewLine + exception.Message);
                return false;
            }
        }

        private bool StoreExists()
        {
            using (UnitOfWork sourceUow = this.SourceLayer.GetNewUnitOfWork())
            {
                this.Store = sourceUow.FindObject<Store>(new BinaryOperator("Code",this.Settings.StoreCode));
                return this.Store != null;
            }
        }

        private void CreateDestinationSchema()
        {
            this.DestinationLayer.UpdateDatabase();
        }

        private void DropConstraintsOnDestination()
        {
            using (UnitOfWork uow = this.DestinationLayer.GetNewUnitOfWork() )
            {
                string query = String.Empty;
                switch( this.DestinationLayer.Databasetype)
                {
                    case DatabaseType.MS_SQL:
                        query = @"  DECLARE @cursor CURSOR
                                    DECLARE @supplumentary_cursor CURSOR
                                    DECLARE @schema NVARCHAR(max)
                                    DECLARE @remaining_constraints int

                                    DECLARE @query NVARCHAR(max)

                                    SET @schema = '"+this.DestinationLayer.Database+"'"
                                 +@"
                                    SET  @cursor = CURSOR FOR
                                    SELECT  CONCAT('ALTER TABLE ""',TABLE_NAME, '"" DROP CONSTRAINT ',CONSTRAINT_NAME, ';' ) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                    WHERE constraint_catalog = @schema

                                    SET @supplumentary_cursor = CURSOR FOR
                                    SELECT count(CONSTRAINT_NAME) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                    WHERE constraint_catalog = @schema

                                    OPEN @supplumentary_cursor

                                        FETCH NEXT FROM @supplumentary_cursor into @remaining_constraints
                                    CLOSE @supplumentary_cursor
                                    DEALLOCATE @supplumentary_cursor


                                    OPEN @cursor


                                        FETCH NEXT FROM @cursor INTO @query

                                        WHILE @@FETCH_STATUS = 0 AND @remaining_constraints > 0

                                        BEGIN
                                            BEGIN TRY
                                                EXECUTE sp_executesql @query

                                                OPEN @supplumentary_cursor

                                                    FETCH NEXT FROM @supplumentary_cursor into @remaining_constraints

                                                CLOSE @supplumentary_cursor

                                                DEALLOCATE @supplumentary_cursor

                                            END TRY

                                            BEGIN CATCH

                                                --PRINT CONCAT(@query, ' failed will try later')

                                            END CATCH

                                            FETCH NEXT FROM @cursor INTO @query

                                        END


                                    CLOSE @cursor
                                    DEALLOCATE @cursor";

                        break;
                    case DatabaseType.Postgres:
                        query = @"DO $$DECLARE r RECORD;
DECLARE number_of_constraints INT;
BEGIN

	SELECT count(c.conname) into number_of_constraints
	FROM pg_constraint c
	  JOIN pg_class t ON c.conrelid = t.oid
	  JOIN pg_namespace n ON t.relnamespace = n.oid;

	WHILE number_of_constraints > 0 LOOP
	
	
		FOR r in
			SELECT 
			       t.relname AS table_name,
			       c.conname AS constraint_name
			FROM pg_constraint c
			  JOIN pg_class t ON c.conrelid = t.oid
			  JOIN pg_namespace n ON t.relnamespace = n.oid
		LOOP
			BEGIN
				EXECUTE 'ALTER TABLE ' || quote_ident(r.table_name)|| ' DROP CONSTRAINT '|| quote_ident(r.constraint_name) || ';';
				
				SELECT count(c.conname) into number_of_constraints
				FROM pg_constraint c
				JOIN pg_class t ON c.conrelid = t.oid
				JOIN pg_namespace n ON t.relnamespace = n.oid;
			EXCEPTION WHEN others THEN
				SELECT count(c.conname) into number_of_constraints
				FROM pg_constraint c
				JOIN pg_class t ON c.conrelid = t.oid
				JOIN pg_namespace n ON t.relnamespace = n.oid;
			END;
		END LOOP;
	END LOOP;
END$$;";
                        break;
                }
                Logger.Message("Dropping all constraints");
                uow.ExecuteNonQuery(query);
                uow.ExecuteNonQuery(query);
                Logger.Message("Constraints dropped");
            }
        }

        private void DropIndicesOnDestination()
        {
            using (UnitOfWork uow = this.DestinationLayer.GetNewUnitOfWork())
            {
                string query = String.Empty;
                switch (this.DestinationLayer.Databasetype)
                {
                    case DatabaseType.MS_SQL:
                        query = @"DECLARE @cursor CURSOR
                                    DECLARE @supplumentary_cursor CURSOR
                                    DECLARE @query NVARCHAR(max)
                                    DECLARE @remaining_indices INT


                                    SET @cursor = CURSOR FOR
                                    SELECT 
                                      CONCAT('DROP INDEX ',i.Name,' ON [', o.name,'];')
                                    FROM sys.indexes i 
                                    JOIN sys.objects o on i.object_id = o.object_id
                                    JOIN sys.index_columns ic on ic.object_id = i.object_id 
                                        and ic.index_id = i.index_id
                                    JOIN sys.columns co on co.object_id = i.object_id 
                                        and co.column_id = ic.column_id
                                    WHERE i.[type] = 2 
                                    AND i.is_unique = 0 
                                    AND i.is_primary_key = 0
                                    AND o.[type] = 'U'

                                    SET @supplumentary_cursor = CURSOR FOR
                                    SELECT COUNT(*)
                                    FROM sys.indexes i 
                                    JOIN sys.objects o on i.object_id = o.object_id
                                    JOIN sys.index_columns ic on ic.object_id = i.object_id 
                                        and ic.index_id = i.index_id
                                    JOIN sys.columns co on co.object_id = i.object_id 
                                        and co.column_id = ic.column_id
                                    WHERE i.[type] = 2 
                                    AND i.is_unique = 0 
                                    AND i.is_primary_key = 0
                                    AND o.[type] = 'U'


                                    OPEN @cursor
	
	                                    OPEN @supplumentary_cursor
		                                    FETCH NEXT FROM @supplumentary_cursor INTO @remaining_indices
	                                    CLOSE @supplumentary_cursor
	                                    DEALLOCATE @supplumentary_cursor

	                                    FETCH NEXT FROM @cursor INTO @query
	                                    WHILE @@FETCH_STATUS = 0 AND @remaining_indices > 0
	                                    BEGIN
		                                    EXECUTE sp_executesql @query
		                                    --PRINT @query
		
		                                    OPEN @supplumentary_cursor
			                                    FETCH NEXT FROM @supplumentary_cursor INTO @remaining_indices
		                                    CLOSE @supplumentary_cursor
		                                    DEALLOCATE @supplumentary_cursor

		                                    FETCH NEXT FROM @cursor INTO @query
	                                    END
                                    CLOSE @cursor
                                    DEALLOCATE @cursor;";
                        break;
                    case DatabaseType.Postgres:
                        query = @"DO $$DECLARE r RECORD;

                                    BEGIN
	                                    FOR r in
	                                    SELECT *
	                                     FROM pg_class, pg_index
	                                     WHERE pg_class.oid = pg_index.indexrelid
	                                     AND pg_class.oid IN (
	                                         SELECT indexrelid
	                                         FROM pg_index, pg_class
	                                         WHERE pg_class.oid=pg_index.indrelid
	                                         AND indisunique != 't'
	                                         AND indisprimary != 't'
	                                         AND relname !~ '^pg_') LOOP
		                                    EXECUTE 'DROP INDEX ""' || r.relname || '"";';
                                        END LOOP;
                                END$$; ";
                        break;
                }
                Logger.Message("Dropping all indices");
                uow.ExecuteNonQuery(query);
                Logger.Message("Indices dropped");
            }
        }

        private static Type[] NUMERIC_TYPES = new Type[] { typeof(int), typeof(short), typeof(long), typeof(float), typeof(double) };

        private void CopyObject(UnitOfWork sourceUnitOfWork, object sourceObject, UnitOfWork destinationUnitOfWork)
        {
            object destinationObject = null;

            string sourceJson = ((BasicObj)sourceObject).ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            JObject jsonObject = JsonConvert.DeserializeObject(sourceJson) as JObject;
            string primaryKey = jsonObject.Property("Oid").Value.ToString();

            XPClassInfo xpClassInfo = sourceUnitOfWork.GetClassInfo(sourceObject);

            destinationObject = destinationUnitOfWork.GetObjectByKey(sourceObject.GetType(), Guid.Parse(primaryKey));
            if (destinationObject == null)
            {
                destinationObject = Activator.CreateInstance(sourceObject.GetType(), destinationUnitOfWork);
            }

            string errorMessage = String.Empty;
            ((BasicObj)destinationObject).FromJson(jsonObject, PlatformConstants.JSON_SERIALIZER_SETTINGS, copyOid: true, errorOnOidNotFound: false, error: out errorMessage);
            if ( !String.IsNullOrWhiteSpace(errorMessage) )
            {
                throw new Exception(errorMessage);
            }

            ((BasicObj)destinationObject).SkipOnSavingProcess = true;
            ((BasicObj)destinationObject).Save();
        }

        private void TransferData(bool documentOnly)
        {
            try
            {
                using (UnitOfWork sourceUnitOfWork = this.SourceLayer.GetNewUnitOfWork())
                {
                    using (UnitOfWork destinationUnitOfWork = this.DestinationLayer.GetNewUnitOfWork())
                    {
                        SaveWRMApplicationSettings(destinationUnitOfWork);

                        CompanyNew owner = sourceUnitOfWork.GetObjectByKey<CompanyNew>(this.Store.Owner.Oid);
                        Store store = sourceUnitOfWork.GetObjectByKey<Store>(this.Store.Oid);

                        if (!documentOnly)
                        {
                            int tableCounter = CopyMasterToStoreControllerEntities(sourceUnitOfWork, destinationUnitOfWork, owner, store, documentOnly);
                            CopyAdditionalTypesBasedOnStore(sourceUnitOfWork, destinationUnitOfWork, tableCounter, documentOnly, ADDITIONAL_TYPES_BASED_ON_STORE);
                        }
                        else
                        {
                            int tableCounter = 140; // fix for bringing total progress bar to 100%
                            CopyAdditionalTypesBasedOnStore(sourceUnitOfWork, destinationUnitOfWork, tableCounter, documentOnly, DOCUMENT_TYPES);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private static void SaveWRMApplicationSettings(UnitOfWork destinationUnitOfWork)
        {
            WRMApplicationSettings wrmApplicationSettings = new WRMApplicationSettings(destinationUnitOfWork)
            {
                ApplicationInstance = eApplicationInstance.STORE_CONTROLER
            };
            wrmApplicationSettings.Save();
            destinationUnitOfWork.CommitChanges();
        }

        private void CopyAdditionalTypesBasedOnStore(UnitOfWork sourceUnitOfWork, UnitOfWork destinationUnitOfWork, int tableCounter, bool documentsOnly, Dictionary<Type, CriteriaOperator> DESIRED_TYPES)
        {
            foreach (KeyValuePair<Type, CriteriaOperator> pair in DESIRED_TYPES)
            {
                tableCounter++;
                Type type = pair.Key;
                CriteriaOperator criteria = pair.Value;

                if (documentsOnly) // if Entity is Document Retail 
                {
                    Int64 StartDateUpdatedOnTicks, EndDateUpdatedOnTicks;
                    StartDateUpdatedOnTicks = this.Settings.StartDate.Ticks;
                    DateTime upperExclusive = this.Settings.EndDate.Date.AddDays(1).AddTicks(-1);
                    EndDateUpdatedOnTicks = upperExclusive.Ticks;

                    if (type == typeof(DocumentHeader) || type == typeof(DocumentDetailDiscount) || type == typeof(DocumentDetail) ||
                        type == typeof(DocumentPromotion) || type == typeof(DocumentPayment) || type == typeof(DocumentPaymentEdps))
                    {
                        CriteriaOperator dateRangeCriteriaDH = CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", StartDateUpdatedOnTicks, BinaryOperatorType.GreaterOrEqual), new BinaryOperator("UpdatedOnTicks", EndDateUpdatedOnTicks, BinaryOperatorType.LessOrEqual));
                        criteria = CriteriaOperator.And(dateRangeCriteriaDH, criteria);
                    }                                    
                }

                XPCursor xpcursor = new XPCursor(sourceUnitOfWork, type, criteria);

                CopyAllObjectsForEntity(sourceUnitOfWork, destinationUnitOfWork, tableCounter, type, criteria, xpcursor);
            }
        }

        private int CopyMasterToStoreControllerEntities(UnitOfWork sourceUnitOfWork, UnitOfWork destinationUnitOfWork, CompanyNew owner, Store store, bool documentsOnly)
        {            
            int tableCounter = 0;

            foreach (Type type in MASTER_TO_STORE_CONTROLLER_TYPES)
            {
                tableCounter++;

                    UpdaterAttribute updaterAttribute = (UpdaterAttribute)type.GetCustomAttributes(typeof(UpdaterAttribute), true).First();
                    if (updaterAttribute.Permissions == eUpdateDirection.MASTER_TO_STORECONTROLLER
                    || updaterAttribute.Permissions == (eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)
                       )
                    {
                        //Direction is Ok
                    }
                    else
                    {
                        continue;
                    }

                    MethodInfo getUpdaterCriteriaMethod = type.GetMethod("GetUpdaterCriteria");
                    CriteriaOperator criteria = null;

                    if (getUpdaterCriteriaMethod != null)
                    {
                        criteria = (CriteriaOperator)getUpdaterCriteriaMethod.Invoke(null, new object[] { eUpdateDirection.MASTER_TO_STORECONTROLLER, owner, store, String.Empty });
                    }

                    if (documentsOnly) // if Entity is Document Retail 
                    {
                        Int64 StartDateUpdatedOnTicks, EndDateUpdatedOnTicks;
                        StartDateUpdatedOnTicks = this.Settings.StartDate.Ticks;
                        DateTime upperExclusive = this.Settings.EndDate.Date.AddDays(1).AddTicks(-1);
                        EndDateUpdatedOnTicks = upperExclusive.Ticks;

                        if (type == typeof(DocumentHeader) || type == typeof(DocumentDetailDiscount) || type == typeof(DocumentDetail) ||
                            type == typeof(DocumentPromotion) || type == typeof(DocumentPayment) || type == typeof(DocumentPaymentEdps))
                        {
                            CriteriaOperator dateRangeCriteriaDH = CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", StartDateUpdatedOnTicks, BinaryOperatorType.GreaterOrEqual), new BinaryOperator("UpdatedOnTicks", EndDateUpdatedOnTicks, BinaryOperatorType.LessOrEqual));
                            criteria = CriteriaOperator.And(dateRangeCriteriaDH, criteria);
                        }                                    
                    }

                    XPCursor xpcursor = new XPCursor(sourceUnitOfWork, type, criteria);

                    CopyAllObjectsForEntity(sourceUnitOfWork, destinationUnitOfWork, tableCounter, type, criteria, xpcursor);
            }
            return tableCounter;
        }

        private void CopyAllObjectsForEntity(UnitOfWork sourceUnitOfWork, UnitOfWork destinationUnitOfWork, int tableCounter, Type type, CriteriaOperator criteria, XPCursor xpcursor)
        {
            xpcursor.PageSize = 150;
            int savedRecords = 0;
            int objectCounter = 0;
            int savePerObjects = 1000;

            int numberOfTotalSourceRecords = (int)sourceUnitOfWork.Evaluate(type, CriteriaOperator.Parse("Count"), criteria);
            string startMessage = String.Format("Begin transfering {0} records of {1}", numberOfTotalSourceRecords, type.FullName);
            LogDispatched(startMessage);

            foreach (object sourceObject in xpcursor)
            {
                try
                {
                    CopyObject(sourceUnitOfWork, sourceObject, destinationUnitOfWork);
                    objectCounter++;
                    savedRecords++;
                    if (objectCounter == savePerObjects)
                    {
                        destinationUnitOfWork.CommitChanges();
                        objectCounter = 0;
                        RefreshTableProgressBar(type.Name, savedRecords, numberOfTotalSourceRecords);
                    }
                }
                catch (Exception exception)
                {
                    LogException(exception);
                }
            }
            try
            {
                destinationUnitOfWork.CommitChanges();
                RefreshTableProgressBar(type.Name, savedRecords, numberOfTotalSourceRecords);
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
            string endMessage = String.Format("End transfering {0}", type.FullName);
            LogDispatched(endMessage);
            RefreshTotalProgressBar(type.Name, tableCounter, NUMBER_OF_TOTAL_TABLES);
        }

        private void RefreshProgress(double percentage, string message, System.Windows.Controls.ProgressBar progressBar)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                progressBar.Value = percentage;
                Logger.Message(message);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                  new Action(() =>
                  {
                      progressBar.Value = percentage;
                      Logger.Message(message);
                  }));
            }
        }

        private void RefreshTotalProgressBar(string tableName,int tableCounter, int numberOfTotalTables)
        {
            string message = String.Format("Table {0} {1} out of {2}", tableName, tableCounter, numberOfTotalTables);
            double percentage = 0;
            if (numberOfTotalTables != 0)
            {
                percentage = ((double)tableCounter / numberOfTotalTables) * 100;
            }
            this.TotalProgresLabel.Content = String.Format("Συνολική Πρόοδος {0}/{1} :", tableCounter, numberOfTotalTables);
            RefreshProgress(percentage, message, this.TotalProgresBar);
        }

        private void RefreshTableProgressBar(string tableName, int savedRecords, int numberOfTotalSourceRecords)
        {
            string message = String.Format("Saved {0} out of {1} from table {2}", savedRecords, numberOfTotalSourceRecords,tableName);
            double percentage = 0;
            if ( numberOfTotalSourceRecords != 0 )
            {
                percentage = ((double)savedRecords / numberOfTotalSourceRecords) * 100;
            }
            this.TableProgressLabel.Content = String.Format(" {0}/{1} ({2}) :", savedRecords, numberOfTotalSourceRecords, tableName);
            RefreshProgress(percentage, message, this.TableProgresBar);
        }

        private void LogDispatched(string startMessage)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Logger.Message(startMessage);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                  new Action(() =>
                  {
                      Logger.Message(startMessage);
                  }));
            }
        }

        private void LogException(Exception exception)
        {
            string errorMessage = exception.Message;
            if (exception.InnerException != null && !String.IsNullOrEmpty(exception.InnerException.Message))
            {
                errorMessage += String.Format("InnerException : {0}{1}", exception.InnerException.Message, Environment.NewLine);
            }
            if (!String.IsNullOrEmpty(exception.StackTrace))
            {
                errorMessage += String.Format("Stack Trace : {0}{1}", exception.StackTrace, Environment.NewLine);
            }
            Logger.Error(errorMessage);
        }

        private void TransferData_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                if (InputIsValid() == false)
                {
                    this.Logger.Error("Please fill all input fields");
                    return;
                }

                this.SourceLayer = new XpoLayerHelper(this.Settings.SourceConnectionSettings);
                this.DestinationLayer = new XpoLayerHelper(this.Settings.DestinationConnectionSettings);

                if (DataSourceIsAvailable() == false)
                {
                    return;
                }

                if (StoreExists() == false)
                {
                    return;
                }
                
                TransferDataProcess(false);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception.Message);
            }            
        }

        private void DocumentFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DateIsValid() < 0)
                {
                    this.Logger.Error("Please fill properly all dates");
                    return;
                }
                else
                {
                    if (InputIsValid() == false)
                    {
                        this.Logger.Error("Please fill all input fields");
                        return;
                    }

                    this.SourceLayer = new XpoLayerHelper(this.Settings.SourceConnectionSettings);
                    this.DestinationLayer = new XpoLayerHelper(this.Settings.DestinationConnectionSettings);

                    if (DataSourceIsAvailable() == false)
                    {
                        return;
                    }

                    if (StoreExists() == false)
                    {
                        return;
                    }

                    TransferDataProcess(true);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception.Message);
            }
        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputIsValid() == false)
                {
                    this.Logger.Error("Please fill all input fields");
                    return;
                }

                this.SourceLayer = new XpoLayerHelper(this.Settings.SourceConnectionSettings);
                this.DestinationLayer = new XpoLayerHelper(this.Settings.DestinationConnectionSettings);
                bool isTestConnectionCheck = true;
                this.SourceLayer.GetDataLayer(isTestConnectionCheck);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception.Message);
            }
        }

        private void TransferDataProcess(bool documentsOnly)
        {
            try
            {
                if (documentsOnly)
                {
                    this.ButtonDocumentFilter.IsEnabled = false;
                }
                else
                {
                    this.ButtonTransferData.IsEnabled = false;
                }

                CreateDestinationSchema();
                //DropConstraintsOnDestination();
                //DropIndicesOnDestination();
                TransferData(documentsOnly);
                if (!documentsOnly) {
                    CreateVersionInfoTable();
                }                         
            }
            catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void CreateVersionInfoTable()
        {
            Type[] scripts = typeof(RetailMigration).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Migration))).ToArray();

            List<RetailMigration> versions = new List<RetailMigration>();

            foreach (Type script in scripts)
            {
                RetailMigration versionInfo = script.GetCustomAttributes(typeof(RetailMigration), false).FirstOrDefault() as RetailMigration;
                versions.Add(versionInfo);
            }
            using (UnitOfWork uow = this.DestinationLayer.GetNewUnitOfWork())
            {
                switch (this.Settings.DestinationConnectionSettings.DatabaseType)
                {
                    case DatabaseType.MS_SQL:
                        uow.ExecuteNonQuery(@"CREATE TABLE VersionInfo(Version bigint,AppliedOn datetime);");
                        foreach (RetailMigration versionInfo in versions.OrderBy(x => x.Version))
                        {
                            long version = versionInfo.Version;
                            uow.ExecuteNonQuery(
                                String.Format(@"INSERT INTO VersionInfo (Version,AppliedOn) VALUES({0},GETDATE()) ;",
                                version)
                            );
                        }
                        break;
                    case DatabaseType.Postgres:
                        uow.ExecuteNonQuery(@"CREATE TABLE ""VersionInfo""(""Version"" bigint,""AppliedOn"" timestamp);");
                        foreach (RetailMigration versionInfo in versions.OrderBy(x => x.Version))
                        {
                            long version = versionInfo.Version;
                            uow.ExecuteNonQuery(
                                String.Format(@"INSERT INTO ""VersionInfo"" (""Version"",""AppliedOn"") VALUES({0},current_timestamp) ;",
                                version)
                            );
                        }
                        break;
                    default:
                        throw new Exception("Database type not supported");
                }
            }
        }
    }
}
