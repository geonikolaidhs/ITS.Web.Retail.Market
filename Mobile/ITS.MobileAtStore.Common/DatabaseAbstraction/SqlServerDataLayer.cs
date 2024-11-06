using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction
{
    public class SqlServerDataLayer : AbstractDataLayer
    {
        public override DbConnection CreateConnection()
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder.DataSource = Settings.ReadConnectionSettings.Server;
            sqlConnectionStringBuilder.InitialCatalog = Settings.ReadConnectionSettings.DatabaseName;
            sqlConnectionStringBuilder.UserID = Settings.ReadConnectionSettings.Username;
            sqlConnectionStringBuilder.Password = Settings.ReadConnectionSettings.Password;
            return new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
        }

        public override void Initialize()
        {

        }

        public override string ReceiptProductQuery
        {
            get
            {
                return @"(select top 1 * from {0} WHERE SUPPLIER  = '{1}' {2} {3} ) 
                        union all ( select top 1 * from {0} where  CODE = '{1}' {2} {3} ) 
                        union all ( select top 1 * from {0} where  BARCODE = '{1}' {2} {3} )";
            }
        }
    }
}
