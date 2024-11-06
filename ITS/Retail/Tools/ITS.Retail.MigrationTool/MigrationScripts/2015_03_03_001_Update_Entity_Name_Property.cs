using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "nst", year: 2015, month: 03, day: 03, order: 1, version: "2.3.0.45")]
    public class Update_Entity_Name_Property : Migration
    {
        public override void Up()
        {
            if (this.Schema.Table("DataFileRecordHeader").Column("EntiyName").Exists())
            {
                if(this.Schema.Table("DataFileRecordHeader").Column("EntityName").Exists())
                {
                    Delete.Column("EntityName").FromTable("DataFileRecordHeader");
                }
                Rename.Column("EntiyName").OnTable("DataFileRecordHeader").To("EntityName");
            }        
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}