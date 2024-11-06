using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;
using System.Data;
using ITS.Retail.Common;
using System.Data.SqlClient;
using Npgsql;
using System.Data.OracleClient;


namespace ITS.Retail.WebClient.Helpers
{
    public static class VersionHelper
    {
        public static bool MigrationVersionExists()
        {
            IDbConnection connection = null;
            try
            {
                bool migration_table_exists = false;
                string connectionString = ConnectionHelper.GetConnectionStringForIDbConnection();
                switch (XpoHelper.databasetype)
                {
                    case DBType.Oracle:
                        connection = new OracleConnection(connectionString);
                        if (connection != null)
                        {
                            connection.Open();
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            command.CommandText = "SELECT count(*) FROM all_tables WHERE owner='" + XpoHelper.username + "' AND TABLE_NAME='VersionInfo'";
                            object count = command.ExecuteScalar();
                            migration_table_exists = count.ToString().Replace(".0", "") == "1";
                            connection.Close();
                        }
                        break;
                    case DBType.postgres:
                        connection = new NpgsqlConnection(connectionString);
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
                        connection = new SqlConnection(connectionString);
                        if (connection != null)
                        {
                            connection.Open();
                            IDbCommand command = connection.CreateCommand();
                            command.Connection = connection;
                            //Εδώ έχουμε ενα θέμα με το case sensitive. 
                            command.CommandText = @"SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = '" + XpoHelper.database + "' and TABLE_NAME = 'VersionInfo' ";
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
                //throw;
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

        public static long GetMigrationVersion()
        {
            long schema_version = -1;//Αρνητικές τιμές σημαίνει ότι δεν υπάρχει τέτοια έκδοση
            IDbConnection connection = ConnectionHelper.GetConnection();

            if (MigrationVersionExists() == false)
            {
                throw new Exception("No migration has been executed.Please Initialise database, or contact system administrator.");
            }

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
                try
                {
                    object count = command.ExecuteScalar();
                    long.TryParse(count.ToString(), out schema_version);
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
            }

            return schema_version;
        }

        public static void FixVersionInfoTableName()
        {
            if (XpoHelper.databasetype == DBType.SQLServer || XpoHelper.databasetype == DBType.Oracle)
            {
                return;
            }

            IDbConnection connection = ConnectionHelper.GetConnection();
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.Connection = connection;
            switch (XpoHelper.databasetype)
            {
                case DBType.postgres:
                    command.CommandText = @"select count(*) as table from pg_tables where schemaname = 'public' and tablename = 'VersionInfo'";
                    object versionInfoCounterObject = command.ExecuteScalar();
                    bool VersionInfoTableExists = false;
                    try
                    {
                        VersionInfoTableExists = (long)versionInfoCounterObject > 0;
                    }
                    catch
                    {
                        VersionInfoTableExists = false;
                    }

                    command.CommandText = @"select count(*) as table from pg_tables where schemaname = 'public' and tablename = 'versioninfo'";
                    object versioninfoCounterObject = command.ExecuteScalar();
                    bool versioninfoTableExists = false;
                    try
                    {
                        versioninfoTableExists = (long)versioninfoCounterObject > 0;
                    }
                    catch
                    {
                        versioninfoTableExists = false;
                    }

                    if (versioninfoTableExists && !VersionInfoTableExists)
                    {
                        command.CommandText = @"ALTER TABLE ""versioninfo"" RENAME TO ""VersionInfo"";
                                                ALTER TABLE ""VersionInfo"" RENAME COLUMN ""version"" TO ""Version"";
                                                ALTER TABLE ""VersionInfo"" RENAME COLUMN ""appliedon"" TO ""AppliedOn""; ";
                        command.ExecuteScalar();
                    }
                    break;
                case DBType.SQLServer:
                    //Should be unreachable code
                    break;
                default:
                    throw new NotSupportedException("GetMigrationVersion");
            }
            connection.Close();
        }
    }
}
