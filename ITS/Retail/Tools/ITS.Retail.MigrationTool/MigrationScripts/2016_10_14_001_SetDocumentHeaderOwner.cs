using System;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "akm", year: 2016, month: 10, day: 14, order: 1, version: "2.3.5.4")]
    public class SetDocumentHeaderOwner : Migration
    {
        public override void Up()
        {
            /* Based on the previews Migration Scripts for PriceCatalogPolicy
             we change ALL DocumentHeaders so as to inform them about the used PriceCatalogPolicy
            */
            #region Add Field PriceCatalogPolicy on DocumentHeader and Update its value
            if (Program.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                if (Schema.Table("DocumentHeader").Column("MainCompany").Exists() == false)
                {
                    Alter.Table("DocumentHeader").AddColumn("MainCompany").AsGuid().Nullable();
                }

                string updateDocumentHeaderMainCompany = 
                    "Update \"DocumentHeader\" set \"MainCompany\" = (select \"Owner\" from \"Store\" where \"Store\".\"Oid\" = \"DocumentHeader\".\"Store\")";
                Execute.Sql(updateDocumentHeaderMainCompany);
            }
            #endregion
        }
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
