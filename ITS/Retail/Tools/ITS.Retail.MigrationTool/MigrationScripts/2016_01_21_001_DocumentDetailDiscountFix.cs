using FluentMigrator;
using ITS.Retail.MigrationTool.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.MigrationTool.MigrationScripts
{
    [RetailMigration(author: "akm", year: 2016, month: 01, day: 21, order: 1, version: "2.3.2.17")]
    public class DocumentDetailDiscountFix: Migration
    {
        public override void Up()
        {
            List<string> checkDocumentDetailTableColumnsExistance = new List<string>()
            { 
                {"TotalDiscountAmountWithVAT"},
                {"TotalDiscountAmountWithoutVAT"}                
            };

            List<string> checkDocumentDetailDiscountTableColumnsExistance = new List<string>()
            { 
                {"DiscountWithVAT"},
                {"DiscountWithoutVAT"}                
            };


            foreach (string column in checkDocumentDetailTableColumnsExistance)
            {
                if (Schema.Table("DocumentDetail").Column(column).Exists() == false)
                {
                    IfDatabase("sqlserver").Create.Column(column).OnTable("DocumentDetail").AsCurrency().Nullable();
                    IfDatabase("postgres").Create.Column(column).OnTable("DocumentDetail").AsDecimal(28,8).Nullable();
                }
            }

            foreach (string column in checkDocumentDetailDiscountTableColumnsExistance)
            {
                if (Schema.Table("DocumentDetailDiscount").Column(column).Exists() == false)
                {
                    IfDatabase("sqlserver").Create.Column(column).OnTable("DocumentDetailDiscount").AsCurrency().Nullable();
                    IfDatabase("postgres").Create.Column(column).OnTable("DocumentDetailDiscount").AsDecimal(28, 8).Nullable();
                }
            }

            IfDatabase("postgres").Execute.Sql(@" 
                -- set TotalDiscountAmountWithVAT and TotalDiscountAmountWithoutVAT from grosstotal,GrossTotalBeforeDiscount,NetTotal,NetTotalBeforeDiscount
                Update ""DocumentDetail"" 
                            set ""TotalDiscountAmountWithVAT"" = round(""GrossTotalBeforeDiscount"" ,2)- ""GrossTotal"" ,
                            ""TotalDiscountAmountWithoutVAT"" = round(""NetTotalBeforeDiscount"",2) - ""NetTotal"";

                -- set DiscountWithVAT  = Value if Totaldiscount in documentdetail.TotalDiscount == TotalDiscountAmountWithVAT
                update ""DocumentDetailDiscount"" 
                    set ""DiscountWithVAT"" = ""Value""
                    where ""DocumentDetail"" in 
                    (select ""Oid"" from ""DocumentDetail"" where ""TotalDiscount""<>0 and abs(""GrossTotalBeforeDiscount""-""GrossTotal""- ""TotalDiscount"") <=0.01);

                -- set DiscountWithoutVAT = value if Totaldiscount in documentdetail.TotalDiscount == TotalDiscountAmountWithoutVAT
                update ""DocumentDetailDiscount"" set ""DiscountWithoutVAT"" = ""Value""
                        where ""DocumentDetail"" in
                    (select ""Oid"" from ""DocumentDetail"" where ""TotalDiscount""<>0 and abs(""NetTotalBeforeDiscount""-""NetTotal"" - ""TotalDiscount"") <= 0.01 );

                -- set DiscountWithoutVAT = DocumentDetailDiscount.TotalDiscountAmountWithoutVAT if Value = DiscountWithVAT
                update ""DocumentDetailDiscount"" set 
                    ""DiscountWithoutVAT"" = dt.""TotalDiscountAmountWithoutVAT""
                from ""DocumentDetail"" dt inner join ""DocumentDetailDiscount"" disc
                    on dt.""Oid"" = disc.""DocumentDetail""
                where abs (dt.""TotalDiscount"" - disc.""Value"") <=0.01 and disc.""Value"" = disc.""DiscountWithVAT""
                    and disc.""Oid"" = ""DocumentDetailDiscount"".""Oid""
                    and disc.""DiscountWithoutVAT"" is null;

                 -- set DiscountWithVAT = DocumentDetailDiscount.TotalDiscountAmountWithVAT if Value = DiscountWithoutVAT
                update ""DocumentDetailDiscount"" set 
                    ""DiscountWithVAT"" = dt.""TotalDiscountAmountWithVAT"" 
                from ""DocumentDetail"" dt inner join ""DocumentDetailDiscount"" disc
                    on dt.""Oid"" = disc.""DocumentDetail""
                where abs (dt.""TotalDiscount"" - disc.""Value"") <=0.01 and disc.""Value"" = disc.""DiscountWithoutVAT""
                    and disc.""Oid"" = ""DocumentDetailDiscount"".""Oid""
                    and disc.""DiscountWithVAT"" is null;


		        -- handle zero discounts
                update ""DocumentDetailDiscount"" set ""DiscountWithoutVAT"" = 0, ""DiscountWithVAT""=0 where round(""Value"",2)=0;

                -- handle multiple discounts per line 
                -- WARNING: Rounding problems may occur
                update ""DocumentDetailDiscount""
                    set ""DiscountWithoutVAT"" = a.""DiscountWithVAT"" /(1+""VatFactor"") 
                    from ""DocumentDetail""
                    inner join ""DocumentDetailDiscount"" a on a.""DocumentDetail"" = ""DocumentDetail"".""Oid""
                    where (""DocumentDetailDiscount"".""DiscountWithoutVAT"" is null or  ""DocumentDetailDiscount"".""DiscountWithVAT"" is null)
                    and a.""Oid"" = ""DocumentDetailDiscount"".""Oid""
                    and a.""DiscountWithVAT"" >0
             ");

            IfDatabase("sqlserver").Execute.Sql(@"
                -- To Check if required    
                -- update DocumentDetail set GrossTotalBeforeDiscount = GrossTotal where GrossTotalBeforeDiscount = 0 and NetTotalBeforeDiscount=NetTotal;
                
                -- set TotalDiscountAmountWithVAT and TotalDiscountAmountWithoutVAT from grosstotal,GrossTotalBeforeDiscount,NetTotal,NetTotalBeforeDiscount
                Update DocumentDetail 
                            set TotalDiscountAmountWithVAT = round(GrossTotalBeforeDiscount ,2)- GrossTotal ,
                            TotalDiscountAmountWithoutVAT = round(NetTotalBeforeDiscount,2) - NetTotal;

                -- set DiscountWithVAT  = Value if Totaldiscount in documentdetail.TotalDiscount == TotalDiscountAmountWithVAT
                update DocumentDetailDiscount 
                            set DiscountWithVAT = Value
                where DocumentDetail in (select oid from DocumentDetail where TotalDiscount<>0 and abs(GrossTotalBeforeDiscount-GrossTotal- TotalDiscount) <=0.01);

                -- set DiscountWithoutVAT = value if Totaldiscount in documentdetail.TotalDiscount == TotalDiscountAmountWithoutVAT
                update DocumentDetailDiscount set DiscountWithoutVAT = Value
                        where DocumentDetail in
                    (select oid from DocumentDetail where TotalDiscount<>0 and abs(NetTotalBeforeDiscount-NetTotal - TotalDiscount) <= 0.01 );

                -- set DiscountWithoutVAT = DocumentDetailDiscount.TotalDiscountAmountWithoutVAT if Value = DiscountWithVAT
                update disc set 
                    disc.DiscountWithoutVAT = dt.TotalDiscountAmountWithoutVAT 
                from DocumentDetail dt inner join DocumentDetailDiscount disc
                    on dt.oid = disc.DocumentDetail
                where abs (dt.TotalDiscount - disc.value) <=0.01 and disc.value = disc.DiscountWithVAT
                    and disc.DiscountWithoutVAT is null;

                -- set DiscountWithVAT = DocumentDetailDiscount.TotalDiscountAmountWithVAT if Value = DiscountWithoutVAT
                update disc set 
                    disc.DiscountWithVAT = dt.TotalDiscountAmountWithVAT 
                from DocumentDetail dt inner join DocumentDetailDiscount disc
                    on dt.oid = disc.DocumentDetail
                where abs (dt.TotalDiscount - disc.value) <=0.01 and disc.value = disc.DiscountWithoutVAT
                    and disc.DiscountWithVAT is null;

                -- handle zero discounts
                update DocumentDetailDiscount set DiscountWithoutVAT = 0, DiscountWithVAT=0 where round(value,2)=0;

                -- handle multiple discounts per line 
                -- WARNING: Rounding problems may occur
                update documentdetaildiscount
                    set DiscountWithoutVAT = DiscountWithVAT /(1+VatFactor) 
                    from DocumentDetail
                    inner join DocumentDetailDiscount on DocumentDetailDiscount.DocumentDetail = DocumentDetail.Oid
                    where (DocumentDetailDiscount.DiscountWithoutVAT is null or  DocumentDetailDiscount.DiscountWithVAT is null)
                    and DiscountWithVAT >0

            ");

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

    }
}
