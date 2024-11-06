using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Linq;
using DevExpress.Data;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Common;

namespace ITS.Retail.WebClient.Helpers
{


    public class PriceCatalogHelper
    {

        private class PriorityPriceCatalogDetail : IComparable
        {
            public PriceCatalogDetail pcd;
            public int priority;


            public int CompareTo(object obj)
            {
                PriorityPriceCatalogDetail detail = (PriorityPriceCatalogDetail)obj;
                return this.priority.CompareTo(detail.priority);
            }
        }

        public static CriteriaOperator GetTreePriceCatalogDetailsFilter(PriceCatalog pc, CriteriaOperator crop)
        {
            List<Guid> pcOids = GetPriceCatalogs(pc);
            return CriteriaOperator.And(crop, new BinaryOperator("IsActive", true), new InOperator("PriceCatalog.Oid", pcOids));
        }

        public static List<PriceCatalogDetail> GetTreePriceCatalogDetails(PriceCatalog pc, CriteriaOperator crop, int skipItems = -1, int len = -1)
        {
            List<Guid> pcOids = GetPriceCatalogs(pc);

            IEnumerable<PriceCatalogDetail> allDetailsGroupBySelectOrderByDescending = new XPCollection<PriceCatalogDetail>(pc.Session,
                 CriteriaOperator.And(crop, new BinaryOperator("IsActive", true), new InOperator("PriceCatalog.Oid", pcOids)),
                 new SortProperty("PriceCatalog.Level", SortingDirection.Descending)).GroupBy(g => g.Barcode.Oid).Select(g => g.First());

            if (skipItems == -1 && len == -1)
            {
                return allDetailsGroupBySelectOrderByDescending.ToList();
            }
            else if (skipItems != -1 && len == -1)
            {
                return allDetailsGroupBySelectOrderByDescending.Skip(skipItems).ToList();
            }
            else
            {
                return allDetailsGroupBySelectOrderByDescending.Skip(skipItems).Take(len).ToList();
            }

        }


        public static List<Guid> GetPriceCatalogs(PriceCatalog pc)
        {
            List<Guid> pcOids;
            if (pc.ParentCatalog != null)
            {
                pcOids = GetPriceCatalogs(pc.ParentCatalog);
            }
            else
            {
                pcOids = new List<Guid>();
            }
            pcOids.Add(pc.Oid);
            return pcOids;
        }

        /// <summary>
        /// Όσο πιο μικρό το priority τόσο πιο "δυνατός" ο PriceCatalog
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="pcGravity"></param>
        /// <param name="priority"></param>
        private static void GetPriceCatalogGravity(PriceCatalog pc, ref Dictionary<Guid, int> pcGravity, ref int priority)
        {
            pcGravity.Add(pc.Oid, priority);
            if (pc.ParentCatalog != null)
            {
                priority += 1;
                GetPriceCatalogGravity(pc.ParentCatalog, ref pcGravity, ref priority);
            }
        }

        /// <summary>
        /// Returns a list of all pricecatalogDetails conserning the given documentHeader.
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="sensitivity">Determines the absolute deviation for price changes</param>
        /// <param name="includeUnChangedPrices">Defaults to false. If FALSE ONLY changes will be retrieved.If TRUE all prices will be retrieved.</param>
        /// <returns></returns>
        public static List<Reevaluate> GetPriceChanges(DocumentHeader documentHeader, decimal sensitivity = .0m, bool includeUnChangedPrices = false)
        {
            List<Reevaluate> valuesChanges = new List<Reevaluate>();
            decimal sensitivityAbsoluteValue = Math.Abs(sensitivity);

            foreach (DocumentDetail currentDocumentDetail in documentHeader.DocumentDetails)
            {

                DocumentDetail documentDetailOfLastValue = GetLastItemValue(currentDocumentDetail);
                if (includeUnChangedPrices ||
                    (documentDetailOfLastValue == null
                       || Math.Abs(currentDocumentDetail.FinalUnitPrice - documentDetailOfLastValue.FinalUnitPrice) >= sensitivityAbsoluteValue
                    )
                   )
                {
                    CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Item.Oid", currentDocumentDetail.Item.Oid),
                                                    new ContainsOperator("PriceCatalog.StorePriceLists", new BinaryOperator("Store", documentHeader.Store.Oid))
                                                    );

                    XPCollection<PriceCatalogDetail> priceCatalogDetails = new XPCollection<PriceCatalogDetail>(documentHeader.Session, crop);

                    foreach (PriceCatalogDetail priceCatalogDetail in priceCatalogDetails.Distinct())
                    {
                        valuesChanges.Add(new Reevaluate(priceCatalogDetail, currentDocumentDetail, documentDetailOfLastValue));
                    }
                }
            }
            return valuesChanges;
        }

        public static List<Reevaluate> GetAllExistingPrices(DocumentHeader documentHeader)
        {
            return GetPriceChanges(documentHeader, 0, true);
        }



        /// <summary>
        /// Gets last value for barcode as also the DocumentDetail (and thus the corresponding DocumentHeader) where it was found.
        /// </summary>
        /// <param name="currentDocumentDetail">The item to search for. The document of currentDocumentDetail must be excluded. It is also used to get Supplier,Store etc...</param>        
        /// <param name="documentDetailOfLastValue"></param>
        /// <param name="costValue">If true cost values are searched otherwise purchase values</param>
        public static DocumentDetail GetLastItemValue(DocumentDetail currentDocumentDetail, bool costValue = false)
        {
            CriteriaOperator crop = CriteriaOperator.And(
                                                    new ContainsOperator("DocumentDetails", new BinaryOperator("Item.Oid", currentDocumentDetail.Item.Oid)),
                                                    new BinaryOperator("Division", currentDocumentDetail.DocumentHeader.Division),
                                                    new NotOperator(new BinaryOperator("Oid", currentDocumentDetail.DocumentHeader.Oid)),
                                                    new BinaryOperator("Supplier.Oid", currentDocumentDetail.DocumentHeader.Supplier.Oid),
                                                    new BinaryOperator("Store.Oid", currentDocumentDetail.DocumentHeader.Store.Oid),
                                                    new BinaryOperator("DocumentType.UsesMarkUp", true)
                                                    );

            SortProperty sortProperty = new SortProperty("FinalizedDate", SortingDirection.Descending);

            DocumentHeader lastValueDocumentHeader = new XPCollection<DocumentHeader>(currentDocumentDetail.Session, crop, sortProperty).FirstOrDefault();

            DocumentDetail lastValueDocumentDetail = lastValueDocumentHeader == null ? null : lastValueDocumentHeader
                                                .DocumentDetails.Where(docDet => docDet.Item.Oid == currentDocumentDetail.Item.Oid)
                                                .OrderByDescending(docDetail => docDetail.CustomUnitPrice).FirstOrDefault();

            DocumentDetail documentDetailOfLastValue = null;

            if (lastValueDocumentDetail != null)
            {
                documentDetailOfLastValue = lastValueDocumentDetail;
            }

            return documentDetailOfLastValue;
        }

        ///// <summary>
        ///// Returns a CriteriaOperator, to be applied on PriceCatalogDetails , where TimeValue has changed or will be changed on the provided Time Space
        ///// </summary>
        ///// <param name="from">Time Value Changed from</param>
        ///// <param name="until">TimeValue Changed until</param>
        ///// <returns>Returns a CriteriaOperator, to be applied on PriceCatalogDetails , where TimeValue has changed or will be changed on the provided Time Space</returns>
        //public static CriteriaOperator GetTimeValueCriteriaForPriceCatalogDetails(DateTime from, DateTime until)
        //{
        //    CriteriaOperator case1Criteria = new ContainsOperator("TimeValues", CriteriaOperator.And(new BetweenOperator("TimeValueChangedOn", from.Ticks, until.Ticks),
        //                                                            new BinaryOperator("TimeValueValidFrom", DateTime.Now.Ticks, BinaryOperatorType.LessOrEqual),
        //                                                            new BinaryOperator("TimeValueValidUntil", DateTime.Now.Ticks, BinaryOperatorType.GreaterOrEqual)));

        //    CriteriaOperator case2Criteria = new BetweenOperator("TimeValueValidFrom", from.Ticks, until.Ticks);
        //    DateTime dayBefore = from == DateTime.MinValue ? from : from.AddDays(-1).Date;
        //    CriteriaOperator case3Criteria = new BetweenOperator("TimeValueValidUntil", dayBefore.Ticks, until.Ticks);
        //    CriteriaOperator timeDatesCriteria = CriteriaOperator.Or(case1Criteria, case2Criteria, case3Criteria);
        //    return timeDatesCriteria;
        //}

        ///// <summary>
        ///// Returns a CriteriaOperator, to be applied on PriceCatalogDetails , where Value has changed on the provided Time Space
        ///// </summary>
        ///// <param name="from">Time Value Changed from</param>
        ///// <param name="until">Time Value Changed until</param>
        ///// <param name="onlyValueChanges"></param>
        ///// <returns>Returns a CriteriaOperator, to be applied on PriceCatalogDetails , where Value has changed on the provided Time Space</returns>
        //public static CriteriaOperator GetValueChangesCriteria(DateTime from, DateTime until, bool onlyValueChanges)
        //{
        //    CriteriaOperator newPricesFromFilter = new BinaryOperator("ValueChangedOn", from.Ticks, BinaryOperatorType.GreaterOrEqual);
        //    CriteriaOperator newPricesToFilter = new BinaryOperator("ValueChangedOn", until.Ticks, BinaryOperatorType.LessOrEqual);
        //    CriteriaOperator dateFilter = CriteriaOperator.And(newPricesFromFilter, newPricesToFilter);
        //    if (onlyValueChanges == false)
        //    {
        //        CriteriaOperator itemChangesFromFilter = new BinaryOperator("Item.UpdatedOnTicks", from.Ticks, BinaryOperatorType.GreaterOrEqual);
        //        CriteriaOperator itemChangesToFilter = new BinaryOperator("Item.UpdatedOnTicks", until.Ticks, BinaryOperatorType.LessOrEqual);

        //        CriteriaOperator bcChangesFromFilter = new BinaryOperator("Barcode.UpdatedOnTicks", from.Ticks, BinaryOperatorType.GreaterOrEqual);
        //        CriteriaOperator bcChangesToFilter = new BinaryOperator("Barcode.UpdatedOnTicks", until.Ticks, BinaryOperatorType.LessOrEqual);

        //        dateFilter = CriteriaOperator.Or(dateFilter, CriteriaOperator.And(itemChangesFromFilter, itemChangesToFilter),
        //                                                    CriteriaOperator.And(bcChangesFromFilter, bcChangesToFilter));
        //    }
        //    return dateFilter;
        //}


        public static PriceCatalogPolicy GetPriceCatalogPolicy(Store store, Customer customer = null)
        {
            PriceCatalogPolicy priceCatalogPolicy = null;
            IEnumerable<Guid> storePriceCatalogGuids = store.StorePriceLists.Select(list => list.PriceList.Oid);
            if (customer != null && customer.PriceCatalogPolicy != null && customer.PriceCatalogPolicy.PriceCatalogPolicyDetails.Count > 0)
            {
                priceCatalogPolicy = customer.PriceCatalogPolicy ?? store.DefaultPriceCatalogPolicy;
            }
            else
            {
                priceCatalogPolicy = store.DefaultPriceCatalogPolicy;
            }
            return priceCatalogPolicy;
        }

        public static List<Guid> GetPriceCatalogsFromPolicy(EffectivePriceCatalogPolicy effectivePriceCatalogPolicy)
        {
            List<Guid> pcOids = new List<Guid>();
            if (effectivePriceCatalogPolicy != null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    foreach (EffectivePriceCatalogPolicyDetail effectivePriceCatalogPolicyDetail in effectivePriceCatalogPolicy.OrderedPriceCatalogDetail)
                    {
                        pcOids.AddRange(GetPriceCatalogs(uow.GetObjectByKey<PriceCatalog>(effectivePriceCatalogPolicyDetail.PriceCatalogOid)).Distinct());
                    }
                }
            }
            return pcOids;
        }

        public static List<Guid> GetPriceCatalogsFromStore(Store store, Customer customer = null)
        {
            List<Guid> pcOids = new List<Guid>();

            if (store != null)
            {
                //PriceCatalogPolicy priceCatalogPolicy = GetPriceCatalogPolicy(store, customer);
                //return GetPriceCatalogsFromPolicy(priceCatalogPolicy.PriceCatalogPolicyDetails.ToList());
                return GetPriceCatalogsFromPolicy(new EffectivePriceCatalogPolicy(store,customer));
            }
            return pcOids;
        }

        public static decimal GetUnitPrice(Store store, Customer customer, string searchCode, PriceType priceType = PriceType.RETAIL)
        {
            return GetUnitPriceFromPolicy(store.Session as UnitOfWork, new EffectivePriceCatalogPolicy(store,customer), searchCode, priceType);
        }

        public static decimal GetUnitPriceFromPolicy(UnitOfWork uow, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy, string searchCode, PriceType priceType = PriceType.RETAIL)
        {
            PriceCatalogPolicyPriceResult priceCatalogDetailPriceResult = GetPriceCatalogDetailFromPolicy(uow,effectivePriceCatalogPolicy, searchCode);
            if (priceCatalogDetailPriceResult != null && priceCatalogDetailPriceResult.PriceCatalogDetail != null)
            {
                switch (priceType)
                {
                    case PriceType.RETAIL:
                        return priceCatalogDetailPriceResult.PriceCatalogDetail.RetailValue;
                    case PriceType.WHOLESALE:
                        return priceCatalogDetailPriceResult.PriceCatalogDetail.WholesaleValue;
                    default:
                        throw new NotSupportedException("PriceCatalogHelper.GetUnitPrice(List<PriceCatalogPolicyDetail> priceCatalogPolicyDetails, Item item, Barcode barcode = null,  PriceType priceType = PriceType.RETAIL )");
                }
            }
            return -1;//TODO explicitly define what is the value for the case when item has no price at all for given policy
        }

        public static PriceCatalogPolicyPriceResult GetPriceCatalogDetail(Store store, string searchCode, Customer customer = null, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            return GetPriceCatalogDetailFromPolicy(store.Session as UnitOfWork,new EffectivePriceCatalogPolicy(store,customer), searchCode, priceCatalogSearchMethod, traces);
        }

        public static PriceCatalogPolicyPriceResult GetPriceCatalogDetailFromPolicy(UnitOfWork uow, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy,
            string searchCode, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            ItemBarcode itemBarcode = uow.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Code", searchCode),
                                                                                            new BinaryOperator("Item.IsActive", true),
                                                                                            new BinaryOperator("Item.Owner.Oid", effectivePriceCatalogPolicy.Owner)));
            if(itemBarcode == null)
            {
                return null;
            }
            return GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, itemBarcode.Item, itemBarcode.Barcode, priceCatalogSearchMethod, traces);           
        }

        public static PriceCatalogPolicyPriceResult GetPriceCatalogDetailFromPolicy(UnitOfWork uow, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy, Item item, 
            Barcode barcode = null,
            PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            PriceCatalogDetail priceCatalogDetail = null;

            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = new PriceCatalogPolicyPriceResult()
            {
                SearchBarcode = barcode,
                PriceCatalogDetail = null
            };

            //TODO explicitly define what is the value for the case when item has no price at all for given policy
            if (effectivePriceCatalogPolicy != null && effectivePriceCatalogPolicy.HasPolicyDetails())
            {
                foreach (EffectivePriceCatalogPolicyDetail effectivePriceCatalogPolicyDetail in effectivePriceCatalogPolicy.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort))
                {
                    PriceCatalog priceCatalog = uow.GetObjectByKey<PriceCatalog>(effectivePriceCatalogPolicyDetail.PriceCatalogOid);
                    priceCatalogDetail = item.GetPriceCatalogDetail(priceCatalog, barcode, priceCatalogSearchMethod, traces);                                               
                    if (priceCatalogDetail != null)
                    {
                        priceCatalogPolicyPriceResult.PriceCatalogDetail = priceCatalogDetail;
                        if ( priceCatalogPolicyPriceResult.SearchBarcode == null )
                        {
                            priceCatalogPolicyPriceResult.SearchBarcode = priceCatalogDetail.Barcode;
                        }
                        return priceCatalogPolicyPriceResult;
                    }
                }
            }
            return priceCatalogPolicyPriceResult;//TODO explicitly define what is the value for the case when item has no price at all for given policy
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pricecatalog"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static PriceCatalogDetail GetPriceCatalogDetailFromPriceCatalog(PriceCatalog priceCatalog, string barcode, PriceCatalogSearchMethod searchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            if (priceCatalog == null || string.IsNullOrEmpty(barcode))
            {
                return null;
            }

            PriceCatalogDetail priceCatalogDetail = null;
            ItemBarcode itemBarcode = priceCatalog.Session.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Code", barcode),
                                                                                                        new BinaryOperator("Item.IsActive", true),
                                                                                                        new BinaryOperator("Item.Owner.Oid", priceCatalog.Owner.Oid)));
            if (itemBarcode != null)
            {
                priceCatalogDetail = itemBarcode.Item.GetPriceCatalogDetail(priceCatalog, itemBarcode.Barcode, searchMethod, traces: traces);
            }
            return priceCatalogDetail;
        }

        public static PriceCatalogDetail GetPriceCatalogDetailFromPriceCatalog(PriceCatalog priceCatalog, Barcode barcode, 
            PriceCatalogSearchMethod searchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            if (priceCatalog == null || barcode == null)
            {
                return null;
            }

            PriceCatalogDetail priceCatalogDetail = null;
            ItemBarcode itemBarcode = priceCatalog.Session.FindObject<ItemBarcode>(CriteriaOperator.And(new BinaryOperator("Barcode.Oid", barcode.Oid),
                                                                                                        new BinaryOperator("Item.IsActive", true),
                                                                                                        new BinaryOperator("Item.Owner.Oid", priceCatalog.Owner.Oid)));
            if (itemBarcode != null)
            {
                priceCatalogDetail = itemBarcode.Item.GetPriceCatalogDetail(priceCatalog, itemBarcode.Barcode, searchMethod, traces: traces);
            }
            return priceCatalogDetail;
        }

       

        static List<PriceCatalog> GetPriceCatalogPathToRoot(PriceCatalog pc)
        {
            List<PriceCatalog> path = pc.ParentCatalog == null ? new List<PriceCatalog>() : GetPriceCatalogPathToRoot(pc.ParentCatalog);
            path.Insert(0, pc);
            return path;
        }

        public static List<Guid> GetActivePriceCatalogDetailOids(Store store, DateTime activeDatetime, DBType? dbtype)
        {
            DBType effectiveType = dbtype.HasValue ? dbtype.Value : XpoHelper.databasetype;
            EffectivePriceCatalogPolicy effectivePolicy = new EffectivePriceCatalogPolicy(store);

            Guid batchId = Guid.NewGuid();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ClearTempPriceCatalogPriority(uow);
                List<PriceCatalogPriority> effectivePriceCatalogs = effectivePolicy.PriceCatalogPolicyDetails
                                                                                   .Select(effectivePolicyDetail => new PriceCatalogPriority(uow)
                                                                                   {
                                                                                       BatchID = batchId,
                                                                                       Order = effectivePolicyDetail.Sort * 1m,
                                                                                       PriceCatalog = uow.GetObjectByKey<PriceCatalog>(effectivePolicyDetail.PriceCatalogOid),
                                                                                       Method = effectivePolicyDetail.PriceCatalogSearchMethod
                                                                                   }).ToList();

                List<PriceCatalogPriority> treeSearches = effectivePriceCatalogs.Where(x => x.Method == PriceCatalogSearchMethod.PRICECATALOG_TREE).ToList();
                List<PriceCatalogPriority> allPriceCatalogsWithOrder = effectivePriceCatalogs.Where(x => x.Method != PriceCatalogSearchMethod.PRICECATALOG_TREE).ToList();
                foreach (var treeSearchCatatlog in treeSearches)
                {
                    List<PriceCatalog> path = GetPriceCatalogPathToRoot(treeSearchCatatlog.PriceCatalog);
                    for (int i = 0; i < path.Count; i++)
                    {
                        allPriceCatalogsWithOrder.Add(new PriceCatalogPriority(uow) { BatchID = batchId, Order = treeSearchCatatlog.Order * 1m + 0.01m * i, PriceCatalog = uow.GetObjectByKey<PriceCatalog>(path[i].Oid), Method = PriceCatalogSearchMethod.CURRENT_PRICECATALOG });
                    }
                }
                List<PriceCatalogPriority> activePriceCatalogsWithOrder = allPriceCatalogsWithOrder.Where(x => x.PriceCatalog.IsActive && x.PriceCatalog.EndDate >= activeDatetime && x.PriceCatalog.StartDate <= activeDatetime).OrderBy(x => x.Order).ToList();

                if (activePriceCatalogsWithOrder.Count == 0)
                {
                    return new List<Guid>();
                }
                uow.GetObjectsToSave().Cast<PriceCatalogPriority>().Where(x => activePriceCatalogsWithOrder.Contains(x) == false).ToList().ForEach(x => x.Delete());
                uow.CommitTransaction();
            }

            string trueValue = "1";
            switch (effectiveType)
            {
                case DBType.SQLServer:
                    trueValue = "1";
                    break;
                case DBType.postgres:
                    trueValue = "true";
                    break;
            }

            //var result1 = store.Session.Query<PriceCatalogDetail>().Where(pcd => pcd.IsActive)
            //                        .Join(store.Session.Query<PriceCatalogPriority>().Where(ppp => ppp.BatchID == batchId),
            //                                pcd => pcd.PriceCatalog.Oid,
            //                                 ppp => ppp.PriceCatalog.Oid,
            //                                 (pcd, ppp) => new TempPriceCatalogDetailPriority() { PriceCatalogPriority = ppp.Order, PriceCatalogDetail = pcd.Oid, Barcode = pcd.Barcode.Oid }
            //                            )
            //                        .GroupBy(anon => anon.Barcode)
            //                        .Select(key => key.OrderBy(x => x.PriceCatalogPriority).FirstOrDefault())
            //                        .Select(x => x.PriceCatalogDetail);

            //store.Session.Query<PriceCatalogPriority>().Where(ppp => ppp.BatchID == batchId)


            //                            .Join(store.Session.Query<PriceCatalogDetail>().Where(pcd => pcd.IsActive),
            //                                    ppp => ppp.PriceCatalog.Oid,
            //                                    pcd => pcd.PriceCatalog.Oid,
            //                                    (ppp, pcd) => new { PriceCatalogPriority = ppp.Order, PriceCatalogDetail = pcd.Oid, Barcode = pcd.Barcode.Oid }
            //                            )

            //                            .GroupBy(anon => anon.Barcode)
            //                            .Select(key => key.OrderBy(x=>x.PriceCatalogPriority).FirstOrDefault())                                           
            //                            .Select(x => x.PriceCatalogDetail);*/
            //var result2 = result1.ToList();
            SelectedData data = store.Session.ExecuteQuery(string.Format(@"select ""Oid"" from 
                    (Select ""Barcode"", min(""Order"") as priority from ""PriceCatalogPriority""
                    inner join ""PriceCatalogDetail"" on ""PriceCatalogDetail"".""PriceCatalog"" = ""PriceCatalogPriority"".""PriceCatalog""
                    where ""PriceCatalogPriority"".""BatchID"" = '{0}' and ""PriceCatalogDetail"".""IsActive"" = {1}
                    group by ""Barcode"") a
                    inner join
                    (
                    Select ""PriceCatalogDetail"".""Oid"", ""Barcode"", ""Order"" as priority from ""PriceCatalogPriority""
                    inner join ""PriceCatalogDetail"" on ""PriceCatalogDetail"".""PriceCatalog"" = ""PriceCatalogPriority"".""PriceCatalog""
                    where ""PriceCatalogPriority"".""BatchID"" = '{0}'  and ""PriceCatalogDetail"".""IsActive"" = {1}
                    ) b on a.""Barcode"" = b.""Barcode"" and a.priority = b.priority", batchId, trueValue));
            var result= data.ResultSet[0].Rows.Select(x => (Guid)x.Values[0]).ToList();
            return result;

        }

        private static void ClearTempPriceCatalogPriority(UnitOfWork uow)
        {
            XPCollection<PriceCatalogPriority> prioritiesToDelete = new XPCollection<PriceCatalogPriority>(uow, new BinaryOperator("CreatedOnTicks", DateTime.Now.Date.AddDays(-1).Ticks, BinaryOperatorType.LessOrEqual));
            uow.Delete(prioritiesToDelete);
        }

        public static IEnumerable<PriceCatalogDetail> GetAllSortedPriceCatalogDetails(Store store, CriteriaOperator crop, Customer customer = null, DBType? dbtype = null)
        {
            //Add basic criteria for price catalog details
            CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IsActive", true), crop);

            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();

            IQueryable<PriceCatalogDetail> result = GetActivePriceCatalogDetailOids(store, DateTime.Now, dbtype)
                .Join(store.Session.Query<PriceCatalogDetail>().AppendWhere(conv, criteria).Cast<PriceCatalogDetail>(), y => y, x => x.Oid, (x, y) => y)
                .Where(x => x != null).Distinct()
                .AsQueryable();                

            return result;
        }

        public static string CheckedPriceCatalogPolicyDetails(PriceCatalogPolicy policy, PriceCatalogPolicyDetail priceCatalogPolicyDetail, out string modelKey)
        {
            string error = "";
            modelKey = "";
            if (policy != null)
            {
                if (policy.PriceCatalogPolicyDetails.Where(detail => detail.PriceCatalog.Oid == priceCatalogPolicyDetail.PriceCatalog.Oid && detail.Oid != priceCatalogPolicyDetail.Oid).Count() > 0)
                {
                    error = Resources.PriceCatalogAlreadyExists;
                    modelKey = "PriceCatalogCb";
                }
                else if (priceCatalogPolicyDetail.IsDefault && policy.PriceCatalogPolicyDetails.Where(detail => detail.IsDefault && detail.Oid != priceCatalogPolicyDetail.Oid).Count() > 0)
                {
                    error = Resources.DefaultAllreadyExists;
                    modelKey = "IsDefault";
                }
                else if (priceCatalogPolicyDetail.Sort != -1 && policy.PriceCatalogPolicyDetails.Where(detail => detail.Sort == priceCatalogPolicyDetail.Sort && detail.Oid != priceCatalogPolicyDetail.Oid).Count() > 0)
                {
                    error = Resources.SortIndexAlreadyExists;
                    modelKey = "Sort";
                }
            }
            return error;
        }

        public static void IncludeCustomerPolicyToPoliciesList(DocumentHeader document, List<PriceCatalogPolicy> policies)
        {
            if (document.Customer != null)
            {
                if (document.Customer.PriceCatalogPolicy != null)
                {
                    bool includeCustomerPolicy = true;
                    IEnumerable<Guid> customerPriceCatalogGuids = document.Customer.PriceCatalogPolicy.PriceCatalogPolicyDetails.Select(detail => detail.PriceCatalog.Oid);
                    foreach (Guid priceCatalogGuid in customerPriceCatalogGuids)
                    {
                        if (!document.Store.StorePriceLists.Select(list => list.PriceList.Oid).Contains(priceCatalogGuid))
                        {
                            includeCustomerPolicy = false;
                        }
                    }
                    if (includeCustomerPolicy)
                    {
                        policies.Add(document.Customer.PriceCatalogPolicy);
                    }
                }
            }
        }
    }
}