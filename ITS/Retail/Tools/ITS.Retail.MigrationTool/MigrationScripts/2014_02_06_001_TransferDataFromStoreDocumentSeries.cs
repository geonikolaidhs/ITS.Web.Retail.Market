using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    /// <summary>
    /// Injected migration to fix the previous migratons drop...
    /// </summary>

    [RetailMigration(author: "sak", year: 2014, month: 2, day: 6, order: 1, version: "2.0.0.42")]
    class TransferDataFromStoreDocumentSeries : Migration
    {
        public override void Up()
        {
            if (Schema.Table("StoreDocumentSeriesType").Column("StoreSeries").Exists() &&
                Schema.Table("StoreDocumentSeriesType").Column("DocumentSeries").Exists() == false &&
                Schema.Table("DocumentSeries").Column("Store").Exists() == false)
            {
                Alter.Table("StoreDocumentSeriesType").AddColumn("DocumentSeries").AsGuid().Nullable();
                Alter.Table("DocumentSeries").AddColumn("Store").AsGuid().Nullable();
                
                Execute.Sql(@"update DocumentSeries set Store = 
                            (select StoreDocumentSeries.Store from StoreDocumentSeries where StoreDocumentSeries.Series = DocumentSeries.Oid)");

                Execute.Sql(@"update StoreDocumentSeriesType set DocumentSeries = 
                            (select StoreDocumentSeries.Series from StoreDocumentSeries where StoreDocumentSeriesType.StoreSeries = StoreDocumentSeries.Oid)");

            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
