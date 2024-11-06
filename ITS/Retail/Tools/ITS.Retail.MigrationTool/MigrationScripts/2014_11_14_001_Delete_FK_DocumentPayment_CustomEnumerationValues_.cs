using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "Kostopoulos", year: 2014, month: 11, day: 14, order: 1, version:"2.3.0.0")]    
    public class Delete_FK_DocumentPayment_CustomEnumerationValues_ : Migration
    {
        private Dictionary<string, string> foreignKeys1 = new Dictionary<string, string>()
        {
            {"FK_DocumentDetailDiscount_CustomEnumerationValue1","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumerationValue2","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumerationValue3","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumerationValue4","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumerationValue5","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumeration1","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumeration2","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumeration3","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumeration4","DocumentDetailDiscount"},
            {"FK_DocumentDetailDiscount_CustomEnumeration5","DocumentDetailDiscount"}
        };

        private Dictionary<string, string> foreignKeys2 = new Dictionary<string, string>()
        {
            {"FK_DocumentPayment_CustomEnumerationValue1","DocumentPayment"},
            {"FK_DocumentPayment_CustomEnumerationValue2","DocumentPayment"},
            {"FK_DocumentPayment_CustomEnumerationValue3","DocumentPayment"},
            {"FK_DocumentPayment_CustomEnumerationValue4","DocumentPayment"},
            {"FK_DocumentPayment_CustomEnumerationValue5","DocumentPayment"}
        };


        public override void Up()
        {
            foreach (KeyValuePair<string,string> pair in this.foreignKeys1)
            {
                if (Schema.Table(pair.Value).Constraint(pair.Key).Exists())
                {
                    Delete.ForeignKey(pair.Key).OnTable(pair.Value);
                }
            }
            foreach (KeyValuePair<string, string> pair in this.foreignKeys2)
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
