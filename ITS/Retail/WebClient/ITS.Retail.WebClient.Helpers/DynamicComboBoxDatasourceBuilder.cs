using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public class DynamicComboBoxDataSourceBuilder<TLookupType> where TLookupType : BaseObj
    {
        protected UnitOfWork UnitOfWork { get; set; }
        public List<string> LookupFields { get; set; }
        public List<SortProperty> SortProperties { get; set; }
        protected CompanyNew Owner { get; set; }

        public DynamicComboBoxDataSourceBuilder(UnitOfWork uow, CompanyNew owner, params string[] lookupFields)
        {
            this.UnitOfWork = uow;
            this.LookupFields = new List<string>(lookupFields);
            this.SortProperties = new List<SortProperty>();
            this.Owner = owner;
        }

        public ItemsRequestedByFilterConditionMethod DataSourceMethod
        {
            get
            {
                return (ListEditItemsRequestedByFilterConditionEventArgs args) =>
                {
                    //if (args.Filter == "") { return null; }
                    string proccessedFilter = args.Filter.Replace("*", "%");
                    if (!proccessedFilter.Contains("%"))
                    {
                        proccessedFilter = String.Format("%{0}%", proccessedFilter);
                    }

                    XPCollection<TLookupType> collection = new XPCollection<TLookupType>(UnitOfWork);
                    collection.SkipReturnedObjects = args.BeginIndex;
                    collection.TopReturnedObjects = args.EndIndex - args.BeginIndex + 1;

                    CriteriaOperator criteria = null;
                    foreach (string lookupField in LookupFields)
                    {
                        criteria = CriteriaOperator.Or(criteria, new BinaryOperator(lookupField, proccessedFilter, BinaryOperatorType.Like));
                    }

                    collection.Criteria = RetailHelper.ApplyOwnerCriteria(criteria, typeof(TLookupType), Owner);
                    foreach (SortProperty sortProperty in SortProperties)
                    {
                        collection.Sorting.Add(sortProperty);
                    }

                    return collection;
                };
            }
        }

        public ItemRequestedByValueMethod RequestedByValueMethod
        {
            get
            {
                return (ListEditItemRequestedByValueEventArgs args) =>
                {
                    Guid id;
                    if (args.Value != null)
                    {
                        if (Guid.TryParse(args.Value.ToString(), out id))
                        {
                            return UnitOfWork.GetObjectByKey<TLookupType>(args.Value);
                        }
                        return null;
                    }
                    return null;
                };
            }
        }
    }
}
