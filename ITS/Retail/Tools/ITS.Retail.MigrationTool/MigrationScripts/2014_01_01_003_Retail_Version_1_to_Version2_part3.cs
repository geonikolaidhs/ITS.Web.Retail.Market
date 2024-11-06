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
    [RetailMigration(author: "kdk", year: 2014, month: 1, day: 1, order: 3, version: "2.0.0.67")]
    public class Retail_Version_1_to_Version2_part3 : Migration
    {      


        private readonly String[] TablesWithOwner = new String[]{
            "AddressType",
            "BarcodeType",
            "Buyer",
            "CategoryNode",
            "DataFileRecordHeader",
            "DocumentStatus",
            "DeliveryType",
            "Offer",
            "PaymentMethod",
            "Phone",
            "PhoneType",
            "PriceCatalog",
            "Role",
            "Seasonality",
            "VATCategory",
            "VATLevel",
            "VATFactor"
        };
        
        public override void Up()
        {
            if (Schema.Table("OwnerApplicationSettings").Exists() == false)
            {

                Create.Table("OwnerApplicationSettings")
                    .WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                    .WithColumn("Owner").AsGuid().Nullable()
                    .WithColumn("DoPadding").AsBoolean().Nullable()
                    .WithColumn("BarcodeLength").AsInt64()
                    .WithColumn("ItemCodeLength").AsInt64()
                    .WithColumn("BarcodePaddingCharacter").AsString(100)
                    .WithColumn("ItemCodePaddingCharacter").AsString(100)
                    .WithColumn("ComputeDigits").AsDouble()
                    .WithColumn("DisplayDigits").AsDouble()
                    .WithColumn("MaxItemOrderQty").AsDouble()
                    .WithColumn("DisplayValueDigits").AsDouble()
                    .WithColumn("ComputeValueDigits").AsDouble()
                    .WithColumn("UseBarcodeRelationFactor").AsBoolean().Nullable()
                    .WithColumn("DiscountPermited").AsBoolean().Nullable()
                    .WithColumn("RecomputePrices").AsBoolean().Nullable()
                    .WithColumn("ApplicationTerms").AsString(Int32.MaxValue)
                    .WithColumn("TrimBarcodeOnDisplay").AsBoolean().Nullable()
                    ;


                Execute.Sql(@"insert into OwnerApplicationSettings
([Oid],[Owner], 	[DoPadding] ,	[BarcodeLength],	[ItemCodeLength],	[BarcodePaddingCharacter] ,	
[ItemCodePaddingCharacter] ,	[ComputeDigits],	[DisplayDigits] ,	[MaxItemOrderQty] ,	
[DisplayValueDigits],	[ComputeValueDigits],	[UseBarcodeRelationFactor] ,	[DiscountPermited] 
,	[RecomputePrices] ,	[ApplicationTerms])
select 
	NEWID(),
	(select top 1 Oid from companynew) as Owner,
	[DoPadding] ,
	[BarcodeLenght],
	[ItemCodeLenght],
	[BarcodePaddingCharacter] ,
	[ItemCodePaddingCharacter] ,
	[ComputeDigits],
	[DisplayDigits] ,
	[MaxItemOrderQty], 
	[DisplayValueDigits],
	[ComputeValueDigits],
	[UseBarcodeRelationFactor] ,
	[DiscountPermited] ,
	[RecomputePrices] ,
	[ApplicationTerms]
	
	
	from ApplicationSettings");
            }


            if (Schema.Table("ApplicationSettings").Column("DoPadding").Exists())
            {
                Delete.Column("DoPadding").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("BarcodeLenght").Exists())
            {
                Delete.Column("BarcodeLenght").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("ItemCodeLenght").Exists())
            {
                Delete.Column("ItemCodeLenght").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("BarcodePaddingCharacter").Exists())
            {
                Delete.Column("BarcodePaddingCharacter").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("ItemCodePaddingCharacter").Exists())
            {
                Delete.Column("ItemCodePaddingCharacter").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("ComputeDigits").Exists())
            {
                Delete.Column("ComputeDigits").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("DisplayDigits").Exists())
            {
                Delete.Column("DisplayDigits").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("MaxItemOrderQty").Exists())
            {
                Delete.Column("MaxItemOrderQty").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("DisplayValueDigits").Exists())
            {
                Delete.Column("DisplayValueDigits").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("ComputeValueDigits").Exists())
            {
                Delete.Column("ComputeValueDigits").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("DiscountPermited").Exists())
            {
                Delete.Column("DiscountPermited").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("UseBarcodeRelationFactor").Exists())
            {
                Delete.Column("UseBarcodeRelationFactor").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("RecomputePrices").Exists())
            {
                Delete.Column("RecomputePrices").FromTable("ApplicationSettings");
            }
            if (Schema.Table("ApplicationSettings").Column("ApplicationTerms").Exists())
            {
                Delete.Column("ApplicationTerms").FromTable("ApplicationSettings");
            }

            //User
            if (Schema.Table("User").Column("IsApproved").Exists() == false)
            {
                Alter.Table("User").AddColumn("IsApproved").AsBoolean().Nullable();
                Update.Table("User").Set(new { IsApproved = true }).Where(new { IsActive = true });
            }

            Alter.Table("DataFileReceived").AlterColumn("Filename").AsString(Int32.MaxValue);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
