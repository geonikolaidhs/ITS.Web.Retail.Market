using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
    using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Settings
{

    public class ZReportSequence : LookupField
    {
        public ZReportSequence()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ZReportSequence(Session session)
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

        // Fields...
        private int _ZReportNumber;
        private Guid _POS;

        public Guid POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }


        public int ZReportNumber
        {
            get
            {
                return _ZReportNumber;
            }
            set
            {
                SetPropertyValue("ZReportNumber", ref _ZReportNumber, value);
            }
        }

    }

}


