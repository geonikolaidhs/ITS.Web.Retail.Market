using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "nst", year: 2016, month: 05, day: 11, order: 1, version: "2.3.2.23")]
    public class Complete_Address_Profession : Migration
    {
        public override void Up()
        {
            if (!this.Schema.Table("Address").Column("Profession").Exists())
            {
                Alter.Table("Address").AddColumn("Profession").AsString(100).Nullable();
            }
            IfDatabase("Postgres").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                            (SELECT ""SupplierNew"".""Profession""
                                                            FROM ""Address"" AS adr
                                                            INNER JOIN ""SupplierNew"" ON ""SupplierNew"".""Trader"" = adr.""Trader""
                                                            WHERE ""Address"".""Oid"" = adr.""Oid""
                                                            limit 1)
                                                    WHERE ""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = ''");
            IfDatabase("Postgres").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                                (SELECT ""Customer"".""Profession""
                                                                FROM ""Address"" AS adr
                                                                INNER JOIN ""Customer"" ON ""Customer"".""Trader"" = adr.""Trader""
                                                                WHERE ""Address"".""Oid"" = adr.""Oid"" 
                                                                AND ""Customer"".""Profession"" IS NOT NULL 
                                                                AND ""Customer"".""Profession"" <> ''
                                                                limit 1) 
                                                    WHERE ""Trader"" IN (SELECT ""Trader"" FROM ""Customer"")
                                                    AND (""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = '')");
            IfDatabase("Postgres").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                        (SELECT ""CompanyNew"".""Profession""
                                                        FROM ""Address""
                                                        INNER JOIN ""CompanyNew"" ON ""CompanyNew"".""Trader"" = ""Address"".""Trader""
                                                        WHERE ""CompanyNew"".""Profession"" IS NOT NULL AND ""CompanyNew"".""Profession"" <> ''
                                                        limit 1) 
                                                    WHERE ""Address"".""Trader"" in (select ""CompanyNew"".""Trader"" FROM ""CompanyNew"")
                                                    AND (""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = '')");
            IfDatabase("SqlServer").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                        (SELECT TOP 1 ""SupplierNew"".""Profession""
                                                        FROM ""Address"" AS adr
                                                        INNER JOIN ""SupplierNew"" ON ""SupplierNew"".""Trader"" = adr.""Trader""
                                                        WHERE ""Address"".""Oid"" = adr.""Oid"")
                                                    WHERE ""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = ''");
            IfDatabase("SqlServer").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                        (SELECT TOP 1 ""Customer"".""Profession""
                                                        FROM ""Address"" AS adr
                                                        INNER JOIN ""Customer"" ON ""Customer"".""Trader"" = adr.""Trader""
                                                        WHERE ""Address"".""Oid"" = adr.""Oid"" 
                                                        AND ""Customer"".""Profession"" IS NOT NULL 
                                                        AND ""Customer"".""Profession"" <> '')
                                                    WHERE ""Trader"" IN (SELECT ""Trader"" FROM ""Customer"")
                                                    AND (""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = '')");
            IfDatabase("SqlServer").Execute.Sql(@"UPDATE ""Address"" 
                                                    SET ""Profession"" = 
                                                        (SELECT TOP 1 ""CompanyNew"".""Profession""
                                                        FROM ""Address""
                                                        INNER JOIN ""CompanyNew"" ON ""CompanyNew"".""Trader"" = ""Address"".""Trader""
                                                        WHERE ""CompanyNew"".""Profession"" IS NOT NULL 
                                                        AND ""CompanyNew"".""Profession"" <> '')
                                                    WHERE ""Address"".""Trader"" in (select ""CompanyNew"".""Trader"" FROM ""CompanyNew"")
                                                    AND (""Address"".""Profession"" IS NULL OR ""Address"".""Profession"" = '')");

            string createdOnTicks = DateTime.Now.Ticks.ToString();

            string insertQuery = @"INSERT INTO ""Address"" (""Oid"", ""IsActive"", ""IsDefault"", ""AutomaticNumbering"", ""CreatedOnTicks"", ""UpdatedOnTicks"",""Trader"",""Profession"") 
                    SELECT {0}, {1}, {1}, 1, " + createdOnTicks + " ," + createdOnTicks + @" ,""Customer"".""Trader"", ""Customer"".""Profession"" 
                    FROM ""Customer"" 
                    WHERE ""Oid"" NOT IN(
                        SELECT ""Customer"".""Oid"" 
                        FROM ""Address"" 
                        INNER JOIN ""Customer"" ON ""Address"".""Trader"" = ""Customer"".""Trader"") 
                    AND ""GCRecord"" IS NULL 
                    AND ""Customer"".""Profession"" IS NOT NULL 
                    AND ""Customer"".""Profession"" <> ''; 
                    
                    INSERT INTO ""Address"" (""Oid"", ""IsActive"", ""IsDefault"", ""AutomaticNumbering"", ""CreatedOnTicks"", ""UpdatedOnTicks"", ""Trader"", ""Profession"") 
                    SELECT {0}, {1}, {1}, 1, " + createdOnTicks + " ," + createdOnTicks + @",""SupplierNew"".""Trader"", ""SupplierNew"".""Profession""
                    FROM ""SupplierNew"" 
                    WHERE ""Oid"" NOT IN(
                        SELECT ""SupplierNew"".""Oid"" 
                        FROM ""Address"" 
                        INNER JOIN ""SupplierNew"" ON ""Address"".""Trader"" = ""SupplierNew"".""Trader"")
                    AND ""GCRecord"" IS NULL 
                    AND ""SupplierNew"".""Profession"" IS NOT NULL
                    AND ""SupplierNew"".""Profession"" <> '';";


            IfDatabase("SqlServer").Execute.Sql(String.Format(insertQuery, "NEWID()", "1", "1"));

            string createExtension = @"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp""; ";

            IfDatabase("Postgres").Execute.Sql(String.Format(createExtension + insertQuery, "uuid_generate_v4()", "TRUE", "TRUE"));
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
