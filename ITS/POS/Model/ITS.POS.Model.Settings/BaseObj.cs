using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class BaseObj : BasicObj
    {
        public BaseObj()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BaseObj(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



    }
}
