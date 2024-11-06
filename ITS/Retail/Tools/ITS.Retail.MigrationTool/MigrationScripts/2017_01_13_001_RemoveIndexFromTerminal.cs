using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "akm", year: 2017, month: 1, day: 13, order: 1, version: "2.3.6.1")]
    public class RemoveIndexFromTerminal : Migration
    {
        public override void Up()
        {
            if (Schema.Table("Terminal").Index("iIDStoreGCRecord_Terminal").Exists() == true)
            {
                Delete.Index("iIDStoreGCRecord_Terminal").OnTable("Terminal");
            }
        }
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
