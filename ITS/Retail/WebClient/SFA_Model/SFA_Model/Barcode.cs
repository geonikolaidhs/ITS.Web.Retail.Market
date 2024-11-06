using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;
using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 270, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Barcode : BasicObj, IBarcode
    {
        public Barcode()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Barcode(Session session)
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
        public string Code { get; set; }

    }
}