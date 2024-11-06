using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2014, month: 2, day: 7, order: 1, version: "2.0.0.42")]
    public class TransferHasAutomatingNumberingToDocumentSeries : Migration
    {
        public override void Up()
        {
            if (Schema.Table("DocumentSeries").Exists() == true)
            {
                if (Schema.Table("DocumentSeries").Column("HasAutomaticNumbering").Exists() == false)
                {
                    Alter.Table("DocumentSeries").AddColumn("HasAutomaticNumbering").AsBoolean().Nullable().WithDefaultValue(false);
                }

                if (Schema.Table("StoreDocumentSeriesType").Column("HasAutomaticNumbering").Exists() == true)
                {

                    IfDatabase("SqlServer", "Postgres").Execute.Sql(@"update DocumentSeries set HasAutomaticNumbering = 
                                                    (select max(convert(int,HasAutomaticNumbering)) from StoreDocumentSeriesType 
                                                        where StoreDocumentSeriesType.GCRecord is null
                                                        and StoreDocumentSeriesType.DocumentSeries = DocumentSeries.Oid
                                                        and StoreDocumentSeriesType.HasAutomaticNumbering is not null
                                                    );"
                                                        );
                }
                Update.Table("DocumentSeries").Set(new { HasAutomaticNumbering = false }).Where(new { HasAutomaticNumbering = (bool?)null });
            }

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
