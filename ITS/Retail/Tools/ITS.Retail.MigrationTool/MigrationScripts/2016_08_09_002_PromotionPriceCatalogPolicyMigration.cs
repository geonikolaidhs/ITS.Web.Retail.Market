using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2016, month: 8, day: 9, order: 2, version: "2.3.5.0")]
    public class PromotionPriceCatalogPolicyMigration : Migration
    {
        public override void Up()
        {
            /*Based on the previous migration (PriceCatalogPolicyMigration) for each PriceCatalog a PriceCatalogPolicyHas been created with the same Description 
              and Code. In this script we connect each Promotion with the relative PriceCatalogs
            */
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                #region setup            
                long nowTicks = DateTime.Now.Ticks;
                string postgresGetNewGuidPart1 = @"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";";
                string postgresGetNewGuidPart2 = "uuid_generate_v4()";
                string sqlServerGetNewGuidPart1 = "";
                string sqlServerGetNewGuidPart2 = "newid()";
                #endregion

                #region Create PriceCatalogPolicyPromotion for each Promotion
                if (Schema.Table("PriceCatalogPolicyPromotion").Exists() == false)
                {
                    Create.Table("PriceCatalogPolicyPromotion")
                        .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                        .WithColumn("Promotion").AsGuid().Nullable()
                        .WithColumn("PriceCatalogPolicy").AsGuid().Nullable()
                        .WithColumn("CreatedOnTicks").AsCustom("bigint")
                        .WithColumn("UpdatedOnTicks").AsCustom("bigint");
                }
                string createPriceCatalogPolicyPromotions = @"{0}
                                                          INSERT INTO ""PriceCatalogPolicyPromotion"" 
                                                                      (""Oid"", ""Promotion"", ""PriceCatalogPolicy"", ""CreatedOnTicks"",""UpdatedOnTicks"")
                                                          SELECT {1} , ""PriceCatalogPromotion"".""Promotion"" , ""PriceCatalogPolicy"".""Oid"" , " + nowTicks + "," + nowTicks +
                                                           @" FROM ""PriceCatalogPromotion""
                                                          JOIN ""PriceCatalog"" ON ""PriceCatalog"".""Oid"" = ""PriceCatalogPromotion"".""PriceCatalog""
                                                          JOIN ""PriceCatalogPolicy"" ON ""PriceCatalogPolicy"".""Code"" = ""PriceCatalog"".""Code""
                                                          WHERE ""PriceCatalogPromotion"".""GCRecord"" IS NULL;";
                IfDatabase("Postgres").Execute.Sql(string.Format(createPriceCatalogPolicyPromotions, postgresGetNewGuidPart1, postgresGetNewGuidPart2));
                IfDatabase("SqlServer").Execute.Sql(string.Format(createPriceCatalogPolicyPromotions, sqlServerGetNewGuidPart1, sqlServerGetNewGuidPart2));
                #endregion
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
