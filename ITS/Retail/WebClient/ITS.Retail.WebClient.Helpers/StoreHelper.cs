using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using System;
using ITS.Retail.Common;
using System;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Helpers
{
    public class StoreHelper
    {

        public static bool StoreHasSeriesForDocumentType(Store store, DocumentType documentType, eModule? module = null, bool includeCancelingSeries = false, bool onlyForOrder = false, bool includePOSSeries = false)
        {
            return (int)store.Session.Evaluate(typeof(DocumentSeries),
                                               CriteriaOperator.Parse("Count"),
                                               new ContainsOperator("StoreDocumentSeriesTypes",
                                                                    CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", documentType.Oid),
                                                                    StoreDocumentSeriesTypeForDocTypeCriteria(store, module, includeCancelingSeries, onlyForOrder, includePOSSeries)))) > 0;
        }

        public static IEnumerable<DocumentSeries> StoreSeriesForDocumentType(Store store, DocumentType documentType, eModule? module = null,
                                                                             bool includeCancelingSeries = false, bool onlyForOrder = false, bool includePOSSeries = false)
        {

            return documentType == null ? null  : new XPCollection<DocumentSeries>(store.Session, new ContainsOperator("StoreDocumentSeriesTypes",
                                                                                   CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", documentType.Oid),
                                                                                   StoreDocumentSeriesTypeForDocTypeCriteria(store, module, includeCancelingSeries, onlyForOrder, includePOSSeries))));
        }

        public static CriteriaOperator StoreDocumentSeriesTypeForDocTypeCriteria(Store store, eModule? module, bool includeCancelingSeries = false, bool onlyForOrder = false, bool includePOSSeries = false)
        {
            CriteriaOperator moduleCriteria = !module.HasValue ? null :
                                              (module == eModule.POS ? new BinaryOperator("DocumentSeries.eModule", module) :
                                                                                         CriteriaOperator.Or(new BinaryOperator("DocumentSeries.eModule", eModule.ALL),
                                                                                                             new BinaryOperator("DocumentSeries.eModule", module),
                                                                                                             includePOSSeries ?
                                                                                                             new BinaryOperator("DocumentSeries.eModule", eModule.POS) : null));

            return CriteriaOperator.And(store != null ? new BinaryOperator("DocumentSeries.Store.Oid", store.Oid) : null,
                                        new BinaryOperator("DocumentSeries.IsCancelingSeries", includeCancelingSeries),
                                        onlyForOrder ? new BinaryOperator("StoreDocumentType", eStoreDocumentType.ORDER) : null,
                                        moduleCriteria);
        }

        public static List<DocumentType> StoreDocumentTypes(Store store, eDivision? division, eModule? module = null, bool includeCancelingSeries = false, bool onlyForOrder = false, bool includePOSSeries = false)
        {
            CriteriaOperator divisionCriteria = division.HasValue ? new BinaryOperator("Division.Section", division) : null;
            return new XPCollection<DocumentType>(store.Session,CriteriaOperator.And(divisionCriteria,
                                                                    new ContainsOperator("StoreDocumentSeriesTypes",
                                                                                StoreDocumentSeriesTypeForDocTypeCriteria(store, module, includeCancelingSeries, onlyForOrder, includePOSSeries))
                                                                )
                                                  ).ToList();
        }

        public static bool StoreMissesDocumentType(Store store, eModule? module = null)
        {
            return (int)store.Session.Evaluate(typeof(StoreDocumentSeriesType), CriteriaOperator.Parse("Count"), new ContainsOperator("StoreDocumentSeriesTypes",
                                                                                    StoreDocumentSeriesTypeForDocTypeCriteria(store, module, false, false))) == 0;
        }

        public static string CheckedStorePriceLists(Store store, StorePriceList storePriceList, out string modelKey)
        {
            string error = "";
            modelKey = "";
            if (store != null)
            {
                if (store.StorePriceLists.Where(spl => spl.PriceList.Oid == storePriceList.PriceList.Oid && spl.Oid != storePriceList.Oid).Count() > 0)
                {
                    error = Resources.PriceCatalogAlreadyExists;
                    modelKey = "PriceCatalogCb";
                }
            }
            return error;
        }

        public static string CheckedStorePriceCatalogPolicies(Store store, StorePriceCatalogPolicy storePriceCatalogPolicy, out string modelKey)
        {
            modelKey = "";
            if (store != null)
            {
                IEnumerable<Guid> storePriceCatalogGuids = store.StorePriceLists.Select(list => list.PriceList.Oid);
                IEnumerable<Guid> priceCatalogGuids = storePriceCatalogPolicy.PriceCatalogPolicy.PriceCatalogPolicyDetails.Select(detail => detail.PriceCatalog.Oid);
                foreach (Guid priceCatalogGuid in priceCatalogGuids)
                {
                    if (!storePriceCatalogGuids.Contains(priceCatalogGuid))
                    {
                        modelKey = "PriceCatalogPolicyCb";
                        return Resources.NotValidPriceCatalogPolicy;
                    }
                }
                if (storePriceCatalogPolicy.IsDefault && store.StorePriceCatalogPolicies.Where(storePolicy => storePolicy.IsDefault && storePolicy.Oid != storePriceCatalogPolicy.Oid).Count() > 0)
                {
                    modelKey = "IsDefault";
                    return Resources.DefaultAllreadyExists;
                }

                return "";
            }
            return Resources.AnErrorOccurred +". "+ Resources.StoreNotFound;
        }

        public static void AddStoreCommand( Guid storeController, eStoreControllerCommand storeCommand, string parameters)
        {
            using ( UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                StoreControllerCommand scCommand = new StoreControllerCommand(uow)
                {
                    Command = storeCommand,
                    StoreController = uow.GetObjectByKey<StoreControllerSettings>(storeController),
                    CommandParameters = parameters
                };
                scCommand.Save();
                uow.CommitChanges();
            }
        }

        public static bool StoreHasSpecialProformaTypeAndSeries( Store store )
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection<ITS.Retail.Model.POS> posses = new XPCollection<ITS.Retail.Model.POS>(uow);
                foreach(ITS.Retail.Model.POS pos in posses)
                {
                    if ( pos.SpecialProformaDocumentType != null && pos.SpecialProformaDocumentSeries != null )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the criteria for Stores based on Current Store.
        /// In case there is no Reference Company Main and Reference Company are actually the same.
        /// Null input will throw a 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static CriteriaOperator GetReferenceCompanyCriteria(Store store)
        {
            CompanyNew mainCompany = store.Owner;
            CompanyNew referenceCompany = store.ReferenceCompany;
            if ( mainCompany == null )
            {
                throw new ArgumentException( String.Format(Resources.InvalidMainCompany, "null" ));
            }

            CriteriaOperator storeCriteria = new NotOperator(new NullOperator("Address"));

            if (referenceCompany == null)
            {
                storeCriteria = CriteriaOperator.And(storeCriteria, CriteriaOperator.Or(  new NullOperator("ReferenceCompanyOid"),
                                                                                          new BinaryOperator("ReferenceCompanyOid",Guid.Empty)
                                                                                       )
                                                    );
            }
            else
            {
                if (mainCompany.Oid == referenceCompany.Oid)
                {
                    //no need to apply extra criteria other than Owner that MUST be applied in this method!!!
                }
                else
                {
                    storeCriteria = CriteriaOperator.And(new BinaryOperator("ReferenceCompanyOid", referenceCompany.Oid));
                }
            }

            storeCriteria = RetailHelper.ApplyOwnerCriteria(storeCriteria, typeof(Store), mainCompany);
            return storeCriteria;
        }

        /// <summary>
        /// Filters Stores based on both Current Store
        /// In case there is no Reference Company Main and Reference Company are actually the same.
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public static XPCollection<Store> GetStoresByReferenceCompany(UnitOfWork uow, Store store)
        {
            CriteriaOperator storeCriteria = GetReferenceCompanyCriteria(store);
            XPCollection<Store> stores = new XPCollection<Store>(uow, storeCriteria);
            return stores;
        }
    }
}