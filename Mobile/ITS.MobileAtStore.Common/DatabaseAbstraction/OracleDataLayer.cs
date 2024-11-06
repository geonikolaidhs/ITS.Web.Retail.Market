using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Data.Common;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction
{
    public class OracleDataLayer : AbstractDataLayer
    {
        public override DbConnection CreateConnection()
        {
            String connectionString = string.Format("Data Source = {0}; User Id = {1}; Password = {2}; Pooling = false",
                Settings.ReadConnectionSettings.Server, Settings.ReadConnectionSettings.Username,
                Settings.ReadConnectionSettings.Password);
            return new OracleConnection(connectionString);
        }

        public override void Initialize()
        {

        }

    }
}
