using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
     [RetailMigration(author: "sak", year: 2015, month: 02, day: 03, order: 1, version: "2.3.0.37")]
    public class Delete_FK_Promotion_PromotionApplicationRuleGroup : Migration
    {
        private Dictionary<string, string> foreignKeys1 = new Dictionary<string, string>()
        {
            {"FK_Promotion_PromotionApplicationRuleGroup","Promotion"},
        };


        public override void Up()
        {
            foreach (KeyValuePair<string, string> pair in this.foreignKeys1)
            {
                if (Schema.Table(pair.Value).Constraint(pair.Key).Exists())
                {
                    Delete.ForeignKey(pair.Key).OnTable(pair.Value);
                }
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
