using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2017, month: 1, day: 9, order: 1, version: "2.3.6.0")]
    public class DecompositionDocumentTypeSettings : Migration
    {
        public override void Up()
        {
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                if (Schema.Table("DocumentType").Column("LinkedLineValueFactor").Exists() == false)
                {
                    Create.Column("LinkedLineValueFactor").OnTable("DocumentType").AsInt32().WithDefaultValue(0);
                    string initialiseLinkedLineValueFactor = @"UPDATE ""DocumentType"" SET ""LinkedLineValueFactor"" = ""ValueFactor"" ";
                    Execute.Sql(initialiseLinkedLineValueFactor);
                }
                if (Schema.Table("DocumentType").Column("LinkedLineQuantityFactor").Exists() == false)
                {
                    Create.Column("LinkedLineQuantityFactor").OnTable("DocumentType").AsInt32().WithDefaultValue(0);
                    string initialiseLinkedLineQuantityFactor = @"UPDATE ""DocumentType"" SET ""LinkedLineQuantityFactor"" = ""QuantityFactor"" ";
                    Execute.Sql(initialiseLinkedLineQuantityFactor);
                }
                if (Schema.Table("DocumentType").Column("ManualLinkedLineInsertion").Exists() == false)
                {
                    string postgresTruthyValue = "true";
                    string sqlServerTruthyValue = "1";
                    Create.Column("ManualLinkedLineInsertion").OnTable("DocumentType").AsBoolean().Nullable();
                    string initialiseManualLinkedLine = 
                        @"UPDATE ""DocumentType"" SET ""ManualLinkedLineInsertion"" = '{0}' where ""Division"" not in ( select ""Oid"" from  ""Division"" Where ""Section"" = 0 )";
                    IfDatabase("Postgres").Execute.Sql(string.Format(initialiseManualLinkedLine, postgresTruthyValue));
                    IfDatabase("SqlServer").Execute.Sql(string.Format(initialiseManualLinkedLine, sqlServerTruthyValue));

                }
            }
        }
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
