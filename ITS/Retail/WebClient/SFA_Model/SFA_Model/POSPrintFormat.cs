using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 651, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class POSPrintFormat : LookupField
    {

        public POSPrintFormat()
        : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public POSPrintFormat(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public string Format { get; set; }

        public eFormatType FormatType { get; set; }

        public Guid DocumentType { get; set; }


    }
}
