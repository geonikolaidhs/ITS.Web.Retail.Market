using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "akm", year: 2016, month: 01, day: 25, order: 1, version: "2.3.2.17")]
    public class UpdateStoreControllerTerminalDeviceAssociation : Migration
    {
        public override void Up()
        {
            if (Schema.Table("StoreControllerTerminalDeviceAssociation").Column("Owner").Exists() == false)
            {
                Create.Column("Owner").OnTable("StoreControllerTerminalDeviceAssociation").AsGuid().Nullable();
            }
            IfDatabase("SQLServer").Execute.Sql(@"Update StoreControllerTerminalDeviceAssociation set Owner = StoreControllerSettings.Owner 
                from StoreControllerTerminalDeviceAssociation inner join StoreControllerSettings on StoreControllerSettings.Oid = StoreControllerTerminalDeviceAssociation.StoreControllerSettings ");

            IfDatabase("postgres").Execute.Sql("Update \"StoreControllerTerminalDeviceAssociation\" set \"Owner\" = \"StoreControllerSettings\".\"Owner\"  from \"StoreControllerTerminalDeviceAssociation\" a inner join \"StoreControllerSettings\" on \"StoreControllerSettings\".\"Oid\" = a.\"StoreControllerSettings\" WHERE \"StoreControllerTerminalDeviceAssociation\".\"Oid\" = a.\"Oid\" ");
        }


        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
