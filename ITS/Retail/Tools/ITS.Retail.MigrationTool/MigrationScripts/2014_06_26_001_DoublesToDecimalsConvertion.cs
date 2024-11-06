using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "sak", year: 2014, month: 6, day: 26, order: 1, version: "2.0.0.65")]
    public class DoublesToDecimalsConvertion : Migration
    {
        //alter Columns to decimal (money in sqlserver) from double
        //======================================================
        //DocumentDetail.PriceListUnitPrice
        //DocumentDetail.UnitPrice
        //DocumentDetail.Qty
        //DocumentDetail.Points
        //DocumentDetail.VatFactor
        //DocumentDetail.FinalUnitPrice
        //DocumentDetail.TotalDiscount
        //DocumentDetail.NetTotal
        //DocumentDetail.GrossTotal
        //DocumentDetail.TotalPoints
        //DocumentDetail.TotalVatAmount
        //DocumentDetail.GrossTotalBeforeDiscount
        //DocumentDetail.CustomUnitPrice
        //DocumentDetail.NetTotalDeviation
        //DocumentDetail.TotalVatAmountDeviation
        //DocumentDetail.GrossTotalDeviation
        //DocumentDetail.VatAmount

        //DocumentHeader.DocumentDiscountValue
        //DocumentHeader.DocumentDiscountPercentage
        //DocumentHeader.VatAmount1
        //DocumentHeader.VatAmount2
        //DocumentHeader.VatAmount3
        //DocumentHeader.VatAmount4
        //DocumentHeader.TotalDiscountAmount
        //DocumentHeader.TotalVatAmount
        //DocumentHeader.TotalVatAmountBeforeDocumentDiscount
        //DocumentHeader.NetTotal
        //DocumentHeader.NetTotalBeforeDocumentDiscount
        //DocumentHeader.GrossTotal
        //DocumentHeader.GrossTotalBeforeDocumentDiscount
        //DocumentHeader.Status
        //DocumentHeader.TotalQty

        //DailyTotalDetail.QtyValue
        //DailyTotalDetail.Amount
        //DailyTotalDetail.VatAmount

        //UserDailyTotalDetail.QtyValue
        //UserDailyTotalDetail.Amount
        //UserDailyTotalDetail.VatAmount

        //PriceCatalogDetail.Discount
        //PriceCatalogDetail.Value
        //PriceCatalogDetail.OldValue
        //PriceCatalogDetail.MarkUp

        //RelativeDocumentDetail.Qty

        //VatFactor.Factor

        //Renamed Columns
        //======================================================
        //DocumentDetail.VatAmount -> TotalVatAmountBeforeDiscount

        //Droped Columns
        //======================================================
        //DocumentDetail.UnitPriceAfterDiscount
        //DocumentDetail.UnitPriceAfterDiscount2
        //DocumentDetail.FirstDiscount
        //DocumentDetail.FirstDiscount2
        //DocumentDetail.SecondDiscount
        //DocumentDetail.SecondDiscount2
        //DocumentDetail.NetTotal2
        //DocumentDetail.NetTotalAfterDiscount
        //DocumentDetail.NetTotalAfterFirstDiscount
        //DocumentDetail.NetTotalAfterSecondDiscount
        //DocumentDetail.NetTotalAfterDiscount2
        //DocumentDetail.GrossTotalBeforeDocumentDiscount
        //DocumentDetail.TotalVatAmountBeforeDocumentDiscount
        //DocumentDetail.NetTotalBeforeDocumentDiscount
        //DocumentDetail.GrossTotalAfterFirstDiscount
        //DocumentDetail.GrossTotalAfterSecondDiscount
        //DocumentDetail.PriceListUnitPrice2
        //DocumentDetail.UnitPrice2
        //DocumentDetail.Qty2
        //DocumentDetail.MeasurementUnit2
        //DocumentDetail.MeasurementUnit2Code
        //DocumentDetail.VatAmount2
        //DocumentDetail.FinalUnitPrice2
        //DocumentDetail.TotalDiscount2
        //DocumentDetail.NetTotal2
        //DocumentDetail.GrossTotal2
        //DocumentDetail.TotalVatAmount2

        //New Columns
        //======================================================
        //DocumentDetail.TotalVatAmountBeforeDiscount
        //DocumentDetail.NetTotalBeforeDiscount

        public override void Up()
        {
            List<Tuple<string, string>> alterColumnsDictionary = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("DocumentDetail","PriceListUnitPrice"),
                new Tuple<string,string>("DocumentDetail","UnitPrice"),
                new Tuple<string,string>("DocumentDetail","Qty"),
                new Tuple<string,string>("DocumentDetail","Points"),
                new Tuple<string,string>("DocumentDetail","VatFactor"),
                new Tuple<string,string>("DocumentDetail","FinalUnitPrice"),
                new Tuple<string,string>("DocumentDetail","TotalDiscount"),
                new Tuple<string,string>("DocumentDetail","NetTotal"),
                new Tuple<string,string>("DocumentDetail","GrossTotal"),
                new Tuple<string,string>("DocumentDetail","TotalPoints"),
                new Tuple<string,string>("DocumentDetail","TotalVatAmount"),
                new Tuple<string,string>("DocumentDetail","GrossTotalBeforeDiscount"),
                new Tuple<string,string>("DocumentDetail","TotalVatAmountBeforeDiscount"),
                new Tuple<string,string>("DocumentDetail","CustomUnitPrice"),
                new Tuple<string,string>("DocumentDetail","NetTotalDeviation"),
                new Tuple<string,string>("DocumentDetail","TotalVatAmountDeviation"),
                new Tuple<string,string>("DocumentDetail","GrossTotalDeviation"),
                new Tuple<string,string>("DocumentDetail","VatAmount"),
                new Tuple<string,string>("DocumentHeader","DocumentDiscountValue"),
                new Tuple<string,string>("DocumentHeader","DocumentDiscountPercentage"),
                new Tuple<string,string>("DocumentHeader","VatAmount1"),
                new Tuple<string,string>("DocumentHeader","VatAmount2"),
                new Tuple<string,string>("DocumentHeader","VatAmount3"),
                new Tuple<string,string>("DocumentHeader","VatAmount4"),
                new Tuple<string,string>("DocumentHeader","TotalDiscountAmount"),
                new Tuple<string,string>("DocumentHeader","TotalVatAmount"),
                new Tuple<string,string>("DocumentHeader","TotalVatAmountBeforeDocumentDiscount"),
                new Tuple<string,string>("DocumentHeader","NetTotal"),
                new Tuple<string,string>("DocumentHeader","NetTotalBeforeDocumentDiscount"),
                new Tuple<string,string>("DocumentHeader","NetTotalBeforeDiscount"),
                new Tuple<string,string>("DocumentHeader","GrossTotal"),
                new Tuple<string,string>("DocumentHeader","GrossTotalBeforeDocumentDiscount"),
                new Tuple<string,string>("DocumentHeader","VatAmount"),
                new Tuple<string,string>("DocumentHeader","TotalQty"),
                new Tuple<string,string>("DailyTotalDetail","QtyValue"),
                new Tuple<string,string>("DailyTotalDetail","Amount"),
                new Tuple<string,string>("DailyTotalDetail","VatAmount"),
                new Tuple<string,string>("UserDailyTotalDetail","QtyValue"),
                new Tuple<string,string>("UserDailyTotalDetail","Amount"),
                new Tuple<string,string>("UserDailyTotalDetail","VatAmount"),
                new Tuple<string,string>("PriceCatalogDetail","Discount"),
                new Tuple<string,string>("PriceCatalogDetail","Value"),
                new Tuple<string,string>("PriceCatalogDetail","OldValue"),
                new Tuple<string,string>("PriceCatalogDetail","MarkUp"),
                new Tuple<string,string>("RelativeDocumentDetail","Qty"),
                new Tuple<string,string>("VatFactor","Factor")
            };

            String[][] indexesToBeDropped = new String[][]{
                new String[]{ "DocumentDetail", "iQty_DocumentDetail"},
                new String[]{ "DocumentDetail", "iGCRecordIsCanceledItemQtyNetTotalAfterDiscountDocumentHeader_DocumentDetail"},
                new String[]{ "DocumentDetail", "iDocumentHeaderGCRecordIsCanceledItemQty_DocumentDetail"},

            };

            //List<Tuple<string, string, string>> renameColumnsDictionary = new List<Tuple<string, string, string>>()
            //{
            //    new Tuple<string,string,string>("DocumentDetail","NetTotalBeforeDocumentDiscount","NetTotalBeforeDiscount")

            //};

            List<Tuple<string, string, string, string>> dropColumnsDictionary = new List<Tuple<string, string, string, string>>()
            {
                /*                                       //TableName, ColumnName, Index, Foreign Key
               new Tuple<string,string,string,string> ("DocumentDetail","UnitPriceAfterDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","UnitPriceAfterDiscount2","",""),
               //new Tuple<string,string,string,string> ("DocumentDetail","FirstDiscount","",""),   //DO NOT DROP FOR REFERENCE
               new Tuple<string,string,string,string> ("DocumentDetail","FirstDiscount2","",""),
               //new Tuple<string,string,string,string> ("DocumentDetail","SecondDiscount","",""),  //DO NOT DROP FOR REFERENCE
               new Tuple<string,string,string,string> ("DocumentDetail","SecondDiscount2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotal2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalAfterDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalAfterFirstDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalAfterSecondDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalAfterDiscount2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","GrossTotalBeforeDocumentDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","TotalVatAmountBeforeDocumentDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","GrossTotalAfterFirstDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","GrossTotalAfterSecondDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","PriceListUnitPrice2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","UnitPrice2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","Qty2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","MeasurementUnit2","iMeasurementUnit2_DocumentDetail","FK_DocumentDetail_MeasurementUnit2"),
               new Tuple<string,string,string,string> ("DocumentDetail","MeasurementUnit2Code","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","VatAmount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","VatAmount2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","FinalUnitPrice2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","TotalDiscount2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","GrossTotal2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","GrossTotalAfterDocumentDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","TotalVatAmountAfterDocumentDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalAfterDocumentDiscount","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","TotalVatAmount2","",""),
               new Tuple<string,string,string,string> ("DocumentDetail","NetTotalBeforeDocumentDiscount","","")-*/
            };

            List<Tuple<string, string>> newColumnsDictionary = new List<Tuple<string, string>>()
            {
                new Tuple<string,string> ("DocumentDetail","TotalVatAmountBeforeDiscount"),
                new Tuple<string,string> ("DocumentDetail","NetTotalBeforeDiscount")
            };

            //Drop indices
            foreach (String[] pair in indexesToBeDropped)
            {
                if (Schema.Table(pair[0]).Index(pair[1]).Exists())
                {
                    Delete.Index(pair[1]).OnTable(pair[0]);
                }
            }

            //double to decimals
            foreach (Tuple<string, string> pair in alterColumnsDictionary)
            {
                string table = pair.Item1;
                string column = pair.Item2;
                if (Schema.Table(table).Column(column).Exists())
                {
                    //Alter.Column(column).OnTable(table).AsDecimal().Nullable();
                    IfDatabase("SqlServer").Execute.Sql("ALTER TABLE " + table + " ALTER COLUMN " + column + " MONEY");
                    IfDatabase("Postgres").Execute.Sql("ALTER TABLE " + table + " ALTER COLUMN " + column + " decimal(28,8)");
                    IfDatabase("Oracle").Execute.Sql("ALTER TABLE " + table + " ALTER COLUMN " + column + " number(19,5)");
                }
            }

            //new columns
            foreach (Tuple<string, string> pair in newColumnsDictionary)
            {
                string table = pair.Item1;
                string column = pair.Item2;
                if (Schema.Table(table).Column(column).Exists() == false)
                {
                    Create.Column(column).OnTable(table).AsCurrency().Nullable();
                }
            }

            ////renames
            //foreach (Tuple<string, string, string> renameTuple in renameColumnsDictionary)
            //{
            //    string table = renameTuple.Item1;
            //    string oldColumnName = renameTuple.Item2;
            //    string newColumnName = renameTuple.Item3;
            //    if (Schema.Table(table).Column(oldColumnName).Exists() && Schema.Table(table).Column(newColumnName).Exists() == false)
            //    {
            //        Execute.Sql("UPDATE DocumentDetail SET "+newColumnName+" = "+oldColumnName);        

            //        //Rename.Column(oldColumnName).OnTable(table).To(newColumnName);
            //    }
            //}


            //Populate Columns
            CalculateDocumentDetailMissingFields();
            CalculateDocumentHeaderMissingFields();

            if (Schema.Table("DocumentDetailDiscount").Exists())
            {
                if (Schema.Table("Temp_DocumentDetailDiscountsAreEmpty").Exists())
                {
                    Delete.Table("Temp_DocumentDetailDiscountsAreEmpty");
                    Delete.Table("DocumentDetailDiscount");
                    CreateDocumentDetailDiscountsTable();
                }
            }
            else
            {
                CreateDocumentDetailDiscountsTable();
            }
            
            CreateDocumentDetailDiscounts();

            //drop columns
            foreach (Tuple<string, string, string, string> pair in dropColumnsDictionary)
            {
                string table = pair.Item1;
                string column = pair.Item2;
                string index = pair.Item3;
                string foreignKey = pair.Item4;
                if (Schema.Table(table).Column(column).Exists())
                {
                    if (String.IsNullOrWhiteSpace(index) == false && Schema.Table(table).Index(index).Exists())
                    {
                        Delete.Index(index).OnTable(table);
                    }

                    if (String.IsNullOrWhiteSpace(foreignKey) == false)
                    {
                        Delete.ForeignKey(foreignKey).OnTable(table);
                    }

                    Delete.Column(column).FromTable(table);
                }
            }
        }

        private void CalculateDocumentDetailMissingFields()
        {
            List<Tuple<string, string>> checkTableColumnsExistance = new List<Tuple<string, string>>{
                         // Tuple.Create("DocumentDetail","TotalVatAmountBeforeDiscount"),
                          Tuple.Create("DocumentDetail","GrossTotalBeforeDiscount")//,
                          //Tuple.Create("DocumentDetail","NetTotalBeforeDiscount")
            };
            CreateColumnsFromTuple(checkTableColumnsExistance);

            if (Schema.Table("DocumentDetail").Column("CustomUnitPrice").Exists() == false)
            {
                Alter.Table("DocumentDetail").AddColumn("CustomUnitPrice").AsDecimal().WithDefaultValue(0.0);
            }


            //Add Value in DocumentDetail
            //TotalVatAmountBeforeDiscount
            //GrossTotalBeforeDiscount
            //NetTotalBeforeDiscount


            const string computeDigits = "4";
            //R E T A I L
            //documentDetail.GrossTotalBeforeDiscount = documentDetail.FinalUnitPrice * documentDetail.Qty;
            Execute.Sql(@"UPDATE DocDetail SET GrossTotalBeforeDiscount = FinalUnitPrice * Qty
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 0");
            //documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount * (decimal)documentDetail.VatFactor / (1 + (decimal)documentDetail.VatFactor), header.Owner);
            Execute.Sql(@"UPDATE DocDetail SET TotalVatAmountBeforeDiscount = ROUND( DocDetail.GrossTotalBeforeDiscount * VatFactor / (1 + VatFactor) ," + computeDigits + @")
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 0");
            //documentDetail.NetTotalBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalVatAmountBeforeDiscount, header.Owner);
            Execute.Sql(@"UPDATE DocDetail SET NetTotalBeforeDiscount = ROUND( DocDetail.GrossTotalBeforeDiscount - DocDetail.TotalVatAmountBeforeDiscount ," + computeDigits + @")
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 0");
            //documentDetail.CustomUnitPrice = documentDetail.FinalUnitPrice;
            Execute.Sql(@"UPDATE DocDetail SET CustomUnitPrice = DocDetail.FinalUnitPrice
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 0");

            //Net total
            Execute.Sql(@"UPDATE DocDetail SET NetTotal = ROUND( DocDetail.GrossTotal - DocDetail.TotalVatAmount ," + computeDigits + @")
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 0");

            //W H O L E S A L E S
            //documentDetail.NetTotalBeforeDiscount = (documentDetail.UnitPrice * (decimal)documentDetail.Qty);
            Execute.Sql(@"UPDATE DocDetail SET NetTotalBeforeDiscount = DocDetail.UnitPrice * Qty
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 1");
            //documentDetail.GrossTotalBeforeDiscount = RoundDisplayValue(documentDetail.NetTotalBeforeDiscount * (1 + (decimal)documentDetail.VatFactor), header.Owner);
            Execute.Sql(@"UPDATE DocDetail SET GrossTotalBeforeDiscount = ROUND( DocDetail.NetTotalBeforeDiscount * (1 + VatFactor) ," + computeDigits + @")
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 1");
            //documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.NetTotalBeforeDiscount, header.Owner);
            Execute.Sql(@"UPDATE DocDetail SET TotalVatAmountBeforeDiscount = ROUND( DocDetail.GrossTotalBeforeDiscount - DocDetail.NetTotalBeforeDiscount ," + computeDigits + @")
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 1");
            //documentDetail.CustomUnitPrice = documentDetail.UnitPrice;
            Execute.Sql(@"UPDATE DocDetail SET CustomUnitPrice = DocDetail.UnitPrice
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 1");

            //Net Total
            Execute.Sql(@"UPDATE DocDetail SET NetTotal = DocDetail.FinalUnitPrice * DocDetail.Qty
                         FROM DocumentDetail DocDetail
                         JOIN DocumentHeader on DocumentHeader.Oid = DocDetail.DocumentHeader
                         JOIN DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                         WHERE DocumentHeader IS NOT NULL
                         AND DocumentType.IsForWholesale = 1");
        }

        private void CalculateDocumentHeaderMissingFields()
        {
            List<Tuple<string, string>> checkTableColumnsExistance = new List<Tuple<string, string>>{
                 Tuple.Create("DocumentHeader","TotalVatAmountBeforeDiscount"),
                 Tuple.Create("DocumentHeader","GrossTotalBeforeDiscount"),
                 Tuple.Create("DocumentHeader","NetTotalBeforeDiscount")
            };
            CreateColumnsFromTuple(checkTableColumnsExistance);

            //Add Value in DocumentHeader 
            //TotalVatAmountBeforeDiscount
            //GrossTotalBeforeDiscount
            //NetTotalBeforeDiscount
            List<string> fields = new List<string>()
            {
                "TotalVatAmountBeforeDiscount",
                "GrossTotalBeforeDiscount",
                "NetTotalBeforeDiscount"
            };

            foreach (string field in fields)
            {
                Execute.Sql("UPDATE DocumentHeader SET " + field + " = "
                    + "(SELECT SUM(" + field + ") FROM DocumentDetail"
                    + " WHERE DocumentDetail.DocumentHeader IS NOT NULL AND DocumentDetail.GCRecord IS NULL AND DocumentHeader.GCRecord IS NULL"
                    + " AND DocumentDetail.DocumentHeader = DocumentHeader.Oid )");
            }

        }

        private void CreateColumnsFromTuple(List<Tuple<string, string>> checkTableColumnsExistance)
        {
            //Item1 = Table
            //Item2 = Column
            //Item3 = Type
            //Item4 = IsCustomType
            foreach (Tuple<string, string> tuple in checkTableColumnsExistance)
            {
                if (Schema.Table(tuple.Item1).Column(tuple.Item2).Exists() == false)
                {
                    Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsDecimal().Nullable();
                }
            }
        }


        protected void CreateDocumentDetailDiscountsTable()
        {

            Create.Table("DocumentDetailDiscount").WithColumn("Oid").AsGuid().NotNullable().PrimaryKey()
                                                  .WithColumn("DocumentDetail").AsGuid().Nullable()
                                                  .WithColumn("DiscountSource").AsInt32().Nullable()
                                                  .WithColumn("DiscountType").AsInt32().Nullable()
                                                  .WithColumn("Type").AsGuid().Nullable()
                                                  .WithColumn("Priority").AsInt32().Nullable()
                                                  .WithColumn("Percentage").AsCurrency().Nullable()
                                                  .WithColumn("Value").AsCurrency().Nullable()
                                                  .WithColumn("CreatedOnTicks").AsInt64().Nullable()
                                                  .WithColumn("UpdatedOnTicks").AsInt64().Nullable();


            //if(Schema.Table("DocumentDetailDiscount").Column("Weight").Exists() &&
            //   Schema.Table("DocumentDetailDiscount").Column("Priority").Exists() == false)
            //{ 
            //    Rename.Column("Weight").OnTable("DocumentDetailDiscount").To("Priority");
            //}

            //if (Schema.Table("DocumentDetailDiscount").Column("Percentage").Exists() == false)
            //{
            //    Alter.Table("DocumentDetailDiscount").AddColumn("Percentage").AsCurrency().Nullable();
            //}

            //if (Schema.Table("DocumentDetailDiscount").Column("DiscountType").Exists() &&
            //    Schema.Table("DocumentDetailDiscount").Column("Type").Exists() == false)
            //{
            //    Rename.Column("DiscountType").OnTable("DocumentDetailDiscount").To("Type");
            //    Create.Column("DiscountType").OnTable("DocumentDetailDiscount").AsInt32().Nullable();
            //}

        }

        protected void CreateDocumentDetailDiscounts()
        {
            Execute.Sql(@"INSERT INTO DocumentDetailDiscount (Oid,DocumentDetail,DiscountSource,DiscountType,Priority,[Percentage],Value,CreatedOnTicks,UpdatedOnTicks) 
                          SELECT NEWID(),DocumentDetail.Oid," + (int)eDiscountSource.PRICE_CATALOG + "," + (int)eDiscountType.PERCENTAGE
                                + @",-1,DocumentDetail.FirstDiscount,
                                 DocumentDetail.FirstDiscount*DocumentDetail.NetTotal,
                                 DocumentDetail.CreatedOnTicks,DocumentDetail.UpdatedOnTicks
                          FROM DocumentDetail 
                                join DocumentHeader on DocumentHeader.Oid = DocumentDetail.DocumentHeader 
                                join DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                          WHERE DocumentType.IsForWholesale = 1 and DocumentDetail.FirstDiscount >0");

            Execute.Sql(@"INSERT INTO DocumentDetailDiscount (Oid,DocumentDetail,DiscountSource,DiscountType,Priority,[Percentage],Value,CreatedOnTicks,UpdatedOnTicks) 
                          SELECT NEWID(),DocumentDetail.Oid," + (int)eDiscountSource.PRICE_CATALOG + "," + (int)eDiscountType.PERCENTAGE
                                + @",-1,DocumentDetail.FirstDiscount,
                                 DocumentDetail.FirstDiscount*DocumentDetail.GrossTotal,
                                 DocumentDetail.CreatedOnTicks,DocumentDetail.UpdatedOnTicks
                          FROM DocumentDetail 
                                join DocumentHeader on DocumentHeader.Oid = DocumentDetail.DocumentHeader 
                                join DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                          WHERE DocumentType.IsForWholesale = 0 and DocumentDetail.FirstDiscount >0");

            Execute.Sql(@"INSERT INTO DocumentDetailDiscount (Oid,DocumentDetail,DiscountSource,DiscountType,Priority,[Percentage],Value,CreatedOnTicks,UpdatedOnTicks) 
                          SELECT NEWID(),DocumentDetail.Oid," + (int)eDiscountSource.CUSTOM + "," + (int)eDiscountType.PERCENTAGE
                               + @",1,DocumentDetail.SecondDiscount,
                                (DocumentDetail.NetTotal - ( DocumentDetail.FirstDiscount*DocumentDetail.NetTotal ) ) *  DocumentDetail.SecondDiscount,
                                DocumentDetail.CreatedOnTicks,DocumentDetail.UpdatedOnTicks
                          FROM DocumentDetail 
                                join DocumentHeader on DocumentHeader.Oid = DocumentDetail.DocumentHeader 
                                join DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                          WHERE DocumentType.IsForWholesale = 1 and DocumentDetail.SecondDiscount >0");

            Execute.Sql(@"INSERT INTO DocumentDetailDiscount (Oid,DocumentDetail,DiscountSource,DiscountType,Priority,[Percentage],Value,CreatedOnTicks,UpdatedOnTicks) 
                          SELECT NEWID(),DocumentDetail.Oid," + (int)eDiscountSource.CUSTOM + "," + (int)eDiscountType.PERCENTAGE
                                + @",1,DocumentDetail.SecondDiscount,
                                (DocumentDetail.GrossTotal - ( DocumentDetail.FirstDiscount*DocumentDetail.GrossTotal ) ) *  DocumentDetail.SecondDiscount ,
                                DocumentDetail.CreatedOnTicks,DocumentDetail.UpdatedOnTicks
                          FROM DocumentDetail 
                                join DocumentHeader on DocumentHeader.Oid = DocumentDetail.DocumentHeader 
                                join DocumentType on DocumentType.Oid=DocumentHeader.DocumentType
                          WHERE DocumentType.IsForWholesale = 0 and DocumentDetail.SecondDiscount >0");
        }



        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
