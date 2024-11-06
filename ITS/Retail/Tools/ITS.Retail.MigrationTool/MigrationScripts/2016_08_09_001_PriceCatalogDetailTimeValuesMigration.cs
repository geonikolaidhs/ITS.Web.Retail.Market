using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "akm", year: 2016, month: 8, day: 9, order: 1, version: "2.3.5.0")]
    public class PriceCatalogDetailTimeValuesMigration : Migration
    {
        public override void Up()
        {
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                if (Schema.Table("PriceCatalogDetailTimeValue").Exists() == false)
                {
                    Create.Table("PriceCatalogDetailTimeValue")
                        .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                        .WithColumn("PriceCatalogDetail").AsGuid().Nullable()
                        .WithColumn("TimeValue").AsDecimal().Nullable()
                        .WithColumn("OldTimeValue").AsDecimal().Nullable()
                        .WithColumn("TimeValueValidUntil").AsCustom("bigint").Nullable()
                        .WithColumn("TimeValueValidFrom").AsCustom("bigint").Nullable()
                        .WithColumn("TimeValueChangedOn").AsCustom("bigint").Nullable()
                        .WithColumn("CreatedOnTicks").AsCustom("bigint").Nullable()
                        .WithColumn("UpdatedOnTicks").AsCustom("bigint").Nullable();
                }

                string ticks = DateTime.Now.Ticks.ToString();
                string postgresGetNewGuidPart1 = @"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";";
                string postgresGetNewGuidPart2 = "uuid_generate_v4()";
                string sqlServerGetNewGuidPart1 = "";
                string sqlServerGetNewGuidPart2 = "newid()";

                string createPriceCatalogDetailTimeValuesQuery = @"{0}
                                                    insert into ""PriceCatalogDetailTimeValue""(""Oid"", ""CreatedOnTicks"", ""UpdatedOnTicks"", ""TimeValue"", 
                                                    ""OldTimeValue"", ""TimeValueValidUntil"",""TimeValueValidFrom"",""TimeValueChangedOn"", ""PriceCatalogDetail"") select 
                                                    {1} as ""Oid"",
                                                     " + ticks + @" + ""CreatedOnTicks"" % 10000 as ""CreatedOnTicks"",
                                                      " + ticks + @" + ""UpdatedOnTicks"" % 10000 as ""UpdatedOnTicks"",
                                                      ""TimeValue"",
                                                      ""OldTimeValue"",
                                                      ""TimeValueValidUntil"",
                                                      ""TimeValueValidFrom"",
                                                      ""TimeValueChangedOn"", ""Oid""
                                                      from ""PriceCatalogDetail""
                                                where ""TimeValue"" > 0 and ""GCRecord"" is null and  ""TimeValueValidUntil"" > " + ticks;

                IfDatabase("Postgres").Execute.Sql(string.Format(createPriceCatalogDetailTimeValuesQuery, postgresGetNewGuidPart1, postgresGetNewGuidPart2));
                IfDatabase("SqlServer").Execute.Sql(string.Format(createPriceCatalogDetailTimeValuesQuery, sqlServerGetNewGuidPart1, sqlServerGetNewGuidPart2));
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
