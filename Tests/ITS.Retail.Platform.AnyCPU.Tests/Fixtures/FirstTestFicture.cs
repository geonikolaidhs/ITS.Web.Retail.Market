using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.MigrationTool;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.Fixtures
{
    public class FirstTestFixture : Fixture
    {
        public UnitOfWork uow { get; set; }

        public DocumentHeader SampleDocumentHeader { get; set; }
        public Barcode SampleBarcode { get; set; }
        public Item SampleItem { get; set; }

        public DocumentHeader SampleCanceledDocument { get; set; }
        public DocumentHeader SampleExecutedDocument { get; set; }
        public DocumentHeader SampleTransformedDocument { get; set; }

        
        public DocumentHeader SampleAutoNumberedDocument { get; set; }

        public override void Initialize()
        {
            XpoHelper.databasetype = DBType.Memory;
            using (MainForm frm = new MainForm())
            {
                frm.InitializationRetailDual(eCultureInfo.Hellenic, false);
            }

            this.uow = XpoHelper.GetNewUnitOfWork();
            OwnerApplicationSettings oas = this.uow.FindObject<OwnerApplicationSettings>(null);
            oas.DoPadding = false;
            oas.Save();
            this.uow.CommitChanges();
            XPCollection<Customer> customers = new XPCollection<Customer>(this.uow);
            Customer customer = customers.First();
            //Assert.NotNull(customer.Trader);
            //Assert.Equal(customer.Trader.Addresses.Count, 1);
            XPCollection<DocumentType> documentTypes = new XPCollection<DocumentType>(this.uow);
            IEnumerable<DocumentType> wholeSaleDoctypes = documentTypes.Where(x => x.IsForWholesale);
            DocumentType docType = wholeSaleDoctypes.First();
            Store store = this.uow.FindObject<Store>(null);
            DocumentSeries series = store.DocumentSeries.First(g => g.StoreDocumentSeriesTypes.Where(x => x.DocumentType == docType).Count() != 0);
            DocumentStatus statusThatTakesSequence = this.uow.FindObject<DocumentStatus>(new BinaryOperator("TakeSequence", true));

            this.SampleItem = new Item(this.uow);
            this.SampleItem.Code = "1";
            this.SampleItem.Name = "11";
            this.SampleItem.VatCategory = this.uow.FindObject<VatCategory>(new BinaryOperator("Code", "%2%", BinaryOperatorType.Like));
            this.SampleItem.Owner = store.Owner;
            this.SampleItem.Save();

            this.uow.CommitChanges();

            this.SampleBarcode = new Barcode(this.uow);
            this.SampleBarcode.Code = "1";
            this.SampleBarcode.Save();
            this.uow.CommitChanges();

            ItemBarcode ibc = new ItemBarcode(this.uow);
            ibc.Owner = store.Owner;
            ibc.Item = this.SampleItem;
            ibc.Barcode = this.SampleBarcode;
            ibc.Save();
            this.SampleItem.Save();
            this.uow.CommitChanges();

            PriceCatalogDetail pcd = new PriceCatalogDetail(this.uow);
            pcd.Barcode = this.SampleBarcode;
            pcd.Item = this.SampleItem;
            pcd.Value = 1.00;
            pcd.VATIncluded = false;
            pcd.PriceCatalog = customer.GetDefaultPriceCatalog();
            pcd.Save();
            this.uow.CommitChanges();

            this.SampleDocumentHeader = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.First(),
                Customer = customer,
                Division = eDivision.Sales,
                DocumentType = docType,
                DocumentSeries = series,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog()
            };

            this.SampleCanceledDocument = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.First(),
                IsCanceled = true,
                DocumentType = docType,
                DocumentSeries = series,
                Division = eDivision.Sales,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog()
            };

            this.SampleExecutedDocument = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.First(),
                HasBeenExecuted = true,
                DocumentType = docType,
                DocumentSeries = series,
                Division = eDivision.Sales,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog()
            };

            this.SampleTransformedDocument = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.First(),
                DocumentType = docType,
                DocumentSeries = series,
                Division = eDivision.Sales,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog()
            };
            //TODO transform the right way
            this.SampleTransformedDocument.DerivedDocuments.Add(new RelativeDocument(this.uow) { DerivedDocument = this.SampleDocumentHeader });

            this.SampleAutoNumberedDocument = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.First(),
                DocumentType = docType,
                DocumentSeries = series,
                Division = eDivision.Sales,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog(),
                DocumentNumber = 10,
                Status = statusThatTakesSequence
            };
            this.uow.CommitChanges();
        }
    }

}
