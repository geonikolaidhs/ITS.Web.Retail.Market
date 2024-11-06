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

    public class FirstTest //: IUseFixture<FirstTestFicture>
    {

        public static IEnumerable<object[]> ExpectedTypesCount
        {
            get
            {
                return (new Dictionary<Type, int>()
                {
                    {typeof(Customer), 1},                    
                    {typeof(CompanyNew), 1},
                    {typeof(User), 5},
                    {typeof(VatLevel), 2},
                    {typeof(VatCategory), 4},
                    {typeof(VatFactor), 7},
                    {typeof(SpecialItem), 2},
                    {typeof(DocumentType), 6},
                    {typeof(DocumentSeries), 1},
                    {typeof(Store), 1},
                    {typeof(StoreDocumentSeriesType), 2},
                    {typeof(Role), 5},
                    {typeof(MinistryDocumentType), 108},
                    {typeof(DocumentStatus), 2}
                }).Select(g => new object[] { g.Key, g.Value });
            }

        }

        [Theory, PropertyDataWithFixture("ExpectedTypesCount", "ITS.Retail.Platform.Tests.Fixtures.FirstTestFixture")]
        public void CheckInitializationScript(Type type, int expectedValue)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection col = new XPCollection(uow, type);
                Assert.Equal(col.Count, expectedValue);
            }
        }


        public static IEnumerable<object[]> ExpectedDocumentLines
        {
            get
            {
                FirstTestFixture _ficture = Fixture.Current as FirstTestFixture;

                return new[]{
                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,1,-1,false,0,0,1.00, 1.23,1.00, 1.23,0.23 },
                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,10,-1,false,0,0,1.00, 1.23,10.00, 12.3,2.3 },
                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,10,-1,false,0.1,0.1,1.00, 1.23,8.10, 9.96,1.86 },

                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,1,2,true,0,0,2.00, 1.23*2,1.00*2, 1.23*2,0.23*2 },
                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,10,2,true,0,0,2.00, 1.23*2,10.00*2, 12.3*2,2.3*2 },
                    new object[]{_ficture.SampleItem, _ficture.SampleBarcode,10,2,true,0.1,0.1,2.00, 1.23*2,16.20, 19.93,3.73 }

                };
            }
        }

        [Theory, PropertyDataWithFixture("ExpectedDocumentLines", "ITS.Retail.Platform.Tests.Fixtures.FirstTestFixture")]//PropertyData("ExpectedDocumentLines")]
        public void CheckComputeDocumentLine(Item item, Barcode barcode, double qty, double unitPrice, bool hasCustomPrice, double firstDiscount, double secondDiscount
            , double expectedUnitPrice, double expectedFinalUnitPrice, double expectedNetTotal, double expectedGrossTotal, double expectedTotalVatAmount)
        {
            FirstTestFixture _ficture = Fixture.Current as FirstTestFixture;
            //var v = Assert.Throws<DocumentDetailException>(()=> BODocumentDetail.ComputeDocumentLine(ref documentHeader, null, null, 10, false, -1, false, null, 0, 0));
            DocumentHeader header = _ficture.SampleDocumentHeader;
            DocumentDetail det = BODocumentDetail.ComputeDocumentLine(ref header, item, barcode, qty, false, unitPrice, hasCustomPrice, null, firstDiscount, secondDiscount);
            Assert.Equal(det.Barcode, barcode);
            Assert.Equal(det.Item, item);
            if (hasCustomPrice)
            {
                Assert.Equal(det.CustomUnitPrice, unitPrice);
            }
            Assert.Equal(det.FinalUnitPrice, expectedFinalUnitPrice);
            Assert.Equal(det.UnitPrice, expectedUnitPrice);
            Assert.Equal(det.NetTotal, expectedNetTotal);
            Assert.Equal(det.GrossTotal, expectedGrossTotal);
            Assert.Equal(det.TotalVatAmount, expectedTotalVatAmount);

        }


        public static IEnumerable<object[]> DocumentCanBeDeletedInput
        {
            get
            {
                FirstTestFixture _ficture = Fixture.Current as FirstTestFixture;

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

        [Theory, PropertyDataWithFixture("DocumentCanBeDeletedInput", "ITS.Retail.Platform.Tests.Fixtures.FirstTestFixture")]
        public void CheckDocumentHelperDocumentCanBeDeleted(Guid documentId,bool expectedResult,string expectedReason)
        {
            string reason;
            bool result = DocumentHelper.DocumentCanBeDeleted(documentId, out reason);

            Assert.Equal(expectedReason, reason);
            Assert.Equal(expectedResult, result);
        }


    }
}
