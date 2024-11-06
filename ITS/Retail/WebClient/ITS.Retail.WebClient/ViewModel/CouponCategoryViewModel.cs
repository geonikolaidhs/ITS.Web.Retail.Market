using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class CouponCategoryViewModel : IPersistableViewModel
    {
        public Guid Oid { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Guid Owner { get; set; }

        public Type PersistedType
        {
            get
            {
                return typeof(CouponCategory);
            }
        }

        public bool IsDeleted { get; set; }

        public void UpdateModel(DevExpress.Xpo.Session uow)
        {
            this.UpdateProperties(uow);
        }

        public bool Validate(out string message)
        {
            message = string.Empty;
            if( Owner == Guid.Empty || Oid == Guid.Empty 
            ||  string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Description)
              )
            {
                message = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                return false;
            }
            return true;
        }
    }
}