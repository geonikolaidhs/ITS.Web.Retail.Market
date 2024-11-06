using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
//using Oracle.DataAccess.Client;
using System.Data.OracleClient;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ConnectionHelper
    {
        public static string GetConnectionStringForIDbConnection()
        {
            string connstring = GetConnectionString();
            String[] toks = connstring.Split(';');
            return toks.Where(g => g.Contains("XpoProvider") == false).Aggregate((f, s) => f + ";" + s);
        }
        public static string GetConnectionString()
        {
            string connection_string;
            switch (XpoHelper.databasetype)
            {
                case DBType.postgres:
                    connection_string = CustomPostgreSqlConnectionProvider.GetConnectionString(XpoHelper.sqlserver, XpoHelper.username, XpoHelper.pass, XpoHelper.database);
                    break;
                case DBType.Oracle:
                    //connection_string = OracleConnectionProvider.GetConnectionString(XpoHelper.sqlserver, XpoHelper.username, XpoHelper.pass);
                    connection_string = string.Format(@"User Id={2};Password={3};Data Source=" + @" (DESCRIPTION = (ADDRESS = (PROTOCOL = tcp)(HOST = {0})(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME = {1})))", XpoHelper.sqlserver, XpoHelper.database, XpoHelper.username, XpoHelper.pass);
                    connection_string = "XpoProvider=Oracle;" + connection_string;
                    break;
                default:
                    connection_string = MSSqlConnectionProvider.GetConnectionString(XpoHelper.sqlserver, XpoHelper.username, XpoHelper.pass, XpoHelper.database);
                    break;
            }
            return connection_string;
        }
        public static IDbConnection GetConnection()
        {
            string connection_string = GetConnectionStringForIDbConnection();
            switch (XpoHelper.databasetype)
            {
                case DBType.postgres:
                    return CustomPostgreSqlConnectionProvider.CreateConnection(connection_string);
                case DBType.Oracle:
                    return new OracleConnection() { ConnectionString = connection_string }; ;
                default:
                    return new SqlConnection(connection_string);
            }
        }

    }
}
