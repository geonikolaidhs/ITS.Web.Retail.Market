
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Xpo;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    public class CouponBase : BaseObj, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner
    {
        public CouponBase()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CouponBase(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.

        }
        public string Description { get; set; }
        
        public Guid CouponAmountIsAppliedAs { get; set; }

        public Guid CouponAppliesOn { get; set; }
        

        public Guid Owner
        {
            get;set;
        }
        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}