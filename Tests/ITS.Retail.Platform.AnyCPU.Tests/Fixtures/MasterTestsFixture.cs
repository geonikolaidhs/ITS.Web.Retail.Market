using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.MigrationTool;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Tests.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.Fixtures
{
    public class MasterTestsFixture : Fixture
    {
        public UnitOfWork uow { get; set; }

        public DocumentHeader SampleDocumentHeader { get; set; }
        public DocumentHeader SampleRetailDocumentHeader { get; set; }

        public Barcode SampleBarcode { get; set; }
        public Item SampleItem { get; set; }

        public DocumentHeader SampleCanceledDocument { get; set; }
        public DocumentHeader SampleExecutedDocument { get; set; }
        public DocumentHeader SampleTransformedDocument { get; set; }

        
        public DocumentHeader SampleAutoNumberedDocument { get; set; }

        public override void Initialize()
        {
            Trace.WriteLine("Start Importing Database");
            ProcessStartInfo processInfo = new ProcessStartInfo(TestSettings.MasterDatabaseInstallBat);
            Process process;
            processInfo.CreateNoWindow = true;
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //processInfo.UseShellExecute = false;
            //processInfo.RedirectStandardError = true;            
            //processInfo.RedirectStandardOutput = true;
            
            using (process = Process.Start(processInfo))
            {
                process.OutputDataReceived += (sender, args) => Trace.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Trace.WriteLine(args.Data);   
                process.WaitForExit();

            }

            Trace.WriteLine("End Importing Database");
            /*using( Process p = Process.Start(TestSettings.MasterDatabaseInstallBat))
            {
                p.WaitForExit();
            }*/
            XpoHelper.database = TestSettings.MasterDatabase;
            XpoHelper.databasetype = DBType.SQLServer;
            XpoHelper.sqlserver = TestSettings.SqlServer;
            XpoHelper.username = TestSettings.Username;
            XpoHelper.pass = TestSettings.Password;
            //XpoHelper.databasetype = DBType.Memory;
            using (MainForm frm = new MainForm())
            {
            //    frm.InitializationRetailDual(eCultureInfo.Hellenic, false);
                frm.UpgradeDatabase();
            }



            this.uow = XpoHelper.GetNewUnitOfWork();
            OwnerApplicationSettings oas = this.uow.FindObject<OwnerApplicationSettings>(null);
            oas.DoPadding = false;
            oas.Save();
            this.uow.CommitChanges();
            //XPCollection<Customer> customers = new XPCollection<Customer>(this.uow);
            Customer customer = this.uow.FindObject<Customer>(new BinaryOperator("Code","1"));
            //Assert.NotNull(customer.Trader);
            //Assert.Equal(customer.Trader.Addresses.Count, 1);
            XPCollection<DocumentType> documentTypes = new XPCollection<DocumentType>(this.uow);
            IEnumerable<DocumentType> wholeSaleDoctypes = documentTypes.Where(x => x.IsForWholesale);
            DocumentType docType = wholeSaleDoctypes.First();
            IEnumerable<DocumentType> retailSaleDoctypes = documentTypes.Where(x => x.IsForWholesale == false);
            DocumentType retailDocType = retailSaleDoctypes.First();
            Store store = this.uow.FindObject<Store>(null);
            DocumentSeries series = store.DocumentSeries.First(g => g.StoreDocumentSeriesTypes.Where(x => x.DocumentType == docType).Count() != 0);
            DocumentSeries retailSeries = store.DocumentSeries.First(g => g.StoreDocumentSeriesTypes.Where(x => x.DocumentType == retailDocType).Count() != 0);

            DocumentStatus statusThatTakesSequence = this.uow.FindObject<DocumentStatus>(new BinaryOperator("TakeSequence", true));

            this.SampleItem = new Item(this.uow);
            this.SampleItem.Code = "1";
            this.SampleItem.Name = "11";
            this.SampleItem.VatCategory = this.uow.FindObject<VatCategory>(new BinaryOperator("MinistryVatCategoryCode", eMinistryVatCategoryCode.C));
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
            pcd.Value = 1.00m;
            pcd.VATIncluded = false;
            pcd.PriceCatalog = customer.GetDefaultPriceCatalog();
            pcd.Save();
            this.uow.CommitChanges();

            this.SampleDocumentHeader = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.Where(x=>x.VatLevel != null && x.VatLevel.Code == "1").First(),
                Customer = customer,
                Division = eDivision.Sales,
                DocumentType = docType,
                DocumentSeries = series,
                Store = store,
                PriceCatalog = customer.GetDefaultPriceCatalog()
            };

            this.SampleRetailDocumentHeader = new DocumentHeader(this.uow)
            {
                BillingAddress = customer.Trader.Addresses.Where(x => x.VatLevel != null && x.VatLevel.Code == "1").First(),
                Customer = customer,
                Division = eDivision.Sales,
                DocumentType = retailDocType,
                DocumentSeries = retailSeries,
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

        public override void Dispose()
        {
            //TODO
        }
    }

}
