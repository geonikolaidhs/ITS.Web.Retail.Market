using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "nst", year: 2016, month: 02, day: 18, order: 1, version: "2.3.2.18")]
    public class Remove_DocumentSeries_IsForPOS : Migration
    {
        public override void Up()
        {
            if (this.Schema.Table("DocumentSeries").Column("eModule").Exists() == false)
            {
                Alter.Table("DocumentSeries").AddColumn("eModule").AsInt32().Nullable();
            }
            IfDatabase("SqlServer").Execute.Sql(@"UPDATE DocumentSeries 
                                                  SET eModule =
                                                          CASE
                                                              WHEN((SELECT COUNT(series.Oid)
                                                                      FROM StoreDocumentSeriesType AS sdst 
                                                                      INNER JOIN DocumentSeries AS series ON series.Oid =  sdst.DocumentSeries
                                                                      WHERE sdst.IsForPOS = 1 AND series.Oid = DocumentSeries.Oid
                                                                      GROUP BY series.Oid) > 0)
                                                              THEN 5
                                                              ELSE 0
                                                          END");
            IfDatabase("Postgres").Execute.Sql(@"UPDATE ""DocumentSeries""
                                                 SET ""eModule"" =
                                                     CASE
                                                         WHEN((SELECT COUNT(series.""Oid"")
                                                             FROM ""StoreDocumentSeriesType"" AS sdst
                                                             INNER JOIN ""DocumentSeries"" AS series ON series.""Oid"" = sdst.""DocumentSeries""
                                                             WHERE sdst.""IsForPOS"" = TRUE AND series.""Oid"" = ""DocumentSeries"".""Oid""
                                                             GROUP BY series.""Oid"") > 0)
                                                         THEN 5
                                                         ELSE 0
                                                     END");

            if (this.Schema.Table("StoreDocumentSeriesType").Column("IsForPOS").Exists())
            {
                Delete.Column("IsForPOS").FromTable("StoreDocumentSeriesType");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}