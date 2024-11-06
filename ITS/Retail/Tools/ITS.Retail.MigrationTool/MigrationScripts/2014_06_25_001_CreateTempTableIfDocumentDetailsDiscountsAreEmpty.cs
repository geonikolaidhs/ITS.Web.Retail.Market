using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "sak", year: 2014, month: 6, day: 25, order: 1, version: "2.0.0.65")]
    public class CreateTempTableIfDocumentDetailsDiscountsAreEmpty : Migration
    {

        public override void Up()
        {
            if (Schema.Table("DocumentDetailDiscount").Exists() &&
                Schema.Table("Temp_DocumentDetailDiscountsAreEmpty").Exists() == false)
            {
                Execute.Sql(@"IF (select count(*) from DocumentDetailDiscount) = 0 
                        BEGIN
                        CREATE TABLE Temp_DocumentDetailDiscountsAreEmpty (Oid uniqueidentifier)
                        END");
            }

        }


        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
