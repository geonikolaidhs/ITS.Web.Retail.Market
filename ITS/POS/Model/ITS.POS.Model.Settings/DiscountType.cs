using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Settings
{
    public class DiscountType : Lookup2Fields , IDiscountType
    {
        public DiscountType()
        {

        }
        public DiscountType(Session session)
            : base(session)
        {

        }
        public DiscountType(string code, string description)
            : base(code, description)
        {

        }
        public DiscountType(Session session, string code, string description)
            : base(session, code, description)
        {

        }


        // Fields...
        private bool _DiscardsOtherDiscounts;
        private bool _IsUnique;
        private eDiscountType _EDiscountType;
        private int _Priority;
        private bool _IsHeaderDiscount;

        public eDiscountType eDiscountType
        {
            get
            {
                return _EDiscountType;
            }
            set
            {
                SetPropertyValue("eDiscountType", ref _EDiscountType, value);
            }
        }


        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
        }


        public bool IsUnique
        {
            get
            {
                return _IsUnique;
            }
            set
            {
                SetPropertyValue("IsUnique", ref _IsUnique, value);
            }
        }


        public bool IsHeaderDiscount
        {
            get
            {
                return _IsHeaderDiscount;
            }
            set
            {
                SetPropertyValue("IsHeaderDiscount", ref _IsHeaderDiscount, value);
            }
        }

        public bool DiscardsOtherDiscounts
        {
            get
            {
                return _DiscardsOtherDiscounts;
            }
            set
            {
                SetPropertyValue("DiscardsOtherDiscounts", ref _DiscardsOtherDiscounts, value);
            }
        }



    }
}
