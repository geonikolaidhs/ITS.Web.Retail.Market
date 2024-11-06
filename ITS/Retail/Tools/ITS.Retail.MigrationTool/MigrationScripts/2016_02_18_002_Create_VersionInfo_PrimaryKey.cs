using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "nst", year: 2016, month: 02, day: 18, order: 2, version: "2.3.2.18")]
    public class Create_VersionInfo_PrimaryKey : Migration
    {
        public override void Up()
        {
            if (this.Schema.Table("VersionInfo").Column("Version").Exists())
            {
                Alter.Table("VersionInfo").AlterColumn("Version").AsInt64().NotNullable();
                Create.PrimaryKey("PK_VersionInfo").OnTable("VersionInfo").Column("Version");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}