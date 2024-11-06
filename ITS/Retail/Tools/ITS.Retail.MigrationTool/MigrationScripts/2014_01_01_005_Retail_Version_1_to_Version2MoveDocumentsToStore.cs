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
using System.Dynamic;

namespace MigrationTool.MigrationScripts
{

    

    //[Migration(20130404)]
    [RetailMigration(author: "kdk", year: 2014, month: 1, day: 1, order: 5, version: "2.0.0.81")]
    public class Retail_Version_1_to_Version2MoveDocumentsToStore: Migration
    {        

        public override void Up()
        {
            //Item1 = Table
            //Item2 = Column
            //Item3 = Type
            //Item4 = IsCustomType
            List<Tuple<string, string,string,bool>> checkTableColumnsExistance = new List<Tuple<string, string,string,bool>>{
                          Tuple.Create("DocumentSeries","HasAutomaticNumbering",typeof(bool).ToString(),false),
                          Tuple.Create("DocumentSeries","MasterObjOid",typeof(Guid).ToString(),false),
                          Tuple.Create("DocumentSeries","Owner",typeof(Guid).ToString(),false),
                          Tuple.Create("DocumentSeries","Remarks","nvarchar(max)",true),
                          Tuple.Create("DocumentSeries","PrintedCode",typeof(string).ToString(),false),
                          Tuple.Create("DocumentSeries","Store",typeof(Guid).ToString(),false),
                          Tuple.Create("DocumentSeries","IsCancelingSeries",typeof(bool).ToString(),false),
                          Tuple.Create("DocumentSeries","IsCanceledBy",typeof(Guid).ToString(),false),
                          
                          
                          Tuple.Create("DocumentSequence","MasterObjOid",typeof(Guid).ToString(),false),
                          
                          Tuple.Create("StoreDocumentSeriesType","MasterObjOid",typeof(Guid).ToString(),false),
                          Tuple.Create("StoreDocumentSeriesType","DocumentSeries",typeof(Guid).ToString(),false),
                          Tuple.Create("StoreDocumentSeriesType","IsForPOS",typeof(bool).ToString(),false),
                          Tuple.Create("StoreDocumentSeriesType","Owner",typeof(Guid).ToString(),false),
                          
                          Tuple.Create("DocumentHeader","Remarks","nvarchar(max)",true),
                          

                          Tuple.Create("DocumentType","IsForWholesale",typeof(bool).ToString(),false)
                          //Tuple.Create("","",typeof().ToString(),false),
            };

            //Item1 = Table
            //Item2 = Column
            //Item3 = Type
            //Item4 = IsCustomType
            foreach(Tuple<string, string,string,bool> tuple in checkTableColumnsExistance)
            {
                if (Schema.Table(tuple.Item1).Column(tuple.Item2).Exists() == false)
                {
                    if (tuple.Item4)
                    {
                        Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsCustom(tuple.Item3).Nullable();
                    }
                    else
                    {
                        switch(tuple.Item3){
                            case "System.Guid":
                                    Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsGuid().Nullable();
                                break;
                            case "System.Boolean":
                                Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsBoolean().Nullable();//.WithDefaultValue(defaultBooleanValue);                                
                                break;
                            case "System.String":
                                Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsString().Nullable().WithDefaultValue(String.Empty);
                                break;
                            case "System.Decimal":
                                Alter.Table(tuple.Item1).AddColumn(tuple.Item2).AsDecimal();
                                break;
                            default:
                                throw new Exception("Please check your settings");
                        }                        
                    }
                }
            }
            //bool defaultBooleanValue = tuple.Item2 == "HasAutomaticNumbering" || tuple.Item2 == "IsForWholesale";
            //Tuple.Create("DocumentSeries","HasAutomaticNumbering",typeof(bool).ToString(),false),
            Update.Table("DocumentSeries").Set(new { HasAutomaticNumbering = true }).Where(new { HasAutomaticNumbering = (bool?)null });
            Update.Table("DocumentType").Set(new { IsForWholesale = true }).Where(new { IsForWholesale = (bool?)null }); ;



            string StoresWithMissingDataTable = "StoresWithMissingData";

            string declareCursor = @" DECLARE @StoreOid uniqueidentifier;
                                      DECLARE @StoreName nvarchar(100);
                                      DECLARE @Owner uniqueidentifier;

                                      DECLARE @CurrentSeries uniqueidentifier;
                                      DECLARE @CurrentSequence uniqueidentifier;
                                      DECLARE @CurrentDocumentType uniqueidentifier;
                                      SELECT @CurrentDocumentType = Oid FROM DocumentType ORDER BY CreatedOnTicks DESC;-- DESC ώστε να μείνει το τελευταίο από το select δηλαδή το πρώτο χρονολογικά...

                                      DECLARE @Division uniqueidentifier;
                                      SELECT  @Division =  Oid FROM  Division WHERE Section = 0 ;

                                      DECLARE @DocumentHeaderOid uniqueidentifier;
                                      DECLARE @DocumentNumber int;
                                      DECLARE @NewDocumentNumber int;
                                      DECLARE @Remarks nvarchar(max);

                                      DECLARE storesCursor CURSOR FOR  
                                      SELECT StoreOid, StoreName , [Owner]
                                      FROM " + StoresWithMissingDataTable + ";"+
                                      @" 
                                      
                                      OPEN storesCursor   
                                      FETCH NEXT FROM storesCursor INTO @StoreOid, @StoreName , @Owner   
                                      
                                      WHILE @@FETCH_STATUS = 0
                                      BEGIN   
                                             {0}
                                      
                                             FETCH NEXT FROM storesCursor INTO @StoreOid, @StoreName , @Owner
                                      END   
                                      
                                      CLOSE storesCursor   
                                      DEALLOCATE storesCursor";

            string documentHeaderCursor = @"

                                      DECLARE documentHeaderCursor CURSOR FOR  
                                      SELECT Oid, DocumentNumber, Remarks
                                      FROM DocumentHeader
                                      WHERE Store = @StoreOid
                                      AND Division = 0
                                      -- AND DocumentType = ''
                                      AND DocumentSeries in (select Oid from DocumentSeries where [Owner] is null or Store is null);
                                      
                                      OPEN documentHeaderCursor   
                                      FETCH NEXT FROM documentHeaderCursor INTO @DocumentHeaderOid, @DocumentNumber, @Remarks
                                      
                                      WHILE @@FETCH_STATUS = 0   
                                      BEGIN   
                                                 IF @DocumentNumber  <=0
                                                     BEGIN
                                                        UPDATE DocumentHeader
                                                                SET DocumentType = @CurrentDocumentType,
                                                                    DocumentSeries = @CurrentSeries,
                                                                    Remarks = @Remarks+ 'Μεταφορά από το παραστατικό με αριθμό: '+ CONVERT(nvarchar(100),@DocumentNumber)
                                                                WHERE Oid = @DocumentHeaderOid;
                                                       END
                                                  ELSE
                                                        BEGIN
                                                            SELECT @NewDocumentNumber = DocumentNumber+1 from DocumentSequence WHERE Oid = @CurrentSequence;
                                                        
                                                            UPDATE DocumentHeader
                                                                    SET DocumentType = @CurrentDocumentType,
                                                                        DocumentSeries = @CurrentSeries,
                                                                        DocumentNumber = @NewDocumentNumber,
                                                                        Remarks = @Remarks+ 'Μεταφορά από το παραστατικό με αριθμό: '+ CONVERT(nvarchar(100),@DocumentNumber)
                                                                    WHERE Oid = @DocumentHeaderOid;

                                                            UPDATE DocumentSequence
                                                                   SET DocumentNumber = @NewDocumentNumber
                                                                   WHERE Oid = @CurrentSequence;

                                                        END
                                      
                                             FETCH NEXT FROM documentHeaderCursor INTO @DocumentHeaderOid, @DocumentNumber, @Remarks
                                      END   
                                      
                                      CLOSE documentHeaderCursor   
                                      DEALLOCATE documentHeaderCursor";

            long nowTicks = DateTime.Now.Ticks;
                        
            Execute.Sql("UPDATE DocumentHeader SET Remarks = '' WHERE Remarks is null or Remarks='null';");
            Alter.Table("DocumentHeader").AlterColumn("Remarks").AsString(int.MaxValue).WithDefaultValue(" ");//AsCustom("nvarchar(max)")

//Step 1 Create a Series for each Store that has no series
            Execute.Sql(@"SELECT  Store.Oid as StoreOid, Store.Name as StoreName , Store.[Owner] as Owner 
                        INTO " + StoresWithMissingDataTable + @"
                        FROM Store
                        -- LEFT JOIN DocumentSeries ON Store.Oid = DocumentSeries.Store
                        -- WHERE DocumentSeries.Store is null;"
                       );

            //μια ομορφιά
            string Query = @"         SET @CurrentSeries = NEWID();
                                      SET @CurrentSequence = NEWID();                                      

                                        INSERT INTO DocumentSeries(
                                       Oid,
                                       CreatedOnTicks,
                                       UpdatedOnTicks,
                                       CreatedBy,
                                       UpdatedBy,
                                       CreatedByDevice,
                                       UpdateByDevice,
                                       RowDeleted,
                                       IsActive,
                                       IsSynchronized,
                                       Description,
                                       IsDefault,
                                       Code,
                                       OptimisticLockField,
                                       GCRecord,
                                       MLValues,
                                       HasAutomaticNumbering,
                                       MasterObjOid,
                                       Owner,
                                       Remarks,
                                       PrintedCode,
                                       Store,
                                       IsCancelingSeries,
                                       IsCanceledBy
                                      )

                                       VALUES
                                       (
                                            @CurrentSeries, -- 'Oid',
                                            " + nowTicks.ToString() + @", -- 'CreatedOnTicks',
                                            " + nowTicks.ToString() + @", -- 'UpdatedOnTicks',
                                            null, -- 'CreatedBy',
                                            null, -- 'UpdatedBy',
                                             null, -- 'CreatedByDevice',
                                             null, -- 'UpdateByDevice',
                                             0, -- 'RowDeleted',
                                             1, -- 'IsActive',
                                             1, -- 'IsSynchronized',
                                             'Σειρά 1 '+@StoreName, -- 'Description',
                                             0, -- 'IsDefault',
                                             'Σειρά 1 '+@StoreName, -- 'Code',
                                            0, -- 'OptimisticLockField',
                                            null, -- 'GCRecord',
                                            null, -- 'MLValues',
                                            1, -- 'HasAutomaticNumbering',
                                            null, -- 'MasterObjOid',
                                            @Owner, -- 'Owner',
                                            null, -- 'Remarks',
                                            null, -- 'PrintedCode',
                                             @StoreOid, -- 'Store',
                                            0, -- 'IsCancelingSeries',
                                            null -- 'IsCanceledBy'
                                        );


-- Step 2 Create a Document Sequence for each Series

                                        INSERT INTO DocumentSequence(
                                                                        Oid,
                                                                        CreatedOnTicks,
                                                                        UpdatedOnTicks,
                                                                        CreatedBy,
                                                                        UpdatedBy,
                                                                        CreatedByDevice,
                                                                        UpdateByDevice,
                                                                        RowDeleted,
                                                                        IsActive,
                                                                        IsSynchronized,
                                                                        Description,
                                                                        IsDefault,
                                                                        DocumentSeries,
                                                                        DocumentNumber,
                                                                        OptimisticLockField,
                                                                        GCRecord,
                                                                        MLValues,
                                                                        MasterObjOid
                                                                    )
                                                          VALUES(
                                                                       @CurrentSequence, --  'Oid',
                                                                       " + nowTicks.ToString() + @", --  'CreatedOnTicks',
                                                                       " + nowTicks.ToString() + @", --  'UpdatedOnTicks',
                                                                       null, --  'CreatedBy',
                                                                       null, --  'UpdatedBy',
                                                                       null, --  'CreatedByDevice',
                                                                       null, --  'UpdateByDevice',
                                                                       0, --  'RowDeleted',
                                                                       1, --  'IsActive',
                                                                       1, --  'IsSynchronized',
                                                                       null, --  'Description',
                                                                       0, --  'IsDefault',
                                                                       @CurrentSeries, --  'DocumentSeries',
                                                                       0, --  'DocumentNumber',
                                                                       0, --  'OptimisticLockField',
                                                                       null, --  'GCRecord',
                                                                       null, --  'MLValues',
                                                                       null --  'MasterObjOid'
                                                                    );


-- Step 4 Connect the series and type of Steps 1 & 2 for each store
                                        
                                        INSERT INTO StoreDocumentSeriesType(
                                                                            Oid,
                                                                            CreatedOnTicks,
                                                                            UpdatedOnTicks,
                                                                            CreatedBy,
                                                                            UpdatedBy,
                                                                            CreatedByDevice,
                                                                            UpdateByDevice,
                                                                            RowDeleted,
                                                                            IsActive,
                                                                            IsSynchronized,
                                                                            StoreSeries,
                                                                            DocumentType,
                                                                            OptimisticLockField,
                                                                            GCRecord,
                                                                            MLValues,
                                                                            MasterObjOid,
                                                                            DocumentSeries,
                                                                            IsForPOS
                                        )
                                        VALUES(
                                                                   NEWID() , -- 'Oid',
                                                                   " + nowTicks.ToString() + @" , -- 'CreatedOnTicks',
                                                                   " + nowTicks.ToString() + @", -- 'UpdatedOnTicks',
                                                                   null, -- 'CreatedBy',
                                                                   null, -- 'UpdatedBy',
                                                                   null, -- 'CreatedByDevice',
                                                                   null, -- 'UpdateByDevice',
                                                                   0, -- 'RowDeleted',
                                                                   1, -- 'IsActive',
                                                                   1, -- 'IsSynchronized',
                                                                   null, -- 'StoreSeries',
                                                                   @CurrentDocumentType, -- 'DocumentType',
                                                                   0, -- 'OptimisticLockField',
                                                                   null, -- 'GCRecord',
                                                                   null, -- 'MLValues',
                                                                   null, -- 'MasterObjOid',
                                                                   @CurrentSeries, -- 'DocumentSeries',
                                                                   0 -- 'IsForPOS'
                                       );
-- Step 5 Tranfer All Documents from the old series to the newly created  TAKE CARE DocumentNumber!!!!Old DocumentNumber copied to Remarks field!!!                                      
                                      "
                                       +documentHeaderCursor;

            string Cursor = String.Format(declareCursor, Query);
            Execute.Sql(Cursor);

//Clear data
            Delete.Table(StoresWithMissingDataTable);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
