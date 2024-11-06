using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Xpo;


namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 70, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class VatCategory : LookUp2Fields, IVatCategory
    {
        public VatCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatCategory(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public VatCategory(string code, string description)
            : base()
        {

        }
        public eMinistryVatCategoryCode MinistryVatCategoryCode { get; set; }
    }
}