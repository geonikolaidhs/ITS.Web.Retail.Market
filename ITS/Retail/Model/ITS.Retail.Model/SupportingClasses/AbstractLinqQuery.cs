using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraReports.Parameters;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public abstract class AbstractLinqQuery : IDisposable
    {
        public DataBaseConnectionType DataBaseType { get; set; }
        public ParameterCollection Parameters { get; set; }
        public IQueryable<Item> ItemsQuery { get; protected set; }
        public IQueryable<ItemCategory> ItemCategoriesQuery { get; protected set; }
        public IQueryable<Customer> CustomersQuery { get; protected set; }
        public IQueryable<Trader> TradersQuery { get; protected set; }
        public IQueryable<CompanyNew> SuppliersQuery { get; protected set; }
        public IQueryable<DocumentHeader> DocumentHeadersQuery { get; protected set; }
        public IQueryable<DocumentDetail> DocumentDetailsQuery { get; protected set; }
        public IQueryable<Barcode> BarcodesQuery { get; protected set; }
        public IQueryable<ItemBarcode> ItemBarcodesQuery { get; protected set; }
        public IQueryable<VatCategory> VatCategoriesQuery { get; protected set; }
        public IQueryable<VatFactor> VatFactorsQuery { get; protected set; }
        public IQueryable<VatLevel> VatLevelsQuery { get; protected set; }
        public IQueryable<Address> AddressesQuery { get; protected set; }
        public IQueryable<Phone> PhonesQuery { get; protected set; }
        public IQueryable<UserDailyTotals> UserDailyTotalsQuery { get; protected set; }
        public IQueryable<UserDailyTotalsDetail> UserDailyTotalsDetailQuery { get; protected set; }
        public IQueryable<PriceCatalogDetail> PriceCatalogDetailsQuery { get; protected set; }
        public IQueryable<PriceCatalog> PriceCatalogsQuery { get; protected set; }
        public IQueryable<DailyTotals> DailyTotalsQuery { get; protected set; }
        public IQueryable<DailyTotalsDetail> DailyTotalsDetailQuery { get; protected set; }
        public IQueryable<SupplierNew> SupplierNewsQuery { get; protected set; }
        public IQueryable<SupplierNew> SuppliersNewQuery { get; protected set; }
        public UnitOfWork Session;

        public void Dispose()
        {
            if (Session != null)
            {
                Session.Dispose();
                Session = null;


            }
        }
        //To add more XPQueries

        public AbstractLinqQuery(UnitOfWork uow, DataBaseConnectionType DataBaseType)
        {
            //if (ITS.Retail.Common.XpoHelper.databasetype)
            this.DataBaseType = DataBaseType;
            init(uow);
        }
        public AbstractLinqQuery(UnitOfWork uow)
        {
           
            init(uow);
        }
        private void init(UnitOfWork uow)
        {
            ItemsQuery = new XPQuery<Item>(uow);
            ItemCategoriesQuery = new XPQuery<ItemCategory>(uow);
            CustomersQuery = new XPQuery<Customer>(uow);
            TradersQuery = new XPQuery<Trader>(uow);
            SuppliersQuery = new XPQuery<CompanyNew>(uow);
            DocumentHeadersQuery = new XPQuery<DocumentHeader>(uow);
            DocumentDetailsQuery = new XPQuery<DocumentDetail>(uow);

            BarcodesQuery = new XPQuery<Barcode>(uow);
            ItemBarcodesQuery = new XPQuery<ItemBarcode>(uow);
            VatCategoriesQuery = new XPQuery<VatCategory>(uow);
            VatFactorsQuery = new XPQuery<VatFactor>(uow);
            VatLevelsQuery = new XPQuery<VatLevel>(uow);
            AddressesQuery = new XPQuery<Address>(uow);
            PhonesQuery = new XPQuery<Phone>(uow);

            UserDailyTotalsQuery = new XPQuery<UserDailyTotals>(uow);
            DailyTotalsQuery = new XPQuery<DailyTotals>(uow);

            UserDailyTotalsDetailQuery = new XPQuery<UserDailyTotalsDetail>(uow);
            DailyTotalsDetailQuery = new XPQuery<DailyTotalsDetail>(uow);
            PriceCatalogDetailsQuery = new XPQuery<PriceCatalogDetail>(uow);
            PriceCatalogsQuery = new XPQuery<PriceCatalog>(uow);
            SupplierNewsQuery = new XPQuery<SupplierNew>(uow);
            SuppliersNewQuery = new XPQuery<SupplierNew>(uow);
            this.Session = uow;
        }
        public abstract IQueryable MainQuerySet();
        public abstract FiveIQueryables SupportingQuerySets();
        public abstract string KeyExpression { get; }


        public Guid CurrentUserOid { get; set; }
        public Guid CurrentOwnerOid { get; set; }
    }

    public struct FiveIQueryables
    {
        public IQueryable SupportingQuery1;
        public IQueryable SupportingQuery2;
        public IQueryable SupportingQuery3;
        public IQueryable SupportingQuery4;
        public IQueryable SupportingQuery5;
    }


}
