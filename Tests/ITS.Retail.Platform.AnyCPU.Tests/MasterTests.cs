using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.BussinesLayer;
using ITS.Retail.Common;
using ITS.Retail.MigrationTool;
using ITS.Retail.Model;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Controllers;
using Xunit;
using Xunit.Extensions;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Tests.Common;
using ITS.Retail.Platform.Tests.Fixtures;


namespace ITS.Retail.Platform.Tests
{

    public class MasterTests 
    {

        public static IEnumerable<object[]> ExpectedDocumentLines
        {
            get
            {
                MasterTestsFixture _ficture = Fixture.Current as MasterTestsFixture;
                UnitOfWork uow = _ficture.uow;

                List<DocumentDetailDiscount> discountsTenPercentDiscardOther = new List<DocumentDetailDiscount>();
                discountsTenPercentDiscardOther.Add(new DocumentDetailDiscount(uow) { DiscardsOtherDiscounts = true, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.10m });
                discountsTenPercentDiscardOther.Add(new DocumentDetailDiscount(uow) { DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.10m });

                List<DocumentDetailDiscount> discounts5PercentThen10Percent = new List<DocumentDetailDiscount>();
                discounts5PercentThen10Percent.Add(new DocumentDetailDiscount(uow) { Priority = 1, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.05m });
                discounts5PercentThen10Percent.Add(new DocumentDetailDiscount(uow) { Priority = 2, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.10m });

                List<DocumentDetailDiscount> discounts5PercentThen10Flat = new List<DocumentDetailDiscount>();
                discounts5PercentThen10Flat.Add(new DocumentDetailDiscount(uow) { Priority = 1, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.05m });
                discounts5PercentThen10Flat.Add(new DocumentDetailDiscount(uow) { Priority = 2, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.VALUE, Value = 10 });

                List<DocumentDetailDiscount> discountsPriceCatalog10PercentThen5PercentThen10Flat = new List<DocumentDetailDiscount>();
                discountsPriceCatalog10PercentThen5PercentThen10Flat.Add(new DocumentDetailDiscount(uow) { Priority = -1, DiscountSource = eDiscountSource.PRICE_CATALOG, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.10m });
                discountsPriceCatalog10PercentThen5PercentThen10Flat.Add(new DocumentDetailDiscount(uow) { Priority = 1, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.PERCENTAGE, Percentage = 0.05m });
                discountsPriceCatalog10PercentThen5PercentThen10Flat.Add(new DocumentDetailDiscount(uow) { Priority = 2, DiscountSource = eDiscountSource.CUSTOM, DiscountType = eDiscountType.VALUE, Value = 10 });



                return new[]{
                    //With discounts / Wholesale -----
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,10m,true,10m, 12.3m, 9m , 11.07m,2.07m,discountsTenPercentDiscardOther },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,10m,true,10m, 12.3m, 18m , 22.14m,4.14m,discountsTenPercentDiscardOther },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,10m,true,10m, 12.3m, 8.55m, 10.52m,1.97m,discounts5PercentThen10Percent },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,10m,true,10m, 12.3m, 9m, 11.07m,2.07m,discounts5PercentThen10Flat },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,10m,true,10m, 12.3m, 7.1m, 8.73m,1.63m,discountsPriceCatalog10PercentThen5PercentThen10Flat },

                    //With discounts / Retail -----
                    new object[]{_ficture.SampleRetailDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,12.3m,true,10m, 12.3m, 9m , 11.07m,2.07m,discountsTenPercentDiscardOther },
                    new object[]{_ficture.SampleRetailDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,12.3m,true,10m, 12.3m, 18m , 22.14m,4.14m,discountsTenPercentDiscardOther },
                    new object[]{_ficture.SampleRetailDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,12.3m,true,10m, 12.3m, 8.55m, 10.52m,1.97m,discounts5PercentThen10Percent },
                    new object[]{_ficture.SampleRetailDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,12.3m,true,10m, 12.3m, 10.87m, 13.37m,2.5m,discounts5PercentThen10Flat },
                    new object[]{_ficture.SampleRetailDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,2m,12.3m,true,10m, 12.3m,8.97m,11.03m,2.06m,discountsPriceCatalog10PercentThen5PercentThen10Flat },
                    //--------------------

                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,-1m,false,1.00m, 1.23m,1.00m, 1.23m,0.23m,null },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,10m,-1m,false,1.00m, 1.23m,10.00m, 12.3m,2.3m,null },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,1m,2m,true,2.00m, 1.23m*2,1.00m*2, 1.23m*2,0.23m*2,null },
                    new object[]{_ficture.SampleDocumentHeader,_ficture.SampleItem, _ficture.SampleBarcode,10m,2m,true,2.00m, 1.23m*2,10.00m*2, 12.3m*2,2.3m*2,null }

                };
            }
        }

        //TODO REWRITE


        [Theory, PropertyDataWithFixture("ExpectedDocumentLines", "ITS.Retail.Platform.Tests.Fixtures.MasterTestsFixture")]//PropertyData("ExpectedDocumentLines")]
        public void CheckComputeDocumentLine(DocumentHeader header, Item item, Barcode barcode, decimal qty, decimal unitPrice, bool hasCustomPrice,
            decimal expectedUnitPrice, decimal expectedFinalUnitPrice, decimal expectedNetTotal, decimal expectedGrossTotal,
            decimal expectedTotalVatAmount, List<DocumentDetailDiscount> discounts)
        {
            MasterTestsFixture _ficture = Fixture.Current as MasterTestsFixture;
            //var v = Assert.Throws<DocumentDetailException>(()=> BODocumentDetail.ComputeDocumentLine(ref documentHeader, null, null, 10, false, -1, false, null, 0, 0));
            DocumentDetail det = BODocumentDetail.ComputeDocumentLine(ref header, item, barcode, qty, false, unitPrice, hasCustomPrice, null, discounts);
            Assert.Equal(det.Barcode, barcode);
            Assert.Equal(det.Item, item);
            if (hasCustomPrice)
            {
                Assert.Equal(det.CustomUnitPrice, unitPrice);
            }
            Assert.Equal(expectedFinalUnitPrice, det.FinalUnitPrice);
            Assert.Equal(expectedUnitPrice,det.UnitPrice);
            Assert.Equal(expectedNetTotal,det.NetTotal);
            Assert.Equal(expectedGrossTotal,det.GrossTotal);
            Assert.Equal(expectedTotalVatAmount,det.TotalVatAmount);
            //((UnitOfWork)det.Session).CommitChanges();
        }


        public static IEnumerable<object[]> DocumentCanBeDeletedInput
        {
            get
            {
                MasterTestsFixture _ficture = Fixture.Current as MasterTestsFixture;

                return new[]
                {
                    new object[]{_ficture.SampleDocumentHeader.Oid,true,""},
                    new object[]{_ficture.SampleCanceledDocument.Oid,false,Resources.IsCanceled},
                    new object[]{_ficture.SampleExecutedDocument.Oid,false,Resources.HasBeenExecuted},
                    new object[]{_ficture.SampleTransformedDocument.Oid,false,Resources.HasBeenTransformed},
                    new object[]{_ficture.SampleAutoNumberedDocument.Oid,false,Resources.HasBeenAutoNumbered}
                    //new object[]{Guid.Parse(""), false, Resources.HasBeenAutoNumbered}
                };
            }
        }

        [Theory, PropertyDataWithFixture("DocumentCanBeDeletedInput", "ITS.Retail.Platform.Tests.Fixtures.MasterTestsFixture")]
        public void CheckDocumentHelperDocumentCanBeDeleted(Guid documentId,bool expectedResult,string expectedReason)
        {
            string reason;
            bool result = DocumentHelper.DocumentCanBeDeleted(documentId, out reason);

            Assert.Equal(expectedReason, reason);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CheckPOSUpdateService()
        {
            MasterTestsFixture ficture = Fixture.Current as MasterTestsFixture;

            //TODO: Test the following methods
            
            //service.GetChanges(typeof();
            //service.GetDocumentSequence
            //service.GetPOSVersion();
            //service.GetVersion();
            //service.PostData
            
        }


    }
}
