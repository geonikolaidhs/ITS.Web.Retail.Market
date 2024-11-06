using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2014, month: 2, day: 14, order: 1, version: "2.0.0.48")]
    public class TransferFieldsFromBarcodeToItemBarcode : Migration
    {
        public override void Up()
        {
            if (Schema.Table("ItemBarcode").Exists() == false)
            {
                return;
            }

            if (Schema.Table("ItemBarcode").Column("RelationFactor").Exists() == false)
            {
                Alter.Table("ItemBarcode").AddColumn("RelationFactor").AsDouble().Nullable().WithDefaultValue(0);
            }

            if (Schema.Table("ItemBarcode").Column("Type").Exists() == false)
            {
                Alter.Table("ItemBarcode").AddColumn("Type").AsGuid().Nullable();
            }

            if (Schema.Table("ItemBarcode").Column("MeasurementUnit").Exists() == false)
            {
                Alter.Table("ItemBarcode").AddColumn("MeasurementUnit").AsGuid().Nullable();
            }

            if (Schema.Table("Barcode").Column("RelationFactor").Exists() == true)
            {
                IfDatabase("SqlServer", "Postgres").Execute.Sql(@"update ItemBarcode set RelationFactor = 
                                                    (select RelationFactor from Barcode 
                                                        where Barcode.GCRecord is null
                                                        and Barcode.Oid = ItemBarcode.Barcode
                                                        and Barcode.RelationFactor is not null
                                                    );"
                                                    );
            }
            if (Schema.Table("Barcode").Column("MeasurementUnit").Exists() == true)
            {
                IfDatabase("SqlServer", "Postgres").Execute.Sql(@"update ItemBarcode set MeasurementUnit = 
                                                    (select MeasurementUnit from Barcode 
                                                        where Barcode.GCRecord is null
                                                        and Barcode.Oid = ItemBarcode.Barcode
                                                        and Barcode.MeasurementUnit is not null
                                                    );"
                                                    );
            }
            if (Schema.Table("Barcode").Column("Type").Exists() == true)
            {
                IfDatabase("SqlServer", "Postgres").Execute.Sql(@"update ItemBarcode set Type = 
                                                    (select Type from Barcode 
                                                        where Barcode.GCRecord is null
                                                        and Barcode.Oid = ItemBarcode.Barcode
                                                        and Barcode.Type is not null
                                                    );"
                                                    );

            }

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
