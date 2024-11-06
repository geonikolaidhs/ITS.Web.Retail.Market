using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WrmDbTransfer;
using ITS.Retail.WrmDbTransfer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ITS.Retail.Model.BasicObj;

namespace DbTrans.Classes
{
    public static class DbTransferService
    {
        public delegate void OnDatabaseUpdate(int totalRows, int rowsTransfered, string currenttable, int remainningRows = 0);
        public static event OnDatabaseUpdate OnDatabaseUpdateEventHandler;
        private static bool IsUpdating = false;
        private static Stopwatch _BatchWatch = new Stopwatch();
        private static IEnumerable<XPClassInfo> Classes = null;
        private static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
        private static List<string> IgnoreProperties = new List<string>() { "Session", "ClassInfo", "IsLoading", "SkipOnSavingProcess", "Loading", "IsLoading", "TempObjExists", "CreatedOn", "UpdatedOn", "OptimisticLockField" };
        private static int counter = 0;
        public static void Init()
        {
            Classes = GetClasses(eUpdateDirection.MASTER_TO_STORECONTROLLER);
        }

        public static async Task<string> TransferDatabase(DbHelper sourceDbHelper, DbHelper targetDbHelper, bool startFromCurrentVersion, int totalRows, int remainningRows = 0)
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                StringBuilder sb = new StringBuilder();
                StringBuilder sbCount = new StringBuilder();
                try
                {
                    targetDbHelper.UpdateDatabase();
                    sourceDbHelper.UpdateDatabase();
                    remainningRows = totalRows;
                    foreach (XPClassInfo cl in Classes)
                    {
                        Stopwatch w1 = new Stopwatch();
                        Program.LogFile.Info("START TRANSFERING TABLE : " + cl.TableName);

                        w1.Start();
                        if (!startFromCurrentVersion)
                        {
                            SetVersion(targetDbHelper.GetNewUnitOfWork(), cl.ClassType, 0);
                        }
                        counter = 0;
                        await Task.Run(() =>
                        {
                            int res = TransferTable(cl.ClassType, sourceDbHelper, targetDbHelper, totalRows);
                            remainningRows = remainningRows - res;
                            RaiseOnTableUpdate(totalRows, 0, cl.TableName, remainningRows);
                        });
                        int sourceTableRows = CountTable(cl.ClassType, sourceDbHelper.GetNewUnitOfWork());
                        int targetTableRows = CountTable(cl.ClassType, targetDbHelper.GetNewUnitOfWork());
                        sb.AppendLine(cl.TableName + " : SOURCE ROWS = " + sourceTableRows + " - TARGET ROWS = " + targetTableRows);
                        w1.Stop();
                        Program.LogFile.Info("TRANSFER COMPLETE FOR TABLE : " + cl.TableName + " ELAPSED TIME " + w1.ElapsedMilliseconds + "ms");
                    }
                    UpdateTableVersionInfo(sourceDbHelper, targetDbHelper);
                }
                catch (Exception ex)
                {
                    Program.LogFile.Error(ex.Message);
                }
                Program.LogFile.Info(sbCount.ToString()); ;
                IsUpdating = false;
                return sb.ToString();
            }
            return "";
        }

        public static int TransferTable(Type classType, DbHelper sourceDbHelper, DbHelper targetDbHelper, int totalRows)
        {
            try
            {
                _BatchWatch.Reset();
                _BatchWatch.Start();
                UnitOfWork sourceUow = sourceDbHelper.GetNewUnitOfWork();
                UnitOfWork targetUow = targetDbHelper.GetNewUnitOfWork();
                int batchCounter = 0;
                int pageSize = 200;
                int resetDatalayerLimit = 0;
                bool reset = false;
                Type[] args = new Type[] { typeof(Session) };
                var myConstructor = BasicObj.CreateConstructor(classType, args);
                long destVersion = GetVersion(targetUow, classType);
                long currentVersion = destVersion;
                var cursor = new XPCursor(sourceUow, classType, CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", destVersion, BinaryOperatorType.GreaterOrEqual)),
                                                                                         new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending))
                {
                    PageSize = pageSize,
                    SelectDeleted = true
                };
                List<BasicObj> batchForSave = new List<BasicObj>();
                foreach (BasicObj obj in cursor)
                {
                    try
                    {
                        BasicObj destObj = null;
                        BasicObj sourceObj = obj as BasicObj;
                        destObj = targetUow.GetObjectByKey(classType, sourceObj.Oid) as BasicObj;
                        if (destObj == null)
                        {
                            destObj = myConstructor(targetUow) as BasicObj;
                        }
                        long objVersion = sourceObj?.UpdatedOnTicks ?? 0;
                        currentVersion = objVersion > currentVersion ? objVersion : currentVersion;
                        string json = sourceObj.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, eUpdateDirection.MASTER_TO_STORECONTROLLER);
                        string error = string.Empty;
                        destObj.FromJson(json, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                        counter++;
                        batchCounter++;
                        resetDatalayerLimit++;
                        if (!string.IsNullOrEmpty(error))
                        {
                            Program.LogFile.Error(error);
                        }
                        destObj.SkipOnSavingProcess = true;
                        if (counter % pageSize == 0)
                        {
                            DbSave(sourceDbHelper, targetDbHelper, targetUow, batchForSave, myConstructor, classType, currentVersion);
                            batchForSave = new List<BasicObj>();
                            _BatchWatch.Stop();
                            RaiseOnTableUpdate(totalRows, batchCounter, classType.Name);
                            //RaiseOnTableUpdate(batchCounter + " objects of " + classType.Name + " Transferd Elapsed Time " + _BatchWatch.ElapsedMilliseconds + " ms  Total objects Transferd: " + counter + Environment.NewLine);
                            batchCounter = 0;
                            _BatchWatch.Reset();
                            _BatchWatch.Start();

                            if (resetDatalayerLimit > 10000)
                            {
                                Program.LogFile.Info("RESET DATALAYERS Total objects Transferd: " + counter + Environment.NewLine);
                                resetDatalayerLimit = 0;
                                sourceDbHelper.ResetDataLayer();
                                targetDbHelper.ResetDataLayer();
                                GC.Collect();
                                reset = true;
                                sourceUow = sourceDbHelper.GetNewUnitOfWork();
                                targetUow = targetDbHelper.GetNewUnitOfWork();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.LogFile.Error(ex.Message);
                        continue;
                    }
                }
                if (reset)
                {
                    TransferTable(classType, sourceDbHelper, targetDbHelper, totalRows);
                    return counter;
                }
                DbSave(sourceDbHelper, targetDbHelper, targetUow, batchForSave, myConstructor, classType, currentVersion);
                RaiseOnTableUpdate(totalRows, batchCounter, classType.Name);
                //RaiseOnTableUpdate("Total objects of " + classType.Name + " Transferd: " + counter + Environment.NewLine);
                return counter;
            }
            catch (Exception ex)
            {
                Program.LogFile.Error(ex.Message);
                return counter;
            }

            //return counter.ToString();
        }


        private static void DbSave(DbHelper sourceDbhelper, DbHelper targetDbHelper, UnitOfWork destinationUow, List<BasicObj> list, ConstructorDelegate constructor, Type classType, long currentVersion)
        {
            try
            {
                destinationUow.CommitChanges();

            }
            catch (Exception ex)
            {
                Program.LogFile.Error(ex.Message);
                destinationUow.RollbackTransaction();
                currentVersion = SaveBatchListOnebyOne(list, targetDbHelper, constructor, classType);
            }
            SetVersion(destinationUow, classType, currentVersion);

        }

        private static int CountTable(Type classType, UnitOfWork uow)
        {
            int rows = 0;
            try
            {
                object count = uow.Evaluate(classType, CriteriaOperator.Parse("Count()"), CriteriaOperator.And(new OperandProperty("GCRecord").IsNull()));
                int.TryParse(count.ToString(), out rows);
            }
            catch (Exception ex)
            {
                Program.LogFile.Error(ex.Message);
            }
            return rows;
        }

        private static long GetVersion(UnitOfWork uow, Type type)
        {
            TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", type.ToString(), BinaryOperatorType.Equal));
            if (obj != null)
            {
                return obj.Number;
            }
            return 0;
        }


        private static void SetVersion(UnitOfWork uow, Type type, long ver)
        {
            TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", type.FullName, BinaryOperatorType.Equal));
            if (obj == null)
            {
                obj = new TableVersion(uow) { EntityName = type.FullName };
            }
            obj.Number = ver;
            uow.CommitChanges();
        }

        public static async Task<string> TruncateTables(DbHelper dbHelper)
        {
            string message = string.Empty;
            StringBuilder successTables = new StringBuilder();
            StringBuilder failTables = new StringBuilder();
            int successCounter = 0;
            int failCounter = 0;
            try
            {
                IDbConnection connection = dbHelper.GetConnection();
                connection.Open();
                List<XPClassInfo> reverseSortedClasses = Classes.OrderByDescending(g => (g.Attributes.Where(x => x is UpdaterAttribute).First() as UpdaterAttribute).Order).ToList();
                await Task.Run(() =>
                {
                    int counter = 0;
                    foreach (XPClassInfo cl in reverseSortedClasses)
                    {
                        try
                        {
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            command.CommandText = string.Format("TRUNCATE TABLE {0}", "\"" + cl.TableName + "\"");
                            object result = command.ExecuteScalar();
                            successCounter++;
                            successTables.Append(cl.TableName + ",");
                        }
                        catch (Exception ex)
                        {
                            failCounter++;
                            failTables.Append(cl.TableName + ",");
                        }
                        counter++;
                    }
                });
            }
            catch (Exception ex2)
            {
                return ex2.Message;
            }
            if (successCounter > 0)
            {
                message = successCounter + " Tables Truncated : " + successTables.ToString() + Environment.NewLine;
            }
            if (failCounter > 0)
            {
                message = failCounter + " Tables Fail to Truncated : " + successTables.ToString();
            }
            return message;
        }

        public static async Task UpdateSchema(DbHelper targetDbHelper, DbHelper sourceDbHelper)
        {
            await Task.Run(() =>
            {
                targetDbHelper.UpdateDatabase();
            });

            await Task.Run(() =>
            {
                sourceDbHelper.UpdateDatabase();
            });

            UpdateTableVersionInfo(sourceDbHelper, targetDbHelper);
        }

        private static IEnumerable<XPClassInfo> GetClasses(eUpdateDirection direction)
        {
            XPDictionary dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(Customer).Assembly);
            IEnumerable<XPClassInfo> xp = dict.Classes.Cast<XPClassInfo>();
            IEnumerable<XPClassInfo> first = xp.Where(g => g.HasAttribute("UpdaterAttribute")).Where(g => (g.Attributes.Where(x => x is UpdaterAttribute).First() as UpdaterAttribute).Permissions.HasFlag(direction)).
                OrderBy(g => (g.Attributes.Where(x => x is UpdaterAttribute).First() as UpdaterAttribute).Order);
            return first;
        }

        private static void RaiseOnTableUpdate(int totalRows, int rowsTransferd, string currentTable, int remainingRows = 0)
        {
            Program.LogFile.Info("Transfered " + rowsTransferd + " From Table " + currentTable);
            OnDatabaseUpdateEventHandler?.Invoke(totalRows, rowsTransferd, currentTable, remainingRows);
        }



        private static void UpdateTableVersionInfo(DbHelper sourceDbHelper, DbHelper targetDbHelper)
        {
            try
            {

                IDbConnection sourceConnection = sourceDbHelper.GetConnection();
                IDbConnection targetConnection = targetDbHelper.GetConnection();

                if (sourceConnection != null && targetConnection != null)
                {
                    long ver = GetMigrationVersion(sourceConnection);
                    bool tableExists = MigrationVersionExists(targetConnection, targetDbHelper.Username, targetDbHelper.Database);

                    UnitOfWork targetUow = targetDbHelper.GetNewUnitOfWork();
                    if (ver != -1)
                    {
                        if (tableExists)
                        {
                            IDbCommand command = sourceConnection.CreateCommand();
                            command.Connection = sourceConnection;
                            command.CommandText = string.Format("TRUNCATE TABLE {0}", "\"" + "VersionInfo" + "\"");
                            object result = command.ExecuteScalar();
                        }

                        if (targetDbHelper.DBType == DatabaseType.Oracle)
                        {
                            if (!tableExists)
                            {
                                targetUow.ExecuteNonQuery(@"CREATE TABLE ""VersionInfo""(
                                                             ""Version"" NUMBER(19,0),
                                                             ""AppliedOn"" DATE)
                                                             PCTFREE     10
                                                             INITRANS    1
                                                             MAXTRANS    255
                                                             NOCACHE
                                                             MONITORING
                                                             NOPARALLEL
                                                             LOGGING");
                            }
                            targetUow.ExecuteNonQuery(String.Format(@"INSERT INTO ""VersionInfo"" (""Version"",""AppliedOn"") VALUES({0},CURRENT_DATE)", ver));
                        }
                        else if (targetDbHelper.DBType == DatabaseType.SQLServer)
                        {
                            if (!tableExists)
                            {
                                targetUow.ExecuteNonQuery(@"CREATE TABLE VersionInfo(Version bigint,AppliedOn datetime);");
                            }
                            targetUow.ExecuteNonQuery(String.Format(@"INSERT INTO VersionInfo (Version,AppliedOn) VALUES({0},GETDATE()) ;", ver));
                        }
                        else if (targetDbHelper.DBType == DatabaseType.Postgres)
                        {
                            if (!tableExists)
                            {
                                targetUow.ExecuteNonQuery(@"CREATE TABLE ""VersionInfo""(""Version"" bigint,""AppliedOn"" timestamp);");
                            }
                            targetUow.ExecuteNonQuery(String.Format(@"INSERT INTO ""VersionInfo"" (""Version"",""AppliedOn"") VALUES({0},current_timestamp) ;", ver));
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Program.LogFile.Error(ex.Message);
            }


        }



        private static long GetMigrationVersion(IDbConnection connection)
        {
            long schema_version = -1;
            try
            {
                if (connection != null)
                {
                    connection.Open();
                    IDbCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    switch (XpoHelper.databasetype)
                    {
                        case DBType.Oracle:
                            command.CommandText = @"select MAX(""Version"") from ""VersionInfo""";
                            break;
                        case DBType.postgres:
                            command.CommandText = @"select MAX(""Version"") from public.""VersionInfo""";
                            break;
                        case DBType.SQLServer:
                            command.CommandText = @"select MAX(Version) from VersionInfo";
                            break;
                        default:
                            throw new NotSupportedException("GetMigrationVersion");
                    }
                    object count = command.ExecuteScalar();
                    long.TryParse(count.ToString(), out schema_version);
                }
            }
            catch
            {
                schema_version = -1;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return schema_version;
        }



        public static bool MigrationVersionExists(IDbConnection connection, string oracleUserName, string pgDatabase)
        {
            try
            {
                bool migration_table_exists = false;
                switch (XpoHelper.databasetype)
                {
                    case DBType.Oracle:
                        if (connection != null)
                        {
                            connection.Open();
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            command.CommandText = "SELECT count(*) FROM all_tables WHERE owner='" + oracleUserName + "' AND TABLE_NAME='VersionInfo'";
                            object count = command.ExecuteScalar();
                            migration_table_exists = count.ToString().Replace(".0", "") == "1";
                            connection.Close();
                        }
                        break;
                    case DBType.postgres:
                        if (connection != null)
                        {
                            connection.Open();
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            command.CommandText = @"select count(*) as table from pg_tables where schemaname = 'public' and tablename = 'VersionInfo'";
                            object count = command.ExecuteScalar();
                            migration_table_exists = count.ToString().Replace(".0", "") == "1";
                            connection.Close();
                        }
                        break;
                    default:
                        if (connection != null)
                        {
                            connection.Open();
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            command.CommandText = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = '" + pgDatabase + "' and TABLE_NAME = 'VersionInfo' ";
                            object count = command.ExecuteScalar();
                            migration_table_exists = count.ToString().Replace(".0", "") == "1";
                            connection.Close();
                        }
                        break;
                }
                return migration_table_exists;
            }
            catch (NotImplementedException ex)
            {
                string errorMessage = ex.GetFullMessage();
                throw;
                //return false;
            }
            catch (Exception exc)
            {
                string errorMessage = exc.GetFullMessage();
                Program.LogFile.Error(errorMessage);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }


        private static long SaveBatchListOnebyOne(List<BasicObj> list, DbHelper destDbHelper, ConstructorDelegate constructor, Type classType)
        {
            destDbHelper.ResetDataLayer();
            UnitOfWork destinationUow = destDbHelper.GetNewUnitOfWork();
            long destVersion = GetVersion(destinationUow, classType);
            long currentVersion = destVersion;
            foreach (BasicObj obj in list)
            {
                try
                {
                    BasicObj sourceObj = obj as BasicObj;
                    string err = string.Empty;
                    BasicObj destObj = destinationUow.GetObjectByKey(classType, sourceObj.Oid) as BasicObj;
                    if (destObj == null)
                    {
                        destObj = constructor(destinationUow) as BasicObj;
                    }
                    long objVersion = sourceObj?.UpdatedOnTicks ?? 0;
                    currentVersion = objVersion > currentVersion ? objVersion : currentVersion;
                    string json = sourceObj.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, eUpdateDirection.MASTER_TO_STORECONTROLLER);
                    string error = string.Empty;
                    destObj.FromJson(json, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    destinationUow.CommitChanges();
                }
                catch (Exception ex)
                {
                    Program.LogFile.Error(ex.Message);
                    continue;
                }
            }
            return currentVersion;
        }

        public static int CalculateTotalRowsToTransfer(DbHelper sourceDbHelper, DbHelper targetDbHelper, bool fromCurrentVersion)
        {
            int totalRows = 0;
            foreach (XPClassInfo cl in Classes)
            {
                CriteriaOperator crit = null;
                try
                {
                    int tableRows = 0;
                    if (fromCurrentVersion)
                    {
                        long ver = GetVersion(targetDbHelper.GetNewUnitOfWork(), cl.ClassType);
                        crit = CriteriaOperator.And(new OperandProperty("GCRecord").IsNull(), new BinaryOperator("UpdatedOnTicks", ver, BinaryOperatorType.GreaterOrEqual));
                    }
                    else
                    {
                        crit = CriteriaOperator.And(new OperandProperty("GCRecord").IsNull());
                    }
                    object count = sourceDbHelper.GetNewUnitOfWork().Evaluate(cl.ClassType, CriteriaOperator.Parse("Count()"), crit);
                    int.TryParse(count.ToString(), out tableRows);
                    totalRows = totalRows + tableRows;
                }
                catch (Exception ex)
                {
                    Program.LogFile.Error(ex.Message);
                }

            }
            return totalRows;

        }

    }
}
