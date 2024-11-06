using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "kdk", year: 2016, month: 8, day: 9, order: 3, version: "2.3.5.0")]
    public class PriceCatalogPolicyDocumentHeaderMigration : Migration
    {
        public override void Up()
        {
            /* Based on the previews Migration Scripts for PriceCatalogPolicy
             we change ALL DocumentHeaders so as to inform them about the used PriceCatalogPolicy
            */
            #region Add Field PriceCatalogPolicy on DocumentHeader and Update its value
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                if (Schema.Table("DocumentHeader").Column("PriceCatalogPolicy").Exists() == false)
                {
                    Alter.Table("DocumentHeader").AddColumn("PriceCatalogPolicy").AsGuid().Nullable();
                }

                string updateDocumentHeaderPriceCatalogPolicy = @"WITH qr as (SELECT ""PriceCatalogPolicy"".""Oid"" as policyOid , ""PriceCatalog"".""Oid"" as priceCatalogOid
                                                                          FROM ""PriceCatalogPolicy""
                                                                          JOIN ""PriceCatalog"" ON ""PriceCatalog"".""Code"" = ""PriceCatalogPolicy"".""Code""
                                                              )
                                                              UPDATE ""DocumentHeader""
                                                              SET ""PriceCatalogPolicy"" = policyOid
                                                              FROM qr
                                                              WHERE ""GCRecord"" IS NULL AND ""PriceCatalog"" IS NOT NULL
                                                              AND   qr.priceCatalogOid = ""DocumentHeader"".""PriceCatalog""  ";
                Execute.Sql(updateDocumentHeaderPriceCatalogPolicy);
            }
            #endregion
        }
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
