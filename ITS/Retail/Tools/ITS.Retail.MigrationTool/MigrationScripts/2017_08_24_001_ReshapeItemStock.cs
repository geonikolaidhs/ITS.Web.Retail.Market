using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;


namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2017, month: 8, day: 24, order: 1, version: "2.3.7.3")]
    public class ReshapeItemStock : Migration
    {
        public override void Up()
        {
            string itemStockTableName = "ItemStock";
            if (Schema.Table(itemStockTableName).Exists())
            {
                Delete.Table(itemStockTableName);
            }
        }


        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
