using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "sak", year: 2015, month: 6, day: 4, order: 1, version: "2.3.0.63")]
    public class DeleteTempTableDocumentDetailsDiscountsAreEmpty : Migration
    {

        public override void Up()
        {
            if (Schema.Table("Temp_DocumentDetailDiscountsAreEmpty").Exists())
            {
                Delete.Table("Temp_DocumentDetailDiscountsAreEmpty");
            }

        }


        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
