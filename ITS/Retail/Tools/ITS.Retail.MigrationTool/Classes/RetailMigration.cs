using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Model;

namespace ITS.Retail.MigrationTool.Classes
{
    /// 
    /// Mark all migrations with this INSTEAD of [Migration].
    /// 
    public class RetailMigration : FluentMigrator.MigrationAttribute
    {
        public RetailMigration(int year, int month, int day, int order, string author,String version)
            : base(CalculateValue(year, month, day, order),transactionBehavior: FluentMigrator.TransactionBehavior.None)
        {
            this.Author = author;
            this.MigrationVersion = System.Version.Parse(version);
        }

        public string Author { get; private set; }
        public Version MigrationVersion { get; private set; }

        private static long CalculateValue(int year, int month, int day, int order)
        {
            return BasicObj.CalculateVersion(year, month, day, order);
            //return year * 100000000L + month * 100000L + day * 10000L + order * 100L ;
        }
    }
}
