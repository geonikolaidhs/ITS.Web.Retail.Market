using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using ITS.MobileAtStore.Common.DatabaseAbstraction.AuxilliaryClasses;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction
{
    public class MysqlDataLayer : AbstractDataLayer
    {
        public override DbConnection CreateConnection()
        {
            String connectionString = String.Format("server={0};user id={1}; password={2}; database={3};",
                Settings.ReadConnectionSettings.Server, Settings.ReadConnectionSettings.Username, Settings.ReadConnectionSettings.Password, Settings.ReadConnectionSettings.DatabaseName);
            return new MySqlConnection(connectionString);
        }

        public override void Initialize()
        {

        }

        public override string ProductQuery
        {
            get
            {
                return "(select * from {0} WHERE  BARCODE = '{1}' {2} {3} limit 1) union all ( select * from {0} where  CODE = '{1}' {2} {3} limit 1)";
            }
        }


        public override string ReceiptProductQuery
        {
            get
            {
                return "(select * from {0} WHERE SUPPLIERCODE  = '{1}' {2} {3} and receiptSupplierid = {4} limit 1 ) union all ( select * from {0} where  CODE = '{1}' {2} {3} limit 1) union all ( select * from {0} where  BARCODE = '{1}' {2} {3} limit 1)";
            }
        }

        public override string GetCode(string code)
        {
            return code.TrimStart('0');
        }
    }
}
