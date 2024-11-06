using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using DevExpress.Xpo;
using System.Data;
using DevExpress.Xpo.DB;
using System.Data.SqlClient;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.WebClient.Helpers;

namespace MigrationTool.MigrationScripts
{

    

    //[Migration(20130404)]
    [RetailMigration(author: "kdk", year: 2014, month: 1, day: 1, order: 2, version: "2.0.0.67")]
    public class Retail_Version_1_to_Version2_part2 : Migration
    {       
        
        public override void Up()
        {

            if (Schema.Table("DocumentDetail").Column("CustomDescription").Exists()==false)
            {
                Alter.Table("DocumentDetail").AddColumn("CustomDescription").AsString(100).Nullable();
            }
            Execute.Sql("Update DocumentDetail set CustomDescription = (Select Item.Name from Item where Item.Oid = DocumentDetail.Item) where documentheader is not null") ;

            //Item & Barcode --> Item & ItemBarcode & Barcode
            /////Create ItemBarcode
            if (Schema.Table("ItemBarcode").Exists() == false)
            {
                Create.Table("ItemBarcode")
                    .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                    .WithColumn("Item").AsGuid().Nullable()
                    .WithColumn("Barcode").AsGuid().Nullable()
                    .WithColumn("Owner").AsGuid().Nullable()
                    .WithColumn("Type").AsGuid().Nullable()
                    .WithColumn("MeasurementUnit").AsGuid().Nullable()
                    .WithColumn("RelationFactor").AsDouble()
                    .WithColumn("IsActive").AsBoolean()
                    .WithColumn("CreatedOnTicks").AsCustom("bigint")
                    .WithColumn("UpdatedOnTicks").AsCustom("bigint")
                    ;
                long nowTicks = DateTime.Now.Ticks;
                /////Fill table ItemBarcode
                Execute.Sql(@"Insert into ItemBarcode (Oid,CreatedOnTicks, UpdatedOnTicks,Item,Barcode, Owner, Type, MeasurementUnit,RelationFactor, IsActive)
                    Select NEWID() as Oid,"+ nowTicks.ToString() +" , "+ nowTicks.ToString() +@" , Barcode.Item as Item, Barcode.Oid as Barcode, (select top 1 Oid from CompanyNew)  
                    as Owner, Barcode.Type as Type, Barcode.MesurmentUnit as MeasurementUnit, Barcode.RelationFactor, Barcode.IsActive
                    from Barcode");

            }


            /////Alter Item
            if (Schema.Table("Item").Column("Owner").Exists() == false)
            {
                Alter.Table("Item").AddColumn("Owner").AsGuid().Nullable();
            }

            if (Schema.Table("Item").Column("AcceptsCustomDescription").Exists() == false)
            {
                Alter.Table("Item").AddColumn("AcceptsCustomDescription").AsBoolean().Nullable();
            }

            if (Schema.Table("Item").Column("CustomPriceOptions").Exists() == false)
            {
                Alter.Table("Item").AddColumn("CustomPriceOptions").AsInt32().WithDefaultValue(0);
            }

            if (Schema.Table("Item").Column("ItemImageOid").Exists())
            {
                Delete.Column("ItemImageOid").FromTable("Item");
            }
            /////Fill table Item
            Execute.Sql("update Item set Owner = (select top 1 Oid from CompanyNew)");

            //DocumentType
            if (Schema.Table("DocumentType").Column("Owner").Exists() == false)
            {
                Alter.Table("DocumentType").AddColumn("Owner").AsGuid().Nullable();
            }
            Execute.Sql("update DocumentType set Owner = (select top 1 Oid from CompanyNew)");

            //////Alter Barcode table
            if (Schema.Table("Barcode").Column("Type").Exists())
            {
                Delete.Column("Type").FromTable("Barcode");
            }
            if (Schema.Table("Barcode").Column("RelationFactor").Exists())
            {
                Delete.Column("RelationFactor").FromTable("Barcode");
            }
            if (Schema.Table("Barcode").Column("Item").Exists())
            {
                Delete.Column("Item").FromTable("Barcode");
            }
            if (Schema.Table("Barcode").Column("MesurmentUnit").Exists())
            {
                Delete.Column("MesurmentUnit").FromTable("Barcode");
            }
           

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
