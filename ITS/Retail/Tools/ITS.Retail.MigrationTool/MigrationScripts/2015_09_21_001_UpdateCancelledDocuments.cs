using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "nst", year: 2015, month: 09, day: 21, order: 1, version: "2.3.2.5")]
    public class UpdateCancelledDocuments : Migration
    {
        public override void Up()
        {
            string query;

            List<string> checkDocumentHeaderTableColumnsExistance = new List<string>()
            {              
                {"DocumentDiscountAmount"},
                {"ConsumedPointsForDiscount"},
                {"PointsDiscountAmount"},
                {"PromotionsDiscountAmount"},
                {"PromotionPoints"},
                {"TotalDiscountAmount"},
                {"TotalVatAmount"},
                {"NetTotal"},
                {"GrossTotal"},
                {"TotalVatAmountBeforeDiscount"},
                {"GrossTotalBeforeDiscount"},
                {"GrossTotalBeforeDocumentDiscount"},
                {"NetTotalBeforeDiscount"},
                {"TotalQty"},
                {"TotalPoints"},
                {"DocumentPoints"}
            };

            query = "UPDATE DocumentHeader SET ";
            foreach (String column in checkDocumentHeaderTableColumnsExistance)
            {
                if (Schema.Table("DocumentHeader").Column(column).Exists())
                {
                    query += column + " = " + column + " * (-1),";
                }
            }
            query = query.ReplaceAt(query.Length -1, ' ');
            query += " WHERE CancelsDocument IS NOT NULL AND GCRecord IS NULL";
            Execute.Sql(query);

            List<string> checkDocumentDetailTableColumnsExistance = new List<string>()
            { 
                {"Qty"},
                {"Points"},
                {"FirstDiscount"},
                {"SecondDiscount"},
                {"TotalVatAmountBeforeDiscount"},
                {"TotalDiscount"},
                {"NetTotalBeforeDiscount"},
                {"NetTotal"},
                {"GrossTotal"},
                {"TotalVatAmount"},
                {"GrossTotalBeforeDiscount"},
                {"GrossTotalBeforeDocumentDiscount"},
                {"PackingQuantity"}
            };

            query = "UPDATE DocumentDetail SET ";
            foreach (String column in checkDocumentDetailTableColumnsExistance)
            {
                if (Schema.Table("DocumentDetail").Column(column).Exists())
                {
                    query += column + " = " + column + " * (-1),";
                }
            }
            query = query.ReplaceAt(query.Length - 1, ' ');
            query += " WHERE DocumentHeader IN(SELECT Oid FROM DocumentHeader WHERE CancelsDocument IS NOT NULL AND GCRecord IS NULL)";
            Execute.Sql(query);

            query = "UPDATE DocumentDetailDiscount SET Value = Value * (-1) WHERE DocumentDetail IN(SELECT Oid FROM DocumentDetail WHERE DocumentHeader IN (SELECT Oid FROM DocumentHeader WHERE CancelsDocument IS NOT NULL AND GCRecord IS NULL))";
            Execute.Sql(query);

            query = "UPDATE DocumentPayment SET Amount = Amount * (-1) WHERE DocumentHeader IN(SELECT Oid FROM DocumentHeader WHERE CancelsDocument IS NOT NULL AND GCRecord IS NULL)";
            Execute.Sql(query);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
