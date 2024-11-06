using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo.DB;

namespace ITS.Licensing.Web.AuxilliaryClasses
{
    public struct Connection
    {
        public string sqlserver, username, pass, database, sqlprovider;

        /*
        public Connection(string _sqlserver, string _sqlprovider, string _username, string _pass, string _database)
        {
            sqlserver = _sqlserver;
            sqlprovider = _sqlprovider;
            username = _username;
            pass = _pass;
            database = _database;
        }
         */

        public string ConnectionString()
        {
            string connection_string = "";
            switch(sqlprovider){
                case "sqlite":
                    connection_string = SQLiteConnectionProvider.GetConnectionString(database);
                    break;
                case "MySQL" :
                    connection_string = MySqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
                    break;
                case "MSSQL" :                    
                default:
                    connection_string = MSSqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
                    break;

            }
            return connection_string;
        }
    }
}
