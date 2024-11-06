using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
     [RetailMigration(author: "akm", year: 2015, month: 02, day: 02, order: 1, version: "2.3.0.36")]
    public class UpgradeRoles : Migration
    {
         public override void Up()
         {
             if (this.Schema.Table("Role").Column("Type").Exists() == false)
             {
                 Alter.Table("Role").AddColumn("Type").AsInt32().Nullable();
             }

             Update.Table("Role").Set(new { Type = 2 }).Where(new { Type = (int?)null, Description = "Store Manager" });
             Update.Table("Role").Set(new { Type = 2 }).Where(new { Type = (int?)null, Description = "Υπεύθυνος Καταστήματος" });
                                                                                                      
             Update.Table("Role").Set(new { Type = 2 }).Where(new { Type = (int?)null, Description = "Store Employee" });
             Update.Table("Role").Set(new { Type = 2 }).Where(new { Type = (int?)null, Description = "Υπάλλλος Καταστήματος" });

             Update.Table("Role").Set(new { Type = 3 }).Where(new { Type = (int?)null, Description = "Company Administrator" });
             Update.Table("Role").Set(new { Type = 3 }).Where(new { Type = (int?)null, Description = "Administrator Εταιρείας" });

             Update.Table("Role").Set(new { Type = 4 }).Where(new { Type = (int?)null, Description = "Administrator" });

             Update.Table("Role").Set(new { Type = 0 }).Where(new { Type = (int?)null });
         }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
