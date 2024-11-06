using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
     [RetailMigration(author: "akm", year: 2015, month: 01, day: 19, order: 1, version: "2.3.0.33")]
    public class Delete_iCodeGCRecordOwner_CategoryNode : Migration
    {
        private Dictionary<string, string> indexes = new Dictionary<string, string>()
        {
            {"iCodeGCRecordOwner_CategoryNode","CategoryNode"},
        };


        public override void Up()
        {
            foreach (KeyValuePair<string, string> pair in this.indexes)
            {
                if (Schema.Table(pair.Value).Index(pair.Key).Exists())
                {
                    Delete.Index(pair.Key).OnTable(pair.Value);
                }
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
