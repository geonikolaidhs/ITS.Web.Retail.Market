using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2016, month: 8, day: 4, order: 1, version: "2.3.5.0")]
    public class PriceCatalogPolicyMigration : Migration
    {
        public override void Up()
        {
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                #region setup            
                long nowTicks = DateTime.Now.Ticks;
                string postgresGetNewGuidPart1 = @"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";";
                string postgresGetNewGuidPart2 = "uuid_generate_v4()";
                string sqlServerGetNewGuidPart1 = "";
                string sqlServerGetNewGuidPart2 = "newid()";
                string postgresTruthyValue = "true";
                string sqlServerTruthyValue = "1";
                #endregion

                #region For each pricecatalog create a PriceCatalogPolicy with one PriceCatalogPolicyDetail
                if (Schema.Table("PriceCatalogPolicy").Exists() == false)
                {
                    Create.Table("PriceCatalogPolicy")
                        .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                        .WithColumn("Description").AsString(100).Nullable()
                        .WithColumn("Code").AsString(100).Nullable()
                        .WithColumn("Owner").AsGuid().Nullable()
                        .WithColumn("IsActive").AsBoolean()
                        .WithColumn("CreatedOnTicks").AsCustom("bigint")
                        .WithColumn("UpdatedOnTicks").AsCustom("bigint");
                }

                string createPriceCatalogPolicyFromPriceCatalogQuery =
                            @"{0} INSERT INTO ""PriceCatalogPolicy"" (""Oid"", ""Description"", ""Code"" ,""Owner"",""IsActive"",""CreatedOnTicks"",""UpdatedOnTicks"" )
                                 SELECT {1},""Description"", ""Code"", ""Owner"", ""IsActive"", " + nowTicks + "," + nowTicks +
                                  @" FROM ""PriceCatalog""
                                 WHERE ""GCRecord"" is null";
                IfDatabase("Postgres").Execute.Sql(string.Format(createPriceCatalogPolicyFromPriceCatalogQuery, postgresGetNewGuidPart1, postgresGetNewGuidPart2));
                IfDatabase("SqlServer").Execute.Sql(string.Format(createPriceCatalogPolicyFromPriceCatalogQuery, sqlServerGetNewGuidPart1, sqlServerGetNewGuidPart2));

                if (Schema.Table("PriceCatalogPolicyDetail").Exists() == false)
                {
                    Create.Table("PriceCatalogPolicyDetail")
                        .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                        .WithColumn("PriceCatalog").AsGuid().Nullable()
                        .WithColumn("PriceCatalogPolicy").AsGuid().Nullable()
                        .WithColumn("Sort").AsInt32()
                        .WithColumn("PriceCatalogSearchMethod").AsInt32().WithDefaultValue(0)
                        .WithColumn("IsDefault").AsBoolean()
                        .WithColumn("IsActive").AsBoolean()
                        .WithColumn("CreatedOnTicks").AsCustom("bigint")
                        .WithColumn("UpdatedOnTicks").AsCustom("bigint");
                }
                string createPriceCatalogPolicyDetailFromPriceCatalogQuery =
                            @"{0} INSERT INTO ""PriceCatalogPolicyDetail"" (""Oid"", ""PriceCatalog"", ""PriceCatalogPolicy"" ,""Sort""
                                    , ""PriceCatalogSearchMethod"",""IsDefault"",""IsActive"",""CreatedOnTicks"",""UpdatedOnTicks"" )
                                 SELECT {1},""PriceCatalog"".""Oid""
                                        , (SELECT ""Oid"" FROM ""PriceCatalogPolicy"" WHERE ""PriceCatalogPolicy"".""Code"" = ""PriceCatalog"".""Code"" )
                                        , 1 , 1, {2} ,  ""IsActive"", " + nowTicks + "," + nowTicks +
                                  @" FROM ""PriceCatalog""
                                 WHERE ""GCRecord"" is null";
                IfDatabase("Postgres").Execute.Sql(string.Format(createPriceCatalogPolicyDetailFromPriceCatalogQuery,
                                                                    postgresGetNewGuidPart1,
                                                                    postgresGetNewGuidPart2,
                                                                    postgresTruthyValue));
                IfDatabase("SqlServer").Execute.Sql(string.Format(createPriceCatalogPolicyDetailFromPriceCatalogQuery,
                                                                    sqlServerGetNewGuidPart1,
                                                                    sqlServerGetNewGuidPart2,
                                                                    sqlServerTruthyValue));
                #endregion

                #region Fix Store table and connect Store with Policy
                if (Schema.Table("Store").Column("DefaultPriceCatalogPolicy").Exists() == false)
                {
                    Alter.Table("Store").AddColumn("DefaultPriceCatalogPolicy").AsGuid().Nullable();
                }

                Execute.Sql(@"UPDATE ""Store""
                          SET ""DefaultPriceCatalogPolicy"" = (SELECT ""Oid"" from ""PriceCatalogPolicy""
                                                               WHERE ""Code"" = (SELECT ""Code"" FROM ""PriceCatalog"" WHERE ""Oid"" = ""Store"".""DefaultPriceCatalog"")
                                                              ), ""UpdatedOnTicks"" = " + nowTicks.ToString() + @"
                          WHERE ""GCRecord"" is null"
                           );

                if (Schema.Table("StorePriceCatalogPolicy").Exists() == false)
                {
                    Create.Table("StorePriceCatalogPolicy")
                        .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                        .WithColumn("Store").AsGuid().Nullable()
                        .WithColumn("PriceCatalogPolicy").AsGuid().Nullable()
                        .WithColumn("IsDefault").AsBoolean();
                }
                string connectStoreWithPolicyQuery = @"{0} INSERT INTO ""StorePriceCatalogPolicy"" (""Oid"",""Store"",""PriceCatalogPolicy"",""IsDefault"")
                                                   SELECT {1}, ""Oid"",""DefaultPriceCatalogPolicy"", {2}
                                                   FROM ""Store""
                                                   WHERE ""GCRecord"" is null";
                IfDatabase("Postgres").Execute.Sql(string.Format(connectStoreWithPolicyQuery, postgresGetNewGuidPart1, postgresGetNewGuidPart2, postgresTruthyValue));
                IfDatabase("SqlServer").Execute.Sql(string.Format(connectStoreWithPolicyQuery, sqlServerGetNewGuidPart1, sqlServerGetNewGuidPart2, sqlServerTruthyValue));
                #endregion

                #region Fix Customer table and connectCustomer with Policy
                if (Schema.Table("Customer").Column("PriceCatalogPolicy").Exists() == false)
                {
                    Alter.Table("Customer").AddColumn("PriceCatalogPolicy").AsGuid().Nullable();
                }

                string copyCustomerStorePriceListToPolicy = @"WITH qr as 
                                                                    ( SELECT ""PriceCatalogPolicy"".""Oid"",""StorePriceList"".""PriceList"", ""CustomerStorePriceList"".""Customer""
                                                                     FROM ""PriceCatalogPolicy""
                                                                     JOIN ""PriceCatalog"" ON ""PriceCatalog"".""Code"" = ""PriceCatalogPolicy"".""Code"" 
                                                                                           AND ""PriceCatalog"".""GCRecord"" IS NULL
                                                                     JOIN ""StorePriceList"" ON ""StorePriceList"".""PriceList"" = ""PriceCatalog"".""Oid""
                                                                                             AND ""StorePriceList"".""GCRecord"" IS NULL
                                                                     JOIN ""CustomerStorePriceList"" ON ""CustomerStorePriceList"".""StorePriceList"" = ""StorePriceList"".""Oid""
                                                                                                     AND ""CustomerStorePriceList"".""GCRecord"" IS NULL
                                                                    )
                                                         UPDATE ""Customer""
                                                         SET ""PriceCatalogPolicy"" = qr.""Oid"", ""UpdatedOnTicks"" = " + nowTicks.ToString() + @" 
                                                         FROM qr
                                                         WHERE (SELECT COUNT(DISTINCT ""StorePriceList"")
                                                                FROM ""CustomerStorePriceList""
                                                                WHERE ""CustomerStorePriceList"".""Customer"" = ""Customer"".""Oid""
                                                                AND ""GCRecord"" IS NULL
                                                               )
                                                         > 1";
                Execute.Sql(copyCustomerStorePriceListToPolicy);
                #endregion
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
