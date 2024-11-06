using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "min", year: 2017, month: 6, day: 26, order: 1, version: "2.3.7.1")]
    public class RecreateItemExtraInfoForEachStore: Migration
    {
        public override void Up()
        {
            string tableName = "ItemExtraInfo";
            string newTableName = "ItemExtraInfoOld";
            if (Schema.Table(tableName).Exists() && Schema.Table(newTableName).Exists() == false)
            {
                if ( Schema.Table("Item").Constraint("FK_Item_ItemExtraInfo").Exists() )
                {
                    Delete.ForeignKey("FK_Item_ItemExtraInfo").OnTable("Item");
                }

                List<string> constraintsToDelete = new List<string>()
                {
                    "FK_ItemExtraInfo_CreatedBy",
                    "FK_ItemExtraInfo_Item",
                    "FK_ItemExtraInfo_UpdatedBy",
                    "FK_ItemExtraInfo_Store",
                    "PK_ItemExtraInfo"
                };

                constraintsToDelete.ForEach(constraint => 
                {
                    if (Schema.Table(tableName).Constraint(constraint).Exists())
                    {
                        Delete.ForeignKey(constraint).OnTable(tableName);
                    }
                });

                List<string> indicesToDelete = new List<string>()
                {
                    "iCreatedBy_ItemExtraInfo",
                    "iGCRecord_ItemExtraInfo",
                    "iItem_ItemExtraInfo",
                    "iReferenceId_ItemExtraInfo",
                    "iUpdatedBy_ItemExtraInfo",
                    "iUpdatedOnTicksGCRecord_ItemExtraInfo",
                    "iStore_ItemExtraInfo",
                    "iStoreGCRecord_ItemExtraInfo"
                };

                indicesToDelete.ForEach(index =>
                {
                    if (Schema.Table(tableName).Index(index).Exists())
                    {
                        Delete.Index(index).OnTable(tableName);
                    }
                });

                Rename.Table(tableName).To(newTableName);
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
