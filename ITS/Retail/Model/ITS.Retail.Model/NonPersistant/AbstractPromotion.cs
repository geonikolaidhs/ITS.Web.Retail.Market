using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class AbstractPromotion: Lookup2Fields
    {
        public AbstractPromotion(Session session)
            : base(session)
        {
            
        }
        public AbstractPromotion(string code, string description)
            : base(code, description)
        {
            
        }
        public AbstractPromotion(Session session, string code, string description)
            : base(session, code, description)
        {
            
        }
        public AbstractPromotion():base()
        {
            
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        // Fields...
        private bool _DropsOtherPromotions;
        private int _Priority;

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


        public bool DropsOtherPromotions
        {
            get
            {
                return _DropsOtherPromotions;
            }
            set
            {
                SetPropertyValue("DropsOtherPromotions", ref _DropsOtherPromotions, value);
            }
        }
    }
}
