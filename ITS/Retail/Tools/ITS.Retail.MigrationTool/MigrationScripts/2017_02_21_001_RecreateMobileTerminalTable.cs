using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2017, month: 2, day: 21, order: 1, version: "2.3.6.10")]
    public class RecreateMobileTerminalTable : Migration
    {
        public override void Up()
        {
            string tableName = "MobileTerminal";
            if ( Schema.Table(tableName).Exists() )
            {
                Delete.Table(tableName);
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
